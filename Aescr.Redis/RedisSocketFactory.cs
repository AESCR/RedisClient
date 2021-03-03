using System.Collections;

namespace Aescr.Redis
{
    public  class RedisSocketFactory
    {
        private  readonly Hashtable Hashtable;

        private  RedisSocketFactory()
        {
            Hashtable = Hashtable.Synchronized(new Hashtable());
        }
        public static RedisSocketFactory CreateRedisSocketFactory()
        {
            return new RedisSocketFactory();
        }
        public  RedisSocket GetRedisSocket(RedisConnection connection)
        {
            if (Hashtable.ContainsKey(connection.Host))
            {
                return new RedisSocket(connection);
            }
            Hashtable.Add(connection.Host, new RedisSocket(connection));
            return Hashtable[connection.Host] as RedisSocket;
        }
        public  RedisSocket GetRedisSocket(string host,string password="")
        {
            var (key, value) = RedisSocket.SplitHost(host);
            if (Hashtable.ContainsKey(host))
            {
                return new RedisSocket(key, value,password);
            }
            Hashtable.Add(host,new RedisSocket(key, value));
            return Hashtable[host] as RedisSocket;
        }
    }
}