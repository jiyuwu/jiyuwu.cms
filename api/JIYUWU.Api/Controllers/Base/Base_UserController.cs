using JIYUWU.Base.IService;
using JIYUWU.Core.Common;
using JIYUWU.Entity.AttributeManager;
using Microsoft.AspNetCore.Mvc;

namespace JIYUWU.Api.Controllers
{
    [Route("api/Base_User")]
    [PermissionTable(Name = "Base_User")]
    public partial class Base_UserController : ApiBaseController<IBase_UserService>
    {
        public Base_UserController(IBase_UserService service)
        : base("System", "System", "Base_User", service)
        {
            //, IMemoryCache cache
        }
    }

}
