using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.CacheManager;
using JIYUWU.Core.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JIYUWU.Api.Controllers
{
    [AllowAnonymous]
    public class ApiHomeController :Controller //ApiBaseController<IBase_UserService>
    {
        //private ICacheService _cache;
        //private IBase_UserRepository _userRepository;
        //private readonly IBase_MenuService _menuService;
        //public ApiHomeController(ICacheService cache, IBase_UserRepository userRepository, IBase_MenuService menuService)
        //{
        //    _cache = cache;
        //    _menuService = menuService;
        //    _userRepository = userRepository;
        //}

        [HttpPost,HttpGet, Route("TestApi")]
        public IActionResult TestApi(Demo_Log demo)
        {
            //_userRepository.SqlSugarClient.Queryable<Demo_Log>().Where(e=>e.LogId.ToString()== "5E349049-0526-4F0D-A853-4BBF95C28321").ToList();
            //return JsonNormal(demo);
            return Json(demo);
        }

    }
    public class Demo_Log
    {
        public Guid LogId { get; set; } = Guid.NewGuid();
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; }
        public int CreateID { get; set; }
        public string TableKey { get; set; }
        public string FlowId { get; set; }
    }
}
