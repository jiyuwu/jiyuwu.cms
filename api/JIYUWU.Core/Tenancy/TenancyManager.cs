using JIYUWU.Core.UserManager;
using JIYUWU.Entity.Base;
using SqlSugar;

namespace UMES.Core.Tenancy
{
    public static class TenancyManager<T> where T : class
    {
        /// <summary>
        /// 数据隔离操作增加了queryable参数可以写EF查询，及增加了返回参数)
        /// 注意(必看)：数据库表字段必须包括appsettings.json配置文件中的CreateMember->UserIdField创建人id字段才会进行数据隔离。
        /// 如果表没有这些字段，请在下面 switch (tableName)单独写过滤逻辑
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <returns></returns>
        public static (string sql, ISugarQueryable<T> query) GetSearchQueryable(string multiTenancyString, string tableName, ISugarQueryable<T> queryable)
        {
           
            //超级管理员不限制(这里可以根据tableName表名自己判断要不要限制超级管理员)
            if (UserContext.Current.IsSuperAdmin)
            {
                return (multiTenancyString, queryable);
            }

            switch (tableName)
            {
                //例如：指定用户表指定查询条件
                case nameof(Base_Role):
                    break;
                case nameof(Base_Group):
                    break;
                case nameof(Base_Department):
                    break;
                default:
                    queryable = queryable.CreateTenancyFilter<T>();
                    break;
            }
            return (multiTenancyString, queryable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="ids">当前操作的所有id</param>
        /// <param name="tableKey">主键字段</param>
        /// <returns></returns>
        public static string GetMultiTenancySql(string tableName, string ids, string tableKey)
        {
            //使用方法同上
            string multiTenancyString;
            switch (tableName)
            {
                default:
                    multiTenancyString = $"select count(*) FROM {tableName} " +
                       $" where CreateID='{UserContext.Current.UserId}'" +
                       $" and  { tableKey} in ({ids}) ";
                    break;
            }
            return multiTenancyString;
        }
    }



}
