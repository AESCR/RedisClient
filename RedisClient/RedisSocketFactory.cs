using System.Collections;

namespace Aescr.Redis
{
    public static class RedisSocketFactory
    {
        private static readonly Hashtable Hashtable;

        static RedisSocketFactory()
        {
            Hashtable = new Hashtable();
        }

        public static RedisSocket GetRedisSocket(string host,string password="")
        {
            var hostPort= RedisSocket.SplitHost(host);
            if (Hashtable.ContainsKey(host))
            {
                return new RedisSocket(hostPort.Key, hostPort.Value,password);
            }
            Hashtable.Add(host,new RedisSocket(hostPort.Key, hostPort.Value));
            return Hashtable[host] as RedisSocket;
        }
    }
}