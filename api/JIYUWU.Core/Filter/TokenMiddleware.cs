using JIYUWU.Core.CacheManager;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JIYUWU.Core.Filter
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenService _tokenService;
        private readonly ICacheService _cacheService;

        public TokenMiddleware(RequestDelegate next, TokenService tokenService, ICacheService cacheService)
        {
            _next = next;
            _tokenService = tokenService;
            _cacheService = cacheService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // 获取请求的 Endpoint
            var endpoint = httpContext.GetEndpoint();

            // 检查是否存在 NoPermissionRequired 特性
            var noPermissionRequired = endpoint?.Metadata.GetMetadata<NoPermissionRequiredAttribute>() != null;

            // 如果标记了 NoPermissionRequired 特性，则跳过 Token 验证
            if (noPermissionRequired)
            {
                await _next(httpContext);
                return;
            }

            var token = httpContext.Request.Headers["Auth-Token"].FirstOrDefault()?.Split(" ").Last(); // 获取 Auth-Token

            if (string.IsNullOrEmpty(token) || !_tokenService.ValidateToken(token, _cacheService)) // 如果没有 Token 或者 Token 无效
            {
                // 设置响应状态码为200
                httpContext.Response.StatusCode = StatusCodes.Status200OK;

                // 设置响应内容为JSON格式
                httpContext.Response.ContentType = "application/json";

                // 返回JSON格式的响应
                var response = new
                {
                    code = 401, // 可以设置为401的错误码
                    msg = "Unauthorized"
                };
                await httpContext.Response.WriteAsJsonAsync(response);
                return;
            }

            // 如果 Token 有效，继续执行
            await _next(httpContext);
        }
    }
    public class TokenService
    {
        private const string _cacheUserKey = "Info";
        public bool ValidateToken(string token, ICacheService _cacheService)
        {
            // 这里是一个简单的示例，你可以根据需要使用 JWT 验证或者其他方式
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            string key = $"{_cacheUserKey}_{token}";
            var user = _cacheService.Get<Base_User>(key);
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    //不需要验证方法权限

    public class NoPermissionRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

    }
}
