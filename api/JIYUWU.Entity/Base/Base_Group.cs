﻿using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "集团管理", TableName = "Base_Group", DBServer = "BaseDbContext")]
    public partial class Base_Group : BaseEntity
    {
        /// <summary>
        ///
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "GroupId")]
        [Column(TypeName = "uniqueidentifier")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public Guid GroupId { get; set; }

        /// <summary>
        ///集团名称
        /// </summary>
        [Display(Name = "集团名称")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public string GroupName { get; set; }

        /// <summary>
        ///电话
        /// </summary>
        [Display(Name = "电话")]
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
        ///
        /// </summary>
        [Display(Name = "Enable")]
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
        [Display(Name = "CreateID")]
        [Column(TypeName = "int")]
        [Editable(true)]
        public int? CreateID { get; set; }

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
        [Display(Name = "ModifyID")]
        [Column(TypeName = "int")]
        [Editable(true)]
        public int? ModifyID { get; set; }

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