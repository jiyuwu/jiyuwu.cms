using JIYUWU.Base.IRepository;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;
namespace JIYUWU.Base.Repository
{
    public class Base_UserRepository : RepositoryBase<Base_User>, IBase_UserRepository
    {
        public Base_UserRepository(BaseDbContext dbContext)
        : base(dbContext)
        {

        }
        public static IBase_UserRepository Instance
        {
            get { return AutofacContainerModule.GetService<IBase_UserRepository>(); }
        }
    }
}
