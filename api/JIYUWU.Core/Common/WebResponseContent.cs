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
        public WebResponseContent(bool status)
        {
            this.Status = status;
        }
        public bool Status { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public WebResponseContent OK()
        {
            this.Status = true;
            return this;
        }
        public WebResponseContent OKData(object data)
        {
            this.Status = true;
            this.Data = data;
            return this;
        }
        public WebResponseContent OKData(string message, object data, bool ts = false)
        {
            this.Message = message;
            this.Status = true;
            this.Data = data;
            return this;
        }
        public static WebResponseContent Instance
        {
            get { return new WebResponseContent(); }
        }
        public WebResponseContent OK(string message = null, object data = null, bool ts = true)
        {
            this.Status = true;
            this.Message = message;
            this.Data = data;
            return this;
        }
        public WebResponseContent OKDataToString(object data = null)
        {
            this.Status = true;
            this.Data = data.Serialize();
            return this;
        }
        public WebResponseContent OK(ResponseType responseType, bool ts = true)
        {
            return Set(responseType, true, true);
        }
        public WebResponseContent Error(string message = null, bool ts = false)
        {
            this.Status = false;
            this.Message = message;
            return this;
        }
        public WebResponseContent Error(ResponseType responseType, bool ts = false)
        {
            return Set(responseType, false, ts);
        }
        public WebResponseContent Set(ResponseType responseType, bool ts = false)
        {
            bool? b = null;
            return this.Set(responseType, b, ts);
        }
        public WebResponseContent Set(ResponseType responseType, bool? status, bool ts = false)
        {
            return this.Set(responseType, null, status, ts);
        }
        public WebResponseContent Set(ResponseType responseType, string msg, bool ts = false)
        {
            bool? b = null;
            return this.Set(responseType, msg, b);
        }
        public WebResponseContent Set(ResponseType responseType, string msg, bool? status, bool ts = false)
        {
            if (status != null)
            {
                this.Status = (bool)status;
            }
            this.Code = ((int)responseType).ToString();
            if (!string.IsNullOrEmpty(msg))
            {
                Message = msg;
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
                Content = new { status = false, message = responseData.Message }.Serialize(),
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
                string desc = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(responseData.Message));
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