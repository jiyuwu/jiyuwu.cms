using JIYUWU.Core.Common;
using JIYUWU.Core.Extension;
using JIYUWU.Core.UserManager;
using SqlSugar;
using System.Reflection;

namespace UMES.Core.Tenancy
{

    public static class TenancyDefault
    {
        /// <summary>
        /// 租户不分库时设置表的租户字段值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T SetTenancyValue<T>(this T entity) where T : class
        {
            PropertyInfo property = GetTenancyProperty<T>();
            if (property != null)
            {
                property.SetValue(entity, UserContext.CurrentServiceId.ToString());
            }
            return entity;
        }

        // 批量设置列表中所有实体的租户字段值
        public static List<T> SetTenancyValue<T>(this List<T> list) where T : class
        {
            PropertyInfo property = GetTenancyProperty<T>();
            if (property != null)
            {
                var tenancyId = UserContext.CurrentServiceId.ToString();
                foreach (var entity in list)
                {
                    property.SetValue(entity, tenancyId);
                }
            }
            return list;
        }
        /// <summary>
        /// 租户不分库时统一过滤当前租户表的数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static ISugarQueryable<T> FilterTenancy<T>(this ISugarQueryable<T> query) where T : class
        {
            PropertyInfo property = GetTenancyProperty<T>();
            if (property != null)
            {
                var where = property.Name.CreateExpression<T>(UserContext.CurrentServiceId.ToString(), LinqExpressionType.Equal);
                return query.Where(where);
            }
            return query;
        }

        // 获取租户字段属性信息
        private static PropertyInfo GetTenancyProperty<T>()
        {
            if (AppSetting.TenancyField == null)
            {
                return null;
            }
            return typeof(T).GetProperty(AppSetting.TenancyField);
        }

    }
}
