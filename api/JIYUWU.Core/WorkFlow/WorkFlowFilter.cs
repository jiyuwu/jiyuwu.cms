using JIYUWU.Core.Common;
using JIYUWU.Core.Extension;
using System.Linq.Expressions;

namespace JIYUWU.Core.WorkFlow
{
    public static class WorkFlowFilter
    {
        public static bool CheckFilter<T>(this List<FieldFilter> filters, List<T> entities, object where) where T : class
        {
            try
            {
                if (where is Func<T, bool> predicate)
                {
                    return entities.Any(predicate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"流程表达式解析异常: {ex.Message} {ex.InnerException}");

                if (filters != null)
                {
                    var expression = filters.Create<T>().Compile();
                    return entities.Any(expression);
                }
            }
            return false;
        }

        public static Expression<Func<T, bool>> Create<T>(this List<FieldFilter> filters) where T : class
        {
            if (filters == null || !filters.Any(f => !string.IsNullOrEmpty(f.Field) && !string.IsNullOrEmpty(f.Value)))
            {
                return x => true;
            }

            var fields = typeof(T).GetProperties().Select(s => s.Name).ToHashSet();
            Expression<Func<T, bool>> andExpression = x => true;
            Expression<Func<T, bool>> orExpression = null;

            foreach (var filter in filters.Where(f => !string.IsNullOrEmpty(f.Field) && !string.IsNullOrEmpty(f.Value)))
            {
                if (!fields.Contains(filter.Field))
                {
                    var msg = $"表【{typeof(T).GetEntityTableName(false)}】不存在字段【{filter.Field}】";
                    Console.WriteLine(msg);
                    throw new Exception(msg);
                }

                filter.Value = filter.Value.Trim();
                LinqExpressionType expressionType = GetExpressionType(filter.FilterType);

                if (expressionType == LinqExpressionType.In)
                {
                    var values = filter.Value.Split(',').Where(v => !string.IsNullOrEmpty(v)).ToList();
                    if (values.Any())
                    {
                        andExpression = andExpression.And(filter.Field.CreateExpression<T>(values, expressionType));
                    }
                }
                else if (filter.FilterType == "or")
                {
                    orExpression ??= x => false;
                    orExpression = orExpression.Or(filter.Field.CreateExpression<T>(filter.Value, LinqExpressionType.Equal));
                }
                else
                {
                    andExpression = andExpression.And(filter.Field.CreateExpression<T>(filter.Value, expressionType));
                }
            }

            return orExpression != null ? andExpression.And(orExpression) : andExpression;
        }

        private static LinqExpressionType GetExpressionType(string filterType)
        {
            return filterType switch
            {
                "!=" => LinqExpressionType.NotEqual,
                ">" => LinqExpressionType.GreaterThan,
                ">=" => LinqExpressionType.ThanOrEqual,
                "小于" or "<" => LinqExpressionType.LessThan,
                "<=" => LinqExpressionType.LessThanOrEqual,
                "in" => LinqExpressionType.In,
                "like" => LinqExpressionType.Like,
                _ => LinqExpressionType.Equal
            };
        }
    }
}