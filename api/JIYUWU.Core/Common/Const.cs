using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Common
{
    #region 常量定义
    public class Secret
    {
        /// <summary>
        /// 用户密码加密key
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 数据库加密key
        /// </summary>
        public string DB { get; set; }

        /// <summary>
        /// redis加密key
        /// </summary>
        public string Redis { get; set; }

        /// <summary>
        /// jwt加密key
        /// </summary>
        public string JWT { get; set; }

        public string Audience { get; set; }
        public string Issuer { get; set; }

        /// <summary>
        /// 导出文件加密key
        /// </summary>
        public string ExportFile = "C5ABA9E202D94C13A3CB66002BF77FAF";

    }
    public struct HtmlElementType
    {
        public const string drop = "drop";
        public const string droplist = "droplist";
        public const string select = "select";
        public const string selectlist = "selectlist";
        public const string checkbox = "checkbox";
        public const string textarea = "textarea";
        public const string thanorequal = "thanorequal";
        public const string lessorequal = "lessorequal";


        public const string gt = "gt";
        public const string lt = "lt";
        public const string GT = ">";
        public const string LT = "<";
        public const string like = "like";

        public const string ThanOrEqual = ">=";
        public const string LessOrequal = "<=";
        public const string Contains = "in";
        public const string Equal = "=";
        public const string NotEqual = "!=";

    }
    public static class DBType
    {
        public static string Name { get; set; }
    }
    public class ApplicationContentType
    {
        public const string FORM = "application/x-www-form-urlencoded; charset=utf-8";
        public const string STREAM = "application/octet-stream; charset=utf-8";
        public const string JSON = "application/json; charset=utf-8";
        public const string XML = "application/xml; charset=utf-8";
        public const string TEXT = "application/text; charset=utf-8";
    }
    public struct SqlDbTypeName
    {
        public const string NVarChar = "nvarchar";
        public const string VarChar = "varchar";
        public const string NChar = "nchar";
        public const string Char = "char";
        public const string Text = "text";
        public const string Int = "int";
        public const string BigInt = "bigint";
        public const string DateTime = "datetime";
        public const string Date = "date";
        public const string SmallDateTime = "smalldatetime";
        public const string SmallDate = "smalldate";
        public const string Float = "float";
        public const string Decimal = "decimal";
        public const string Double = "double";
        public const string Bit = "bit";
        public const string Bool = "bool";
        public const string UniqueIdentifier = "uniqueidentifier";

    }
    #endregion


    #region 枚举类型
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
    public enum DbCurrentType
    {
        Default = 0,
        MySql = 1,
        MsSql = 2,
        PgSql = 3,
        Kdbndp,//人大金仓
        Oracle,
        DM,
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
    public enum QueryOrderBy
    {
        Desc = 1,
        Asc = 2
    }
    #endregion
}
