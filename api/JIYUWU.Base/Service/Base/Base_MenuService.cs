using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.CacheManager;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Http;

namespace JIYUWU.Base.Service
{
    public class Base_MenuService
        : ServiceBase<Base_Menu, IBase_MenuRepository>
    , IBase_MenuService, IDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBase_MenuRepository _repository; //访问数据库

        public Base_MenuService(IBase_MenuRepository repository,
            IHttpContextAccessor httpContextAccessor)
        : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            Init(repository);
        }

        public static IBase_MenuService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_MenuService>(); }
        }
    }
}
