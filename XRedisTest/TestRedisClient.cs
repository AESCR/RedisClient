#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。 
//
// 文件名：TestRedisClient
// 文件功能描述：
//
// 
// 创建者：名字 AESCR
// 时间：2021/2/25 15:51:44
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
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XRedisTest
{
    [TestClass]
    public class TestRedisClient
    {
        private RedisClient.RedisClient redis = new RedisClient.RedisClient("127.0.0.1", 6379);
        [TestMethod]
        public void TestInfo()
        {
           var x=  redis.Info("replication");
        }
    }
}
