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
using System.Threading;
using System.Threading.Tasks;
using Aescr.Redis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XRedisTest
{
    [TestClass]
    public class TestRedisClient
    {
        private readonly RedisClient redis = new RedisClient("120.26.161.113:6379,defaultDatabase=0,password=AESCR");
        [TestInitialize]
        public void TestInit()
        {
            redis.SetPrefix("AESCR");
        }
        [TestMethod]
        public void TestAutoMasterSlave()
        {
            //redis.AutoMasterSlave();
        }
        [TestMethod]
        public void TestConnect()
        {
            var connected = redis.IsConnected;
            Assert.IsFalse(connected);
            redis.Connect();
            Assert.IsTrue(redis.IsConnected);
            var r = redis.Quit();
            Assert.AreEqual(r, false);
            redis.Connect();
            Assert.IsTrue(redis.IsConnected);
             connected = redis.IsConnected;
            Assert.IsTrue(connected);
            redis.Connect();
            Assert.IsTrue(redis.IsConnected);
             r = redis.Quit();
            Assert.AreEqual(r, false);
            redis.Connect();
            Assert.IsTrue(redis.IsConnected);
            connected = redis.IsConnected;
            Assert.IsTrue(connected);
            redis.Connect();
            Assert.IsTrue(redis.IsConnected);
            r = redis.Quit();
            Assert.AreEqual(r, false);
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
            redis.Select(10);
            Assert.AreEqual(redis.Database, 10);
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
        public void TestInfo()
        {
            var info= redis.Info();
            var s= redis.Info("Server");
        }

        #region  Redis 键(key) 命令

        [TestMethod]
        public void TestType()
        {
            redis.Add("RandomKey", TimeSpan.FromSeconds(10));
            var key = redis.RandomKey();
            var s = redis.Type(key);
        }
        [TestMethod]
        public void TestPExpire()
        {
            var x= redis.Add("RandomKey");
            var key = redis.RandomKey();
            redis.PExpire(key, TimeSpan.FromSeconds(10));
            var xx= redis.PTtl(key);
        }
        [TestMethod]
        public void TestSubscribe()
        {
            var x1 = redis.Ping();
            var x12 = redis.Ping("AESCR");
            var r= redis.Subscribe("mychannel");
            var x2= redis.Ping();
            var x22 = redis.Ping("AESCR");
            var x= redis.Add("123");
        }
        private static readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        [TestMethod]
        public void TestRecieved()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var id = Snowflake.Instance().GetId().ToString();
                    //var intxPublish = redis.Publish("AESCR", id);
                    redis.Add(id, TimeSpan.FromSeconds(1));
                }
            });
            RedisSubscribe redisSubscribe = new RedisSubscribe("120.26.161.113:6379,defaultDatabase=0,password=AESCR");
            redisSubscribe.KeyExpiredListener();
            redisSubscribe.SubscribeReceive += Redis_SubscribeReceive;
            Task.Run(() =>
            {
                while (true)
                {
                    var x = redisSubscribe.PSubscribe("__keyevent@0__:expired");
                }
            });
            manualResetEvent.WaitOne();
        }

        private void Redis_SubscribeReceive(string[] msg)
        {
            
        }
        [TestMethod]
        public void TestSetPassword()
        {
            redis.SetPassword("AESCR");
        }
        #endregion
    }
}
