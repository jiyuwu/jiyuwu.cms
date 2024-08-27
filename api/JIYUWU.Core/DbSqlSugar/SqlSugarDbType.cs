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
                default:
                    break;
            }
            return dbType;
        }
    }
}
