using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.Common;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Service
{
    public partial class Base_MenuService : ServiceBase<Base_Menu, IBase_MenuRepository>, IBase_MenuService, IDependency
    {
        public Base_MenuService(IBase_MenuRepository repository)
             : base(repository)
        {
            Init(repository);
        }
        public static IBase_MenuService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_MenuService>(); }
        }
    }
}
