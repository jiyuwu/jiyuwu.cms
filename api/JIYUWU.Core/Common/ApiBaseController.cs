using JIYUWU.Core.Extension;
using JIYUWU.Core.Filter;
using JIYUWU.Entity.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using static Dm.net.buffer.ByteArrayBuffer;

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
        #region 新增
        [HttpPost, Route("Add")]
        public virtual ActionResult Add([FromBody] SaveModel saveModel)
        {
            _baseWebResponseContent = InvokeService("Add",
                new Type[] { typeof(SaveModel) },
                new object[] { saveModel }) as WebResponseContent;
            Logger.Info(LoggerType.Add, null, _baseWebResponseContent.Code == 200 ? "Ok" : _baseWebResponseContent.Msg);
            _baseWebResponseContent.Data = _baseWebResponseContent.Data?.Serialize();
            return Json(_baseWebResponseContent);
        }
        #endregion

        #region 修改
        [HttpPost, Route("Update")]
        public virtual ActionResult Update([FromBody] SaveModel saveModel)
        {
            _baseWebResponseContent = InvokeService("Update", new object[] { saveModel }) as WebResponseContent;
            Logger.Info(LoggerType.Edit, null, _baseWebResponseContent.Code == 200 ? "Ok" : _baseWebResponseContent.Msg);
            _baseWebResponseContent.Data = _baseWebResponseContent.Data?.Serialize();
            return Json(_baseWebResponseContent);
        }
        #endregion

        #region 删除
        [HttpPost, Route("Del")]
        public virtual ActionResult Del([FromBody] object[] keys)
        {
            _baseWebResponseContent = InvokeService("Del", new object[] { keys, true }) as WebResponseContent;
            Logger.Info(LoggerType.Del, keys.Serialize(), _baseWebResponseContent.Code == 200 ? "Ok" : _baseWebResponseContent.Msg);
            return Json(_baseWebResponseContent);
        }
        #endregion

        #region 查询
        [HttpPost, Route("GetPageData")]
        public virtual ActionResult GetPageData([FromBody] PageDataOptions loadData)
        {
            return JsonNormal(InvokeService("GetPageData", new object[] { loadData }));
        }
        [HttpPost, Route("GetDetailPage")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ActionResult GetDetailPage([FromBody] PageDataOptions loadData)
        {
            return Content(InvokeService("GetDetailPage", new object[] { loadData }).Serialize());
        }
        #endregion

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
