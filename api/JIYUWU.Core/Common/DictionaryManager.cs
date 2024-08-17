using JIYUWU.Core.CacheManager;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Core.UserManager;
using JIYUWU.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Common
{
    public static class DictionaryManager
    {
        private static List<Base_Dictionary> _dictionaries { get; set; }

        private static object _dicObj = new object();
        private static string _dicVersionn = "";
        public const string Key = "inernalDic";

        public static List<Base_Dictionary> Dictionaries
        {
            get
            {
                return GetAllDictionary();
            }
        }

        public static Base_Dictionary GetDictionary(string dicNo)
        {
            return GetDictionaries(new string[] { dicNo }).FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicNos"></param>
        /// <param name="executeSql">是否执行自定义sql</param>
        /// <returns></returns>
        public static List<Base_Dictionary> GetDictionaries(IEnumerable<string> dicNos, bool executeSql = true)
        {
            static List<Base_DictionaryList> query(string sql, string DBServer)
            {
                return DbManger.GetConnection(DBServer).QueryList<SourceKeyVaule>(sql, null).Select(s => new Base_DictionaryList()
                {
                    DicName = s.Value,
                    DicValue = s.Key.ToString()
                }).ToList();

            }
            List<Base_Dictionary> dictionaries = new List<Base_Dictionary>();
            foreach (var item in Dictionaries.Where(x => dicNos.Contains(x.DicNo)))
            {
                if (executeSql)
                {
                    //  2020.05.01增加根据用户信息加载字典数据源sql
                    string sql = DictionaryHandler.GetCustomDBSql(item.DicNo, item.DbSql);
                    if (!string.IsNullOrEmpty(item.DbSql))
                    {
                        item.Base_DictionaryList = query(sql, item.DBServer);
                    }
                }
                dictionaries.Add(item);
            }
            return dictionaries;
        }
        /// <summary>
        /// 每次变更字典配置的时候会重新拉取所有配置进行缓存(自行根据实际处理)
        /// </summary>
        /// <returns></returns>
        private static List<Base_Dictionary> GetAllDictionary()
        {
            ICacheService cacheService = AutofacContainerModule.GetService<ICacheService>();
            //每次比较缓存是否更新过，如果更新则重新获取数据
            if (_dictionaries != null && _dicVersionn == cacheService.Get(Key))
            {
                return _dictionaries;
            }

            lock (_dicObj)
            {
                if (_dicVersionn != "" && _dictionaries != null && _dicVersionn == cacheService.Get(Key)) return _dictionaries;
                _dictionaries = DbServerProvider.DbContext
                    .Set<Base_Dictionary>()
                    .Includes(c => c.Base_DictionaryList)
                    .Where(x => x.Enable == 1)
                    .ToList();

                string cacheVersion = cacheService.Get(Key);
                if (string.IsNullOrEmpty(cacheVersion))
                {
                    cacheVersion = DateTime.Now.ToString("yyyyMMddHHMMssfff");
                    cacheService.Add(Key, cacheVersion);
                }
                else
                {
                    _dicVersionn = cacheVersion;
                }
            }
            return _dictionaries;
        }
    }

    public class SourceKeyVaule
    {
        public object Key { get; set; }
        public string Value { get; set; }
    }
    public static class DictionaryHandler
    {
        /*2020.05.01增加根据用户信息加载字典数据源sql*/

        /// <summary>
        /// 获取自定义数据源sql
        /// </summary>
        /// <param name="dicNo"></param>
        /// <param name="originalSql"></param>
        /// <returns></returns>
        public static string GetCustomDBSql(string dicNo, string originalSql)
        {
            switch (dicNo)
            {
                case "部门级联":
                    originalSql = GetDeptSql(originalSql);
                    break;
                //2020.05.24增加绑定table表时，获取所有的角色列表
                //注意，如果是2020.05.24之前获取的数据库脚本
                //请在菜单【下拉框绑定设置】添加一个字典编号【t_roles】,除了字典编号，其他内容随便填写
                case "roles":
                case "t_roles":
                case "tree_roles":
                    originalSql = GetRolesSql(originalSql);
                    break;
                case "集团":

                    if (IsPgSql)
                    {
                        originalSql = "SELECT \"GroupId\" AS key,\"GroupName\" AS value FROM  PUBLIC.\"Sys_Group\"";
                    }
                    break;
                default:
                    break;
            }
            return originalSql;
        }

        private static bool IsPgSql
        {
            get { return DBType.Name == DbCurrentType.PgSql.ToString() || DBType.Name == DbCurrentType.Kdbndp.ToString(); }
        }
        private static bool IsOracle
        {
            get { return DBType.Name == DbCurrentType.Oracle.ToString(); }
        }

        /// <summary>
        /// 获取解决的数据源，只能看到自己与下级所有角色
        /// </summary>
        /// <param name="context"></param>
        /// <param name="originalSql"></param>
        /// <returns></returns>
        public static string GetRolesSql(string originalSql)
        {
            if (IsPgSql)
            {
                return GetRolesPgSql(originalSql);
            }
            originalSql = "SELECT Role_Id AS id,parentId,Role_Id AS  'key',RoleName AS value FROM Sys_Role where 1=1 ";
            if (UserContext.Current.IsSuperAdmin)
            {
                return originalSql;
            }

            var roleIds = UserContext.Current.GetAllChildrenRoleIds();
            string sql = $@" {originalSql}  and  Role_Id in ({string.Join(',', roleIds)})";
            return sql;
        }

        public static string GetRolesPgSql(string originalSql)
        {
            originalSql = "SELECT \"Role_Id\" AS id,\"ParentId\" as  \"parentId\",\"Role_Id\" AS  key,\"RoleName\" AS value FROM \"public\".\"Sys_Role\" where 1=1 ";
            if (UserContext.Current.IsSuperAdmin)
            {
                return originalSql;
            }

            var roleIds = UserContext.Current.GetAllChildrenRoleIds();
            string sql = $" {originalSql}  and  \"Role_Id\" in ({string.Join(',', roleIds)})";
            return sql;
        }

        /// <summary>
        /// 部门数据源
        /// </summary>
        /// <param name="originalSql"></param>
        /// <returns></returns>
        public static string GetDeptSql(string originalSql)
        {
            if (IsPgSql)
            {
                originalSql = "SELECT \"DepartmentId\" AS id,\"DepartmentId\" AS key,\"ParentId\" AS parentId,\"DepartmentName\" AS value FROM PUBLIC.\"Sys_Department\" ";
            }
            else if (IsOracle)
            {
                originalSql = "SELECT DEPARTMENTID AS \"id\",DEPARTMENTID AS \"key\",PARENTID as \"parentId\",DEPARTMENTNAME as \"value\" FROM SYS_DEPARTMENT ";
            }
            else
            {
                originalSql = "SELECT DepartmentId AS id,DepartmentId AS 'key',ParentId AS parentId,DepartmentName AS value FROM Sys_Department";
            }
            if (UserContext.Current.IsSuperAdmin)
            {
                return originalSql;
            }
            var deptIds = UserContext.Current.DeptIds;
            deptIds = DepartmentContext.GetAllChildrenIds(deptIds);

            switch (DBType.Name)
            {
                //mysql如果端口不是3306，这里也需要修改
                case "MySql":
                    originalSql = $@"{originalSql}
                           WHERE DepartmentId in ('{string.Join("','", deptIds)}')";
                    break;
                case "PgSql":
                    originalSql = $"{originalSql}  WHERE \"DepartmentId\" in ('{string.Join("','", deptIds)}')";
                    break;
                case "MsSql":
                    originalSql = $@"{originalSql}
                           WHERE DepartmentId in ('{string.Join("','", deptIds)}')";
                    break;
                case "Oracle":
                    originalSql = $@"{originalSql}
                           WHERE DEPARTMENTID in ('{string.Join("','", deptIds)}')";
                    break;
            }
            return originalSql;
        }

    }
}
