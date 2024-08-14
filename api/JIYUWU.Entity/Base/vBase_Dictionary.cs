using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "字典视图")]
    public class vBase_Dictionary: BaseEntity
    {
        /// <summary>
       ///
       /// </summary>
       [Key]
       [Display(Name ="Dic_ID")]
       [Column(TypeName="int")]
       [Required(AllowEmptyStrings=false)]
       public int Dic_ID { get; set; }

       /// <summary>
       ///
       /// </summary>
       [Display(Name ="DicValue")]
       [MaxLength(100)]
       [Column(TypeName="nvarchar(100)")]
       public string DicValue { get; set; }

       /// <summary>
       ///
       /// </summary>
       [Display(Name ="DicList_ID")]
       [Column(TypeName="int")]
       public int DicList_ID { get; set; }

       /// <summary>
       ///
       /// </summary>
       [Display(Name ="DicName")]
       [MaxLength(100)]
       [Column(TypeName="nvarchar(100)")]
       public string DicName { get; set; }

       /// <summary>
       ///
       /// </summary>
       [Display(Name ="Enable")]
       [Column(TypeName="tinyint")]
       public byte? Enable { get; set; }

       /// <summary>
       ///
       /// </summary>
       [Display(Name ="DicNo")]
       [MaxLength(100)]
       [Column(TypeName="nvarchar(100)")]
       [Required(AllowEmptyStrings=false)]
       public string DicNo { get; set; }

       
    }
}
