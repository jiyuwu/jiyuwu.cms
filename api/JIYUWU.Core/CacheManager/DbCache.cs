using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Entity.Base;

namespace JIYUWU.Core.CacheManager
{
    public static class DbCache
    {
        private static List<Base_DbService> DbServices = null;
        private static object _lock_sbcnew = new object();


        public static void Init()
        {
            DbServices = DbManger.BaseDbContext.Set<Base_DbService>().Select(s => new
            {
                s.Pwd,
                s.DbIpAddress,
                s.DatabaseName,
                s.DbServiceId,
                s.GroupId,
                s.UserId,
                s.Remark,
                s.PhoneNo,
                s.DbServiceName
            })
                .ToList()
                .Select(s => new Base_DbService()
                {
                    Pwd = s.Pwd,
                    DbIpAddress = s.DbIpAddress,
                    DatabaseName = s.DatabaseName,
                    DbServiceName = s.DbServiceName,
                    DbServiceId = s.DbServiceId,
                    GroupId = s.GroupId,
                    UserId = s.UserId,
                    Remark = s.Remark,
                    PhoneNo = s.PhoneNo
                }).ToList();
            InitConnection();


        }
        public static List<Base_DbService> GetList()
        {
            return DbServices;
        }

        public static WebResponseContent Reload(WebResponseContent webResponse)
        {
            if (webResponse.Status)
            {
                Init();
            }
            return webResponse;
        }

        public static void InitConnection()
        {
            foreach (var item in DbServices)
            {
                InitConnection(item);
            }
        }

        public static string InitConnection(Base_DbService item, string databaseName = null)
        {
            string connectionString = GetConnectionString(item, databaseName);

            if (databaseName == null)
            {
                DbServerProvider.SetConnection(item.DbServiceId.ToString(), connectionString);
            }
            return connectionString;
        }

        public static string GetConnectionString(Base_DbService item, string databaseName = null)
        {
            string connectionString = null;
            switch (DBType.Name)
            {
                //mysql如果端口不是3306，这里也需要修改
                case "MySql":
                    connectionString = @$" Data Source={item.DbIpAddress};Database={databaseName ?? item.DatabaseName};AllowLoadLocalInfile=true;User ID={item.UserId};Password={item.Pwd};allowPublicKeyRetrieval=true;pooling=true;CharSet=utf8;port=3306;sslmode=none;";
                    break;
                case "PgSql":
                    connectionString = $"Host={item.DbIpAddress};Port=5432;User id={item.UserId};password={item.Pwd};Database={databaseName ?? item.DatabaseName};";

                    break;
                case "MsSql":
                    connectionString = @$"Data Source={item.DbIpAddress};Initial Catalog={databaseName ?? item.DatabaseName};Persist Security Info=True;User ID={item.UserId};Password={item.Pwd};Connect Timeout=500;Max Pool Size = 512;";

                    break;
                case "DM":
                    // 老版本 ：PORT=5236;DATABASE=DAMENG;HOST=localhost;PASSWORD=SYSDBA;USER ID=SYSDBA
                    //新版本： Server=localhost; User Id=SYSDBA; PWD=SYSDBA;DATABASE=新DB
                    connectionString = $" Server={item.DbIpAddress}; User Id={item.UserId}; PWD={item.Pwd};DATABASE={databaseName ?? item.DatabaseName}";
                    break;
                case "Oracle":
                    Console.WriteLine($"未实现数据库：{DBType.Name}");
                    break;
            }
            return connectionString;
        }


        public static Base_DbService GetDbInfo(Guid dbServiceId)
        {
            return DbServices.Where(x => x.DbServiceId == dbServiceId).FirstOrDefault();
        }

        public static IEnumerable<Base_DbService> GetDbInfo(Func<Base_DbService, bool> where)
        {
            return DbServices.Where(where);
        }


    }
}
