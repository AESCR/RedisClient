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