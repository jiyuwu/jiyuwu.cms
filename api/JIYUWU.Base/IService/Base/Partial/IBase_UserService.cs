using JIYUWU.Core.Common;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.IService
{
    public partial interface IBase_UserService
    {
        Task<WebResponseContent> Login(LoginInfo loginInfo, bool verificationCode = true);
        Task<WebResponseContent> ReplaceToken();
        Task<WebResponseContent> ModifyPwd(string oldPwd, string newPwd);
        Task<WebResponseContent> GetCurrentUserInfo();
    }
}
