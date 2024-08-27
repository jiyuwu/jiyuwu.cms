using JIYUWU.Core.DbSqlSugar;
using JIYUWU.Core.Extension;
using Microsoft.Extensions.Primitives;
using SqlSugar;
using System.Collections.Concurrent;
using System.Security.Claims;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Routing;

namespace JIYUWU.Core.UserManager
{
    public class UserContext
    {
        /// <summary>
        /// 为了尽量减少redis或Memory读取,保证执行效率,将UserContext注入到DI，
        /// 每个UserContext的属性至多读取一次redis或Memory缓存从而提高查询效率
        /// </summary>
        public static UserContext Current
        {
            get
            {
                return Context.RequestServices.GetService(typeof(UserContext)) as UserContext;
            }
        }

        private static Microsoft.AspNetCore.Http.HttpContext Context
        {
            get
            {
                return Common.HttpContext.Current;
            }
        }
        private static ICacheService CacheService
        {
            get { return GetService<ICacheService>(); }
        }

        private static T GetService<T>() where T : class
        {
            return AutofacContainerModule.GetService<T>();
        }
        public int UserId
        {
            get
            {
                //return (Context.User.FindFirstValue(JwtRegisteredClaimNames.Jti)
                //    ??
                //    Context.User.FindFirstValue(ClaimTypes.NameIdentifier)).GetInt();
                return 1;
            }
        }
        public UserInfo UserInfo
        {
            get
            {
                if (_userInfo != null)
                {
                    return _userInfo;
                }
                return GetUserInfo(UserId);
            }
        }

        private UserInfo _userInfo { get; set; }
        /// <summary>
        /// 角色ID为1的默认为超级管理员
        /// </summary>
        public static bool IsRoleIdSuperAdmin(int[] roleIds)
        {
            return roleIds.Contains(1);
        }

        public static bool IsRoleIdSuperAdmin(int roleId)
        {
            return roleId == 1;
        }

        public UserInfo GetUserInfo(int userId)
        {
            if (_userInfo != null) return _userInfo;
            if (userId <= 0)
            {
                _userInfo = new UserInfo() { RoleIds = new int[] { } };
                return _userInfo;
            }
            //string key = userId.GetUserIdKey();
            //_userInfo = CacheService.Get<UserInfo>(key);
            //if (_userInfo != null && _userInfo.User_Id > 0) return _userInfo;

            //_userInfo = DbServerProvider.DbContext.Set<Base_User>()
            //    .Where(x => x.User_Id == userId).Select(s => new
            //    {
            //        User_Id = userId,
            //        Role_Id = s.Role_Id ?? 0,
            //        Token = s.Token,
            //        UserName = s.UserName,
            //        UserTrueName = s.UserTrueName,
            //        Enable = 1,
            //        s.RoleIds,
            //        s.DeptIds
            //    }).ToList().Select(s => new UserInfo()
            //    {
            //        User_Id = userId,
            //        Role_Id = s.Role_Id,
            //        Token = s.Token,
            //        UserName = s.UserName,
            //        UserTrueName = s.UserTrueName,
            //        Enable = 1,
            //        RoleIds = s.Role_Id == 1 ? new int[] { 1 } : (string.IsNullOrEmpty(s.RoleIds) ? new int[] { } : s.RoleIds.Split(",").Select(x => x.GetInt()).ToArray()),
            //        DeptIds = string.IsNullOrEmpty(s.DeptIds) ? new List<Guid>() : s.DeptIds.Split(",").Select(x => (Guid)x.GetGuid()).ToList(),
            //        TenancyValue = null //用户租户字段请在此处返回实现
            //    }).FirstOrDefault();
            return _userInfo ?? new UserInfo() { RoleIds = new int[] { } };
        }
        /// <summary>
        /// 当前选中的数据库
        /// </summary>
        public static Guid CurrentServiceId
        {
            get
            {
                if (Context.Request.Headers.TryGetValue("serviceId", out StringValues value))
                {
                    var val = value.GetGuid() ?? Guid.NewGuid();
                    //if (Current.IsSuperAdmin)
                    //{
                    //    return val;
                    //}
                    //var roleIds = Current.RoleIds;
                    //if (RoleContext.GetRoles(x => roleIds.Contains(x.Id)).Any(x => x.DbServiceId == val))
                    //{
                    //    return val;
                    //}
                }
                return Guid.Empty;
            }
        }
    }
}
