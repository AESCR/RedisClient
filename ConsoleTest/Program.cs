using RedisClient;
using System;
using System.Threading;
using System.Threading.Tasks;
using XLibrary.Random;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RandomNum randomNum = new RandomNum();
            RedisClusterClient redisCluster = new RedisClusterClient();
            Task.Run(() =>
            {
                while (true)
                {
                    redisCluster.GetClusterStatus();
                    Thread.Sleep(5000);
                }
            });
            int i = 0;
            while (true)
            {
                try
                {
                    var index = randomNum.GenerateCheckCodeNum(10);
                    var randomStr = "aescr" + index;
                    var redisClient = redisCluster.Set(randomStr, "100");
                    i++;
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}