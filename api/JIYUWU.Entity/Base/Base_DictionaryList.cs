using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "字典列表", TableName = "Base_DictionaryList", DBServer = "BaseDbContext")]
    public partial class Base_DictionaryList : BaseEntity
    {
        /// <summary>
        /// 字典项ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "DicListId")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string DicListId { get; set; }

        /// <summary>
        /// 字典名称
        /// </summary>
        [Display(Name = "字典名称")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string DicName { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        [Display(Name = "字典值")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string DicValue { get; set; }

        /// <summary>
        /// 字典ID
        /// </summary>
        [Display(Name = "字典ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string DicId { get; set; }

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
