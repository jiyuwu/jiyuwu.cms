using JIYUWU.Base.IRepository;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Repository.Base
{
    public class Base_DepartmentRepository : RepositoryBase<Base_Department>, IBase_DepartmentRepository
    {
        public Base_DepartmentRepository(BaseDbContext dbContext)
        : base(dbContext)
        {
        }

        public static IBase_DepartmentRepository Instance
        {
            get { return AutofacContainerModule.GetService<IBase_DepartmentRepository>(); }
        }
    }
}
