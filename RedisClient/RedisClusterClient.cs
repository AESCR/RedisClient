using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            if (!File.Exists("redis.json"))
            {
                var txt = JsonSerializer.Serialize(_master);
                using (FileStream fileStream = new FileStream("redis.json", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var bytes = Encoding.UTF8.GetBytes(txt);
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Close();
                }
            }
            else
            {
                var txt = string.Empty;
                using (FileStream fileStream = new FileStream("redis.json", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    StreamReader sr = new StreamReader(fileStream, Encoding.UTF8);
                    txt = sr.ReadToEnd();
                    sr.Close();
                    fileStream.Close();
                }
                List < MasterServer > master= JsonSerializer.Deserialize<List<MasterServer>>(txt);
                if (string.IsNullOrEmpty(txt)) return;
                lock (_master)
                {
                    _master = master;
                }
            }
         
        }

        private KetamaNodeLocator KetamaNode => new KetamaNodeLocator(_master);
        private static List<MasterServer> _master=new List<MasterServer>();

        private MasterServer AllotRedis(string key)
        {
            var ms = KetamaNode.GetNodes(key);
            Console.WriteLine($"Key:{key}------分配给了{ms.HostPort}");
            return ms;
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
            var slave= master.GetRandomSlave();
            return new RedisClient(slave.Host, slave.Port, slave.Password);
        }
    }

    public class MasterServer
    {
        private readonly Random _ra;

        public MasterServer()
        {
            _ra = new System.Random();
        }

        public string HostPort => $"{Host}:{Port}";
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 6379;
        public string Password { get; set; }
        public List<SlaveServer> Slave { get; set; }=new List<SlaveServer>();
        public SlaveServer GetRandomSlave()
        {
            var index=  _ra.Next(0, Slave.Count);
            return Slave[index];
        }
    }

    public class SlaveServer
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
    }
}
