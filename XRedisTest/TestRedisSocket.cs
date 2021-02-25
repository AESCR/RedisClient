#region << 版 本 注 释 >>

/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。
//
// 文件名：TestRedisSocket
// 文件功能描述：
//
//
// 创建者：名字 AESCR
// 时间：2021/2/25 13:00:44
//
// 修改人：
// 时间：
// 修改说明：
//
// 修改人：
// 时间：
// 修改说明：
//
// 版本：V1.0.0
//----------------------------------------------------------------*/

#endregion << 版 本 注 释 >>

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedisClient;

namespace XRedisTest
{
    [TestClass]
    public class TestRedisSocket
    {
        private RedisSocket redisSocket = new RedisSocket("127.0.0.1", 6379);

        [TestMethod]
        public void TestSend()
        {
            var x = redisSocket.SendCommand("CONFIG", "GET", "*");
        }
        [TestMethod]
        public void TestInfo()
        {
            var x = redisSocket.SendCommand("iNF", "GET", "*");
        }
    }
}