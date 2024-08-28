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

        object GetDetailPage(PageDataOptions pageData);
    }
}
