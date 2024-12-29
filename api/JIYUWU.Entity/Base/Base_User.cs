using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "用户", TableName = "Base_User", DBServer = "BaseDbContext")]
    public partial class Base_User : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "UserId")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Display(Name = "密码")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string UserPwd { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        [Display(Name = "真实姓名")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string UserTrueName { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Display(Name = "角色ID")]
        [MaxLength(2000)]
        [Column(TypeName = "varchar(2000)")]
        public string RoleId { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [Display(Name = "部门ID")]
        [MaxLength(2000)]
        [Column(TypeName = "nvarchar(2000)")]
        public string DeptId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        [MaxLength(11)]
        [Column(TypeName = "nvarchar(11)")]
        public string PhoneNo { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [Display(Name = "Token")]
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string Token { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        [Column(TypeName = "bit")]
        public bool Enable { get; set; } = true; // 默认值为 true，数据库会自动处理

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        [Column(TypeName = "int")]
        public int? Gender { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Display(Name = "邮箱")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string Email { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        [Display(Name = "头像URL")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string HeadImageUrl { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string Remark { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        [Display(Name = "上次登录时间")]
        [Column(TypeName = "datetime")]
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// 上次修改密码时间
        /// </summary>
        [Display(Name = "上次修改密码时间")]
        [Column(TypeName = "datetime")]
        public DateTime? LastModifyPwdDate { get; set; }

        /// <summary>
        /// 数据库服务ID
        /// </summary>
        [Display(Name = "数据库服务ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string DbServiceId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }

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
    }
}
