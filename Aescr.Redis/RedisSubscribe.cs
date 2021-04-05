using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aescr.Redis
{
    public class RedisSubscribe : IDisposable
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
        private readonly RedisSocket _redisSocket;
        private readonly RedisConnection _connection;
        public bool IsConnected => _redisSocket?.IsConnected ?? false;
        public string Host => _connection?.Host ?? "127.0.0.1";
        public int Database => _connection?.Database ?? 0;
        public event Action<string[]> SubscribeReceive;
        private readonly List<string> _channel = new();
        private readonly System.Threading.Timer _receiveTimer;
        /// <summary>
        /// 订阅key过期事件监听
        /// </summary>
        public void KeyExpiredListener()
        {
            var status= _redisSocket.SendCommand("CONFIG SET notify-keyspace-events Ex");
            AddChannel($"__keyevent@{Database}__:expired");
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
            _connection = connection;
            _redisSocket = new RedisSocket(_connection.Host, _connection.Ssl, _connection.Encoding);
            _redisSocket.Connected += redisSocket_Connected;
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
            return _redisSocket.SendExpectedArray("PSUBSCRIBE", channel);
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
            return _redisSocket.SendExpectedArray("SUBSCRIBE", channel);
        }
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string[] Unsubscribe(params string[] channel)
        {
            RemoveChannel(channel);
            return _redisSocket.SendExpectedArray("UNSUBSCRIBE", channel);
        }
        /// <summary>
        /// 取消全部订阅
        /// </summary>
        public void Close()
        {
            Unsubscribe(_channel.ToArray());
        }
        private void redisSocket_Connected(object sender, System.EventArgs e)
        {
            _redisSocket.Auth(_connection.Password);
            _redisSocket.SendExpectedOk("SELECT", Database.ToString());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                _redisSocket?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}