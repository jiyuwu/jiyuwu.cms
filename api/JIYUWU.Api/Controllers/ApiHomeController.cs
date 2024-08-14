using Microsoft.AspNetCore.Mvc;

namespace JIYUWU.Api.Controllers
{
    public class ApiHomeController : Controller
    {
        [HttpPost,HttpGet, Route("TestApi")]
        public IActionResult TestApi(DemoModel demo)
        {
            return Json(demo);
        }

    }
    public class DemoModel
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
