using JIYUWU.Base.IRepository;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Repository.Base
{
    public class Base_PostRepository : RepositoryBase<Base_Post>, IBase_PostRepository
    {
        public Base_PostRepository(BaseDbContext dbContext)
        : base(dbContext)
        {
        }

        public static IBase_PostRepository Instance
        {
            get { return AutofacContainerModule.GetService<IBase_PostRepository>(); }
        }
    }
}
