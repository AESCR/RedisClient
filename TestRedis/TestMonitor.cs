#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。 
//
// 文件名：TestMonitor
// 文件功能描述：
//
// 
// 创建者：名字 AESCR
// 时间：2021/3/23 15:58:06
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

namespace TestRedis
{
    /// <summary>
    /// https://blog.csdn.net/qq_41453285/article/details/103290903
    /// </summary>
    [TestClass]
    public class TestMonitor
    {
       

        //, "127.0.0.1:6381", "127.0.0.1:6382", "127.0.0.1:6383", "127.0.0.1:6384", "127.0.0.1:6385", "127.0.0.1:6386"
        [TestInitialize]
        public void TestInit()
        {
        }
        [TestMethod]
        public void TestInfo()
        {
            RedisClient redis2 = new RedisClient("121.36.213.19:6380,defaultDatabase=0,password=redis@2020");
            redis2.Connected += Redis2_Connected;
            redis2.Connected += Redi3_Connected;
            redis2.Connected += Redi4_Connected;
            redis2.Connect();
            redis2.Connect();
            while (true)
            {
          
                var X= redis2.Add("AESCR",TimeSpan.FromSeconds(10));
            }
        }
        private void Redi4_Connected(object sender, EventArgs e)
        {

        }
        private void Redi3_Connected(object sender, EventArgs e)
        {
            
        }
        private void Redis2_Connected(object sender, EventArgs e)
        {
           
        }
    }
}
