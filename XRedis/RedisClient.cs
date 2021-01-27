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
using System.Net.Sockets;
using System.Text;

namespace XRedis
{
    public class RedisClient:IDisposable
    {
        RedisSocket socket;
        private bool disposedValue;
        private bool disposedValue1;

        public string Host { get; private set; }
        public int Port { get; private set; }
        public int SendTimeout { get; set; }
        public string Password { get; set; }
        public event EventHandler Connected;
        public RedisClient(string host= "localhost", int port= 6379,string password="")
        {
            if (host == null)
                throw new ArgumentNullException("host");
            Port = port;
            Host = host;
            if (!string.IsNullOrEmpty(password))
            {
                Password = password;
            }
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
        /// <summary>
        /// 执行Redis命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool SendCommand(string cmd, params object[] args)
        {
            return false;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue1)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    socket.Close();
                    socket = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue1 = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~RedisClient()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
