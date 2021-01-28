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
    public class RedisClient: IRedisClient
    {
        RedisSocket _redisSocket;
        private bool _disposedValue;
    
        public string Host { get; private set; }
        public int Port { get; private set; }
        private string _password=String.Empty;
        public bool IsConnected => _redisSocket?.IsConnected??false;
        public RedisClient(string host= "localhost", int port= 6379,string password="")
        {
            Port = port;
            Host = host ?? throw new ArgumentNullException(nameof(host));
            if (!string.IsNullOrEmpty(password))
            {
                _password = password;
            }

            Connect(1000);
        }

        private string GetUniqueKey()
        {
            return Guid.NewGuid().ToString("N");
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
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    _redisSocket.Dispose();
                    _redisSocket = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                _disposedValue = true;
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

        public event EventHandler Connected;

        public bool Connect(int timeout)
        {
            if (!IsConnected)
            {
                _redisSocket?.Dispose(); ;
                _redisSocket = null;
                _redisSocket = new RedisSocket();
                _redisSocket.Connect(Host, Port, timeout);
                if (string.IsNullOrEmpty(_password) == false)
                {
                    return Auth();
                }
                OnConnected();
            }
            return _redisSocket.IsConnected;
        }

        public string SendCommand(string command, params string[] args)
        {
            return _redisSocket.SendCommandAnswer(command, args);
        }

        public bool Auth(string password)
        {
            if (_redisSocket.SendCommandAnswerOk("Auth", password)==false)
            {
                _redisSocket.Close();
                return false;
            }
            return true;
        }
        public bool Auth()
        {
            return Auth(_password);
        }
        public string Echo(string message)
        {
            throw new NotImplementedException();
        }

        public string Ping()
        {
            return _redisSocket.SendCommandAnswer("PING", GetUniqueKey());
        }

        public bool Quit()
        {
            if (_redisSocket.SendCommandAnswerOk("QUIT"))
            {
                _redisSocket.Close();
            }
            return _redisSocket.IsConnected;
        }

        public bool Select(int index)
        {
            return _redisSocket.SendCommandAnswerOk("SELECT",index.ToString());
        }

        public int Del(params string[] keys)
        {
            return _redisSocket.SendCommandAnswerInt("DEL", keys);
        }

        public bool Exists(string key)
        {
            return _redisSocket.SendCommandAnswerInt("EXISTS", key)==1;
        }

        public long Exists(string[] keys)
        {
            throw new NotImplementedException();
        }

        public bool Expire(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public bool Expire(string key, int seconds)
        {
            throw new NotImplementedException();
        }

        public bool ExpireAt(string key, DateTime expirationDate)
        {
            throw new NotImplementedException();
        }

        public bool ExpireAt(string key, int timestamp)
        {
            throw new NotImplementedException();
        }

        public string[] Keys(string pattern)
        {
            throw new NotImplementedException();
        }

        public string Migrate(string host, int port, string key, int destinationDb, int timeoutMilliseconds)
        {
            throw new NotImplementedException();
        }

        public string Migrate(string host, int port, string key, int destinationDb, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public bool Move(string key, int database)
        {
            throw new NotImplementedException();
        }

        public string RandomKey()
        {
            throw new NotImplementedException();
        }

        public string Rename(string key, string newKey)
        {
            throw new NotImplementedException();
        }

        public bool RenameNx(string key, string newKey)
        {
            throw new NotImplementedException();
        }

        public string Restore(string key, long ttlMilliseconds, byte[] serializedValue)
        {
            throw new NotImplementedException();
        }

        public string Type(string key)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }
    }
}
