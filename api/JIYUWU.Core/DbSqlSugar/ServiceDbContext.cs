using JIYUWU.Core.Extension;

namespace JIYUWU.Core.DbSqlSugar
{
    public class ServiceDbContext : BaseDbContext, IDependency
    {
        private string dbServiceId { get; set; }

        public ServiceDbContext() : base()
        {
            this.dbServiceId = dbServiceId;
            base.SqlSugarClient = DbManger.ServiceDb;
        }


        public ServiceDbContext(string dbServiceId) : base()
        {
            this.dbServiceId = dbServiceId;
            base.SqlSugarClient = DbManger.ServiceDb;
        }
    }
}
