using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.CacheManager;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;
using JIYUWU.Entity.Sys;
using Microsoft.AspNetCore.Http;

namespace JIYUWU.Base.Service
{
    public class Base_DictionaryService
        : ServiceBase<Base_Dictionary, IBase_DictionaryRepository>
    , IBase_DictionaryService, IDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBase_DictionaryRepository _repository; //访问数据库

        public Base_DictionaryService(IBase_DictionaryRepository repository,
            IHttpContextAccessor httpContextAccessor)
        : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            Init(repository);
        }

        public static IBase_DictionaryService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_DictionaryService>(); }
        }
    }
}
