using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.Common;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Service
{
    public partial class Base_RoleService : ServiceBase<Base_Role, IBase_RoleRepository>, IBase_RoleService, IDependency
    {
        public Base_RoleService(IBase_RoleRepository repository)
             : base(repository)
        {
            Init(repository);
        }
        public static IBase_RoleService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_RoleService>(); }
        }
    }
}
