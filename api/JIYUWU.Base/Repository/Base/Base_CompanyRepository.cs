using JIYUWU.Base.IRepository;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Repository.Base
{
    public class Base_CompanyRepository : RepositoryBase<Base_Company>, IBase_CompanyRepository
    {
        public Base_CompanyRepository(BaseDbContext dbContext)
        : base(dbContext)
        {
        }

        public static IBase_CompanyRepository Instance
        {
            get { return AutofacContainerModule.GetService<IBase_CompanyRepository>(); }
        }
    }
}
