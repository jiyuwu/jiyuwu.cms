using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace JIYUWU.Entity.Base
{
    [Entity(TableCnName = "菜单", TableName = "Base_Menu", DBServer = "BaseDbContext")]
    public partial class Base_Menu : BaseEntity
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        [Key]
        [Display(Name = "MenuId")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string MenuId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [Display(Name = "菜单名称")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string MenuName { get; set; }

        /// <summary>
        /// 按钮
        /// </summary>
        [Display(Name = "按钮")]
        [MaxLength(2000)]
        [Column(TypeName = "nvarchar(2000)")]
        public string Button { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Display(Name = "图标")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Icon { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string Description { get; set; }

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
        /// 表名
        /// </summary>
        [Display(Name = "表名")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string TableName { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>
        [Display(Name = "父菜单ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        [Required(AllowEmptyStrings = false)]
        public string ParentId { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [Display(Name = "链接")]
        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string Url { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        [Display(Name = "菜单类型")]
        [Column(TypeName = "int")]
        public int? MenuType { get; set; }

        /// <summary>
        /// 链接类型
        /// </summary>
        [Display(Name = "链接类型")]
        [Column(TypeName = "int")]
        public int? LinkType { get; set; }

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
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 修改人ID
        /// </summary>
        [Display(Name = "修改人ID")]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string ModifyId { get; set; }
    }
    #region 菜单树形路由
    public class RouteMeta
    {
        public string Title { get; set; }
        public List<string> Actions { get; set; }  // Actions 是一个数组，有些节点包含这个属性
    }

    public class RouteMenuItem
    {
        public string Name { get; set; }
        public RouteMeta Meta { get; set; }
        public List<RouteMenuItem> Children { get; set; }  // 子节点是递归的结构，可以包含更多子项
    }
    #endregion
}
