namespace JIYUWU.Core.WorkFlow
{
    // FieldFilter类用于表示字段过滤器的属性，包括字段名、值和过滤类型
    public  class FieldFilter
    {
        // 字段名属性，表示要过滤的字段
        public string Field { get; set; }
        
        // 值属性，表示过滤条件中的值
        public string Value { get; set; }

        // 过滤类型属性，表示过滤操作的类型
        public string FilterType { get; set; }
    }
}

