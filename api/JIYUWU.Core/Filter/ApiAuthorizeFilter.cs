using JIYUWU.Core.Common;
using JIYUWU.Core.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace JIYUWU.Core.Filter
{
    public class ApiAuthorizeFilter : IAuthorizationFilter
    {

        public ApiAuthorizeFilter()
        {

        }
        /// <summary>
        /// 只判断token是否正确，不判断权限
        /// 如果需要判断权限的在Action上加上ApiActionPermission属性标识权限类别，ActionPermissionFilter作权限处理
        ///(string,string,string)1、请求参数,2、返回消息，3,异常消息,4状态
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.Any(item => item is IAllowAnonymous))
            {
                if (context.Filters
                    .Where(item => item is IApiTaskFilter)
                    .FirstOrDefault() is IApiTaskFilter apiTaskFilter) 
                {
                    apiTaskFilter.OnAuthorization(context);
                    return;
                }
                //如果使用了固定Token不过期，直接对token的合法性及token是否存在进行验证
                else if (context.Filters
                    .Where(item => item is IFixedTokenFilter)
                    .FirstOrDefault() is IFixedTokenFilter tokenFilter)
                {
                    tokenFilter.OnAuthorization(context);
                    return;
                }
                //匿名并传入了token，需要将用户的ID缓存起来，保证UserHelper里能正确获取到用户信息
                if (!context.HttpContext.User.Identity.IsAuthenticated
                    && !string.IsNullOrEmpty(context.HttpContext.Request.Headers[AppSetting.TokenHeaderName]))
                {
                    context.AddIdentity();
                }
                return;
            }
            //限定一个帐号不能在多处登陆   UserContext.Current.Token != ((ClaimsIdentity)context.HttpContext.User.Identity)?.BootstrapContext?.ToString()

            DateTime expDate = context.HttpContext.User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Exp)
                .Select(x => x.Value).FirstOrDefault().GetTimeSpmpToDate();
            //动态标识刷新token
            if ((expDate - DateTime.Now).TotalMinutes < AppSetting.ExpMinutes/ 3 && context.HttpContext.Request.Path != replaceTokenPath)
            {
                context.HttpContext.Response.Headers.Add("jiyuwu_exp", "1");
            }
        }
        private static readonly string replaceTokenPath = "/api/User/replaceToken";
    }
}
