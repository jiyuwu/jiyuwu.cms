using JIYUWU.Core.CacheManager;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace JIYUWU.Core.Common
{
    public interface IService<T> where T : BaseEntity
    {

        ICacheService CacheContext { get; }
        Microsoft.AspNetCore.Http.HttpContext Context { get; }

        #region 增
        int Add(T entity);
        #endregion

        #region 删
        int Delete(T entity);
        int DeleteById(object id);
        int DeleteByIds(object[] ids);
        #endregion

        #region 改      
        int Update(T entity);
        #endregion

        #region 查   
        PageGridData<T> GetPageData(PageDataOptions pageData);
        object GetDetailPage(PageDataOptions pageData);
        #endregion
    }
}
