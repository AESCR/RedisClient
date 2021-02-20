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
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using XRedis;

namespace RedisClient
{
    public class AllotRedisClient
    {
        private Random random=new Random();
        private RedisClusterLoader redisCluster=new RedisClusterLoader();

        private RedisClusterOption GetCluster(string key,bool readOnly=false, bool oldPos=false)
        {
            var ketamaNodeLocator = oldPos ? new KetamaNodeLocator(redisCluster.GetNodeList()) : new KetamaNodeLocator(redisCluster.GetOldNodeList());
            var nodeKey = ketamaNodeLocator.GetNodes(key);
            List<RedisClusterOption> redisOptions;
            if (readOnly)
            {
                 redisOptions = redisCluster.GetReadCluster(nodeKey);
            }
            else
            {
                redisOptions = redisCluster.GetWriteCluster(nodeKey);
            }
            var index = random.Next(0, redisOptions.Count);
            var cluster = redisOptions[index];
            return cluster;
        }
        public RedisClient GetReadClient(string key)
        {
            var cluster = GetCluster(key, true);
            var redis=new RedisClient(cluster.Host, cluster.Port, cluster.Password);
            if (redis.Exists(key)==1)
            {
                return redis;
            }
            var cluster2 = GetCluster(key,true,true);
            var redis2= new RedisClient(cluster2.Host, cluster2.Port, cluster2.Password);
            if (redis2.Exists(key) == 1)
            {
                return redis2;
            }
            return redis;
        }
        public RedisClient GetWriteClient(string key)
        {
            var cluster = GetCluster(key);
            var redis = new RedisClient(cluster.Host, cluster.Port, cluster.Password);
            if (redis.Exists(key) == 1)
            {
                return redis;
            }
            var cluster2 = GetCluster(key, false, true);
            var redis2 = new RedisClient(cluster2.Host, cluster2.Port, cluster2.Password);
            if (redis2.Exists(key) == 1)
            {
                return redis2;
            }
            return redis;
        }
    }
}
