using JIYUWU.Core.Common;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.IService
{
    public partial interface IBase_UserService : JIYUWU.Core.Common.IService<Base_User>
    {
        bool SetUserInfo(Base_User user, string token);
        Base_User GetUserInfoByToken(string token);
    }
}
