using JIYUWU.Entity.Base;

namespace JIYUWU.Core.WorkFlow
{
    public class WorkFlowTableOptions:Base_WorkFlow
    {
        public AuditStatus DefaultAuditStatus { get; set; }
        public List<FilterOptions> FilterList { get; set; }
    }

    public class FilterOptions : Base_WorkFlowStep 
    {
       public List<FieldFilter> FieldFilters { get; set; }

        public object Expression { get; set; }

        public string[] ParentIds { get; set; }


    }
}
