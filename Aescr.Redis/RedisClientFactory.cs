#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。 
//
// 文件名：RedisClientFactory
// 文件功能描述：
//
// 
// 创建者：名字 AESCR
// 时间：2021/3/1 16:41:02
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aescr.Redis
{
    public  class RedisClientFactory
    {
        private readonly Hashtable _hashtable;

        private RedisClientFactory()
        {
            _hashtable = Hashtable.Synchronized(new Hashtable());
        }

        public static RedisClientFactory CreateClientFactory()
        {
            return new RedisClientFactory();
        }
        public RedisClient GetRedisClient(RedisConnection connection)
        {
            if (!_hashtable.ContainsKey(connection.Host))
            {
                RedisClient redisClient = new RedisClient(connection);
                _hashtable.Add(connection.Host, redisClient);
                return redisClient;
            }
            else
            {
                return _hashtable[connection.Host] as RedisClient;
            }
        }
    }
}
