using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XRedis;

namespace XRedisTest
{
    [TestClass]
    public class UnitTest1
    {
        private readonly RedisClient _redisClient = new RedisClient("121.36.213.19",6380, "redis@2020");

        public UnitTest1()
        {
            _redisClient.Select(2);
        }
        #region  Redis ¼ü(key) ÃüÁî
        [TestMethod]
        public void TestType()
        {
            var x= _redisClient.Select(2);
            var key = _redisClient.RandomKey();
            var result = _redisClient.Type(key);
            Console.WriteLine(result);
        }
        [TestMethod]
        public void TestPexpireat()
        {
           var x=  _redisClient.Set("w3ckey", "redis");
           Assert.AreEqual(x, "OK");
           var y= _redisClient.PExpireAt("w3ckey", 1612083519000);
           Assert.AreEqual(y, 1);
        }
        #endregion

    }
}
