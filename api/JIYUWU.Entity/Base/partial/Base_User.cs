using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JIYUWU.Entity.Base
{
    public partial class Base_User
    {
        //此处配置字段(字段配置见此model的另一个partial),如果表中没有此字段请加上 [NotMapped]属性，否则会异常

        /// <summary>
        ///是否在线 2023.12.10增加是否在线字段,显示用户在线状态，强制下线功能
        /// </summary>
        [Display(Name = "是否在线")]
        [Column(TypeName = "int")]
        [NotMapped]
        [SugarColumn(IsIgnore = true)]
        public int IsOnline { get; set; }
    }
}
