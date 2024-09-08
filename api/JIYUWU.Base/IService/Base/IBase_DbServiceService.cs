using JIYUWU.Core.Common;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.IService
{
    public partial interface IBase_DbServiceService : JIYUWU.Core.Common.IService<Base_DbService>
    {
        WebResponseContent CreateDb(string id);
    }
}
