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
using JIYUWU.Core.UserManager;
using Microsoft.VisualBasic.FileIO;
using SqlSugar.DistributedSystem.Snowflake;

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
            Response = new WebResponseContent(200);
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
        private static string[] _userIgnoreFields { get; set; }
        /// <summary>
        /// 获取配置的创建人ID创建时间创建人,修改人ID修改时间、修改人与数据相同的字段
        /// </summary>
        private static string[] UserIgnoreFields
        {
            get
            {
                if (_userIgnoreFields != null) return _userIgnoreFields;
                List<string> fields = new List<string>();
                //逻辑删除字段
                if (!string.IsNullOrEmpty(AppSetting.LogicDelField))
                {
                    fields.Add(AppSetting.LogicDelField);
                }
                fields.AddRange(CreateFields);
                fields.AddRange(ModifyFields);
                _userIgnoreFields = fields.ToArray();
                return _userIgnoreFields;
            }
        }
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
        public virtual WebResponseContent Add(SaveModel saveDataModel)
        {
            if (AddOnExecute != null)
            {
                Response = AddOnExecute(saveDataModel);
                if (CheckResponseResult()) return Response;
            }
            if (saveDataModel == null
                || saveDataModel.MainData == null
                || saveDataModel.MainData.Count == 0)
                return Response.Set(ResponseType.ParametersLack, false);

            saveDataModel.DetailData = saveDataModel.DetailData?.Where(x => x.Count > 0).ToList();
            Type type = typeof(T);

            if (saveDataModel.MainData.Count == 0)
                return Response.Error("保存的数据为空，请检查model是否配置正确!");

            //过滤逻辑删除
            var logicDelProperty = GetLogicDelProperty<T>();
            if (logicDelProperty != null)
            {
                saveDataModel.MainData[logicDelProperty.Name] = (int)DelStatus.正常;
            }

            UserInfo userInfo = UserContext.Current.UserInfo;
            saveDataModel.SetDefaultVal(AppSetting.CreateMember, userInfo);

            //2024.06.10增加数据版本号管理
            if (!string.IsNullOrEmpty(saveDataModel.DataVersionField))
            {
                saveDataModel.MainData.TryAdd(saveDataModel.DataVersionField, Guid.NewGuid().ToString());
            }

            PropertyInfo keyPro = type.GetKeyProperty();
            if (keyPro.PropertyType == typeof(Guid))
            {
                saveDataModel.MainData.Add(keyPro.Name, Guid.NewGuid());
            }
            else if (keyPro.PropertyType == typeof(long) && AppSetting.UseSnow)
            {
                saveDataModel.MainData.Add(keyPro.Name, new IdWorker(1,1).NextId());
            }
            else
            {
                saveDataModel.MainData.Remove(keyPro.Name);
            }

            //一对多
            //if (saveDataModel.Details != null && saveDataModel.Details.Count() > 0)
            //{
            //    return AddMultipleDetail(saveDataModel);
            //}

            //没有明细直接保存返回
            if (saveDataModel.DetailData == null || saveDataModel.DetailData.Count == 0)
            {
                T mainEntity = saveDataModel.MainData.DicToEntity<T>();

                if (base.AddOnExecuting != null)
                {
                    Response = base.AddOnExecuting(mainEntity, null);
                    if (CheckResponseResult()) return Response;
                }
                Response = repository.DbContextBeginTransaction(() =>
                {
                    repository.Add(mainEntity, true);
                    saveDataModel.MainData[keyPro.Name] = keyPro.GetValue(mainEntity);
                    Response.OK(ResponseType.SaveSuccess);
                    if (base.AddOnExecuted != null)
                    {
                        Response = base.AddOnExecuted(mainEntity, null);
                    }
                    return Response;
                });
                if (Response.Code == 200) Response.Data = new { data = saveDataModel.MainData };
                return Response;
            }

            //Type detailType = GetRealDetailType();

            //return typeof(ServiceBase<T, TRepository>)
            //    .GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)
            //    .MakeGenericMethod(new Type[] { detailType })
            //    .Invoke(this, new object[] { saveDataModel })
            //    as WebResponseContent;
            return Response.Error("暂不支持多表编辑!");
        }
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
        public virtual WebResponseContent Update(SaveModel saveModel)
        {

            if (UpdateOnExecute != null)
            {
                Response = UpdateOnExecute(saveModel);
                if (CheckResponseResult()) return Response;
            }
            if (saveModel == null)
                return Response.Error(ResponseType.ParametersLack);

            Type type = typeof(T);

            //设置修改时间,修改人的默认值
            UserInfo userInfo = UserContext.Current.UserInfo;
            saveModel.SetDefaultVal(AppSetting.ModifyMember, userInfo);

            PropertyInfo mainKeyProperty = type.GetKeyProperty();

            object keyDefaultVal = null;
            if (mainKeyProperty.PropertyType == typeof(string))
            {
                keyDefaultVal = "";
            }
            else
            {
                //获取主建类型的默认值用于判断后面数据是否正确,int long默认值为0,guid :0000-000....
                keyDefaultVal = mainKeyProperty.PropertyType.Assembly.CreateInstance(mainKeyProperty.PropertyType.FullName);//.ToString();
            }
            //判断是否包含主键
            if (mainKeyProperty == null
                || !saveModel.MainData.ContainsKey(mainKeyProperty.Name)
                || saveModel.MainData[mainKeyProperty.Name] == null
                )
            {
                return Response.Error(ResponseType.NoKey);
            }

            object mainKeyVal = saveModel.MainData[mainKeyProperty.Name];
            //判断主键类型是否正确
            (bool, string, object) validation = mainKeyProperty.ValidationValueForDbType(mainKeyVal).FirstOrDefault();
            if (!validation.Item1)
                return Response.Error(ResponseType.KeyError);

            object valueType = mainKeyVal.ToString().ChangeType(mainKeyProperty.PropertyType);
            //判断主键值是不是当前类型的默认值
            if (valueType == null ||
                (!valueType.GetType().Equals(mainKeyProperty.PropertyType)
                || valueType.ToString() == keyDefaultVal.ToString()
                ))
                return Response.Error(ResponseType.KeyError);

            if (saveModel.MainData.Count <= 1) return Response.Error("系统没有配置好编辑的数据，请检查model或设置编辑行再点击生成model!");

            Expression<Func<T, bool>> expression = mainKeyProperty.Name.CreateExpression<T>(mainKeyVal.ToString(), LinqExpressionType.Equal);
            if (!repository.Exists(expression)) return Response.Error("保存的数据不存在!");

            saveModel.DetailData = saveModel.DetailData == null
                                         ? new List<Dictionary<string, object>>()
                                         : saveModel.DetailData.Where(x => x.Count > 0).ToList();

            //没有明细的直接保存主表数据
            if (!(saveModel.DetailData.Count > 0 || saveModel.DelKeys?.Count > 0 || (saveModel.Details != null && saveModel.Details.Count > 0)))
            {
                saveModel.SetDefaultVal(AppSetting.ModifyMember, userInfo);
                T mainEntity = saveModel.MainData.DicToEntity<T>();
                if (UpdateOnExecuting != null)
                {
                    Response = UpdateOnExecuting(mainEntity, null, null, null);
                    if (CheckResponseResult()) return Response;
                }
                //不修改!CreateFields.Contains创建人信息
                repository.Update(mainEntity, type.GetEditField().Where(c => saveModel.MainData.Keys.Contains(c) && !CreateFields.Contains(c)).ToArray());
                if (base.UpdateOnExecuted == null)
                {
                    repository.SaveChanges();
                    Response.OK(ResponseType.SaveSuccess);
                }
                else
                {
                    Response = repository.DbContextBeginTransaction(() =>
                    {
                        repository.SaveChanges();
                        Response = UpdateOnExecuted(mainEntity, null, null, null);
                        return Response;
                    });
                }
                if (Response.Code == 200) Response.Data = new { data = mainEntity };
                if (Response.Code == 200 && string.IsNullOrEmpty(Response.Msg))
                    Response.OK(ResponseType.SaveSuccess);
                return Response;
            }

            return Response.Error("暂不支持多表编辑!");
        }
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
        public virtual WebResponseContent Del(object[] keys, bool delList = true)
        {
            Type entityType = typeof(T);
            var keyProperty = entityType.GetKeyProperty();
            if (keyProperty == null || keys == null || keys.Length == 0) return Response.Error(ResponseType.NoKeyDel);
            string tKey = keyProperty.Name;
            if (string.IsNullOrEmpty(tKey))
                return Response.Error("没有主键不能删除");
                        if (DelOnExecuting != null)
            {
                Response = DelOnExecuting(keys);
                if (CheckResponseResult()) return Response;
            }
            FieldType fieldType = entityType.GetFieldType();
            string joinKeys = (fieldType == FieldType.Int || fieldType == FieldType.BigInt)
                            ? string.Join(",", keys): $"'{string.Join("','", keys)}'";

            string sql = $"DELETE FROM {entityType.GetEntityTableName()} where {tKey} in ({joinKeys});";
            // 2020.08.06增加pgsql删除功能
            if (DBType.Name == DbCurrentType.PgSql.ToString())
            {
                sql = $"DELETE FROM \"public\".\"{entityType.GetEntityTableName()}\" where \"{tKey}\" in ({joinKeys});";
            }

            //可能在删除后还要做一些其它数据库新增或删除操作，这样就可能需要与删除保持在同一个事务中处理
            //采用此方法 repository.DbContextBeginTransaction(()=>{//do delete......and other});
            //做的其他操作，在DelOnExecuted中加入委托实现
            Response = repository.DbContextBeginTransaction(() =>
            {
                repository.ExecuteSqlCommand(sql);
                if (DelOnExecuted != null)
                {
                    Response = DelOnExecuted(keys);
                }
                return Response;
            });
            if (Response.Code == 200 && string.IsNullOrEmpty(Response.Msg)) Response.OK(ResponseType.DelSuccess);
            return Response;
        }
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

        #region 返回
        private bool CheckResponseResult()
        {
            return !Response.Code.Equals("200");
        }
        #endregion
    }

    public enum DelStatus
    {
        正常 = 0,
        已删除 = 1
    }
}
