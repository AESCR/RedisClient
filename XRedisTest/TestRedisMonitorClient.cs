using Aescr.Redis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XRedisTest
{
    [TestClass]
    public class TestRedisMonitorClient
    {
        private readonly RedisMonitorClient redisMonitorClient = new RedisMonitorClient();
        [TestMethod]
        public void TestLoading()
        {
            redisMonitorClient.AddMonitor("127.0.0.1");
        }
        [TestMethod]
        public void TestPing()
        {
            redisMonitorClient.AddMonitor("127.0.0.1");
            redisMonitorClient.PingAll();
            var nodes= redisMonitorClient.GetNodes();
        }
    }
}