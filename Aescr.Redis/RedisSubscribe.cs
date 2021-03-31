using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aescr.Redis
{
    public class RedisSubscribe : IDisposable
    {
        private readonly RedisSocket _redisSocket;
        private readonly RedisConnection _connection;
        public bool IsConnected => _redisSocket?.IsConnected ?? false;
        public string Host => _connection?.Host ?? "127.0.0.1";
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        public int Database => _connection?.Database ?? 0;
        public event Action<string, string> SubscribeReceive;
        private List<string> _channel = new();
        private string[] _channelArray;
        private Task _receiveTask;
        private System.Threading.Timer _receiveTimer;
        public RedisSubscribe(string connection)
        {
            _connection = connection;
            _redisSocket = new RedisSocket(_connection.Host, _connection.Ssl, _connection.Encoding);
            _redisSocket.Connected += redisSocket_Connected;
            _receiveTask = new Task(Receive);
        }
        private void Receive()
        {
            while (true)
            {
                if (_channelArray.Any()==false) continue;
                var msg = Subscribe(_channelArray);
                if (msg[0] == "message")
                {
                    SubscribeReceive?.Invoke(msg[1], msg[2]);
                }
            }
        }
        public string[] PSubscribe(params string[] channel)
        {
            foreach (var cl in channel)
            {
                if (string.IsNullOrWhiteSpace(cl) == false && _channel.Contains(cl) == false)
                {
                    _channel.Add(cl);
                }
            }
            _channelArray = _channel.ToArray();
            return _redisSocket.SendExpectedArray("PSUBSCRIBE", channel);
        }
        public string[] Subscribe(params string[] channel)
        {
            foreach (var cl in channel)
            {
                if (string.IsNullOrWhiteSpace(cl) == false && _channel.Contains(cl) == false)
                {
                    _channel.Add(cl);
                }
            }
            _channelArray = _channel.ToArray();
            return _redisSocket.SendExpectedArray("SUBSCRIBE", channel);
        }
        public string[] Unsubscribe(params string[] channel)
        {
            foreach (var cl in channel)
            {
                if (string.IsNullOrWhiteSpace(cl) == false && _channel.Contains(cl))
                {
                    _channel.Remove(cl);
                }
            }
            _channelArray = _channel.ToArray();
            return _redisSocket.SendExpectedArray("UNSUBSCRIBE", channel);
        }
        public void Close()
        {
            _tokenSource.Cancel(false);
            Unsubscribe(_channel.ToArray());
        }

        private void redisSocket_Connected(object sender, System.EventArgs e)
        {
            _redisSocket.Auth(_connection.Password);
            _redisSocket.SendExpectedOk("SELECT", Database.ToString());
            if (_receiveTask.Status == TaskStatus.Created)
            {
               _receiveTask.Start();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _redisSocket?.Dispose();
                _tokenSource?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}