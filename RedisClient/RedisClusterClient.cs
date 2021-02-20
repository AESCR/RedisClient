using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using XRedis;

namespace RedisClient
{
    public class RedisClusterClient
    {
        private readonly AllotRedisClient _allotRedisClient=new AllotRedisClient();

        public bool Set(string key, string value)
        {
            var master = _allotRedisClient.GetWriteClient(key);
            return master.Set(key, value);
        }

        public string Get(string key)
        {
            var master = _allotRedisClient.GetReadClient(key);
            var result = master.Get(key);
            return result;
        }
    }
}
