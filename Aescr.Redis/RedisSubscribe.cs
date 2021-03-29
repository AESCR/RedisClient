using System;
using System.Threading.Tasks;

namespace Aescr.Redis
{
    public class RedisSubscribe:IDisposable
    {
        private RedisSocket _redisSocket;
        private RedisConnection _connection;
        public bool IsConnected => _redisSocket?.IsConnected ?? false;
        public string Host => _connection?.Host ?? "127.0.0.1";
        public int Database => _connection?.Database ?? 0;
        public event EventHandler<string> Subscribe
        {
            add => _redisSocket.Subscribe += value;
            remove => _redisSocket.Subscribe -= value;
        }
        public RedisSubscribe(string connection)
        {
             _connection = connection;
            _redisSocket = new RedisSocket(_connection.Host, _connection.Ssl, _connection.Encoding);
            _redisSocket.Connected += redisSocket_Connected;
        }

        public void Receive()
        {
            Task.Run(() =>
            {
                _redisSocket.ReceiveSubscribe();
            });
        }

        private void redisSocket_Connected(object sender, System.EventArgs e)
        {
            _redisSocket.Auth(_connection.Password);
            _redisSocket.SendExpectedOk("SELECT", Database.ToString());
        }

        public void Dispose()
        {
            _redisSocket?.Dispose();
        }
    }
}