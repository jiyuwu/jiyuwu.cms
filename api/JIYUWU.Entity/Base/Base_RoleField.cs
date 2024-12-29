using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "角色字段权限", TableName = "Base_RoleField", DBServer = "BaseDbContext")]
    public partial class Base_RoleField : BaseEntity
    {
        /// <summary>
        /// 角色字段权限ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "Id")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }

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
        /// 数据库服务ID
        /// </summary>
        [Display(Name = "数据库服务ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string DbServiceId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Display(Name = "表名")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string TableName { get; set; }

        /// <summary>
        /// 授权字段
        /// </summary>
        [Display(Name = "授权字段")]
        [MaxLength(2000)]
        [Column(TypeName = "nvarchar(2000)")]
        public string AuthField { get; set; }

        /// <summary>
        /// 启用标识
        /// </summary>
        [Display(Name = "启用标识")]
        [Column(TypeName = "bit")]
        public bool? Enable { get; set; }

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
        public DateTime? CreateDate { get; set; }

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
