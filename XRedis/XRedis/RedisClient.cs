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
// 时间：2021/1/27 16:10:55
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
using System.Text;

namespace XRedis
{
    public class RedisClient
    {
        private string _host;
        private int _port;
        public RedisClient()
        {

        }
        public RedisClient(string host,int port= 6379)
        {
            _port = port;
            _host = host;
        }

        public bool SetNx(string key, string str)
        {
            throw new NotImplementedException();
        }

        public bool Expire(string key, in TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        public bool HSetNx(string key, string filed, string str)
        {
            throw new NotImplementedException();
        }

        public string HMSet(string key, Dictionary<string, string> result)
        {
            throw new NotImplementedException();
        }
    }
}
