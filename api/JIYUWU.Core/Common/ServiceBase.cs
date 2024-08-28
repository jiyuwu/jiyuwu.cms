using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using JIYUWU.Core.Extension;
using CC = JIYUWU.Core.CacheManager;
using JIYUWU.Entity.Base;
using JIYUWU.Core.Filter;
using JIYUWU.Core.DbSqlSugar;

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

        protected virtual Type GetRealDetailType()
        {
            return typeof(T).GetCustomAttribute<EntityAttribute>()?.DetailTable?[0];
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
            //这里sqlsugar会把IQueryablePage里面的排序字段也添加进去了就会导致异常2023.10.17
            queryeable = repository.DbContext.Set<Detail>().Where(whereExpression);
            gridData.summary = GetDetailSummary<Detail>(queryeable);
            return gridData;
        }
        #region 编辑

        /// <summary>
        /// 获取编辑明细主键
        /// </summary>
        /// <typeparam name="DetailT"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="detailKeyName"></param>
        /// <param name="mainKeyName"></param>
        /// <param name="mainKeyValue"></param>
        /// <returns></returns>
        public List<Tkey> GetUpdateDetailSelectKeys<DetailT, Tkey>(string detailKeyName, string mainKeyName, string mainKeyValue) where DetailT : class
        {
            ISugarQueryable<DetailT> queryable = repository.DbContext.Set<DetailT>();
            Expression<Func<DetailT, Tkey>> selectExpression = detailKeyName.GetExpression<DetailT, Tkey>();
            Expression<Func<DetailT, bool>> whereExpression = mainKeyName.CreateExpression<DetailT>(mainKeyValue, LinqExpressionType.Equal);
            List<Tkey> detailKeys = queryable.Where(whereExpression).Select(selectExpression).ToList();
            return detailKeys;
        }



        /// <summary>
        /// 获取配置的创建人ID创建时间创建人,修改人ID修改时间、修改人与数据相同的字段
        /// </summary>
        private static string[] _userIgnoreFields { get; set; }

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

        /// <summary>
        /// 提交的数据版号检测
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveModel"></param>
        private void CheckDataVersion(T entity, SaveModel saveModel)
        {
            Response.Status = true;
            if (string.IsNullOrEmpty(saveModel.DataVersionField) || string.IsNullOrEmpty(saveModel.DataVersionValue))
            {
                return;
            }
            var versionProperty = typeof(T).GetProperty(saveModel.DataVersionField);
            if (versionProperty == null)
            {
                return;
            }
            var keyProperty = typeof(T).GetKeyProperty();
            object keyValue = keyProperty.GetValue(entity);
            var where = keyProperty.Name.CreateExpression<T>(keyValue, LinqExpressionType.Equal);
            var selectExpression = saveModel.DataVersionField.GetExpression<T, string>();
            string dataVersionValue = repository.FindAsIQueryable(where).Select(selectExpression).FirstOrDefault();
            if (string.IsNullOrEmpty(dataVersionValue))
            {
                return;
            }
            if (dataVersionValue != saveModel.DataVersionValue)
            {
                Response.Error("数据已发生变化,请刷新页面后重新编辑");
            }
            string value = Guid.NewGuid().ToString();
            versionProperty.SetValue(entity, value);
            saveModel.MainData[saveModel.DataVersionField] = value;
        }

        private List<MultipleTableEntity> multipleTableEntities = null;


        /// <summary>
        /// 2023.09.10增加主从、一对多编辑时可修改原对象属性
        /// </summary>
        private void SetMultipleTableEntities()
        {
            if (multipleTableEntities == null || multipleTableEntities.Count == 0)
            {
                return;
            }
            foreach (var item in multipleTableEntities)
            {
                typeof(ServiceBase<T, TRepository>).GetMethod("EntryDbContextMultipleTableEntities", BindingFlags.Instance | BindingFlags.NonPublic)
                             .MakeGenericMethod(new Type[] { item.Type })
                             .Invoke(this, new object[] { item });
            }
        }
        /// <summary>
        /// 2023.09.10增加主从、一对多编辑时可修改原对象属性
        /// </summary>
        /// <typeparam name="Detail"></typeparam>
        /// <param name="multipleTable"></param>
        private void EntryDbContextMultipleTableEntities<Detail>(MultipleTableEntity multipleTable) where Detail : class, new()
        {
            if (multipleTable.Flag == TableFlag.Add)
            {
                foreach (var detail in (List<Detail>)multipleTable.List)
                {
                    detail.SetCreateDefaultVal();
                    repository.AddWithSetIdentity<Detail>(detail);
                }
                if (multipleTable.SubType != null)
                {
                    typeof(ServiceBase<T, TRepository>).GetMethod("EntryDbContextSubType", BindingFlags.Instance | BindingFlags.NonPublic)
                       .MakeGenericMethod(new Type[] { multipleTable.SubType, typeof(Detail) })
                       .Invoke(this, new object[] { multipleTable.List });
                }
                return;
            }

            if (multipleTable.Flag == TableFlag.Update)
            {
                repository.UpdateRange((List<Detail>)multipleTable.List, multipleTable.Fields.ToArray());
                if (multipleTable.SubType != null)
                {
                    // 设置三级明细表
                    typeof(ServiceBase<T, TRepository>).GetMethod("EntryDbContextSubType", BindingFlags.Instance | BindingFlags.NonPublic)
                          .MakeGenericMethod(new Type[] { multipleTable.SubType, typeof(Detail) })
                          .Invoke(this, new object[] { multipleTable.List });
                }
                return;
            }
            var detailKeyInfo = typeof(Detail).GetKeyProperty();
            foreach (var key in (List<object>)multipleTable.List)
            {
                Detail delT = Activator.CreateInstance<Detail>();
                detailKeyInfo.SetValue(delT, key);
                repository.Delete<Detail>(delT, false);
            }
        }

        /// <summary>
        /// 三级明细删除
        /// </summary>
        private void CreateSubDel(SaveModel saveModel)
        {
            if (saveModel.SubDelInfo == null || saveModel.SubDelInfo.Count == 0)
            {
                return;
            }

            var detailTypes = typeof(T).GetCustomAttribute<EntityAttribute>()?.DetailTable;
            if (detailTypes == null)
            {
                return;
            }

            foreach (var item in detailTypes)
            {
                var subTypes = item.GetCustomAttribute<EntityAttribute>()?.DetailTable;
                if (subTypes != null)
                {
                    foreach (var type in subTypes)
                    {
                        typeof(ServiceBase<T, TRepository>).GetMethod("CreateSubDelContext", BindingFlags.Instance | BindingFlags.NonPublic)
                             .MakeGenericMethod(new Type[] { type })
                             .Invoke(this, new object[] { saveModel, type.Name });
                    }

                }
            }
        }
        /// <summary>
        /// 三级表删除
        /// </summary>
        /// <typeparam name="TSub"></typeparam>
        /// <param name="saveModel"></param>
        /// <param name="tableName"></param>
        private void CreateSubDelContext<TSub>(SaveModel saveModel, string tableName) where TSub : class, new()
        {
            foreach (var item in saveModel.SubDelInfo.Where(x => !x.IsProescc && x.Table == tableName))
            {
                item.IsProescc = true;
                var keyPro = typeof(TSub).GetKeyProperty();
                foreach (var key in item.DelKeys)
                {
                    TSub entity = Activator.CreateInstance<TSub>();
                    keyPro.SetValue(entity, key.ChangeType(keyPro.PropertyType));
                    repository.Delete(entity);
                }

            };
        }

        private void DelDetails<Entity, KeyType>(object[] keys) where Entity : class, new()
        {
            var values = keys.Select(s => (KeyType)(s.ChangeType(typeof(KeyType)))).ToList();
            var expression = typeof(T).GetKeyName().CreateExpression<Entity>(values, LinqExpressionType.In);
            repository.DbContext.Deleteable<Entity>().Where(expression).ExecuteCommand();
        }
        private object[] GetLogicDelIds<TKey>(object[] keys, PropertyInfo keyProperty)
        {
            var keyCondition = keyProperty.Name.CreateExpression<T>(keys, LinqExpressionType.In);
            var selectExp = keyProperty.Name.GetExpression<T, TKey>();
            return repository.FindAsIQueryable(keyCondition).Select(selectExp).ToArray().Select(s => s as object).ToArray();
        }
        /// <summary>
        /// 明细表逻辑删除
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <typeparam name="KeyType"></typeparam>
        /// <param name="keys"></param>
        private void LogicDelDetails<Entity, KeyType>(object[] keys, PropertyInfo keyProperty) where Entity : class, new()
        {
            var values = keys.Select(s => (KeyType)(s.ChangeType(typeof(KeyType)))).ToList();
            var expression = keyProperty.Name.CreateExpression<Entity>(values, LinqExpressionType.In);

            repository.SqlSugarClient.Updateable<Entity>()
            .SetColumns(AppSetting.LogicDelField, (int)DelStatus.已删除)
            .Where(expression)
            .ExecuteCommand();
        }



        /// <summary>
        ///code="-1"强制返回，具体使用见：后台开发文档->后台基础代码扩展实现
        /// </summary>
        /// <returns></returns>
        private bool CheckResponseResult()
        {
            return !Response.Status || Response.Code == "-1";
        }

        private PropertyInfo GetLogicDelProperty<TLg>()
        {

            return GetLogicDelProperty(typeof(TLg));
        }

        private PropertyInfo GetLogicDelProperty(Type type)
        {
            if (string.IsNullOrEmpty(AppSetting.LogicDelField))
            {
                return null;
            }
            return type.GetProperty(AppSetting.LogicDelField);
        }
    }

    public enum DelStatus
    {
        正常 = 0,
        已删除 = 1
    }
    public enum TableFlag
    {
        Add = 1,
        Update = 2,
        Del = 3
    }
    public class MultipleTableEntity
    {

        public Type Type { get; set; }
        public object List { get; set; }

        public TableFlag Flag { get; set; }

        public List<string> Fields { get; set; }

        public Type SubType { get; set; }

        public List<string> SubFields { get; set; }
    }
}
