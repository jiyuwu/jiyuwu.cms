namespace JIYUWU.Core.WorkFlow
{
    // 定义审核状态的枚举类型
public enum AuditStatus
    {
        待审核 = 0,
        审核通过 = 1,
        审核中 = 2,
        审核未通过 = 3,
        驳回 = 4,
        终止= 5,
        草稿 = 90,
        待提交=100
    }

// 定义审核类型的枚举类型
    public enum AuditType
    {
        用户审批 = 1,
        角色审批 = 2,
        部门审批 = 3,
        提交人上级部门审批 = 4,
        提交人上级角色审批 = 5
    }

}
