using JIYUWU.Base.IRepository;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Repository.Base
{
    public class Base_MenuRepository : RepositoryBase<Base_Menu>, IBase_MenuRepository
    {
        public Base_MenuRepository(BaseDbContext dbContext)
        : base(dbContext)
        {

        }

        public static IBase_MenuRepository Instance
        {
            get { return AutofacContainerModule.GetService<IBase_MenuRepository>(); }
        }
    }
}
