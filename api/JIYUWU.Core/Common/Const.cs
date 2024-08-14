using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Common
{
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
    public enum DbCurrentType
    {
        Default = 0,
        MySql = 1,
        MsSql = 2,//2020.08.08修改sqlserver拼写
        PgSql = 3,
        Kdbndp,//人大金仓
        Oracle, //2022.12.26
        DM, //2024.02.27
    }
    public enum ActionPermissionOptions
    {
        Add = 0,
        Delete = 1,
        Update = 2,
        Search = 3,
        Export = 4,
        Audit,
        Upload,//上传文件
        Import //导入表数据Excel
    }
}
