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

        public void AddChannel(params string[] channels)
        {
            foreach (var cl in channels)
            {
                if (string.IsNullOrWhiteSpace(cl) == false && _channel.Contains(cl) == false)
                {
                    _channel.Add(cl);
                }
            }
        }
        public void RemoveChannel(params string[] channels)
        {
            foreach (var cl in channels)
            {
                if (string.IsNullOrWhiteSpace(cl) == false && _channel.Contains(cl))
                {
                    _channel.Remove(cl);
                }
            }
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
            AddChannel(channel);
            return _redisSocket.SendExpectedArray("PSUBSCRIBE", channel);
        }
        public string[] Subscribe(List<string> channel)
        {
            return Subscribe(channel.ToArray());
        }
        public string[] Subscribe(params string[] channel)
        {
            AddChannel(channel);
            return _redisSocket.SendExpectedArray("SUBSCRIBE", channel);
        }
        public string[] Unsubscribe(params string[] channel)
        {
            RemoveChannel(channel);
            return _redisSocket.SendExpectedArray("UNSUBSCRIBE", channel);
        }
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