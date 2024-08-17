using JIYUWU.Core.Common;
using JIYUWU.Core.UserManager;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.IService
{
    public partial interface IBase_RoleService
    {
        Task<WebResponseContent> GetUserTreePermission(int[] role_Id);

        Task<WebResponseContent> GetCurrentUserTreePermission();

        Task<WebResponseContent> GetCurrentTreePermission();

        Task<WebResponseContent> SavePermission(List<UserPermissions> userPermissions, int roleId);
        /// <summary>
        /// 获取角色下所有的角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<RoleNodes>> GetAllChildrenAsync(int[] roleId);

        /// <summary>
        /// 获取角色下所有的角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<RoleNodes> GetAllChildren(int roleId);

        /// <summary>
        /// 获取角色下所有的角色Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<int>> GetAllChildrenRoleIdAsync(int roleId);

        List<int> GetAllChildrenRoleId(int roleId);

        List<int> GetAllChildrenRoleId(int[] roleIds);
        /// <summary>
        /// 获取当前角色下的所有角色包括自己的角色Id
        /// </summary>
        /// <returns></returns>
        List<int> GetAllChildrenRoleIdAndSelf();
    }
}
