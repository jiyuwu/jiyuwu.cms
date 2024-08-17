using JIYUWU.Base.IRepository;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Repository.Base
{
    public class Base_RoleRepository : RepositoryBase<Base_Role>, IBase_RoleRepository
    {
        public Base_RoleRepository(BaseDbContext dbContext)
        : base(dbContext)
        {

        }
        public static IBase_RoleRepository Instance
        {
            get { return AutofacContainerModule.GetService<IBase_RoleRepository>(); }
        }
    }
}
