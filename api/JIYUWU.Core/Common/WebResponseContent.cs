using JIYUWU.Core.Extension;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace JIYUWU.Core.Common
{
    public class WebResponseContent
    {
        public WebResponseContent()
        {
        }
        public WebResponseContent(int code)
        {
            this.Code = code;
        }
        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }

        public WebResponseContent OK()
        {
            this.Code = 200;
            return this;
        }

        public static WebResponseContent Instance
        {
            get { return new WebResponseContent(); }
        }
        public WebResponseContent OK(string msg = null, object data = null, bool ts = true)
        {
            this.Code = 200;
            this.Msg = msg;
            this.Data = data;
            return this;
        }
        public WebResponseContent OKDataToString(object data = null)
        {
            this.Code = 200;
            this.Data = data.Serialize();
            return this;
        }
        public WebResponseContent OK(ResponseType responseType, bool ts = true)
        {
            return Set(responseType, 200, true);
        }
        public WebResponseContent Error(string msg = null, bool ts = false)
        {
            this.Code = 500;
            this.Msg = msg;
            return this;
        }
        public WebResponseContent Error(ResponseType responseType, bool ts = false)
        {
            return Set(responseType, 500, ts);
        }
        public WebResponseContent Set(ResponseType responseType, bool ts = false)
        {
            int? b = null;
            return this.Set(responseType, b, ts);
        }
        public WebResponseContent Set(ResponseType responseType, int? code, bool ts = false)
        {
            return this.Set(responseType, null, code, ts);
        }
        public WebResponseContent Set(ResponseType responseType, string msg, int? code, bool ts = false)
        {
            if (code != null)
            {
                this.Code = (int)code;
            }
            this.Code = (int)responseType;
            if (!string.IsNullOrEmpty(msg))
            {
                return this;
            }
            return this;
        }

    }
    public static class FilterResponse
    {
        public static void GetContentResult(FilterContext context, IActionResult actionResult)
        {
            GetContentResult(context, actionResult, null);
        }

        public static void SetActionResult(ActionExecutingContext context, WebResponseContent responseData)
        {
            context.Result = new ContentResult()
            {
                Content = new { code = 200, msg = responseData.Msg }.Serialize(),
                ContentType = ApplicationContentType.JSON,
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }

        public static void GetContentResult(FilterContext context, IActionResult actionResult, WebResponseContent responseData)
        {
            responseData = responseData ?? new WebResponseContent();
            responseData.Set(ResponseType.ServerError);

            if (context.HttpContext.IsAjaxRequest())
            {
                actionResult = new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(responseData),
                    ContentType = ApplicationContentType.JSON,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
            else
            {
                string desc = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(responseData.Msg));
                actionResult = new ContentResult()
                {
                    Content = $@"<html><head><title></title></head><body>{desc}</body></html>",
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}