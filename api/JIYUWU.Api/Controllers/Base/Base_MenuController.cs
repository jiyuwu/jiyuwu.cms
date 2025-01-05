using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace JIYUWU.Api.Controllers.Base
{
    [Route("Base_Menu")]
    public class Base_MenuController : ApiBaseController<IBase_MenuService>
    {
        private IBase_MenuRepository _Repository;
        private IBase_MenuService _Service;

        [ActivatorUtilitiesConstructor]
        public Base_MenuController(IBase_MenuService service,
                                    IBase_MenuRepository Repository)
            : base(service)
        {
            _Repository = Repository;
            _Service = service;
        }
    }
}
