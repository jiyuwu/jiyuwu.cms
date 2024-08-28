namespace JIYUWU.Entity.Base
{
    public class SaveModel
    {
        public Dictionary<string, object> MainData { get; set; }
        public List<Dictionary<string, object>> DetailData { get; set; }
        public List<object> DelKeys { get; set; }

        /// <summary>
        /// 从前台传入的其他参数(自定义扩展可以使用)
        /// </summary>
        public object Extra { get; set; }

        /// <summary>
        /// 一对多明细
        /// </summary>
        public List<DetailInfo> Details { get; set; }

        public List<SubDelInfo> SubDelInfo { get; set; }
        /// <summary>
        /// 是否审批流程
        /// </summary>
        public bool IsFlow { get; set; }

        public string DataVersionField { get; set; }
        public string DataVersionValue { get; set; }
    }

    public class DetailInfo
    {
        public string Table { get; set; }

        public List<Dictionary<string, object>> Data { get; set; }
        public List<object> DelKeys { get; set; }
    }

    public class SubDelInfo
    {
        public bool IsProescc { get; set; }
        public string Table { get; set; }
        public List<object> DelKeys { get; set; }
    }

    public class PageDataOptions
    {
        public int Page { get; set; }
        public int Rows { get; set; }
        public int Total { get; set; }
        public string TableName { get; set; }

        /// <summary>
        /// 三级明细表
        /// </summary>
        public string DetailTable { get; set; }
        public string Sort { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public string Order { get; set; }
        public string Wheres { get; set; }
        public bool Export { get; set; }
        public object Value { get; set; }


        /// <summary>
        /// 查询条件
        /// </summary>
        public List<SearchParameters> Filter { get; set; }
        /// <summary>
        ///  导出列表与界面显示字段一致
        /// </summary>
        public string[] Columns { get; set; }
    }
    public class SearchParameters
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string DisplayType { get; set; }
    }
    public class PageGridData<T>
    {
        public int status { get; set; }
        public string msg { get; set; }
        public int total { get; set; }
        public List<T> rows { get; set; }
        public object summary { get; set; }
        /// <summary>
        /// 可以在返回前，再返回一些额外的数据，比如返回其他表的信息，前台找到查询后的方法取出来
        /// </summary>
        public object extra { get; set; }
    }
}
