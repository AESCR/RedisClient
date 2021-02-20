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
        private AllotRedisClient redisCluster = new AllotRedisClient();
        private RandomNum random = new RandomNum();
        [TestMethod]
        public void TestGetRedisClient()
        {
            var randomStr = "aescr";
            var redisClient= redisCluster.GetWriteClient(randomStr);
            redisClient.Set(randomStr, "100");
        }
    }
}
