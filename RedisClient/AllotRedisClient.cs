#region << 版 本 注 释 >>

/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。
//
// 文件名：AllotRedisClient
// 文件功能描述：
//
//
// 创建者：名字 AESCR
// 时间：2021/2/20 11:05:11
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

using System;
using System.Collections.Generic;
using XRedis;

namespace RedisClient
{
    public class AllotRedisClient
    {
     
        private readonly RedisClusterLoader _redisClusterLoader = new RedisClusterLoader();

        public RedisClusterLoader GetRedisClusterLoader()
        {
            return _redisClusterLoader;
        }
        private KetamaNodeLocator GetKetamaNodeLocator(bool oldPos = false)
        {
            KetamaNodeLocator ketamaNodeLocator = oldPos ? new KetamaNodeLocator(_redisClusterLoader.GetOldNodes()) : new KetamaNodeLocator(_redisClusterLoader.GetNodes());
            return ketamaNodeLocator;
        }

        private RedisOption GetCluster(string key, bool read = false, bool oldPos = false)
        {
            var nodeKey = GetKetamaNodeLocator(oldPos);
            var masterCode = nodeKey.GetNodes(key);
            return  _redisClusterLoader.GetCluster(masterCode, read);
        }

        public RedisClient GetClient(string key, int dbIndex = 0, bool read = false)
        {
            var redis = read ? GetReadClient(key, dbIndex) : GetWriteClient(key, dbIndex);
            return redis;
        }

        public RedisClient GetReadClient(string key, int dbIndex = 0)
        {
            var cluster = GetCluster(key, true);
            var redis = new RedisClient(cluster.Host, cluster.Port, cluster.Password);
            if (dbIndex > 0)
            {
                redis.Select(dbIndex);
            }
            if (!_redisClusterLoader.HasOldNodes) return redis;
            if (redis.Exists(key))
            {
                return redis;
            }
            var oldCluster = GetCluster(key, false, true);
            using (var oldRedis = new RedisClient(oldCluster.Host, oldCluster.Port, oldCluster.Password))
            {
                if (oldRedis.Exists(key))
                {
                    oldRedis.Migrate(new string[] { key }, cluster.Host, cluster.Port, cluster.Password, dbIndex);
                }
            }
            return redis;
        }

        public RedisClient GetWriteClient(string key, int dbIndex = 0)
        {
            var cluster = GetCluster(key);
            var redis = new RedisClient(cluster.Host, cluster.Port, cluster.Password);
            if (dbIndex > 0)
            {
                redis.Select(dbIndex);
            }
            return redis;
        }
    }
}