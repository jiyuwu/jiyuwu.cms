using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extensions;
using JIYUWU.Core.UserManager;
using JIYUWU.Entity.Base;
using SqlSugar;

namespace UMES.Core.Tenancy
{
    public static class TenancyExpression
    {
        /// <summary>
        /// 注意：数据库表中必须包括appsettings.json配置文件UserIdField的字段才会进行数据隔离。如果表没有这些字段，请在上面单独写过滤逻辑
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static ISugarQueryable<T> CreateTenancyFilter<T>(this ISugarQueryable<T> query)
        {
            //2023.12.10实现租户字段过滤
            //if (AppSetting.TenancyField != null
            //    && typeof(T).GetProperty(AppSetting.TenancyField) != null
            //    && !string.IsNullOrEmpty(UserContext.Current.UserInfo.TenancyValue))
            //{
            //    query = query.Where(AppSetting.TenancyField.CreateExpression<T>(UserContext.Current.UserInfo.TenancyValue, LinqExpressionType.Equal));
            //}

            //是否用户表
            bool isUserTable = typeof(T) == typeof(Base_User);

            //默认通过创建人id过滤数据
            string filterCreateId = null;
            //用户表通过user_id过滤数据
            if (isUserTable)
            {
                filterCreateId = "User_Id";
            }
            else
            {
                //获取表的创建人id字段，在配置appsettings文件中UserIdField值
                var properties = typeof(T).GetProperties();
                //使用创建人id过滤数据
                filterCreateId = properties.Where(x => x.Name == AppSetting.CreateMember.UserIdField).FirstOrDefault()?.Name;
                if (filterCreateId == null)
                {
                    filterCreateId = properties.Where(x => x.Name == "CreateId").FirstOrDefault()?.Name;
                }
                //没有配置创建人id的表不执行数据权限过滤
                if (filterCreateId == null)
                {
                    return query;
                }
            }

            string tableName = typeof(T).Name;
            //使用用户数据权限(用户管理界面配置的指定看到某些用户创建的数据库)
            if (AppSetting.UserAuth)
            {
                int[] userIds = UserContext.Current.GetCurrentUserAuthUserIds(tableName);
                //设置查看指定用户的数据
                if (userIds != null && userIds.Length > 0)
                {
                    return query.Where(filterCreateId.CreateExpression<T>(userIds, LinqExpressionType.In));
                }
            }
            var roleIds = UserContext.Current.RoleIds;
            //2024.08.11增加菜单数据权限(优先级高级角色数据权限)
            List<int> authDataTypes = null;
            string authMenuData = UserContext.Current.GetPermissions(tableName.ToLower())?.AuthMenuData;
            if (!string.IsNullOrEmpty(authMenuData))
            {
                authDataTypes = new List<int>() { authMenuData.GetInt() };
            }
            else
            {
                authDataTypes = RoleContext.GetRoles(x => roleIds.Contains(x.Id))
                   .Where(x => x.AuthData > 0)
                   .Select(s => s.AuthData).ToList();
            }

            if (!isUserTable)
            {
                if (authDataTypes.Count == 0)
                {
                    return query;
                }
            }
            //!!不要给超级管理员设置部门，否则可能会被组织权限共享显示出来
            if (!isUserTable&&(authDataTypes.Contains((int)AuthData.本组织及下数据) || authDataTypes.Contains((int)AuthData.本组织数据)))
            {
                var deptIds = UserContext.Current.DeptIds;
                var userDeptQuery = DbServerProvider.DbContext.Set<Base_UserDepartment>().Where(x => x.Enable == 1);
                if (authDataTypes.Contains((int)AuthData.本组织及下数据))
                {
                    deptIds = DepartmentContext.GetAllChildrenIds(deptIds);
                }

                //分库
                if (CheckDb<T>())
                {
                    userDeptQuery = userDeptQuery.Where(x => deptIds.Contains(x.DepartmentId));
                    var userIds = userDeptQuery.Select(s => s.UserId).Distinct();
                    query = query.QueryTenancyDynamicShareDBFilter<T>(filterCreateId, userIds);
                }
                else
                {
                    query = query.QueryTenancyFilter<T, Base_UserDepartment>(filterCreateId, "UserId",deptIds:deptIds);
                }
                return query;
            }
            //如果角色没有配置数据权限，当前页面是isUserTable=true用户表时，默认显示当前角色下的数据
            if (isUserTable || authDataTypes.Contains((int)AuthData.本角色以及下数据) || authDataTypes.Contains((int)AuthData.本角色数据))
            {
                var userRoleQuery = DbServerProvider.DbContext.Set<Base_UserRole>().Where(x => x.Enable == 1 && x.RoleId > 1);
                if (isUserTable||authDataTypes.Contains((int)AuthData.本角色以及下数据))
                {
                    //获取所有子角色
                    roleIds = RoleContext.GetAllChildrenIds(roleIds).ToArray();
                 
                }
                //分库
                if (CheckDb<T>())
                {
                    userRoleQuery = userRoleQuery.Where(x => roleIds.Contains(x.RoleId));
                    var userIds = userRoleQuery.Select(s => s.UserId).Distinct();
                    query = query.QueryTenancyDynamicShareDBFilter<T>(filterCreateId, userIds);
                }
                else
                {
                    query = query.QueryTenancyFilter<T, Base_UserRole>(filterCreateId, "UserId", roleIds);
                }
                return query;
            }
            if (authDataTypes.Contains((int)AuthData.仅自己数据))
            {
                return query.Where(filterCreateId.CreateExpression<T>(UserContext.Current.UserId, LinqExpressionType.Equal));
            }
            return query;
        }

        private static bool CheckDb<T>()
        {
            //是否使用分库
            return AppSetting.UseDynamicShareDB || typeof(T).BaseType.Name != typeof(BaseEntity).Name;
        }
        private static ISugarQueryable<T1> QueryTenancyDynamicShareDBFilter<T1>(this ISugarQueryable<T1> query, string createIdField, ISugarQueryable<int> userIds)
        {
            return query.Where(createIdField.CreateExpression<T1>(userIds.Take(5000).ToArray(), LinqExpressionType.In));
        }
        private static ISugarQueryable<T1> QueryTenancyFilter<T1, T2>(this ISugarQueryable<T1> query, string createIdField, 
            string userIdField, int[] roleIds = null, List<Guid> deptIds = null) where T2 : class, new()
        {
            createIdField = "it." + createIdField;
            userIdField = "s." + userIdField;

            if (typeof(T2) == typeof(Base_UserDepartment))
            {
                query = query.Where(it => SqlSugar.SqlFunc.Subqueryable<Base_UserDepartment>()
                         .Where(s => s.Enable == 1 && deptIds.Contains(s.DepartmentId) && SqlFunc.MappingColumn<string>(createIdField) == SqlFunc.MappingColumn<string>(userIdField)).Any());
            }
            else
            {
                query = query.Where(it => SqlSugar.SqlFunc.Subqueryable<Base_UserRole>()
                .Where(s => s.Enable == 1 && roleIds.Contains(s.RoleId) && SqlFunc.MappingColumn<string>(createIdField) == SqlFunc.MappingColumn<string>(userIdField)).Any());
            }
            return query;
        }
    }
}
