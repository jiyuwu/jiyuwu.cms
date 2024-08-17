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
    public class ObjectModelValidatorState
    {
        public ObjectModelValidatorState()
        {
            this.Status = true;
        }

        public bool Status { get; set; }
        public bool HasModelContent { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
    public class ObjectValidatorResult
    {
        public ObjectValidatorResult()
        {

        }
        public ObjectValidatorResult(bool status)
        {
            this.Status = status;
        }
        public ObjectValidatorResult OK(string message)
        {
            this.Status = true;
            this.Message = message;
            return this;
        }
        public ObjectValidatorResult Error(string message)
        {
            this.Status = false;
            this.Message = message;
            return this;
        }
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
