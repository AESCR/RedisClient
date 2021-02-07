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
           
        }
    }
}
