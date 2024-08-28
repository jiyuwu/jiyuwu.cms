using JIYUWU.Base.IRepository;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Repository.Base
{
    public class Base_DbServiceRepository : RepositoryBase<Base_DbService>, IBase_DbServiceRepository
    {
        public Base_DbServiceRepository(BaseDbContext dbContext)
        : base(dbContext)
        {

        }
        public static IBase_DbServiceRepository Instance
        {
            get { return AutofacContainerModule.GetService<IBase_DbServiceRepository>(); }
        }
    }
}
