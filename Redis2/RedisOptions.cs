#region << 版 本 注 释 >>

/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。
//
// 文件名：RedisOptions
// 文件功能描述：
//
//
// 创建者：名字 AESCR
// 时间：2021/1/26 14:52:29
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

namespace Common.Utility.Memory.Redis2
{
    public class RedisOptions
    {
        public int ReconnectAttempts { get; set; } = 5;

        public int ReconnectWait { get; set; } = 500;

        /// <summary>
        /// Connection timeout in milliseconds
        /// </summary>
        public int Timeout { get; set; } = 500;

        public int SendTimeout { get; set; } = 3000;
        public int ReceiveTimeout { get; set; } = 3000;
        public List<ClusterOptions> MasterRedis = new List<ClusterOptions>();

        public ClusterOptions GetConnect(string hostPort)
        {
            var findRedis = MasterRedis.Find(x => x.HostPort == hostPort);
            return findRedis;
        }
    }

    public class ClusterOptions
    {
        public ClusterOptions()
        {
        }

        public ClusterOptions(string host, int port = 6379, string password = "")
        {
            Host = host;
            Port = port;
            Password = password;
        }

        public int DbIndex { get; set; } = 0;
        public string HostPort => $"{Host}:{Port}";
        public string Host { get; set; } = "localhost";
        public string Password { get; set; } = "";
        public int Port { get; set; } = 6379;
        public List<ClusterOptions> SlaveRedis { get; set; }
    }
}