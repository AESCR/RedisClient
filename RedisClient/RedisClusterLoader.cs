#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。 
//
// 文件名：RedisCluster
// 文件功能描述：
//
// 
// 创建者：名字 AESCR
// 时间：2021/2/20 15:06:40
//
// 修改人：
// 时间：
// 修改说明：
//
// 修改人：
// 时间：
// 修改说明：
//
// 版本：V1.0.0
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace RedisClient
{
    /// <summary>
    /// 集群加载器
    /// </summary>
    public class RedisClusterLoader
    {
        public RedisClusterLoader()
        {
            Load();
        }

        public List<string> GetNodeList()
        {
            lock (RedisClusters)
            {
                return RedisClusters.Keys.ToList();
            }
        }
        public List<string> GetOldNodeList()
        {
            var path = "OldNodes.json";
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Write))
                {
                    StreamReader sr = new StreamReader(fileStream, Encoding.UTF8);
                    var json= sr.ReadToEnd();
                    sr.Close();
                    fileStream.Close();
                    return JsonSerializer.Deserialize<List<string>>(json);
                }
            }
            return GetNodeList();
        }
        private static readonly Dictionary<string, List<RedisClusterOption>> RedisClusters = new Dictionary<string, List<RedisClusterOption>>();
        private List<RedisClusterOption> GeRedisCluster(string masterCode)
        {
            lock (RedisClusters)
            {
                if (!RedisClusters.ContainsKey(masterCode)) return new List<RedisClusterOption>();
                var slaves = RedisClusters[masterCode].Where(x =>x.IsDisable == false).OrderByDescending(x=>x.IsRootNode).ToList();
                return slaves;
            }
        }

        public List<RedisClusterOption> GetReadCluster(string masterCode)
        {
            lock (RedisClusters)
            {
                if (!RedisClusters.ContainsKey(masterCode)) return new List<RedisClusterOption>();
                var slaves = RedisClusters[masterCode].Where(x => x.IsDisable == false&&x.CanRead).OrderByDescending(x => x.IsRootNode).ToList();
                return slaves;
            }
        }
        public List<RedisClusterOption> GetWriteCluster(string masterCode)
        {
            lock (RedisClusters)
            {
                if (!RedisClusters.ContainsKey(masterCode)) return new List<RedisClusterOption>();
                var slaves = RedisClusters[masterCode].Where(x => x.IsDisable == false && x.CanWrite).OrderByDescending(x => x.IsRootNode).ToList();
                return slaves;
            }
        }
        private  RedisClusterOption GeRedisCluster(string masterCode, string host, int port)
        {
            lock (RedisClusters)
            {
                if (!RedisClusters.ContainsKey(masterCode)) return null;
                var slaves = RedisClusters[masterCode].FirstOrDefault(x => x.Host == host && x.Port == port&&x.IsDisable==false);
                return slaves;
            }
        }
        public  void AddClusters(string masterCode, RedisClusterOption option)
        {
            lock (RedisClusters)
            {
                if (RedisClusters.ContainsKey(masterCode))
                {
                    if (option.MasterRedis == null)
                    {
                        var rootNode = RedisClusters[masterCode].Find(x => x.IsRootNode == true && x.IsDisable == false);
                        if (rootNode !=null)
                        {
                            option.MasterRedis = rootNode;
                        }
                    }
                    RedisClusters[masterCode].Add(option);
                }
                else
                {
                    option.MasterRedis = null;
                    RedisClusters.Add(masterCode, new List<RedisClusterOption>(){ option });
                }
            }

            Save();
        }

        public void RefreshSlaveOf(string masterCode,string host,int port)
        {
            var slave = GeRedisCluster(masterCode, host, port);
            using var redis = new RedisClient(slave.Host, slave.Port, slave.Password);
            if (slave.IsRootNode)
            {
                redis.SlaveOf();
            }
            else
            {
                redis.SlaveOf(slave.MasterRedis.Host, slave.MasterRedis.Port, slave.MasterRedis.Password);
            }
        }
        public  void RefreshSlaveOf()
        {
            lock (RedisClusters)
            {
                foreach (string key in RedisClusters.Keys)
                {
                    var slaves = GeRedisCluster(key);
                    foreach (var slave in slaves)
                    {
                        using var redis = new RedisClient(slave.Host, slave.Port, slave.Password);
                        if (slave.IsRootNode)
                        {
                            redis.SlaveOf();
                        }
                        else
                        {
                            redis.SlaveOf(slave.MasterRedis.Host, slave.MasterRedis.Port, slave.MasterRedis.Password);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 读取集群配置文件
        /// </summary>
        private void Load()
        {
            if (RedisClusters.Count == 0)
            {
                lock (RedisClusters)
                {
                    if (RedisClusters.Count == 0)
                    {
                        var host = "192.168.2.84";
                        var redis1 = new RedisClusterOption(host, 6379);
                        var redis2 = new RedisClusterOption(host, 6380);
                        var redis3 = new RedisClusterOption(host, 6381);
                        var redis4 = new RedisClusterOption(host, 6382);
                        var redis5= new RedisClusterOption(host, 6383);
                        var redis6 = new RedisClusterOption(host, 6384);
                        redis1.MasterRedis = redis4;
                        redis2.MasterRedis = redis5;
                        redis3.MasterRedis = redis6;
                        AddClusters("001", redis4);
                        AddClusters("001", redis1);

                        AddClusters("002", redis5);
                        AddClusters("002", redis2);

                        AddClusters("002", redis6);
                        AddClusters("003", redis3);
                        RefreshSlaveOf();
                    }
                }
            }
        }
        /// <summary>
        /// 保存集群配置文件
        /// </summary>
        public  bool Save()
        {
         
            lock (RedisClusters)
            {
                var path = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000+".json";
                var json = JsonSerializer.Serialize(RedisClusters);
                if (File.Exists(path))
                {
                    return false;
                }
                using (FileStream fileStream=new FileStream(path,FileMode.CreateNew,FileAccess.Write))
                {
                    var bytes = Encoding.UTF8.GetBytes(json);
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Close();
                }
                return true;
            }
        }

        public bool SaveNodes()
        {
            lock (RedisClusters)
            {
                var path ="OldNodes.json";
                var json = JsonSerializer.Serialize(GetNodeList());
                if (File.Exists(path))
                {
                    return false;
                }
                using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    var bytes = Encoding.UTF8.GetBytes(json);
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Close();
                }
                return true;
            }
        }
    }
}
