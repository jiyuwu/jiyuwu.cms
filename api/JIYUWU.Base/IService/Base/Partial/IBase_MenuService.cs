using JIYUWU.Core.Common;
using JIYUWU.Entity.Base;

namespace JIYUWU.Base.IService
{
    public partial interface IBase_MenuService
    {
        Task<object> GetMenu();
        List<Base_Menu> GetCurrentMenuList();

        List<Base_Menu> GetUserMenuList(int[] roleId);

        Task<object> GetCurrentMenuActionList();

        Task<object> GetMenuActionList(int[] roleIds);
        Task<WebResponseContent> Save(Base_Menu menu);

        Task<WebResponseContent> DelMenu(int menuId);


        Task<object> GetTreeItem(int menuId);
    }
}
