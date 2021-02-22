using System;

namespace RedisClient
{
    public class RedisClusterClient
    {
        public int DbIndex = 0;
        private readonly AllotRedisClient _allotRedisClient = new AllotRedisClient();

        public bool Set(string key, string value)
        {
            var redis= _allotRedisClient.GetWriteClient(key, DbIndex);
            Console.WriteLine($"{redis.HostPort}----{key}");
            return redis.Set(key, value);
        }

       
    }
}