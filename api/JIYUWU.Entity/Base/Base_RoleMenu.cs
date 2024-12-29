using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "角色菜单权限", TableName = "Base_RoleMenu", DBServer = "BaseDbContext")]
    public partial class Base_RoleMenu : BaseEntity
    {
        /// <summary>
        /// 角色菜单权限ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "Id")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }

        /// <summary>
        /// 操作数据（如：Search, Add, Delete, Update, Import, Export, Upload）
        /// </summary>
        [Display(Name = "操作数据")]
        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        [Required]
        public string ActionData { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Display(Name = "角色ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string RoleId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name = "用户ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string UserId { get; set; }

        /// <summary>
        /// 菜单权限数据（如：不分配，全部，本组织+本角色及下）
        /// </summary>
        [Display(Name = "菜单权限数据")]
        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        public string AuthMenuData { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [Display(Name = "创建人ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string CreateId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; } = DateTime.Now; // 默认值为当前时间

        /// <summary>
        /// 修改人ID
        /// </summary>
        [Display(Name = "修改人ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string ModifyId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }
    }
}
