﻿#region << 版 本 注 释 >>
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
using Aescr.Redis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XRedisTest
{
    [TestClass]
    public class TestRedisClient
    {
        private readonly RedisClient redis = new RedisClient();
        [TestMethod]
        public void TestConnect()
        {
            var connected= redis.IsConnected;
            Assert.IsFalse(connected);
            redis.Connect();
            Assert.IsTrue(redis.IsConnected);
        }
        [TestMethod]
        public void TestAdd()
        {
            var key = redis.Add("123",TimeSpan.Zero);
            var val= redis.Get(key);
            Assert.AreEqual("123",val);
        }
        [TestMethod]
        public void TestExists()
        {
            var key = redis.Add("123", TimeSpan.Zero);
            var val = redis.Exists(key);
            Assert.IsTrue( val);
        }
        [TestMethod]
        public void TestRole()
        {
            var key = redis.Role();
        }
        [TestMethod]
        public void TestSelect()
        {
            var key = redis.Select(1);
            Assert.AreEqual(redis.Database, 1);
        }
        [TestMethod]
        public void TestAppend()
        {
            var key = redis.Add("123",TimeSpan.Zero);
            var keyLen = redis.Append(key,"456");
            var val = redis.Get(key);
            Assert.AreEqual("123456", val);
        }
        [TestMethod]
        public void TestCommand()
        {
            var key = redis.Command();
        }
        [TestMethod]
        public void TestAuth()
        {
            Random random=new Random();
            var rNext= random.Next(100, 999);
            var key = redis.Auth(rNext.ToString());
            Assert.IsFalse(key);
        }
        [TestMethod]
        public void TestDbSize()
        {
            var dbSize= redis.DbSize();
            redis.Add("54",TimeSpan.Zero);
            var dbSize2 = redis.DbSize();
            Assert.AreEqual(dbSize, dbSize2-1);
        }
        [TestMethod]
        public void TestPing()
        {
            var ping = redis.Ping();
            Assert.AreEqual(ping, true);
        }
        [TestMethod]
        public void TestPing2()
        {
            var r = redis.Set("123123", "232");
            Assert.AreEqual(r, true);
        }
    }
}
