using System.ComponentModel.DataAnnotations;

namespace JIYUWU.Entity.Base
{
    public class Permissions
    {
        public int Menu_Id { get; set; }
        public int ParentId { get; set; }
        public string TableName { get; set; }
        public string MenuAuth { get; set; }
        public string UserAuth { get; set; }
        /// <summary>
        /// 当前用户权限,存储的是权限的值，如:Add,Search等
        /// </summary>
        public string[] UserAuthArr { get; set; }

        /// <summary>
        /// 菜单类型1:移动端，0:PC端
        /// </summary>
        public int MenuType { get; set; }


        /// <summary>
        ///菜单数据权限
        /// </summary>
        [Display(Name = "菜单数据权限")]

        public string AuthMenuData { get; set; }
    }
}
