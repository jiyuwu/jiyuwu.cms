using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using JIYUWU.Entity.Base;

namespace JIYUWU.Entity.Sys
{
    [Entity(TableCnName = "字典管理", TableName = "Base_Dictionary", DBServer = "BaseDbContext")]
    public partial class Base_Dictionary : BaseEntity
    {
        /// <summary>
        /// 字典ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "DicId")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string DicId { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        [Display(Name = "配置")]
        [Column(TypeName = "nvarchar(max)")]
        public string Config { get; set; }

        /// <summary>
        /// 数据库服务器
        /// </summary>
        [Display(Name = "数据库服务器")]
        [Column(TypeName = "nvarchar(max)")]
        public string DBServer { get; set; }

        /// <summary>
        /// SQL查询
        /// </summary>
        [Display(Name = "SQL查询")]
        [Column(TypeName = "nvarchar(max)")]
        public string DbSql { get; set; }

        /// <summary>
        /// 字典名称
        /// </summary>
        [Display(Name = "字典名称")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        [Required(AllowEmptyStrings = false)]
        public string DicName { get; set; }

        /// <summary>
        /// 字典编号
        /// </summary>
        [Display(Name = "字典编号")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        [Required(AllowEmptyStrings = false)]
        public string DicNo { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        [Column(TypeName = "bit")]
        public bool Enable { get; set; } = true; // 默认值为 true，数据库会自动处理

        /// <summary>
        /// 排序号
        /// </summary>
        [Display(Name = "排序号")]
        [Column(TypeName = "int")]
        public int? OrderNo { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [Display(Name = "父级ID")]
        [Column(TypeName = "int")]
        [Required]
        public int ParentId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string Remark { get; set; }

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
