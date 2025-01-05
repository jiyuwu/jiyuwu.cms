using CSRedis;
using JIYUWU.Core.Common;
using Newtonsoft.Json;

namespace JIYUWU.Core.CacheManager
{
    public class RedisCacheService : ICacheService
    {
        private readonly string _prefixKey = AppSetting.GetSection("ConnectionStrs")["CacheTag"];
        public RedisCacheService()
        {
            var csredis = new CSRedisClient(AppSetting.RedisConnectionString);
            RedisHelper.Initialization(csredis);
        }

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            key = $"{_prefixKey}_{key}";
            return RedisHelper.Exists(key);
        }

        public void LPush(string key, string val)
        {
            key = $"{_prefixKey}_{key}";
            RedisHelper.LPush(key, val);
        }

        public void RPush(string key, string val)
        {
            key = $"{_prefixKey}_{key}";
            RedisHelper.RPush(key, val);
        }

        public T ListDequeue<T>(string key) where T : class
        {
            key = $"{_prefixKey}_{key}";
            string value = RedisHelper.RPop(key);
            if (string.IsNullOrEmpty(value))
                return null;
            return JsonConvert.DeserializeObject<T>(value);
        }
        public object ListDequeue(string key)
        {
            key = $"{_prefixKey}_{key}";
            string value = RedisHelper.RPop(key);
            if (string.IsNullOrEmpty(value))
                return null;
            return value;
        }

        /// <summary>
        /// 移除list中的数据，keepIndex为保留的位置到最后一个元素如list 元素为1.2.3.....100
        /// 需要移除前3个数，keepindex应该为4
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keepIndex"></param>
        public void ListRemove(string key, int keepIndex)
        {
            key = $"{_prefixKey}_{key}";
            RedisHelper.LTrim(key, keepIndex, -1);
        }
        public bool Add(string key, string value, int expireSeconds = -1, bool isSliding = false)
        {
            key = $"{_prefixKey}_{key}";
            return RedisHelper.Set(key, value, expireSeconds);
        }
        public bool AddObject(string key, object value, int expireSeconds = -1, bool isSliding = false)
        {
            key = $"{_prefixKey}_{key}";
            return RedisHelper.Set(key, value, expireSeconds);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            key = $"{_prefixKey}_{key}";
            RedisHelper.Del(key);
            return true;
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public void RemoveAll(IEnumerable<string> keys)
        {
            IEnumerable<string> keysStr = keys.Select(x => $"{_prefixKey}_{x}");
            RedisHelper.Del(keysStr.ToArray());
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            key = $"{_prefixKey}_{key}";
            return RedisHelper.Get<T>(key);
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public string Get(string key)
        {
            key = $"{_prefixKey}_{key}";
            return RedisHelper.Get(key);
        }
        public void Dispose()
        {
        }
    }
}
