using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace JIYUWU.Api.Controllers.Base
{
    [Route("api/Base_User")]
    public class Base_UserController: ApiBaseController<IBase_UserService>
    {
        private IBase_UserRepository _Repository;
        private IBase_UserService _Service;
        [ActivatorUtilitiesConstructor]
        public Base_UserController(IBase_UserService service,
                           IBase_UserRepository Repository)
       : base(service)
        {
            _Repository = Repository;
            _Service = service;
        }
    }
}
