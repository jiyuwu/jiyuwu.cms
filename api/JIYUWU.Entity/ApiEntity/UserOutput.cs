using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JIYUWU.Entity.ApiEntity
{
    public class UserOutput
    {
        /// <summary>
        ///用户名
        /// </summary>
        [Display(Name = "用户名")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        /// <summary>
        ///部门
        /// </summary>
        [Display(Name = "部门")]
        [MaxLength(300)]
        [Column(TypeName = "nvarchar(300)")]
        [Editable(true)]
        public string DeptName { get; set; }

        /// <summary>
        ///角色
        /// </summary>
        [Display(Name = "角色")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int Role_Id { get; set; }


    }
}
