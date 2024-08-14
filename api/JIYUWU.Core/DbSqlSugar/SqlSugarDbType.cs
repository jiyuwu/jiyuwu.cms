using SqlSugar;

namespace JIYUWU.Core.DbSqlSugar
{
    public static class SqlSugarDbType
    {
        public static DbType GetType(string dbContextName, DbType dbType)
        {
            //配置连接不同的数据库类型，比如同时使用mysql、sqlserver、pgsql数据库
            switch (dbContextName)
            {
                ////这里case填写appsettings.json中Connection节点下一样的名称，根据不同的配置使用不同的数据库
                //case "ServiceDbContext":
                //    dbType = DbType.MySql;
                //   break;
                default:
                    break;
            }
            return dbType;
        }
    }
}
