using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "角色", TableName = "Base_Role", DBServer = "BaseDbContext")]
    public partial class Base_Role : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "RoleId")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string RoleId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        [Column(TypeName = "bit")]
        public bool Enable { get; set; } = true; // 默认值为 true，数据库会自动处理

        /// <summary>
        /// 排序编号
        /// </summary>
        [Display(Name = "排序编号")]
        [Column(TypeName = "int")]
        public int? OrderNo { get; set; }

        /// <summary>
        /// 父角色ID
        /// </summary>
        [Display(Name = "父角色ID")]
        [Column(TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int ParentId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name = "角色名称")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string RoleName { get; set; }

        /// <summary>
        /// 数据库服务ID
        /// </summary>
        [Display(Name = "数据库服务ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string DbServiceId { get; set; }

        /// <summary>
        /// 数据查询权限
        /// </summary>
        [Display(Name = "数据查询权限")]
        [MaxLength(10)]
        [Column(TypeName = "nvarchar(10)")]
        public string DataAuth { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; } = DateTime.Now; // 默认值为当前时间

        /// <summary>
        /// 创建人ID
        /// </summary>
        [Display(Name = "创建人ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string CreateId { get; set; }

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

        /// <summary>
        /// 角色标识
        /// </summary>
        [Display(Name = "角色标识")]
        [Column(TypeName = "int")]
        public int? RoleSign { get; set; }
    }
}
