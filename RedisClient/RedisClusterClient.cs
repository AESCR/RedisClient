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
        static RedisClusterClient()
        {
            Load();
        }
        public static void Load()
        {
            var redis1 = new RedisClient("127.0.0.1", 6379);
            var redis2= new RedisClient("127.0.0.1", 6380);
            var redis3 = new RedisClient("127.0.0.1", 6381);
            redis1.AddSlave("127.0.0.1", 6382);
            redis2.AddSlave("127.0.0.1", 6383);
            redis3.AddSlave("127.0.0.1", 6384);
            Master.Add(redis1);
            Master.Add(redis2);
            Master.Add(redis3);
        }
        private KetamaNodeLocator KetamaNode => new KetamaNodeLocator(Master.Select(x=>x.HostPort).ToList());
        private static readonly List<RedisClient> Master=new List<RedisClient>();

        private RedisClient AllotRedis(string key)
        {
            var ms = KetamaNode.GetNodes(key);
            Console.WriteLine($"Key:{key}------分配给了{ms}");
            return Master.Find(x=>x.HostPort==ms);
        }
        public RedisClient GetRedisClient(string key)
        {
            return GetMasterRedisClient(key);
        }
        public RedisClient GetMasterRedisClient(string key)
        {
            var master= AllotRedis(key);
            return new RedisClient(master.Host, master.Port, master.Password);
        }
        public RedisClient GetSlaveRedisClient(string key)
        {
            var master = AllotRedis(key);
            var slave= master.GetRandomSlaveClient();
            return new RedisClient(slave.Host, slave.Port, slave.Password);
        }
    }
}
