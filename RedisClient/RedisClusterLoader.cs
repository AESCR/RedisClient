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

#endregion << 版 本 注 释 >>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace RedisClient
{
    /// <summary>
    /// 本地集群配置表
    /// </summary>
    public class RedisConfig
    {
        public List<string> OldNodes { get; set; } = new List<string>();
        public List<RedisOption> RedisOptions = new List<RedisOption>();
    }

    /// <summary>
    /// 集群加载器
    /// </summary>
    public class RedisClusterLoader
    {
        private string _saveFile = "redisConfig.json";

        public RedisClusterLoader()
        {
            Init();
        }

        private List<string> NowNodes => _redisCollection.Where(x => x.IsRootNode == true && x.IsDisable == false).Select(x => x.MasterCode).Distinct().ToList();
        private List<RedisOption> _redisCollection = new List<RedisOption>();
        private List<string> _oldNodes = new List<string>();
        public bool HasOldNodes => _oldNodes.Count > 0;

        public List<RedisOption> GetCluster(string masterCode, bool read = false)
        {
            if (read)
            {
                return GetReadCluster(masterCode);
            }

            return GetWriteCluster(masterCode);
        }

        public List<RedisOption> GetReadCluster(string masterCode)
        {
            return _redisCollection.FindAll(x => x.MasterCode == masterCode && x.IsDisable == false && x.CanRead);
        }

        public List<RedisOption> GetWriteCluster(string masterCode)
        {
            return _redisCollection.FindAll(x => x.MasterCode == masterCode && x.IsDisable == false && x.CanWrite);
        }

        public void AddClusters(string masterCode, RedisOption option)
        {
            option.MasterCode = masterCode;
            option.ToBeEffective = true;
            var exists = _redisCollection.Exists(x => x.HostPort == option.HostPort);
            if (exists)
            {
                return;
            }
            _redisCollection.Add(option);
        }

        public void Refresh()
        {
            var paras = _redisCollection.AsParallel();
            paras.ForAll(slave =>
            {
                using var redis = new RedisClient(slave.Host, slave.Port, slave.Password);
                if (slave.IsRootNode)
                {
                    redis.SlaveOf();
                    redis.ConfigSet("slave-read-only", "no");
                }
                else
                {
                    redis.SlaveOf(slave.MasterRedis.Host, slave.MasterRedis.Port, slave.MasterRedis.Password);
                }
            });
        }

        private void Init()
        {
            Load();
            TestLoad();
            Refresh();
        }

        private void TestLoad()
        {
            if (_redisCollection.Count == 0)
            {
                var host = "192.168.2.84";
                var redis1 = new RedisOption(host, 6379);
                var redis2 = new RedisOption(host, 6380);
                var redis3 = new RedisOption(host, 6381);
                var redis4 = new RedisOption(host, 6382);
                var redis5 = new RedisOption(host, 6383);
                var redis6 = new RedisOption(host, 6384);
                redis1.SetMasterRedis(redis4);
                redis2.SetMasterRedis(redis5);
                redis3.SetMasterRedis(redis6);
                AddClusters("001", redis4);
                AddClusters("001", redis1);

                AddClusters("002", redis5);
                AddClusters("002", redis2);

                AddClusters("002", redis6);
                AddClusters("003", redis3);
            }
        }

        public void Load()
        {
            _redisCollection.Clear();
            if (File.Exists(_saveFile))
            {
                using (FileStream fileStream = new FileStream(_saveFile, FileMode.Open, FileAccess.Write))
                {
                    StreamReader sr = new StreamReader(fileStream, Encoding.UTF8);
                    var json = sr.ReadToEnd();
                    var redisConfig = JsonSerializer.Deserialize<RedisConfig>(json);
                    _redisCollection = redisConfig.RedisOptions;
                    _oldNodes = redisConfig.OldNodes;
                }
            }
        }

        public List<string> GetOldNodes()
        {
            return _oldNodes;
        }

        public List<string> GetNodes()
        {
            return NowNodes;
        }

        /// <summary>
        /// 保存集群配置文件
        /// </summary>
        public void Save()
        {
            if (_redisCollection == null) return;
            _redisCollection.ForEach(x =>
            {
                if (x.ToBeEffective)
                {
                    x.ToBeEffective = false;
                }
            });
            var redisConfig = new RedisConfig { RedisOptions = _redisCollection, OldNodes = _oldNodes };
            using (FileStream fileStream = new FileStream(_saveFile, FileMode.Create, FileAccess.Write))
            {
                var json = JsonSerializer.Serialize(redisConfig);
                var bytes = Encoding.UTF8.GetBytes(json);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
        }
    }
}