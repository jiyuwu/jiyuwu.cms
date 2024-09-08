using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using JIYUWU.Core.Extension;
using CC = JIYUWU.Core.CacheManager;
using JIYUWU.Entity.Base;
using JIYUWU.Core.Filter;
using JIYUWU.Core.DbSqlSugar;
using Microsoft.AspNetCore.Routing;

namespace JIYUWU.Core.Common
{
    public abstract class ServiceBase<T, TRepository> : ServiceFunFilter<T>
            where T : BaseEntity, new()
            where TRepository : IRepository<T>
    {
        public ServiceBase()
        {

        }
        public CC.ICacheService CacheContext
        {
            get
            {
                return AutofacContainerModule.GetService<CC.ICacheService>();
            }
        }

        public Microsoft.AspNetCore.Http.HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }
        private WebResponseContent Response { get; set; }

        protected IRepository<T> repository;

        private PropertyInfo[] _propertyInfo { get; set; } = null;
        private PropertyInfo[] TProperties
        {
            get
            {
                if (_propertyInfo != null)
                {
                    return _propertyInfo;
                }
                _propertyInfo = typeof(T).GetProperties();
                return _propertyInfo;
            }
        }

        public ServiceBase(TRepository repository)
        {
            Response = new WebResponseContent(true);
            this.repository = repository;
        }

        protected virtual void Init(IRepository<T> repository)
        {

        }

        #region 获取列表数据

        #region 单列表
        /// <summary>
        /// 加载页面数据
        /// </summary>
        /// <param name="loadSingleParameters"></param>
        /// <returns></returns>
        public virtual PageGridData<T> GetPageData(PageDataOptions options)
        {
            //获取排序字段
            Dictionary<string, QueryOrderBy> orderbyDic = GetPageDataSort(options, TProperties);
            var queryable = repository.DbContext.Set<T>();
            PageGridData<T> pageGridData = new PageGridData<T>();
            if (QueryRelativeExpression != null)
            {
                queryable = QueryRelativeExpression.Invoke(queryable);
            }

            //过滤逻辑删除
            var logicDelProperty = GetLogicDelProperty<T>();
            if (logicDelProperty != null)
            {
                queryable = queryable.Where(logicDelProperty.Name.CreateExpression<T>((int)DelStatus.正常, LinqExpressionType.Equal));
            }


            if (options.Export)
            {
                queryable = queryable.GetIQueryableOrderBy(orderbyDic);
                if (Limit > 0)
                {
                    queryable = queryable.Take(Limit);
                }
                pageGridData.rows = FilterQueryableAuthFields(queryable);
            }
            else
            {
                //查询界面统计求等字段
                if (SummaryExpress != null)
                {
                    pageGridData.summary = SummaryExpress.Invoke(queryable);
                }
                queryable = repository.IQueryablePage(queryable,
                               options.Page,
                               options.Rows,
                               out int rowCount,
                               orderbyDic);
                pageGridData.rows = FilterQueryableAuthFields(queryable);
                pageGridData.total = rowCount;

            }
            GetPageDataOnExecuted?.Invoke(pageGridData);
            return pageGridData;

        }
        private List<T> FilterQueryableAuthFields(ISugarQueryable<T> queryable)
        {
            string tableName = typeof(T).Name;
            var authFields = new string[0]; //RoleContext.GetCurrentRoleAuthFields(tableName);
            if (authFields.Length == 0)
            {
                return queryable.ToList();
            }
            var source = typeof(T);
            var target = typeof(T);

            var t = Expression.Parameter(source, "t");

            List<MemberAssignment> assignments = new();

            //获取隐藏的字段
            var hideFields =new List<string>();//TableColumnContext.GetTableHideFields(tableName);

            var fields = source.GetProperties().Where(x => authFields.Contains(x.Name) || hideFields.Contains(x.Name)).Select(s => s.Name).ToList();

            foreach (var item in fields)
            {
                var member1 = Expression.MakeMemberAccess(t, source.GetProperty(item));

                var member2 = Expression.Bind(target.GetProperty(item), member1);
                assignments.Add(member2);
            }
            var newExpression = Expression.New(target);
            var memberInit = Expression.MemberInit(newExpression, assignments);
            var expression = (Expression<Func<T, T>>)Expression.Lambda(memberInit, t);
            return queryable.Select(expression).ToList();

        }
        private PropertyInfo GetLogicDelProperty<MyEntity>()
        {
            return GetLogicDelProperty(typeof(MyEntity));
        }
        private PropertyInfo GetLogicDelProperty(Type type)
        {
            if (string.IsNullOrEmpty(AppSetting.LogicDelField))
            {
                return null;
            }
            return type.GetProperty(AppSetting.LogicDelField);
        }
        #endregion

        #region 明细
        public virtual object GetDetailPage(PageDataOptions pageData)
        {
            var tables = typeof(T).GetCustomAttribute<EntityAttribute>();
            if (tables == null)
            {
                return null;
            }
            string keyName = typeof(T).GetKeyName();

            Type detailType = null;

            if (string.IsNullOrEmpty(pageData.TableName) && string.IsNullOrEmpty(pageData.DetailTable))
            {
                detailType = tables.DetailTable.FirstOrDefault();
            }
            else
            {  //三级明细表查询
                if (!string.IsNullOrEmpty(pageData.DetailTable))
                {
                    //获取二级明细表
                    detailType = tables.DetailTable.Where(c => c.Name == pageData.DetailTable).FirstOrDefault();
                    keyName = detailType.GetKeyName();
                    detailType = detailType.GetCustomAttribute<EntityAttribute>()?.DetailTable
                                       ?.Where(x => x.Name == pageData.TableName)?.FirstOrDefault();
                }
                else
                {
                    //多表二级明细表查询
                    detailType = tables.DetailTable.Where(c => c.Name == pageData.TableName).FirstOrDefault();
                }
            }

            if (detailType == null)
            {
                string message = $"未找到配置{pageData.TableName},请检查代码生成器明细表配置及是否生成model";
                Console.WriteLine(message);
                return new { message = message };
            }
            object obj = typeof(ServiceBase<T, TRepository>)
                 .GetMethod("GetDetailPage", BindingFlags.Instance | BindingFlags.NonPublic)
                 .MakeGenericMethod(new Type[] { detailType }).Invoke(this, new object[] { pageData, keyName });
            return obj;
        }
        protected override object GetDetailSummary<Detail>(ISugarQueryable<Detail> queryeable)
        {
            return null;
        }

        private PageGridData<Detail> GetDetailPage<Detail>(PageDataOptions options, string keyName) where Detail : class
        {
            //校验查询值，排序字段，分页大小规则待完
            PageGridData<Detail> gridData = new PageGridData<Detail>();
            if (options.Value == null) return gridData;
            ////主表主键字段
            //string keyName = typeof(T).GetKeyName();

            //生成查询条件
            Expression<Func<Detail, bool>> whereExpression = keyName.CreateExpression<Detail>(options.Value, LinqExpressionType.Equal);

            var queryeable = repository.DbContext.Set<Detail>().Where(whereExpression);

            gridData.total = queryeable.Count();
            options.Sort = options.Sort ?? typeof(Detail).GetKeyName();
            Dictionary<string, QueryOrderBy> orderBy = GetPageDataSort(options, typeof(Detail).GetProperties());

            gridData.rows = queryeable
                 .GetISugarQueryableOrderBy(orderBy)
                .Skip((options.Page - 1) * options.Rows)
                .Take(options.Rows)
                .ToList();

            //查询界面统计求等字段
            queryeable = repository.DbContext.Set<Detail>().Where(whereExpression);
            gridData.summary = GetDetailSummary<Detail>(queryeable);
            return gridData;
        }
        private const string _asc = "asc";
        /// <summary>
        /// 生成排序字段
        /// </summary>
        /// <param name="pageData"></param>
        /// <param name="propertyInfo"></param>
        private Dictionary<string, QueryOrderBy> GetPageDataSort(PageDataOptions pageData, PropertyInfo[] propertyInfo)
        {
            if (base.OrderByExpression != null)
            {
                return base.OrderByExpression.GetExpressionToDic();
            }
            if (!string.IsNullOrEmpty(pageData.Sort))
            {
                if (pageData.Sort.Contains(","))
                {
                    var sortArr = pageData.Sort.Split(",").Where(x => propertyInfo.Any(c => c.Name == x)).Select(s => s).Distinct().ToList();
                    Dictionary<string, QueryOrderBy> sortDic = new Dictionary<string, QueryOrderBy>();
                    foreach (var name in sortArr)
                    {
                        sortDic[name] = pageData.Order?.ToLower() == _asc ? QueryOrderBy.Asc : QueryOrderBy.Desc;
                    }
                    return sortDic;
                }
                else if (propertyInfo.Any(x => x.Name == pageData.Sort))
                {
                    return new Dictionary<string, QueryOrderBy>() { {
                            pageData.Sort,
                            pageData.Order?.ToLower() == _asc? QueryOrderBy.Asc: QueryOrderBy.Desc
                     } };
                }
            }
            //如果没有排序字段，则使用主键作为排序字段

            PropertyInfo property = propertyInfo.GetKeyProperty();
            //如果主键不是自增类型则使用appsettings.json中CreateMember->DateField配置的创建时间作为排序
            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(long))
            {
                if (!propertyInfo.Any(x => x.Name.ToLower() == pageData.Sort))
                {
                    pageData.Sort = propertyInfo.GetKeyName();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(AppSetting.CreateMember.DateField)
                    && propertyInfo.Any(x => x.Name == AppSetting.CreateMember.DateField))
                {
                    pageData.Sort = AppSetting.CreateMember.DateField;
                }
                else
                {
                    pageData.Sort = propertyInfo.GetKeyName();
                }
            }
            return new Dictionary<string, QueryOrderBy>() { {
                    pageData.Sort, pageData.Order?.ToLower() == _asc? QueryOrderBy.Asc: QueryOrderBy.Desc
                } };
        }
        #endregion

        #endregion


        #region 编辑及新增自动字段
        /// <summary>
        /// 获取配置的创建人ID创建时间创建人,修改人ID修改时间、修改人与数据相同的字段
        /// </summary>
        private static string[] _createFields { get; set; }
        private static string[] CreateFields
        {
            get
            {
                if (_createFields != null) return _createFields;
                _createFields = AppSetting.CreateMember.GetType().GetProperties()
                    .Select(x => x.GetValue(AppSetting.CreateMember)?.ToString())
                    .Where(w => !string.IsNullOrEmpty(w)).ToArray();
                return _createFields;
            }
        }

        private static string[] _modifyFields { get; set; }
        private static string[] ModifyFields
        {
            get
            {
                if (_modifyFields != null) return _modifyFields;
                _modifyFields = AppSetting.ModifyMember.GetType().GetProperties()
                    .Select(x => x.GetValue(AppSetting.ModifyMember)?.ToString())
                    .Where(w => !string.IsNullOrEmpty(w)).ToArray();
                return _modifyFields;
            }
        }

        #endregion

        #region 新增
        public virtual int Add(T entity)
        {
            // 新增自动字段
            if (CreateFields.Length > 0)
            {
                foreach (var field in CreateFields)
                {
                    var propertyInfo = entity.GetType().GetProperty(field);
                    if (propertyInfo != null && propertyInfo.CanWrite)
                    {
                        // 根据字段名称进行赋值，假设 AppSetting.CreateMember 中有对应的值
                        var value = AppSetting.CreateMember.GetType().GetProperty(field)?.GetValue(AppSetting.CreateMember);
                        propertyInfo.SetValue(entity, value);
                    }
                }
            }

            // 调用仓储层的添加方法，将实体插入数据库
            return repository.SqlSugarClient.Insertable(entity).ExecuteCommand();
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改实体，并自动处理修改人ID、修改时间等字段
        /// </summary>
        /// <param name="entity">要修改的实体对象</param>
        /// <returns>更新的记录数</returns>
        public virtual int Update(T entity)
        {
            // 修改自动处理的修改字段
            if (ModifyFields.Length > 0)
            {
                foreach (var field in ModifyFields)
                {
                    var propertyInfo = entity.GetType().GetProperty(field);
                    if (propertyInfo != null && propertyInfo.CanWrite)
                    {
                        // 从 AppSetting.ModifyMember 获取修改人、修改时间等自动字段的值
                        var value = AppSetting.ModifyMember.GetType().GetProperty(field)?.GetValue(AppSetting.ModifyMember);
                        propertyInfo.SetValue(entity, value);
                    }
                }
            }

            // 调用仓储层的 Updateable 方法进行更新操作
            return repository.SqlSugarClient.Updateable(entity).ExecuteCommand();
        }
        #endregion

        #region 删除
        /// <summary>
        /// 根据实体对象删除数据
        /// </summary>
        /// <param name="entity">要删除的实体对象</param>
        /// <returns>删除的记录数</returns>
        public virtual int Delete(T entity)
        {
            // 调用仓储层的 Deleteable 方法，根据实体删除
            return repository.SqlSugarClient.Deleteable(entity).ExecuteCommand();
        }

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <param name="id">实体的主键ID</param>
        /// <returns>删除的记录数</returns>
        public virtual int DeleteById(object id)
        {
            // 调用仓储层的 Deleteable 方法，根据主键ID删除
            return repository.SqlSugarClient.Deleteable<T>().In(id).ExecuteCommand();
        }

        /// <summary>
        /// 根据多个主键删除数据
        /// </summary>
        /// <param name="ids">多个主键ID</param>
        /// <returns>删除的记录数</returns>
        public virtual int DeleteByIds(object[] ids)
        {
            // 调用仓储层的 Deleteable 方法，批量删除多个主键ID对应的记录
            return repository.SqlSugarClient.Deleteable<T>().In(ids).ExecuteCommand();
        }
        #endregion
    }

    public enum DelStatus
    {
        正常 = 0,
        已删除 = 1
    }
}
