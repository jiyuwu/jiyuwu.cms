using JIYUWU.Base.IRepository;
using JIYUWU.Base.IService;
using JIYUWU.Core.Common;
using JIYUWU.Core.Extension;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.Service
{
    public partial class Base_DbServiceService
        : ServiceBase<Base_DbService, IBase_DbServiceRepository>
    , IBase_DbServiceService, IDependency
    {
        public Base_DbServiceService(IBase_DbServiceRepository repository)
        : base(repository)
        {
            Init(repository);
        }
        public static IBase_DbServiceService Instance
        {
            get { return AutofacContainerModule.GetService<IBase_DbServiceService>(); }
        }
    }
}
