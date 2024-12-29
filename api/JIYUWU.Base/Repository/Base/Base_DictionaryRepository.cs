using JIYUWU.Base.IRepository;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;
using JIYUWU.Entity.Sys;

namespace JIYUWU.Base.Repository.Base
{
    public class Base_DictionaryRepository : RepositoryBase<Base_Dictionary>, IBase_DictionaryRepository
    {
        public Base_DictionaryRepository(BaseDbContext dbContext)
        : base(dbContext)
        {
        }

        public static IBase_DictionaryRepository Instance
        {
            get { return AutofacContainerModule.GetService<IBase_DictionaryRepository>(); }
        }
    }
}
