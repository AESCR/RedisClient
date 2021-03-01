#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。 
//
// 文件名：RedisClient
// 文件功能描述：
//
// 
// 创建者：名字 AESCR
// 时间：2021/3/1 16:51:01
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aescr.Redis
{
    public class RedisRoundRobinClient
    {
        private readonly RedisConnection[] connections;
        private WeightedRoundRobin weightedRound;
        private RedisClientFactory _redisClientFactory = RedisClientFactory.CreateClientFactory();
        public RedisRoundRobinClient(params string[] connectionStr)
        {
            connections=new RedisConnection[connectionStr.Length];
            for (var index = 0; index < connectionStr.Length; index++)
            {
                var t = connectionStr[index];
                connections[index] = t;
            }
            weightedRound = new WeightedRoundRobin(connections);
        }

        private RedisClient GetRoundRobinRedis()
        {
            var r= weightedRound.GetServer();
            for (int i = 0; i < connections.Length; i++)
            {
                var c = connections[i];
                if (c.Host== r.Host)
                {
                    return _redisClientFactory.GetRedisClient(c);
                }
            }

            throw new Exception("未获取到服务器");
        }
    }
}
