using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XRedis;

namespace XRedisTest
{
    [TestClass]
    public class UnitTest1
    {
        private readonly RedisClient _redisClient = new RedisClient("121.36.213.19",6380, "redis@2020");
        [TestMethod]
        public void TestConnected()
        {
            _redisClient.Connect(1000);
            Assert.IsTrue(_redisClient.IsConnected);
        }
        [TestMethod]
        public void TestQuit()
        {
            _redisClient.Quit();
            Assert.IsFalse(_redisClient.IsConnected);
        }
        [TestMethod]
        public void TestSelectDb()
        {
            var result= _redisClient.Select(1);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void TestDel()
        {
            var result = _redisClient.Del("03cd7048f76649a88ab01d9c1c80108f");
            //Assert.e(result);
        }
        [TestMethod]
        public void TestExist()
        {
            var result = _redisClient.Exists("03cd7048f76649a88ab01d9c1c80108f");
            //Assert.e(result);
        }
        [TestMethod]
        public void TestPing()
        {
            for (int i = 0; i < 10; i++)
            {
                var result = _redisClient.Ping();
                Console.WriteLine(result);
            }
          
            //Assert.e(result);
        }
    }
}
