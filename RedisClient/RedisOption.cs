#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。 
//
// 文件名：RedisOption
// 文件功能描述：
//
// 
// 创建者：名字 AESCR
// 时间：2021/2/20 9:40:10
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

namespace RedisClient
{
    public class RedisOption
    {
        public string Password { get; set; }
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 6379;
        public string HostPort => $"{Host}:{Port}";
        public RedisOption(string host, int port) : this(host,port,null){}
        public RedisOption(string host, int port, string password)
        {
            Password = password;
            Port = port;
            Host = host;
        }
        public RedisOption(){}

    }

    public class RedisClusterOption
    {
        public RedisClusterOption(){}
        public RedisClusterOption(string host, int port) : this(host, port, null) { }
        public RedisClusterOption(string host, int port, string password)
        {
            Password = password;
            Port = port;
            Host = host;
        }
        public string Password { get; set; }
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 6379;
        public RedisClusterOption MasterRedis { get; set; } = null;
        public bool IsRootNode => MasterRedis == null;
        public bool CanWrite{get;private set;} = true;

        public bool CanRead { get; private set; } = true;

        public bool IsDisable { get; set; } = false;
        public bool Ping()
        {
            using (var redis=new RedisClient(Host,Port,Password))
            {
                try
                {
                    redis.Ping();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
