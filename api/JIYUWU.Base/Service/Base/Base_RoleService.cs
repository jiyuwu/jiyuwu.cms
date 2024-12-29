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
    public class Base_RoleService
        : ServiceBase<Base_Role, IBase_RoleRepository>
    , IBase_RoleService, IDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBase_RoleRepository _repository; //访问数据库

        public Base_RoleService(IBase_RoleRepository repository,
            IHttpContextAccessor httpContextAccessor)
        : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            Init(repository);
        }

        public static IBase_RoleService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_RoleService>(); }
        }
    }
}
