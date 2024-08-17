using JIYUWU.Api.Controllers.Hubs;
using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Base.Repository;
using JIYUWU.Core.CacheManager;
using JIYUWU.Core.Common;
using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using JIYUWU.Core.Filter;
using JIYUWU.Core.ObjectActionValidator;
using JIYUWU.Core.UserManager;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace JIYUWU.Api.Controllers
{
    [Route("api/User")]
    public partial class Base_UserController
    {
        private IBase_UserRepository _userRepository;
        private ICacheService _cache;
        [ActivatorUtilitiesConstructor]
        public Base_UserController(
               IBase_UserService userService,
               IBase_UserRepository userRepository,
               ICacheService cahce
              )
          : base(userService)
        {
            _userRepository = userRepository;
            _cache = cahce;
        }

        [HttpPost, HttpGet, Route("login"), AllowAnonymous]
        [ObjectModelValidatorFilter(ValidatorModel.Login)]
        public async Task<IActionResult> Login([FromBody] LoginInfo loginInfo)
        {
            return Json(await Service.Login(loginInfo));
        }

        private readonly ConcurrentDictionary<int, object> _lockCurrent = new ConcurrentDictionary<int, object>();
        [HttpPost, Route("replaceToken")]
        public IActionResult ReplaceToken()
        {
            WebResponseContent responseContent = new WebResponseContent();
            string error = "";
            string key = $"rp:Token:{UserContext.Current.UserId}";
            UserInfo userInfo = null;
            try
            {
                //如果5秒内替换过token,直接使用最新的token(防止一个页面多个并发请求同时替换token导致token错位)
                if (_cache.Exists(key))
                {
                    return Json(responseContent.OK(null, _cache.Get(key)));
                }
                var _obj = _lockCurrent.GetOrAdd(UserContext.Current.UserId, new object() { });
                lock (_obj)
                {
                    if (_cache.Exists(key))
                    {
                        return Json(responseContent.OK(null, _cache.Get(key)));
                    }
                    string requestToken = HttpContext.Request.Headers[AppSetting.TokenHeaderName];
                    requestToken = requestToken?.Replace("Bearer ", "");

                    if (JwtHelper.IsExp(requestToken)) return Json(responseContent.Error("Token已过期!"));

                    int userId = UserContext.Current.UserId;

                    userInfo = _userRepository.FindAsIQueryable(x => x.User_Id == userId).Select(
                             s => new UserInfo()
                             {
                                 User_Id = userId,
                                 UserName = s.UserName,
                                 UserTrueName = s.UserTrueName,
                                 Role_Id = s.Role_Id ?? 0,
                                 //   RoleName = s.RoleName
                             }).FirstOrDefault();

                    if (userInfo == null) return Json(responseContent.Error("未查到用户信息!"));

                    int expir = UserContext.MenuType == 1 ? 43200 : AppSetting.ExpMinutes;
                    string token = JwtHelper.IssueJwt(userInfo, expir);
                    //移除当前缓存
                    _cache.Remove(userId.GetUserIdKey());
                    //只更新的token字段
                    _userRepository.Update(new Base_User() { User_Id = userId, Token = token }, x => x.Token, true);
                    //添加一个5秒缓存
                    _cache.Add(key, token, 5);
                    string accessToken = null;
                    if (AppSetting.FileAuth)
                    {
                        expir = expir + 30;
                        string dt = DateTime.Now.AddMinutes(expir).ToString("yyyy-MM-dd HH:mm");
                        accessToken = $"{userId}_{dt}".EncryptDES(AppSetting.Secret.User);
                        _cache.Add(accessToken, dt, expir);
                        responseContent.OK(null, new { accessToken, token });
                    }
                    else
                    {
                        responseContent.OK(null, token);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message + ex.StackTrace;
                responseContent.Error("token替换异常");
            }
            finally
            {
                _lockCurrent.TryRemove(UserContext.Current.UserId, out object val);
                string _message = $"用户{userInfo?.User_Id}_{userInfo?.UserTrueName},({(responseContent.Status ? "token替换成功" : "token替换失败")})";
                Logger.Info(LoggerType.ReplaceToeken, _message, null, error);
            }
            return Json(responseContent);
        }


        [HttpPost, Route("modifyPwd")]
        [ApiActionPermission]
        //通过ObjectGeneralValidatorFilter校验参数，不再需要if esle判断OldPwd与NewPwd参数
        [ObjectGeneralValidatorFilter(ValidatorGeneral.OldPwd, ValidatorGeneral.NewPwd)]
        public async Task<IActionResult> ModifyPwd(string oldPwd, string newPwd)
        {
            return Json(await Service.ModifyPwd(oldPwd, newPwd));
        }

        [HttpPost, Route("getCurrentUserInfo")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            return Json(await Service.GetCurrentUserInfo());
        }

        //只能超级管理员才能修改密码
        //2020.08.01增加修改密码功能
        //[HttpPost, Route("modifyUserPwd"), ApiActionPermission(ActionRolePermission.SuperAdmin)]
        [HttpPost, Route("modifyUserPwd"), ApiActionPermission(ActionPermissionOptions.Add | ActionPermissionOptions.Update)]
        public IActionResult ModifyUserPwd(string password, string userName)
        {
            WebResponseContent webResponse = new WebResponseContent();
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userName))
            {
                return Json(webResponse.Error("参数不完整"));
            }
            if (password.Length < 6) return Json(webResponse.Error("密码长度不能少于6位"));

            IBase_UserRepository repository = Base_UserRepository.Instance;
            Base_User user = repository.FindFirst(x => x.UserName == userName);
            if (user == null)
            {
                return Json(webResponse.Error("用户不存在"));
            }
            user.UserPwd = password.EncryptDES(AppSetting.Secret.User);
            repository.Update(user, x => new { x.UserPwd }, true);
            //如果用户在线，强制下线
            UserContext.Current.LogOut(user.User_Id);
            return Json(webResponse.OK("密码修改成功"));
        }

        /// <summary>
        /// 2020.06.15增加登陆验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("getVierificationCode"), AllowAnonymous]
        public IActionResult GetVierificationCode()
        {
            string code = VierificationCode.RandomText();
            var data = new
            {
                img = VierificationCode.CreateBase64Imgage(code),
                uuid = Guid.NewGuid()
            };
            HttpContext.GetService<IMemoryCache>().Set(data.uuid.ToString(), code, new TimeSpan(0, 5, 0));
            return Json(data);
        }
        [ApiActionPermission()]
        public override IActionResult Upload(IEnumerable<IFormFile> fileInput)
        {
            return base.Upload(fileInput);
        }
        [HttpPost, Route("updateUserInfo")]
        public IActionResult UpdateUserInfo([FromBody] Base_User user)
        {
            user.User_Id = UserContext.Current.UserId;

            _userRepository.Update(user, x => new { x.UserTrueName, x.Gender, x.Remark, x.HeadImageUrl }, true);
            return Content("修改成功");
        }

        public override ActionResult GetPageData([FromBody] PageDataOptions loadData)
        {
            PageGridData<Base_User> gridData = Service.GetPageData(loadData);
            //是否使用用户数据权限
            gridData.extra = AppSetting.UserAuth;
            //设置用户是否在线
            foreach (var item in gridData.rows)
            {
                item.IsOnline = UserCache.GetOnline(item.UserName);
            }
            return JsonNormal(gridData);
        }
        [HttpPost, Route("getUserAuth"), ApiActionPermission(ActionPermissionOptions.Add | ActionPermissionOptions.Update)]
        public async Task<IActionResult> GetUserAuth(int userId)
        {
            var data = await _userRepository.DbContext.Set<Base_UserAuth>().Where(x => x.UserId == userId && x.AuthUserIds != "")
                 .Select(s => new { id = s.MenuId, userIds = s.AuthUserIds })
                 .ToListAsync();

            var userIds = data.Select(s => s.userIds.Split(",")).SelectMany(x => x).Select(s => s.GetInt()).Distinct();

            var users = await _userRepository.FindAsIQueryable(x => userIds.Contains(x.User_Id))
                  .Select(s => new { userId = s.User_Id, userName = s.UserTrueName })
                  .ToListAsync();

            return Json(new { data, users });
        }

        [HttpPost, Route("saveUserAuth"), ApiActionPermission(ActionPermissionOptions.Add | ActionPermissionOptions.Update)]
        public async Task<IActionResult> SaveUserAuth([FromBody] List<Base_UserAuth> userAuths, int userId)
        {
            if (userAuths == null || userId < 0)
            {
                return Content("参数不完整");
            }
            var query = _userRepository.DbContext.Set<Base_UserAuth>();
            var data = await query.Where(x => x.UserId == userId).ToListAsync();

            List<Base_UserAuth> add = new List<Base_UserAuth>();
            List<Base_UserAuth> update = new List<Base_UserAuth>();

            foreach (var item in userAuths)
            {
                var auth = data.Where(x => item.MenuId == x.MenuId).FirstOrDefault();
                if (auth == null)
                {
                    if (!string.IsNullOrEmpty(item.AuthUserIds))
                    {
                        item.UserId = userId;
                        add.Add(item);
                    }
                }
                else
                {
                    item.Id = auth.Id;
                    update.Add(item);
                }
            }
            _userRepository.AddRange(add);
            _userRepository.UpdateRange(update, x => new { x.AuthUserIds }, true);

            UserContext.Current.RemoveUserAuthData(userId);
            return Content("保存成功");
        }
    }
}
