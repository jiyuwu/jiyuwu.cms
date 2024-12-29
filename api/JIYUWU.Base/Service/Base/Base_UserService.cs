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
    public class Base_UserService
        : ServiceBase<Base_User, IBase_UserRepository>
    , IBase_UserService, IDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBase_UserRepository _repository; //访问数据库

        public Base_UserService(IBase_UserRepository repository,
            IHttpContextAccessor httpContextAccessor)
        : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            Init(repository);
        }

        public static IBase_UserService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_UserService>(); }
        }
    }
}
