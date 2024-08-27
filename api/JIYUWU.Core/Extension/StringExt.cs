using JIYUWU.Core.Common;
using System.Collections;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using JIYUWU.Core.UserManager;

namespace JIYUWU.Core.Extension
{
    public static class StringExt
    {
        public static bool _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static string ReplacePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            if (_windows)
                return path.Replace("/", "\\");
            return path.Replace("\\", "/");

        }
        private static DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);

        private static long longTime = 621355968000000000;

        private static int samllTime = 10000000;
        /// <summary>
        /// 获取时间戳 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimeStamp(this DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - longTime) / samllTime;
        }
        /// <summary>
        /// 时间戳转换成日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetTimeSpmpToDate(this object timeStamp)
        {
            if (timeStamp == null) return dateStart;
            DateTime dateTime = new DateTime(longTime + Convert.ToInt64(timeStamp) * samllTime, DateTimeKind.Utc).ToLocalTime();
            return dateTime;
        }
        public static string CreateHtmlParas(this string urlPath, int? userId = null)
        {
            if (string.IsNullOrEmpty(urlPath))
                return null;
            userId = userId ?? UserContext.Current.UserInfo.User_Id;
            return $"{urlPath}{(urlPath.IndexOf("?token") > 0 ? "&" : "?")}uid={userId}&rt_v={DateTime.Now.ToString("HHmmss")}";
        }

        public static bool IsUrl(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            string Url = @"^(?i)(?:http://|https://)([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            return Regex.IsMatch(str, Url);

        }
        /// <summary>
        /// 判断是不是正确的手机号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsPhoneNo(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            if (input.Length != 11)
                return false;

            if (new Regex(@"^1[3578][01379]\d{8}$").IsMatch(input)
                || new Regex(@"^1[34578][01256]\d{8}").IsMatch(input)
                || new Regex(@"^(1[012345678]\d{8}|1[345678][0123456789]\d{8})$").IsMatch(input)
                )
                return true;
            return false;
        }

        public static string GetDBCondition(this string stringType)
        {
            string reslut = "";
            switch (stringType?.ToLower())
            {
                case HtmlElementType.droplist:
                case HtmlElementType.selectlist:
                case HtmlElementType.textarea:
                case HtmlElementType.checkbox:
                    reslut = HtmlElementType.Contains;
                    break;
                case HtmlElementType.thanorequal:
                    reslut = HtmlElementType.ThanOrEqual;
                    break;
                case HtmlElementType.lessorequal:
                    reslut = HtmlElementType.LessOrequal;
                    break;
                case HtmlElementType.gt:
                    reslut = HtmlElementType.GT;
                    break;
                case HtmlElementType.lt:
                    reslut = HtmlElementType.lt;
                    break;
                case HtmlElementType.like:
                    reslut = HtmlElementType.like;
                    break;
                default:
                    reslut = HtmlElementType.Equal;
                    break;
            }
            return reslut;
        }

        public static LinqExpressionType GetLinqCondition(this string stringType)
        {
            LinqExpressionType linqExpression;
            switch (stringType)
            {
                case "!=":
                    linqExpression = LinqExpressionType.NotEqual;
                    break;
                case "like"://模糊查询
                    linqExpression = LinqExpressionType.Like;
                    break;
                case "likeStart"://like 'xx%'
                    linqExpression = LinqExpressionType.LikeStart;
                    break;
                case "likeEnd"://like '%xx'
                    linqExpression = LinqExpressionType.LikeEnd;
                    break;
                case "selectList": //多选 
                case "checkbox": //多选 
                case "droplist":
                    linqExpression = LinqExpressionType.In;
                    break;
                case "notIn": //多选不包括
                    linqExpression = LinqExpressionType.NotContains;
                    break;
                case "thanorequal": //>=
                case ">=":
                    linqExpression = LinqExpressionType.ThanOrEqual;
                    break;
                case "lessorequal": //<=
                case "<=":
                    linqExpression = LinqExpressionType.LessThanOrEqual;
                    break;
                case "gt": //>=
                case ">":
                    linqExpression = LinqExpressionType.GreaterThan;
                    break;
                case "lt": //<=
                case "<":
                    linqExpression = LinqExpressionType.LessThan;
                    break;
                default:
                    linqExpression = LinqExpressionType.Equal;
                    break;
            }
            return linqExpression;
        }

        public static bool GetGuid(this string guid, out Guid outId)
        {
            Guid emptyId = Guid.Empty;
            return Guid.TryParse(guid, out outId);
        }

        public static bool IsGuid(this string guid)
        {
            Guid newId;
            return guid.GetGuid(out newId);
        }

        public static bool IsInt(this object obj)
        {
            if (obj == null)
                return false;
            bool reslut = Int32.TryParse(obj.ToString(), out int _number);
            return reslut;

        }
        public static bool IsDate(this object str)
        {
            return str.IsDate(out _);
        }
        public static bool IsDate(this object str, out DateTime dateTime)
        {
            dateTime = DateTime.Now;
            if (str == null || str.ToString() == "")
            {
                return false;
            }
            return DateTime.TryParse(str.ToString(), out dateTime);
        }
        /// <summary>
        /// 根据传入格式判断是否为小数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="formatString">18,5</param>
        /// <returns></returns>
        public static bool IsNumber(this string str, string formatString)
        {
            if (string.IsNullOrEmpty(str)) return false;

            return Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$");
        }
        /// <summary>
        /// 判断一个字符串是否为合法数字(指定整数位数和小数位数)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="precision">整数位数</param>
        /// <param name="scale">小数位数</param>
        /// <returns></returns>
        public static bool IsNumber(this string str, int precision, int scale)
        {
            if ((precision == 0) && (scale == 0))
            {
                return false;
            }
            string pattern = @"(^\d{1," + precision + "}";
            if (scale > 0)
            {
                pattern += @"\.\d{0," + scale + "}$)|" + pattern;
            }
            pattern += "$)";
            return Regex.IsMatch(str, pattern);
        }


        public static bool IsNullOrEmpty(this object str)
        {
            if (str == null)
                return true;
            return str.ToString() == "";
        }


        public static int GetInt(this object obj)
        {
            if (obj == null)
                return 0;
            int.TryParse(obj.ToString(), out int _number);
            return _number;

        }

        /// <summary>
        /// 获取 object 中的枚举值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long GetLong(this string obj)
        {
            if (obj == null)
                return 0;

            if (long.TryParse(obj, out long value))
            {
                return value;
            }
            return 0;
        }

        /// <summary>
        /// 获取 object 中的 float
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>

        public static float GetFloat(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return 0;

            try
            {
                return float.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }

        public static double GetDouble(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return 0;

            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取 object 中的 decimal
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static decimal GetDecimal(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return 0;

            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取 object 中的 decimal
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static dynamic GetDynamic(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return null;

            try
            {
                string str = obj.ToString();
                if (str.IsNumber(25, 15)) return Convert.ToDecimal(obj);
                else return str;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static DateTime? GetDateTime(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return null;
            bool result = DateTime.TryParse(obj.ToString(), out DateTime dateTime);
            if (!result)
                return null;
            return dateTime;
        }



        public static object ParseTo(this string str, string type)
        {
            switch (type)
            {
                case "System.Boolean":
                    return ToBoolean(str);
                case "System.SByte":
                    return ToSByte(str);
                case "System.Byte":
                    return ToByte(str);
                case "System.UInt16":
                    return ToUInt16(str);
                case "System.Int16":
                    return ToInt16(str);
                case "System.uInt32":
                    return ToUInt32(str);
                case "System.Int32":
                    return str.ToInt32();
                case "System.UInt64":
                    return ToUInt64(str);
                case "System.Int64":
                    return ToInt64(str);
                case "System.Single":
                    return ToSingle(str);
                case "System.Double":
                    return ToDouble(str);
                case "System.Decimal":
                    return ToDecimal(str);
                case "System.DateTime":
                    return ToDateTime(str);
                case "System.Guid":
                    return ToGuid(str);
            }
            throw new NotSupportedException(string.Format("The string of \"{0}\" can not be parsed to {1}", str, type));
        }

        public static sbyte? ToSByte(this string value)
        {
            sbyte value2;
            if (sbyte.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static byte? ToByte(this string value)
        {
            byte value2;
            if (byte.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static ushort? ToUInt16(this string value)
        {
            ushort value2;
            if (ushort.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static short? ToInt16(this string value)
        {
            if (short.TryParse(value, out short value2))
            {
                return value2;
            }
            return null;
        }

        public static uint? ToUInt32(this string value)
        {
            uint value2;
            if (uint.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static ulong? ToUInt64(this string value)
        {
            ulong value2;
            if (ulong.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static long? ToInt64(this string value)
        {
            long value2;
            if (long.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static float? ToSingle(this string value)
        {
            float value2;
            if (float.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static double? ToDouble(this string value)
        {
            double value2;
            if (double.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static decimal? ToDecimal(this string value)
        {
            decimal value2;
            if (decimal.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static bool? ToBoolean(this string value)
        {
            bool value2;
            if (bool.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }



        public static Guid? ToGuid(this string str)
        {
            Guid value;
            if (Guid.TryParse(str, out value))
            {
                return value;
            }
            return null;
        }

        public static DateTime? ToDateTime(this string value)
        {
            DateTime value2;
            if (DateTime.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        public static int? ToInt32(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            int value;
            if (int.TryParse(input, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        ///     替换空格字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replacement">替换为该字符</param>
        /// <returns>替换后的字符串</returns>
        public static string ReplaceWhitespace(this string input, string replacement = "")
        {
            return string.IsNullOrEmpty(input) ? null : Regex.Replace(input, "\\s", replacement, RegexOptions.Compiled);
        }

        private static char[] randomConstant ={
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
      };
        /// <summary>
        /// 生成指定长度的随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomNumber(this int length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(randomConstant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }

    }
    public static class ObjectExtension
    {

        public static bool DicKeyIsNullOrEmpty(this Dictionary<string, object> dic, string key)
        {
            if (dic == null)
                return true;
            if (!dic.ContainsKey(key)) return true;
            object value = dic[key];
            if (value == null || value.ToString() == "")
            {
                return true;
            }
            return false;
        }
        public static Dictionary<string, object> ReaderToDictionary(this IDataReader Reader)
        {
            List<Dictionary<string, object>> rowList = Reader.ReaderToDictionaryList();
            return rowList.Count() > 0 ? rowList[0] : null;
        }
        /// <summary>
        /// IDataReader转换成DictionaryList
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ReaderToDictionaryList(this IDataReader Reader)
        {
            List<Dictionary<string, object>> rowList = new List<Dictionary<string, object>>();
            try
            {
                while (Reader.Read())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    for (var fieldCount = 0; fieldCount < Reader.FieldCount; fieldCount++)
                    {
                        row.Add(Reader.GetName(fieldCount), Reader[fieldCount]);
                    }
                    rowList.Add(row);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally
            {
                Reader.Close();
                Reader.Dispose();
            }
            return rowList;
        }

        public static T DicToEntity<T>(this Dictionary<string, object> dic)
        {
            return new List<Dictionary<string, object>>() { dic }.DicToList<T>().ToList()[0];
        }
        public static List<T> DicToList<T>(this List<Dictionary<string, object>> dicList)
        {
            return dicList.DicToIEnumerable<T>().ToList();
        }
        public static object DicToList(this List<Dictionary<string, object>> dicList, Type type)
        {
            return typeof(ObjectExtension).GetMethod("DicToList")
               .MakeGenericMethod(new Type[] { type })
               .Invoke(typeof(ObjectExtension), new object[] { dicList });
        }

        public static IEnumerable<T> DicToIEnumerable<T>(this List<Dictionary<string, object>> dicList)
        {
            foreach (Dictionary<string, object> dic in dicList)
            {
                T model = Activator.CreateInstance<T>();
                foreach (PropertyInfo property in model.GetType()
                    .GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!dic.TryGetValue(property.Name, out object value)) continue;
                    property.SetValue(model, value?.ToString().ChangeType(property.PropertyType), null);
                }
                yield return model;
            }
        }
        /// <summary>
        /// IDataReader转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static List<T> ReaderToList<T>(this IDataReader Reader)
        {
            List<string> objectField = new List<string>(Reader.FieldCount);
            for (int i = 0; i < Reader.FieldCount; i++)
            {
                objectField.Add(Reader.GetName(i).ToLower());
            }
            List<T> objectList = new List<T>();
            try
            {
                while (Reader.Read())
                {
                    T model = Activator.CreateInstance<T>();
                    foreach (PropertyInfo property in model.GetType()
                        .GetProperties(BindingFlags.GetProperty
                        | BindingFlags.Public
                        | BindingFlags.Instance))
                    {
                        if (!objectField.Contains(property.Name.ToLower())) { continue; }
                        if (Reader[property.Name].IsNullOrEmpty()) { continue; }
                        property.SetValue(model, Reader[property.Name].ToString().ChangeType(property.PropertyType), null);
                    }
                    objectList.Add(model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                Reader.Close();
                Reader.Dispose();
            }
            return objectList;
        }


        public static object ChangeType(this object convertibleValue, Type type)
        {
            if (null == convertibleValue) return null;

            try
            {
                if (type == typeof(Guid) || type == typeof(Guid?))
                {
                    string value = convertibleValue.ToString();
                    if (value == "") return null;
                    return Guid.Parse(value);
                }

                if (!type.IsGenericType) return Convert.ChangeType(convertibleValue, type);
                if (type.ToString() == "System.Nullable`1[System.Boolean]" || type.ToString() == "System.Boolean")
                {
                    if (convertibleValue.ToString() == "0")
                        return false;
                    return true;
                }
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(type));
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <typeparam name="T">转换的元素类型。</typeparam>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        public static DataSet ToDataSet<T>(this IEnumerable<T> list, bool generic = true)
        {
            return ListToDataSet(list, generic);
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        public static DataSet ToDataSet(this IEnumerable list, bool generic = true)
        {
            return ListToDataSet(list, generic);
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <typeparam name="T">转换的元素类型。</typeparam>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        public static DataSet ToDataSet<T>(this IEnumerable list, bool generic = true)
        {
            return ListToDataSet(list, typeof(T), generic);
        }

        /// <summary>
        /// 将实例转换为集合数据集。
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        /// <param name="o">实例。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        public static DataSet ToListSet<T>(this T o, bool generic = true)
        {
            if (o is IEnumerable)
            {
                return ListToDataSet(o as IEnumerable, generic);
            }
            else
            {
                return ListToDataSet(new T[] { o }, generic);
            }
        }

        /// <summary>
        /// 将可序列化实例转换为XmlDocument。
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        /// <param name="o">实例。</param>
        /// <returns>XmlDocument。</returns>
        public static XmlDocument ToXmlDocument<T>(this T o)
        {
            XmlDocument xmlDocument = new XmlDocument
            {
                InnerXml = o.ToListSet().GetXml()
            };
            return xmlDocument;
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <param name="list">集合。</param>
        /// <param name="t">转换的元素类型。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>转换后的数据集。</returns>
        private static DataSet ListToDataSet(IEnumerable list, Type t, bool generic)
        {
            DataSet ds = new DataSet("Data");
            if (t == null)
            {
                if (list != null)
                {
                    foreach (var i in list)
                    {
                        if (i == null)
                        {
                            continue;
                        }
                        t = i.GetType();
                        break;
                    }
                }
                if (t == null)
                {
                    return ds;
                }
            }
            ds.Tables.Add(t.Name);
            //如果集合中元素为DataSet扩展涉及到的基本类型时，进行特殊转换。
            if (t.IsValueType || t == typeof(string))
            {
                ds.Tables[0].TableName = "Info";
                ds.Tables[0].Columns.Add(t.Name);
                if (list != null)
                {
                    foreach (var i in list)
                    {
                        DataRow addRow = ds.Tables[0].NewRow();
                        addRow[t.Name] = i;
                        ds.Tables[0].Rows.Add(addRow);
                    }
                }
                return ds;
            }
            //处理模型的字段和属性。
            var fields = t.GetFields();
            var properties = t.GetProperties();
            foreach (var j in fields)
            {
                if (!ds.Tables[0].Columns.Contains(j.Name))
                {
                    if (generic)
                    {
                        ds.Tables[0].Columns.Add(j.Name, j.FieldType);
                    }
                    else
                    {
                        ds.Tables[0].Columns.Add(j.Name);
                    }
                }
            }
            foreach (var j in properties)
            {
                if (!ds.Tables[0].Columns.Contains(j.Name))
                {
                    if (generic)
                    {
                        ds.Tables[0].Columns.Add(j.Name, j.PropertyType);
                    }
                    else
                    {
                        ds.Tables[0].Columns.Add(j.Name);
                    }
                }
            }
            if (list == null)
            {
                return ds;
            }
            //读取list中元素的值。
            foreach (var i in list)
            {
                if (i == null)
                {
                    continue;
                }
                DataRow addRow = ds.Tables[0].NewRow();
                foreach (var j in fields)
                {
                    MemberExpression field = Expression.Field(Expression.Constant(i), j.Name);
                    LambdaExpression lambda = Expression.Lambda(field, new ParameterExpression[] { });
                    Delegate func = lambda.Compile();
                    object value = func.DynamicInvoke();
                    addRow[j.Name] = value;
                }
                foreach (var j in properties)
                {
                    MemberExpression property = Expression.Property(Expression.Constant(i), j);
                    LambdaExpression lambda = Expression.Lambda(property, new ParameterExpression[] { });
                    Delegate func = lambda.Compile();
                    object value = func.DynamicInvoke();
                    addRow[j.Name] = value;
                }
                ds.Tables[0].Rows.Add(addRow);
            }
            return ds;
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <typeparam name="T">转换的元素类型。</typeparam>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否生成泛型数据集。</param>
        /// <returns>数据集。</returns>
        private static DataSet ListToDataSet<T>(IEnumerable<T> list, bool generic)
        {
            return ListToDataSet(list, typeof(T), generic);
        }

        /// <summary>
        /// 将集合转换为数据集。
        /// </summary>
        /// <param name="list">集合。</param>
        /// <param name="generic">是否转换为字符串形式。</param>
        /// <returns>转换后的数据集。</returns>
        private static DataSet ListToDataSet(IEnumerable list, bool generic)
        {
            return ListToDataSet(list, null, generic);
        }

        /// <summary>
        /// 获取DataSet第一表，第一行，第一列的值。
        /// </summary>
        /// <param name="ds">DataSet数据集。</param>
        /// <returns>值。</returns>
        public static object GetData(this DataSet ds)
        {
            if (
                ds == null
                || ds.Tables.Count == 0
                )
            {
                return string.Empty;
            }
            else
            {
                return ds.Tables[0].GetData();
            }
        }

        /// <summary>
        /// 获取DataTable第一行，第一列的值。
        /// </summary>
        /// <param name="dt">DataTable数据集表。</param>
        /// <returns>值。</returns>
        public static object GetData(this DataTable dt)
        {
            if (
                dt.Columns.Count == 0
                || dt.Rows.Count == 0
                )
            {
                return string.Empty;
            }
            else
            {
                return dt.Rows[0][0];
            }
        }

        /// <summary>
        /// 获取DataSet第一个匹配columnName的值。
        /// </summary>
        /// <param name="ds">数据集。</param>
        /// <param name="columnName">列名。</param>
        /// <returns>值。</returns>
        public static object GetData(this DataSet ds, string columnName)
        {
            if (
                ds == null
                || ds.Tables.Count == 0
                )
            {
                return string.Empty;
            }
            foreach (DataTable dt in ds.Tables)
            {
                object o = dt.GetData(columnName);
                if (!string.IsNullOrEmpty(o.ToString()))
                {
                    return o;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取DataTable第一个匹配columnName的值。
        /// </summary>
        /// <param name="dt">数据表。</param>
        /// <param name="columnName">列名。</param>
        /// <returns>值。</returns>
        public static object GetData(this DataTable dt, string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return GetData(dt);
            }
            if (
                dt.Columns.Count == 0
                || dt.Columns.IndexOf(columnName) == -1
                || dt.Rows.Count == 0
                )
            {
                return string.Empty;
            }
            return dt.Rows[0][columnName];
        }


        /// <summary>
        /// 将DateTime?转换为string类型信息。
        /// </summary>
        /// <param name="o">DateTime?。</param>
        /// <param name="format">标准或自定义日期和时间格式的字符串。</param>
        /// <param name="t">默认值。</param>
        /// <returns>string。</returns>
        public static string ToString(this DateTime? date, string format)
        {
            return date.Value.ToString(format);
        }











        private static Regex BoolRegex = new Regex("(?<info>(true|false))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 从object中获取bool类型信息。
        /// </summary>
        /// <param name="o">object。</param>
        /// <returns>bool。</returns>
        public static bool GetBool(this string value)
        {
            bool.TryParse(value, out bool result);
            return result;
        }

        private static Regex IntRegex = new Regex("(?<info>-?\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);



        private static Regex DecimalRegex = new Regex("(?<info>-?\\d+(\\.\\d+)?)", RegexOptions.IgnoreCase | RegexOptions.Singleline);



        /// <summary>
        /// 读取XElement节点的文本内容。
        /// </summary>
        /// <param name="xElement">XElement节点。</param>
        /// <param name="t">默认值。</param>
        /// <returns>文本内容。</returns>
        public static string Value(this XElement xElement, string t = default(string))
        {
            if (xElement == null)
            {
                return t;
            }
            else
            {
                return xElement.Value;
            }
        }

        /// <summary>
        /// 获取与指定键相关的值。
        /// </summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="dictionary">表示键/值对象的泛型集合。</param>
        /// <param name="key">键。</param>
        /// <param name="t">默认值。</param>
        /// <returns>值。</returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue t = default(TValue))
        {
            TValue value = default(TValue);
            if (dictionary == null || key == null)
            {
                return t;
            }
            if (!dictionary.TryGetValue(key, out value))
            {
                value = t;
            }
            return value;
        }

        /// <summary>
        /// 获取与指定键相关或者第一个的值。
        /// </summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="dictionary">表示键/值对象的泛型集合。</param>
        /// <param name="key">键。</param>
        /// <param name="t">默认值。</param>
        /// <returns>值。</returns>
        public static TValue GetFirstOrDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue t = default(TValue))
        {
            TValue value = default(TValue);
            if (dictionary == null || key == null)
            {
                return t;
            }
            if (!dictionary.TryGetValue(key, out value))
            {
                if (dictionary.Count() == 0)
                {
                    value = t;
                }
                else
                {
                    value = dictionary.FirstOrDefault().Value;
                }
            }
            return value;
        }

        /// <summary>
        /// 获取具有指定 System.Xml.Linq.XName 的第一个（按文档顺序）子元素。
        /// </summary>
        /// <param name="xContainer">XContainer。</param>
        /// <param name="xName">要匹配的 System.Xml.Linq.XName。</param>
        /// <param name="t">是否返回同名默认值。</param>
        /// <returns>与指定 System.Xml.Linq.XName 匹配的 System.Xml.Linq.XElement，或者为 null。</returns>
        public static XElement Element(this XContainer xContainer, XName xName, bool t)
        {
            XElement info;
            if (xContainer == null)
            {
                info = null;
            }
            else
            {
                info = xContainer.Element(xName);
            }
            if (t && info == null)
            {
                info = new XElement(xName);
            }
            return info;
        }

        /// <summary>
        /// 按文档顺序返回此元素或文档的子元素集合。
        /// </summary>
        /// <param name="xContainer">XContainer。</param>
        /// <param name="t">是否返回非空默认值。</param>
        /// <returns>System.Xml.Linq.XElement 的按文档顺序包含此System.Xml.Linq.XContainer 的子元素，或者非空默认值。</returns>
        public static IEnumerable<XElement> Elements(this XContainer xContainer, bool t)
        {
            IEnumerable<XElement> info;
            if (xContainer == null)
            {
                info = null;
            }
            else
            {
                info = xContainer.Elements();
            }
            if (t && info == null)
            {
                info = new List<XElement>();
            }
            return info;
        }

        /// <summary>
        /// 按文档顺序返回此元素或文档的经过筛选的子元素集合。集合中只包括具有匹配 System.Xml.Linq.XName 的元素。
        /// </summary>
        /// <param name="xContainer">XContainer。</param>
        /// <param name="xName">要匹配的 System.Xml.Linq.XName。</param>
        /// <param name="t">是否返回非空默认值。</param>
        /// <returns>System.Xml.Linq.XElement 的按文档顺序包含具有匹配System.Xml.Linq.XName 的 System.Xml.Linq.XContainer 的子级，或者非空默认值。</returns>
        public static IEnumerable<XElement> Elements(this XContainer xContainer, XName xName, bool t)
        {
            IEnumerable<XElement> info;
            if (xContainer == null)
            {
                info = null;
            }
            else
            {
                info = xContainer.Elements(xName);
            }
            if (t && info == null)
            {
                info = new List<XElement>();
            }
            return info;
        }

        public static Guid? GetGuid(this object value)
        {
            if (Guid.TryParse(value?.ToString(), out Guid guid))
            {
                return guid;
            }
            return null;
        }

        public static string ToString(this decimal? value, string format)
        {
            return value?.ToString(format);
        }

        public static string ToString(this int? value, string format)
        {
            return value?.ToString(format);
        }

        /// <summary>
        /// 获取默认非空字符串。
        /// </summary>
        /// <param name="s">首选默认非空字符串。</param>
        /// <param name="args">依次非空字符串可选项。</param>
        /// <returns>默认非空字符串。若无可选项则返回string.Empty。</returns>
        public static string DefaultStringIfEmpty(this string s, params string[] args)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            foreach (string i in args)
            {
                if (!string.IsNullOrEmpty(i) && !string.IsNullOrEmpty(i.Trim()))
                {
                    return i;
                }
            }

            return (s ?? string.Empty);
        }

        /// <summary>
        /// 对 URL 字符串进行编码。
        /// </summary>
        /// <param name="s">要编码的文本。</param>
        /// <param name="regex">匹配要编码的文本。</param>
        /// <param name="encoding">指定编码方案的 System.Text.Encoding 对象。</param>
        /// <returns>一个已编码的字符串。</returns>
        public static string ToUrlEncodeString(this string s, Regex regex = default(Regex), Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            if (regex == null)
            {
                return HttpUtility.UrlEncode(s, encoding);
            }
            List<string> l = new List<string>();
            foreach (char i in s)
            {
                string t = i.ToString();
                l.Add(regex.IsMatch(t) ? HttpUtility.UrlEncode(t, encoding) : t);
            }
            return string.Join(string.Empty, l);
        }

        /// <summary>
        /// 对 URL 字符串进行编码。
        /// </summary>
        /// <param name="s">要编码的文本。</param>
        /// <param name="regex">匹配要编码的文本。</param>
        /// <param name="encoding">指定编码方案的 System.Text.Encoding 对象。</param>
        /// <returns>一个已编码的字符串。</returns>
        public static string ToUrlEncodeString(this string s, string regex, Encoding encoding = null)
        {
            return ToUrlEncodeString(s, new Regex(regex), encoding);
        }

        /// <summary>
        /// 将日期转换为UNIX时间戳字符串
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToUnixTimeStamp(this DateTime date)
        {
            DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1));
            string timeStamp = date.Subtract(startTime).Ticks.ToString();
            return timeStamp.Substring(0, timeStamp.Length - 7);
        }

        private static readonly Regex MobileRegex = new Regex("^1[3-9][0-9]\\d{4,8}$");
        private static readonly Regex EmailRegex = new Regex("^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\\.[a-zA-Z0-9_-]{2,3}){1,2})$");

        /// <summary>
        /// 判断当前字符串是否是移动电话号码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsMobile(this string mobile)
        {
            return MobileRegex.IsMatch(mobile);
        }

        /// <summary>
        /// 判断当前字符串是否为邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(this string email)
        {
            return EmailRegex.IsMatch(email);
        }

    }
}
