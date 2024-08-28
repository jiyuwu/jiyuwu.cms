using JIYUWU.Core.Common;
using JIYUWU.Core.UserManager;
using JIYUWU.Entity.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Extension
{
    public static class EntityProperty
    {
        /// <summary>
        /// 获取类的单个指定属性的值(只会返回第一个属性的值)
        /// </summary>
        /// <param name="member">当前类</param>
        /// <param name="type">指定的类</param>
        /// <param name="expression">指定属性的值 格式 Expression<Func<entityt, object>> exp = x => new { x.字段1, x.字段2 };</param>
        /// <returns></returns>
        public static string GetTypeCustomValue<TEntity>(this MemberInfo member, Expression<Func<TEntity, object>> expression)
        {
            var propertyKeyValues = member.GetTypeCustomValues(expression);
            if (propertyKeyValues == null || propertyKeyValues.Count == 0)
            {
                return null;
            }
            return propertyKeyValues.First().Value ?? "";
        }
        /// <summary>
        /// 获取类的多个指定属性的值
        /// </summary>
        /// <param name="member">当前类</param>
        /// <param name="type">指定的类</param>
        /// <param name="expression">指定属性的值 格式 Expression<Func<entityt, object>> exp = x => new { x.字段1, x.字段2 };</param>
        /// <returns>返回的是字段+value</returns>
        public static Dictionary<string, string> GetTypeCustomValues<TEntity>(this MemberInfo member, Expression<Func<TEntity, object>> expression)
        {
            var attr = member.GetTypeCustomAttributes(typeof(TEntity));
            if (attr == null)
            {
                return null;
            }

            string[] propertyName = expression.GetExpressionProperty();
            Dictionary<string, string> propertyKeyValues = new Dictionary<string, string>();

            foreach (PropertyInfo property in attr.GetType().GetProperties())
            {
                if (propertyName.Contains(property.Name))
                {
                    propertyKeyValues[property.Name] = (property.GetValue(attr) ?? string.Empty).ToString();
                }
            }
            return propertyKeyValues;
        }
        /// <summary>
        /// 获取属性的指定属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetTypeCustomAttributes(this MemberInfo member, Type type)
        {
            object[] obj = member.GetCustomAttributes(type, false);
            if (obj.Length == 0) return null;
            return obj[0];
        }
        public static ISugarQueryable<T> WhereNotEmpty<T>(this ISugarQueryable<T> queryable, [NotNull] Expression<Func<T, object>> field, string value, LinqExpressionType linqExpression = LinqExpressionType.Equal)
        {
            if (string.IsNullOrEmpty(value)) return queryable;
            return queryable.Where(field.GetExpressionPropertyFirst<T>().CreateExpression<T>(value, linqExpression));
        }
        public static string GetExpressionPropertyFirst<TEntity>(this Expression<Func<TEntity, object>> properties)
        {
            string[] arr = properties.GetExpressionProperty();
            if (arr.Length > 0)
                return arr[0];
            return "";
        }
        public static TSource SetCreateDefaultVal<TSource>(this TSource source, UserInfo userInfo = null)
        {
            return SetDefaultVal(source, AppSetting.CreateMember, userInfo);
        }
        public static TSource SetModifyDefaultVal<TSource>(this TSource source, UserInfo userInfo = null)
        {
            return SetDefaultVal(source, AppSetting.ModifyMember, userInfo);
        }
        /// <summary>
        /// 
        /// 设置默认字段的值如:"CreateID", "Creator", "CreateDate"，"ModifyID", "Modifier", "ModifyDate"
        /// </summary>
        /// <param name="saveDataModel"></param>
        /// <param name="setType">true=新增设置"CreateID", "Creator", "CreateDate"值
        /// false=编辑设置"ModifyID", "Modifier", "ModifyDate"值
        /// </param>
        private static TSource SetDefaultVal<TSource>(this TSource source, TableDefaultColumns defaultColumns, UserInfo userInfo = null)
        {
            userInfo = userInfo ?? UserContext.Current.UserInfo;
            foreach (PropertyInfo property in typeof(TSource).GetProperties())
            {
                string filed = property.Name.ToLower();
                if (filed == defaultColumns.UserIdField?.ToLower())
                    property.SetValue(source, userInfo.User_Id);

                if (filed == defaultColumns.UserNameField?.ToLower())
                    property.SetValue(source, userInfo.UserTrueName);

                if (filed == defaultColumns.DateField?.ToLower())
                    property.SetValue(source, DateTime.Now);
            }
            return source;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">字段名</param>
        /// <param name="propertyValue">表达式的值</param>
        /// <param name="expressionType">创建表达式的类型,如:p=>p.propertyName != propertyValue 
        /// p=>p.propertyName.Contains(propertyValue)</param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> CreateExpression<T>(
          this string propertyName,
          object propertyValue,
          ParameterExpression parameter,
          LinqExpressionType expressionType)
        {
            Type proType = typeof(T).GetProperty(propertyName).PropertyType;
            //创建节点变量如p=>的节点p
            //  parameter ??= Expression.Parameter(typeof(T), "p");//创建参数p
            parameter = parameter ?? Expression.Parameter(typeof(T), "p");

            //创建节点的属性p=>p.name 属性name
            MemberExpression memberProperty = Expression.PropertyOrField(parameter, propertyName);
            if (expressionType == LinqExpressionType.In)
            {
                if (!(propertyValue is System.Collections.IList list) || list.Count == 0) return x => false;

                var res = typeof(LambdaExt).GetMethod("GetContainsExpression", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
                           .MakeGenericMethod(new Type[] { typeof(T), proType })
                           .Invoke(null, new object[] { propertyName, propertyValue, parameter }) as Expression<Func<T, bool>>;
                return res;
            }

            //  object value = propertyValue;
            ConstantExpression constant = proType.ToString() == "System.String"
                ? Expression.Constant(propertyValue) : Expression.Constant(propertyValue.ToString().ChangeType(proType));

            // DateTime只选择了日期的时候自动在结束日期加一天，修复DateTime类型使用日期区间查询无法查询到结束日期的问题
            if ((proType == typeof(DateTime) || proType == typeof(DateTime?)) && expressionType == LinqExpressionType.LessThanOrEqual && propertyValue.ToString().Length == 10)
            {
                expressionType = LinqExpressionType.LessThan;
                constant = Expression.Constant(Convert.ToDateTime(propertyValue.ToString()).AddDays(1));
            }

            UnaryExpression member = Expression.Convert(memberProperty, constant.Type);
            Expression<Func<T, bool>> expression;
            switch (expressionType)
            {
                //p=>p.propertyName != propertyValue
                case LinqExpressionType.NotEqual:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(member, constant), parameter);
                    break;
                //   p => p.propertyName > propertyValue
                case LinqExpressionType.GreaterThan:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(member, constant), parameter);
                    break;
                //   p => p.propertyName < propertyValue
                case LinqExpressionType.LessThan:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.LessThan(member, constant), parameter);
                    break;
                // p => p.propertyName >= propertyValue
                case LinqExpressionType.ThanOrEqual:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(member, constant), parameter);
                    break;
                // p => p.propertyName <= propertyValue
                case LinqExpressionType.LessThanOrEqual:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(member, constant), parameter);
                    break;
                //   p => p.propertyName.Contains(propertyValue)
                // p => !p.propertyName.Contains(propertyValue)
                case LinqExpressionType.Like:
                case LinqExpressionType.NotLike:
                case LinqExpressionType.Contains:
                case LinqExpressionType.NotContains:
                    MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    constant = Expression.Constant(propertyValue, typeof(string));
                    if (expressionType == LinqExpressionType.Like || expressionType == LinqExpressionType.Contains)
                    {
                        expression = Expression.Lambda<Func<T, bool>>(Expression.Call(member, method, constant), parameter);
                    }
                    else
                    {
                        expression = Expression.Lambda<Func<T, bool>>(Expression.Not(Expression.Call(member, method, constant)), parameter);
                    }
                    break;
                case LinqExpressionType.LikeStart:
                case LinqExpressionType.LikeEnd:
                    string m = expressionType == LinqExpressionType.LikeStart ? "StartsWith" : "EndsWith";
                    var startsWithMethod = typeof(string).GetMethod(m, new[] { typeof(string) });
                    var searchTermConstant = Expression.Constant(propertyValue, typeof(string));
                    var startsWithCall = Expression.Call(member, startsWithMethod, searchTermConstant);
                    expression = Expression.Lambda<Func<T, bool>>(startsWithCall, parameter);
                    break;
                default:
                    expression = Expression.Lambda<Func<T, bool>>(Expression.Equal(member, constant), parameter);
                    break;
            }
            return expression;
        }
        /// <summary>
        /// 获取对象里指定成员名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="properties"> 格式 Expression<Func<entityt, object>> exp = x => new { x.字段1, x.字段2 };或x=>x.Name</param>
        /// <returns></returns>
        public static string[] GetExpressionProperty<TEntity>(this Expression<Func<TEntity, object>> properties)
        {
            if (properties == null)
                return new string[] { };
            if (properties.Body is NewExpression)
                return ((NewExpression)properties.Body).Members.Select(x => x.Name).ToArray();
            if (properties.Body is MemberExpression)
                return new string[] { ((MemberExpression)properties.Body).Member.Name };
            if (properties.Body is UnaryExpression)
                return new string[] { ((properties.Body as UnaryExpression).Operand as MemberExpression).Member.Name };
            throw new Exception("未实现的表达式");
        }
        public static SplitTableAttribute GetSugarSplitTable(this Type type)
        {
            Attribute attribute = type.GetCustomAttribute(typeof(SplitTableAttribute));
            if (attribute != null && attribute is SplitTableAttribute)
            {
                return attribute as SplitTableAttribute;
            }
            return null;
        }
        public static string GetKeyName(this Type typeinfo)
        {
            return typeinfo.GetProperties().GetKeyName();
        }
        public static string GetKeyType(this Type typeinfo)
        {
            string keyType = typeinfo.GetProperties().GetKeyName(true);
            if (keyType == "varchar")
            {
                return "varchar(max)";
            }
            else if (keyType != "nvarchar")
            {
                return keyType;
            }
            else
            {
                return "nvarchar(max)";
            }
        }
        public static string GetKeyName(this PropertyInfo[] properties)
        {
            return properties.GetKeyName(false);
        }
        /// <summary>
        /// 获取key列名
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="keyType">true获取key对应类型,false返回对象Key的名称</param>
        /// <returns></returns>
        public static string GetKeyName(this PropertyInfo[] properties, bool keyType)
        {
            string keyName = string.Empty;
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (!propertyInfo.IsKey())
                    continue;
                if (!keyType)
                    return propertyInfo.Name;
                var attributes = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false);
                //如果没有ColumnAttribute的需要单独再验证，下面只验证有属性的
                if (attributes.Length > 0)
                    return ((ColumnAttribute)attributes[0]).TypeName.ToLower();
                else
                    return GetColumType(new PropertyInfo[] { propertyInfo }, true)[propertyInfo.Name];
            }
            return keyName;
        }
        public static Dictionary<string, string> GetColumType(this PropertyInfo[] properties, bool containsKey)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (PropertyInfo property in properties)
            {
                if (!containsKey && property.IsKey())
                {
                    continue;
                }
                var keyVal = GetColumnType(property, true);
                dictionary.Add(keyVal.Key, keyVal.Value);
            }
            return dictionary;
        }
        /// <summary>
        /// 返回属性的字段及数据库类型
        /// </summary>
        /// <param name="property"></param>
        /// <param name="lenght">是否包括后字段具体长度:nvarchar(100)</param>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetColumnType(this PropertyInfo property, bool lenght = false)
        {
            string colType = "";
            object objAtrr = property.GetTypeCustomAttributes(typeof(ColumnAttribute), out bool asType);
            if (asType)
            {
                colType = ((ColumnAttribute)objAtrr).TypeName.ToLower();
                if (!string.IsNullOrEmpty(colType))
                {
                    //不需要具体长度直接返回
                    if (!lenght)
                    {
                        return new KeyValuePair<string, string>(property.Name, colType);
                    }
                    if (colType == "decimal" || colType == "double" || colType == "float")
                    {
                        objAtrr = property.GetTypeCustomAttributes(typeof(DisplayFormatAttribute), out asType);
                        colType += "(" + (asType ? ((DisplayFormatAttribute)objAtrr).DataFormatString : "18,5") + ")";

                    }
                    ///如果是string,根据 varchar或nvarchar判断最大长度
                    if (property.PropertyType.ToString() == "System.String")
                    {
                        colType = colType.Split("(")[0];
                        objAtrr = property.GetTypeCustomAttributes(typeof(MaxLengthAttribute), out asType);
                        if (asType)
                        {
                            int length = ((MaxLengthAttribute)objAtrr).Length;
                            colType += "(" + (length < 1 || length > (colType.StartsWith("n") ? 8000 : 4000) ? "max" : length.ToString()) + ")";
                        }
                        else
                        {
                            colType += "(max)";
                        }
                    }
                    return new KeyValuePair<string, string>(property.Name, colType);
                }
            }
            if (entityMapDbColumnType.TryGetValue(property.PropertyType, out string value))
            {
                colType = value;
            }
            else
            {
                colType = SqlDbTypeName.NVarChar;
            }
            if (lenght && colType == SqlDbTypeName.NVarChar)
            {
                colType = "nvarchar(max)";
            }
            return new KeyValuePair<string, string>(property.Name, colType);
        }
        private static readonly Dictionary<Type, string> entityMapDbColumnType = new Dictionary<Type, string>() {
                    {typeof(int),SqlDbTypeName.Int },
                    {typeof(int?),SqlDbTypeName.Int },
                    {typeof(long),SqlDbTypeName.BigInt },
                    {typeof(long?),SqlDbTypeName.BigInt },
                    {typeof(decimal),"decimal(18, 5)" },
                    {typeof(decimal?),"decimal(18, 5)"  },
                    {typeof(double),"decimal(18, 5)" },
                    {typeof(double?),"decimal(18, 5)" },
                    {typeof(float),"decimal(18, 5)" },
                    {typeof(float?),"decimal(18, 5)" },
                    {typeof(Guid),"UniqueIdentifier" },
                    {typeof(Guid?),"UniqueIdentifier" },
                    {typeof(byte),"tinyint" },
                    {typeof(byte?),"tinyint" },
                    {typeof(string),"nvarchar" }
        };
        /// <summary>
        /// 获取PropertyInfo指定属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetTypeCustomAttributes(this PropertyInfo propertyInfo, Type type, out bool asType)
        {
            object[] attributes = propertyInfo.GetCustomAttributes(type, false);
            if (attributes.Length == 0)
            {
                asType = false;
                return new string[0];
            }
            asType = true;
            return attributes[0];
        }
        /// <summary>
        /// 获取主键字段
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static PropertyInfo GetKeyProperty(this Type entity)
        {
            return entity.GetProperties().GetKeyProperty();
        }
        public static PropertyInfo GetKeyProperty(this PropertyInfo[] properties)
        {
            return properties.Where(c => c.IsKey()).FirstOrDefault();
        }
        public static bool IsKey(this PropertyInfo propertyInfo)
        {
            object[] keyAttributes = propertyInfo.GetCustomAttributes(typeof(KeyAttribute), false);
            if (keyAttributes.Length > 0)
                return true;
            return false;
        }
    }
}
