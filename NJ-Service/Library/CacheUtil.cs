using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace NJ_Service.Library
{
    public class CacheUtil
    {
        public static ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }
        public static bool Contains(string key)
        {
            return Cache.Contains(key);
        }

        public static T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        public static void Set(string key, object data, string region = null)
        {
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromHours(24);
            Cache.Add(key, data, policy, region);
        }

        public static void Remove(string key)
        {
            Cache.Remove(key);
        }
    }
}