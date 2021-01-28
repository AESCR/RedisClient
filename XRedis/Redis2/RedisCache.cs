#region << 版 本 注 释 >>

/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。
//
// 文件名：RedisCache
// 文件功能描述：
//
//
// 创建者：名字 AESCR
// 时间：2021/1/26 14:48:31
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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using XRedis;

#if false
    
namespace Common.Utility.Memory.Redis2
{
    public class RedisCache : IRedisCache
    {
        private readonly KetamaNodeLocator ketamaNode;
        private readonly RedisOptions options = new RedisOptions();
        private SortedList<string, RedisClient> _redisPools = new SortedList<string, RedisClient>();

        public RedisCache(Action<RedisOptions> optionAction = null)
        {
            optionAction?.Invoke(options);
            var ips = options.MasterRedis.Select(x => x.HostPort).ToList();
            ketamaNode = new KetamaNodeLocator(ips);
        }

        /// <summary>
        /// 分配客户端
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private RedisClient AllotRedisClient(string key)
        {
            var host = ketamaNode.GetNodes(key);
            var op = options.GetConnect(host);
            lock (_redisPools)
            {
                if (_redisPools.ContainsKey(op.HostPort))
                {
                    return _redisPools[op.HostPort];
                }
                var redisClient = new RedisClient(op.Host, op.Port);
                if (!string.IsNullOrWhiteSpace(op.Password))
                {
                    redisClient.Connected += (s, e) =>
                    {
                        redisClient.Auth(op.Password);
                    };
                }
                redisClient.ReconnectAttempts = options.ReconnectAttempts;//失败后重试3次
                redisClient.ReconnectWait = options.ReconnectWait;//在抛出异常之前，连接将在200ms之间重试3次
                redisClient.Connect(options.Timeout);
                _redisPools.Add(op.HostPort, redisClient);
                return redisClient;
            }
        }

        /// <summary>
        /// 序列化String
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string SerializeObject(object value)
        {
            return value != null ? JsonConvert.SerializeObject(value) : String.Empty;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private T DeserializeObject<T>(string value)
        {
            return value != null ? JsonConvert.DeserializeObject<T>(value) : default;
        }

        private bool StringOk(string val)
        {
            if (val == null)
            {
                return false;
            }
            return val.ToLower() == "ok";
        }

        public bool Add<T>(string key, T obj, bool overwrite = false)
        {
            var redisClient = AllotRedisClient(key);
            var str = SerializeObject(obj);
            if (overwrite)
            {
                DelKey(key);
            }
            lock (redisClient)
            {
                var result = redisClient.SetNx(key, str);
                return result;
            }
        }

        public bool Add<T>(string key, T obj, TimeSpan timeSpan, bool relative = false, bool overwrite = false)
        {
            var redisClient = AllotRedisClient(key);
            var str = SerializeObject(obj);
            if (overwrite)
            {
                DelKey(key);
            }
            lock (redisClient)
            {
                var result = redisClient.SetNx(key, str);
                redisClient.Expire(key, timeSpan);
                return result;
            }
        }

        public string Add<T>(T obj, TimeSpan timeSpan, bool relative = false, bool overwrite = false)
        {
            var key = Guid.NewGuid().ToString("N");
            return Add(key, obj, timeSpan, relative, overwrite) ? key : String.Empty;
        }

        public string Add<T>(T obj, bool overwrite = false)
        {
            var key = Guid.NewGuid().ToString("N");
            return Add(key, obj, overwrite) ? key : String.Empty;
        }

        public bool AddHash<T>(string key, string filed, T value, bool overwrite = false)
        {
            var redisClient = AllotRedisClient(key);
            if (overwrite)
            {
                DelHashField(key, filed);
            }
            var str = SerializeObject(value);
            lock (redisClient)
            {
                return redisClient.HSetNx(key, filed, str);
            }
        }

        public bool AddHash<T>(string key, Dictionary<string, T> kv, bool overwrite = false)
        {
            var redisClient = AllotRedisClient(key);
            if (overwrite)
            {
                DelKey(key);
            }
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string k in kv.Keys)
            {
                result.Add(k, SerializeObject(kv[k]));
            }
            lock (redisClient)
            {
                return StringOk(redisClient.HMSet(key, result));
            }
        }

        public bool AddList<T>(string key, params T[] value)
        {
            var redisClient = AllotRedisClient(key);
            string[] str = new string[value.Length];
            for (var i = 0; i < value.Length; i++)
            {
                var val = value[i];
                str[i] = SerializeObject(val);
            }
            lock (redisClient)
            {
                redisClient.RPush(key, str);
            }
            return true;
        }

        public bool AddList<T>(string key, T value)
        {
            var redisClient = AllotRedisClient(key);
            var str = SerializeObject(value);
            lock (redisClient)
            {
                return redisClient.RPush(key, str) > 0;
            }
        }

        public bool AddSet<T>(string key, params T[] value)
        {
            throw new NotImplementedException();
        }

        public bool AddSortSet<T>(string key, params T[] value)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            var redisClient = AllotRedisClient(key);
            return redisClient.Exists(key);
        }

        public bool DelKey(params string[] keys)
        {
            foreach (string key in keys)
            {
                var redisClient = AllotRedisClient(key);
                if (string.IsNullOrWhiteSpace(key)) return false;
                redisClient.Del(key);
            }
            return true;
        }

        public bool DelKey(string key)
        {
            var redisClient = AllotRedisClient(key);
            if (string.IsNullOrWhiteSpace(key)) return false;
            return redisClient.Del(key) > 0;
        }

        public bool DelHashField(string key, string field)
        {
            throw new NotImplementedException();
        }

        public bool ReName(string key, string newKey)
        {
            var redisClient = AllotRedisClient(key);
            return StringOk(redisClient.Rename(key, newKey));
        }

        public bool Expire(string key, TimeSpan timeSpan)
        {
            var redisClient = AllotRedisClient(key);
            return redisClient.Expire(key, timeSpan);
        }

        public bool Expire(string[] keys, TimeSpan timeSpan)
        {
            foreach (var key in keys)
            {
                var redisClient = AllotRedisClient(key);
                redisClient.Expire(key, timeSpan);
            }
            return true;
        }

        public bool Persist(string[] keys)
        {
            foreach (var key in keys)
            {
                var redisClient = AllotRedisClient(key);
                redisClient.Persist(key);
            }
            return true;
        }

        public bool Persist(string key)
        {
            var redisClient = AllotRedisClient(key);
            return redisClient.Persist(key);
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public T GetString<T>(string key)
        {
            var redisClient = AllotRedisClient(key);
            return DeserializeObject<T>(redisClient.Get(key));
        }

        public T[] GetStrings<T>(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public string[] GetHashFileds(string key)
        {
            throw new NotImplementedException();
        }

        public string GetHashValue(string key, string files)
        {
            throw new NotImplementedException();
        }

        public string[] GetHashValues(string key, params string[] files)
        {
            throw new NotImplementedException();
        }

        private void Close(RedisClient redisClient)
        {
            if (redisClient != null)
            {
                lock (redisClient)
                {
                    if (redisClient.IsConnected)
                    {
                        redisClient?.Quit();
                    }
                    redisClient?.Dispose();
                }
            }
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                foreach (var redisClient in _redisPools.Values)
                {
                    Close(redisClient);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RedisCache()
        {
            Dispose(false);
        }
    }
}
#endif
