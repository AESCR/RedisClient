using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedisClient;
using XLibrary.Random;

namespace XRedisTest
{
    [TestClass]
    public class TestCluster
    {
        private RedisClusterClient redisCluster = new RedisClusterClient();
        private RandomNum random = new RandomNum();
        [TestMethod]
        public void TestGetRedisClient()
        {
            for (int i = 0; i < 1000; i++)
            {
                var key = random.GetRandomString(10);
                var x = redisCluster.GetRedisClient(key);
                x.Set(key, x.HostPort);
            }
        }
    }
}
