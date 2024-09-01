using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "租户管理", TableName = "Base_DbService", DBServer = "BaseDbContext")]
    public partial class Base_DbService : BaseEntity
    {
        /// <summary>
        ///
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "DbServiceId")]
        [Column(TypeName = "nvarchar(50)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string DbServiceId { get; set; }

        /// <summary>
        ///租户名称
        /// </summary>
        [Display(Name = "租户名称")]
        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string DbServiceName { get; set; }

        /// <summary>
        ///所属集团
        /// </summary>
        [Display(Name = "所属集团")]
        [Column(TypeName = "nvarchar(50)")]
        [Editable(true)]
        public string GroupId { get; set; }

        /// <summary>
        /// 数据库IP
        /// </summary>
        [Display(Name = " 数据库IP")]
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        [Editable(true)]
        public string DbIpAddress { get; set; }

        /// <summary>
        ///数据库名
        /// </summary>
        [Display(Name = "数据库名")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        [Editable(true)]
        public string DatabaseName { get; set; }

        /// <summary>
        ///账号
        /// </summary>
        [Display(Name = "账号")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        [Editable(true)]
        public string UserId { get; set; }

        /// <summary>
        ///密码
        /// </summary>
        [Display(Name = "密码")]
        [MaxLength(500)]
        [JsonIgnore]
        [SugarColumn(NoSerialize = true)]
        [Column(TypeName = "nvarchar(500)")]
        [Editable(true)]
        public string Pwd { get; set; }

        /// <summary>
        ///手机号
        /// </summary>
        [Display(Name = "手机号")]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Editable(true)]
        public string PhoneNo { get; set; }

        /// <summary>
        ///地址
        /// </summary>
        [Display(Name = "地址")]
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        [Editable(true)]
        public string Address { get; set; }

        /// <summary>
        ///是否可用
        /// </summary>
        [Display(Name = "是否可用")]
        [Column(TypeName = "int")]
        [Editable(true)]
        public int? Enable { get; set; }

        /// <summary>
        ///备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        [Editable(true)]
        public string Remark { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Display(Name = "CreateId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        public int? CreateId { get; set; }

        /// <summary>
        ///创建人
        /// </summary>
        [Display(Name = "创建人")]
        [MaxLength(30)]
        [Column(TypeName = "nvarchar(30)")]
        [Editable(true)]
        public string Creator { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Column(TypeName = "datetime")]
        [Editable(true)]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Display(Name = "ModifyId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        public int? ModifyId { get; set; }

        /// <summary>
        ///修改人
        /// </summary>
        [Display(Name = "修改人")]
        [MaxLength(30)]
        [Column(TypeName = "nvarchar(30)")]
        [Editable(true)]
        public string Modifier { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        [Column(TypeName = "datetime")]
        [Editable(true)]
        public DateTime? ModifyDate { get; set; }


    }
}
