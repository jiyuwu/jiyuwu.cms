using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SqlSugar;

namespace JIYUWU.Entity.Base
{
    [Table("Sys_RoleAuth")]
    public class Base_RoleAuth : BaseEntity
    {
        /// <summary>
        ///
        /// </summary>
        [Key]
        [Display(Name = "")]
        [Required(AllowEmptyStrings = false)]
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]

        public int Auth_Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Display(Name = "")]
        [Column(TypeName = "int")]
        public int? Role_Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Display(Name = "")]
        [Column(TypeName = "int")]
        public int? User_Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Display(Name = "")]
        [Column(TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int Menu_Id { get; set; }

        /// <summary>
        ///用户权限
        /// </summary>
        [Display(Name = "用户权限")]
        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        [Required(AllowEmptyStrings = false)]
        public string AuthValue { get; set; }


        /// <summary>
        ///菜单数据权限
        /// </summary>
        [Display(Name = "菜单数据权限")]
        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        public string AuthMenuData { get; set; }
        /// <summary>
        ///
        /// </summary>
        [Display(Name = "")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(1000)")]
        public string Creator { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Display(Name = "")]
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Display(Name = "")]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(1000)")]
        public string Modifier { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Display(Name = "")]
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }


    }
}
