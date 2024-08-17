using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Common
{
    [ApiController]
    public class BaseController : Controller
    {
        public BaseController()
        {

        }
        protected JsonResult JsonNormal(object data, JsonSerializerSettings serializerSettings = null, bool formateDate = true)
        {
            serializerSettings = serializerSettings ?? new JsonSerializerSettings();
            serializerSettings.ContractResolver = null;
            if (formateDate)
            {
                serializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }
            serializerSettings.Converters.Add(new LongCovert());
            return  new JsonResult(data, serializerSettings);
        }

        protected IActionResult Success(string message)
        {
            return JsonNormal(new { status = true, message });
        }
        protected IActionResult Success(string message, object data)
        {
            return JsonNormal(new { status = true, message, data });
        }

        protected IActionResult Error(string message)
        {
            return JsonNormal(new { status = false, message });
        }
        protected IActionResult Error(string message, object data)
        {
            return JsonNormal(new { status = false, message, data });
        }
    }
    public class LongCovert : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }
            long.TryParse(reader.Value.ToString(), out long value);
            return value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(long) == objectType || typeof(long?) == objectType;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            serializer.Serialize(writer, value.ToString());
        }
    }
}
