using JIYUWU.Core.Common;
using JIYUWU.Core.Filter;
using JIYUWU.Core.Language;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UMES.WebApi.Controllers.Hubs;

namespace JIYUWU.Api.Controllers.Hubs
{
    [Route("api/signalRUser")]
    public class SignalRUserController : BaseController
    {
        IHubContext<HomePageMessageHub> _hubClients;
        public SignalRUserController(IHubContext<HomePageMessageHub> hubClients)
        {
            _hubClients = hubClients;
        }
        /// <summary>
        /// 强制下线
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet, Route("loginout")]
        [ApiActionPermission((nameof(Base_User)), ActionPermissionOptions.Add | ActionPermissionOptions.Update)]
        public async Task<IActionResult> Loginout(string userName)
        {
            await _hubClients.Clients.Clients(UserCache.GetCnnectionIds(userName)).SendAsync("ReceiveHomePageMessage", new
            {
                msg = "您已被强制下线,即将自动退出登录...".Translator(),
                code = "-1",
                value="logout",
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")
            });
            return Content("操作成功".Translator());
        }
    }
}
