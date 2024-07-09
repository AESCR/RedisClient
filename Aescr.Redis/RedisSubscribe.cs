using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aescr.Redis
{
    public class RedisSubscribe :IRedisSubscribe, IDisposable
    {
        private int _speed = TimeSpan.FromMilliseconds(100).Milliseconds;

        public int Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                Reload();
            }
        }

        public bool ReceiveEnabled
        {
            get => _timerEnabled;
            set
            {
                if (value && _timerEnabled ==false)
                {
                    _timerEnabled = true;
                    Reload();
                }
                _timerEnabled = value;
            }
        }
        private bool _timerEnabled=false;
        private readonly RedisClient _redisClient;
        public bool IsConnected => _redisClient.IsConnected;
        public string Host => _redisClient.Host;
        public event Action<string[]> SubscribeReceive;
        private readonly List<string> _channel = new();
        private readonly System.Threading.Timer _receiveTimer;
        /// <summary>
        /// 订阅key过期事件监听
        /// </summary>
        public void KeyExpiredListener()
        {
            var status= _redisClient.SendCommand("CONFIG SET notify-keyspace-events Ex");
            AddChannel($"__keyevent@{_redisClient.Database}__:expired");
        }
        public void Reload()
        {
            if (_channel.Count==0)
            {
                _timerEnabled = false;
                _receiveTimer.Change(Timeout.Infinite, Speed);
            }
            if (_timerEnabled)
            {
                _receiveTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(Speed));
            }
            else
            {
                _receiveTimer.Change(Timeout.Infinite, Speed);
            }
        }
        public RedisSubscribe(string connection)
        {
            _redisClient = new RedisClient(connection);
            _receiveTimer = new Timer(ReceiveSubscribe, _channel, Timeout.Infinite, Speed);
        }
        /// <summary>
        /// 订阅频道
        /// </summary>
        /// <param name="channels"></param>
        public void AddChannel(params string[] channels)
        {
            foreach (var cl in channels)
            {
                if (string.IsNullOrWhiteSpace(cl) == false && _channel.Contains(cl) == false)
                {
                    _channel.Add(cl);
                }
            }
            ReceiveEnabled = _channel.Count>0;
        }
        /// <summary>
        /// 移除的订阅频道
        /// </summary>
        /// <param name="channels"></param>
        public void RemoveChannel(params string[] channels)
        {
            foreach (var cl in channels)
            {
                if (string.IsNullOrWhiteSpace(cl) == false && _channel.Contains(cl))
                {
                    _channel.Remove(cl);
                }
            }
            ReceiveEnabled = _channel.Count > 0;
        }
        private void ReceiveSubscribe(object channel)
        {
            if (channel is List<string> channelArray)
            {
                if (channelArray.Count>0)
                {
                    var msg = Subscribe(channelArray);
                    if (msg[0] == "message")
                    {
                        SubscribeReceive?.Invoke(msg);
                    }
                }
            }
            else
            {
                ReceiveEnabled = false;
            }
        }

        public int Publish(string channel, string message)
        {
            throw new NotImplementedException();
        }

        public string[] PSubscribe(params string[] channel)
        {
            if (channel == null || channel.Length == 0)
            {
                channel = _channel.ToArray();
                if (channel.Length==0)
                {
                    throw new Exception("请先订阅频道");
                }
            }
            AddChannel(channel);
            return _redisClient.SendCommand("PSUBSCRIBE", channel).ResponseArray();
        }
        private string[] Subscribe(List<string> channel)
        {
            return Subscribe(channel.ToArray());
        }
        /// <summary>
        /// 订阅频道
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string[] Subscribe(params string[] channel)
        {
            if (channel==null||channel.Length==0)
            {
                channel = _channel.ToArray();
                if (channel.Length == 0)
                {
                    throw new Exception("请先订阅频道");
                }
            }
            AddChannel(channel);
            return _redisClient.SendCommand("SUBSCRIBE", channel).ResponseArray();
        }

        public string[] PubSub(string subCommand, params string[] argument)
        {
            return _redisClient.SendCommand(subCommand, argument).ResponseArray();
        }

        public string[] PunSubscribe(string pattern)
        {
            return _redisClient.SendCommand(pattern).ResponseArray();
        }
        public string[] Unsubscribe(params string[] channel)
        {
            RemoveChannel(channel);
            return _redisClient.SendCommand("UNSUBSCRIBE", channel).ResponseArray();
        }
        /// <summary>
        /// 取消全部订阅
        /// </summary>
        public void Close()
        {
            Unsubscribe(_channel.ToArray());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                _redisClient?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}