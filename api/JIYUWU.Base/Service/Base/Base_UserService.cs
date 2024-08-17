using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.Common;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Service
{
    public partial class Base_UserService : ServiceBase<Base_User, IBase_UserRepository>, IBase_UserService, IDependency
    {
        public Base_UserService(IBase_UserRepository repository)
             : base(repository)
        {
            Init(repository);
        }
        public static IBase_UserService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_UserService>(); }
        }
    }
}
