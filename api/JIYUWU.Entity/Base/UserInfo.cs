namespace JIYUWU.Entity.Base
{
    public class UserInfo
    {
        public int User_Id { get; set; }
        /// <summary>
        /// 多个角色ID
        /// </summary>
        public int Role_Id { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string UserTrueName { get; set; }
        public int  Enable { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public int DeptId { get; set; }


        public List<Guid> DeptIds { get; set; }


        public int[] RoleIds { get; set; }
        public string Token { get; set; }


        /// <summary>
        /// 2023.12.10实现租户字段过滤
        /// 租户值, 请在UserContext类GetUserInfo方法中设置TenancyValue的值
        /// </summary>
        public string TenancyValue { get; set; }
    }
}
