namespace JIYUWU.Entity.Base
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string UserTrueName { get; set; }
        public int  Enable { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public int DeptId { get; set; }


        public List<string> DeptIds { get; set; }


        public List<string> RoleIds { get; set; }
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
            this.Code = 200;
        }

        public bool HasModelContent { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
    }
    public class ObjectValidatorResult
    {
        public ObjectValidatorResult()
        {

        }
        public ObjectValidatorResult(int code)
        {
            this.Code = code;
        }
        public ObjectValidatorResult OK(string msg)
        {
            this.Code =  200;
            this.Msg = msg;
            return this;
        }
        public ObjectValidatorResult Error(string msg)
        {
            this.Code = 500;
            this.Msg = msg;
            return this;
        }
        public int Code { get; set; }
        public string Msg { get; set; }
    }
}
