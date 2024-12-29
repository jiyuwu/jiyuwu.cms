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
    public class Base_CompanyService
        : ServiceBase<Base_Company, IBase_CompanyRepository>
    , IBase_CompanyService, IDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBase_CompanyRepository _repository; //访问数据库

        public Base_CompanyService(IBase_CompanyRepository repository,
            IHttpContextAccessor httpContextAccessor)
        : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            Init(repository);
        }

        public static IBase_CompanyService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_CompanyService>(); }
        }
    }
}
