using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.Common
{
    public class HttpManager
    {
        public static async Task<string> CallJsonApiAsync(string urlWithJson, int timeOut = 30)
        {
            string contentType = "application/json";
            // 解析 URL 中的 JSON 部分
            string url = urlWithJson.Split('?')[0];
            string json = urlWithJson.Contains("?") ? urlWithJson.Split('?')[1] : string.Empty;

            // 处理 JSON 字符串
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            string postData = JsonConvert.SerializeObject(jsonObject);

            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(timeOut) })
            {
                var content = new StringContent(postData, Encoding.UTF8, contentType);
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
