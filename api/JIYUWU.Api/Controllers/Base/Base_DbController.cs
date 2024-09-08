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
    [Route("api/Base_User")]
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
        [HttpGet, Route("getVierificationCode"), AllowAnonymous]
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
        [HttpPost, Route("AddDB"), AllowAnonymous]
        public IActionResult Add()
        {
            Base_DbService service = new Base_DbService();
            service.DbServiceName = "test";
            service.DbServiceId=Guid.NewGuid().ToString();
            service.PhoneNo = "test";
            service.Pwd = "123";
            int count= _Service.Add(service);
            return Ok();
        }
    }
}
