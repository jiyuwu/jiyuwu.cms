using JIYUWU.Core.Extension;
using JIYUWU.Core.Filter;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JIYUWU.Core.Common
{
    [ApiController]
    public class ApiBaseController<IServiceBase> : BaseController
    {
        protected IServiceBase Service;
        private WebResponseContent _baseWebResponseContent { get; set; }
        public ApiBaseController()
        {
        }
        public ApiBaseController(IServiceBase service)
        {
            Service = service;
        }
        public ApiBaseController(string projectName, string folder, string tablename, IServiceBase service)
        {
            Service = service;
        }
        /// <summary>
        /// 调用service方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private object InvokeService(string methodName, object[] parameters)
        {
            return Service.GetType().GetMethod(methodName).Invoke(Service, parameters);
        }
        /// <summary>
        /// 调用service方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="types">为要调用重载的方法参数类型：new Type[] { typeof(SaveDataModel)</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private object InvokeService(string methodName, Type[] types, object[] parameters)
        {
            return Service.GetType().GetMethod(methodName, types).Invoke(Service, parameters);
        }
    }
}
