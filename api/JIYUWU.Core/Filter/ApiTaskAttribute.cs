using JIYUWU.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JIYUWU.Core.Filter
{
    public interface IApiTaskFilter : IFilterMetadata
        {
            AuthorizationFilterContext OnAuthorization(AuthorizationFilterContext context);
        }
        public class ApiTaskAttribute : Attribute, IApiTaskFilter, IAllowAnonymous
        {
            public AuthorizationFilterContext OnAuthorization(AuthorizationFilterContext context)
            {
                return QuartzAuthorization.Validation(context) ;
            }
        }
 
}
