namespace JIYUWU.Core.WorkFlow
{
    // 定义一个枚举类型AuditBack，用于表示审核流程中的返回状态
    public enum AuditBack
    {
        流程结束 = 0,
        返回上一节点 = 1,
        流程重新开始 = 2
    }

}
