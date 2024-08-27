namespace JIYUWU.Core.WorkFlow
{
    // 定义一个枚举类型AuditRefuse，用于表示审核拒绝时的处理方式
    public enum AuditRefuse
    {
        流程结束 = 0,
        返回上一节点 = 1,
        流程重新开始 = 2
    }

}
