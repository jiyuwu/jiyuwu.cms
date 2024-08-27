using JIYUWU.Entity.Base;

namespace JIYUWU.Core.WorkFlow
{
    // 工作流表选项类，继承自Base_WorkFlow类
    public class WorkFlowTableOptions:Base_WorkFlow
    {
        // 默认审核状态属性
        public AuditStatus DefaultAuditStatus { get; set; }
        // 过滤器列表属性
        public List<FilterOptions> FilterList { get; set; }
    }

    // 过滤器选项类，继承自Base_WorkFlowStep类
    public class FilterOptions : Base_WorkFlowStep 
    {
       // 字段过滤器列表属性
       public List<FieldFilter> FieldFilters { get; set; }

        // 表达式属性
        public object Expression { get; set; }

        // 父ID数组属性
        public string[] ParentIds { get; set; }


    }
}

