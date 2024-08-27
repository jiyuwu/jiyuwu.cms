using JIYUWU.Core.Extension;

namespace JIYUWU.Core.DbSqlSugar
{
    public class BaseDbContext : MyDbContext, IDependency
    {
        public BaseDbContext() : base()
        {
            base.SqlSugarClient = DbManger.BaseDbContext;
        }
    }
}
