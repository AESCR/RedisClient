using System;
using System.Collections.Generic;

namespace RedisClient
{
    public class RedisClusterClient
    {
        public int DbIndex = 0;
        private readonly AllotRedisClient _allotRedisClient = new AllotRedisClient();

        public List<string> GetClusterStatus()
        {
            return _allotRedisClient.GetRedisClusterLoader().GetClusterStatus();
        }
        public bool Set(string key, string value)
        {
            var redis= _allotRedisClient.GetWriteClient(key, DbIndex);
            //Console.WriteLine($"{redis.HostPort}----{key}");
            return redis.Set(key, value);
        }

        public string Get(string key)
        {
            var redis = _allotRedisClient.GetReadClient(key, DbIndex);
            return redis.Get(key);
        }

    }
}