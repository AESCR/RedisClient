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
            for (int i = 0; i < 100; i++)
            {
                var randomStr = "aescr"+ i;
                var redisClient = redisCluster.Set(randomStr, "100");
            }
        
        }
    }
}