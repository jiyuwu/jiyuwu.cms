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
    public class Base_DepartmentService
        : ServiceBase<Base_Department, IBase_DepartmentRepository>
    , IBase_DepartmentService, IDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBase_DepartmentRepository _repository; //访问数据库

        public Base_DepartmentService(IBase_DepartmentRepository repository,
            IHttpContextAccessor httpContextAccessor)
        : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            Init(repository);
        }

        public static IBase_DepartmentService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_DepartmentService>(); }
        }
    }
}
