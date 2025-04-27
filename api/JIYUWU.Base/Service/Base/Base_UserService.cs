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
        private readonly ICacheService _cacheService; //缓存
        private const string _cacheUserKey = "Info";

        public Base_UserService(IBase_UserRepository repository,ICacheService cacheService,
            IHttpContextAccessor httpContextAccessor)
        : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            _cacheService=cacheService;
            Init(repository);
        }

        public static IBase_UserService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_UserService>(); }
        }
        #region 用户信息缓存
        public bool SetUserInfo(Base_User user, string token)
        {
            if (user == null)
            {
                return false;
            }
            bool result =_cacheService.AddObject($"{_cacheUserKey}_{token}", user, 24 * 60 * 60);
            return result;
        }

        public Base_User GetUserInfoByToken(string token)
        {
            string key = $"{_cacheUserKey}_{token}";
            var user = _cacheService.Get<Base_User>(key);
            return user;
        }
        public bool RemoveUserInfo(string token)
        {
            string key = $"{_cacheUserKey}_{token}";
            bool result = _cacheService.Remove(key);
            return result;
        }
        #endregion
    }
}
