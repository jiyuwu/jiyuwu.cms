using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "岗位", TableName = "Base_Post", DBServer = "BaseDbContext")]
    public partial class Base_Post : BaseEntity
    {
        /// <summary>
        /// 岗位ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "PostId")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string PostId { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        [Display(Name = "岗位名称")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string PostName { get; set; }

        /// <summary>
        /// 上级岗位ID
        /// </summary>
        [Display(Name = "上级岗位ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string ParentId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        [Column(TypeName = "bit")]
        public bool Enable { get; set; } = true; // 默认值为 true，数据库会自动处理

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string Remark { get; set; }

        /// <summary>
        /// 数据库服务ID
        /// </summary>
        [Display(Name = "数据库服务ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string DbServiceId { get; set; }

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
