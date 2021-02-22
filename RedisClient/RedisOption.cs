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

#endregion << 版 本 注 释 >>

namespace RedisClient
{
    public class RedisOption
    {
        /// <summary>
        /// redis客户端超时设置 0表示不开启空闲清除
        /// </summary>
        public int RedisTimeout { get; set; } = 600;
        private static bool _isCluster = false;
        public RedisOption MasterRedis { get; private set; }

        public void SetMasterRedis(string host, int port, string password)
        {
            MasterRedis = new RedisOption(host, port, password);
            _isCluster = true;
        }

        public void SetMasterRedis(in RedisOption option)
        {
            MasterRedis = option;
            _isCluster = true;
        }

        public void ClearMasterRedis()
        {
            MasterRedis = null;
        }

        public bool IsCluster => _isCluster;
        public string Password { get; set; }
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 6379;
        public bool IsRootNode => MasterRedis == null;
        private bool _canWrite = false;

        public bool CanWrite
        {
            get => IsRootNode || _canWrite;
            set => _canWrite = value;
        }

        public bool CanRead { get; private set; } = true;
        private bool _isDisable = false;

        public bool IsDisable
        {
            get => !ToBeEffective && _isDisable;
            set => _isDisable = value;
        }

        /// <summary>
        /// 待生效
        /// </summary>
        public bool ToBeEffective = false;

        public string HostPort => $"{Host}:{Port}";
        public string MasterCode { get; set; }

        public RedisOption(string host, int port) : this(host, port, null)
        {
        }

        public RedisOption(string host, int port, string password)
        {
            Password = password;
            Port = port;
            Host = host;
        }
        public RedisOption()
        {
        }
    }
}