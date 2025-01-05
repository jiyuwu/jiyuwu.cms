using Microsoft.AspNetCore.Mvc;
using JIYUWU.Core.Common;
using JIYUWU.Base.IService;
using JIYUWU.Base.IRepository;
using JIYUWU.Core.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using JIYUWU.Entity.Base;

namespace JIYUWU.Api.Controllers.Base
{
    [Route("Base_Db")]
    public class Base_DbController : ApiBaseController<IBase_DbServiceService>
    {
        private IBase_DbServiceRepository _Repository;
        private IBase_DbServiceService _Service;
        [ActivatorUtilitiesConstructor]
        public Base_DbController(IBase_DbServiceService service,
                           IBase_DbServiceRepository Repository)
       : base(service)
        {
            _Repository = Repository;
            _Service=service;
        }
        [HttpGet, Route("getVierificationCode")]
        public IActionResult GetVierificationCode()
        {
            string code = "123";
            var data = new
            {
                img = "123",
                uuid = Guid.NewGuid()
            };
            var list=_Repository.SqlSugarClient.Queryable<Base_DbService>().ToList();
            HttpContext.GetService<IMemoryCache>().Set(data.uuid.ToString(), code, new TimeSpan(0, 5, 0));
            return Json(data);
        }
        [HttpPost, Route("AddDB")]
        public IActionResult AddDB()
        {
            Base_DbService service = new Base_DbService();
            service.DbServiceName = "test";
            service.DbServiceId=Guid.NewGuid().ToString();
            service.PhoneNo = "test";
            service.Pwd = "123";
            int count= _Service.Add(service);
            return Ok();
        }
        [HttpGet, Route("Dynamic")]
        public IActionResult Dynamic()
        {
            var data = new[]
{
    new
    {
        avatar = "data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%2260%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379f0f2%22%2F%3E%3Ctext%20x%3D%2230%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E7%A7%A6%3C%2Ftext%3E%3C%2Fsvg%3E",
        id = "710000199911202235",
        title = "党养已就青",
        datetime = "2022-07-14 21:08:18"
    },
    new
    {
        avatar = "data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%2260%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f2d179%22%2F%3E%3Ctext%20x%3D%2230%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E7%99%BD%3C%2Ftext%3E%3C%2Fsvg%3E",
        id = "500000201307255446",
        title = "大标三",
        datetime = "1999-09-09 11:39:22"
    },
    new
    {
        avatar = "data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%2260%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23ad79f2%22%2F%3E%3Ctext%20x%3D%2230%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E5%AD%94%3C%2Ftext%3E%3C%2Fsvg%3E",
        id = "460000201004082135",
        title = "认太山采先",
        datetime = "2004-05-27 05:53:39"
    },
    new
    {
        avatar = "data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%2260%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%2379f28a%22%2F%3E%3Ctext%20x%3D%2230%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E6%9D%A8%3C%2Ftext%3E%3C%2Fsvg%3E",
        id = "82000019851101034X",
        title = "全得片除来再",
        datetime = "1973-01-23 03:29:33"
    },
    new
    {
        avatar = "data:image/svg+xml;charset=UTF-8,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2Fsvg%22%20version%3D%221.1%22%20baseProfile%3D%22full%22%20width%3D%2260%22%20height%3D%22100%22%3E%3Crect%20width%3D%22100%25%22%20height%3D%22100%25%22%20fill%3D%22%23f2798b%22%2F%3E%3Ctext%20x%3D%2230%22%20y%3D%2250%22%20font-size%3D%2220%22%20alignment-baseline%3D%22middle%22%20text-anchor%3D%22middle%22%20fill%3D%22white%22%3E%E9%82%B9%3C%2Ftext%3E%3C%2Fsvg%3E",
        id = "410000200107161596",
        title = "图道构率计",
        datetime = "1994-11-08 20:25:54"
    }
};
return Success(data);
        }
    }
}
