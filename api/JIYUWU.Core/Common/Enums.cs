using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Common
{
    public enum ResponseType
    {
        ServerError = 1,
        LoginExpiration = 302,
        ParametersLack = 303,
        TokenExpiration,
        PINError,
        NoPermissions,
        NoRolePermissions,
        LoginError,
        AccountLocked,
        LoginSuccess,
        SaveSuccess,
        AuditSuccess,
        OperSuccess,
        RegisterSuccess,
        ModifyPwdSuccess,
        EidtSuccess,
        DelSuccess,
        NoKey,
        NoKeyDel,
        KeyError,
        Other
    }
    public enum LinqExpressionType
    {
        Equal = 0,//=
        NotEqual = 1,//!=
        GreaterThan,//>
        LessThan,//<
        ThanOrEqual,//>=
        LessThanOrEqual,//<=
        In,
        Contains,//Contains
        Like,//Contains
        LikeStart,
        LikeEnd,
        NotLike,
        NotContains//NotContains
    }
    public enum QueryOrderBy
    {
        Desc = 1,
        Asc = 2
    }
    public enum Flag
    {
        /// <summary>
        /// 默认。
        /// </summary>
        Default,

        /// <summary>
        /// 真。
        /// </summary>
        True,

        /// <summary>
        /// 假。
        /// </summary>
        False
    }
    public enum CPrefix
    {
        Role = 0,
        //UserIDkey
        UID = 1,
        /// <summary>
        /// 头像KEY
        /// </summary>
        HDImg = 2,
        Token = 3,
        CityList

    }
    public enum AuthData
    {
        全部 = 1,
        本组织与本角色以及下数据 = 10,
        本组织及下数据 = 20,
        本组织数据 = 30,
        本角色以及下数据 = 40,
        本角色数据 = 50,
        仅自己数据 = 60
    }
    public enum LoggerType
    {
        System = 0,
        Info,
        Success,
        Error,
        Authorzie,
        Global,
        Login,
        Exception,
        ApiException,
        HandleError,
        OnActionExecuted,
        GetUserInfo,
        Edit,
        Search,
        Add,
        Del,
        AppHome,
        ApiLogin,
        ApiPINLogin,
        ApiRegister,
        ApiModifyPwd,
        ApiSendPIN,
        ApiAuthorize,
        Ask,
        JoinMeeting,
        JoinUs,
        EditUserInfo,
        Sell,
        Buy,
        ReportPrice,
        Reply,
        TechData,
        TechSecondData,
        DelPublicQuestion,
        DelexpertQuestion,
        CreateTokenError,
        IPhoneTest,
        SDKSuccess,
        SDKSendError,
        ExpertAuthority,
        ParEmpty,
        NoToken,
        ReplaceToeken,
        KafkaException
    }
    public enum UserAgent
    {
        IOS = 0,
        Android = 1,
        Windows = 2,
        Linux
    }
    public struct ApiMessage
    {

        /// <summary>
        /// 参数有误
        /// </summary>
        public const string ParameterError = "请求参数不正确!";
        /// <summary>
        /// 没有配置好输入参数
        /// </summary>
        public const string NotInputEntity = "没有配置好输入参数!";
        /// <summary>
        /// token丢失
        /// </summary>
        public const string TokenLose = "token丢失!";

        /// <summary>
        /// 版本号不能为空
        /// </summary>

        public const string VersionEmpty = "版本号不能为空!";
        /// <summary>
        /// content不能为空
        /// </summary>

        public const string ContentEmpty = "biz_content不能为空!";
        /// <summary>
        /// content不能为空
        /// </summary>
        public const string TokenError = "token不正确";

        public const string AccountLocked = "帐号已被锁定!";

        public const string PhoneNoInvalid = "输入的不是手机号";


        public const string PINTypeNotRange = "获取验证的类型不正确";
        public const string OperToBusy = "操作太频繁，请稍后再试";

        public const string SendSTKError = "短信发送异常,请稍后再试";
        public const string SendSTKSuccess = "短信发送成功";
        public const string STKNotSend = "请先获取验证码";
        public const string AccountExists = "手机号已经被注册";

        public const string AccountNotExists = "手机号没有注册";

        public const string PINExpire = "验证码已过期,请重新获取";

        public const string PINError = "验证码不正确";

        public const string ParameterEmpty = "参数不能为空";
    }
}
