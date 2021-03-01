#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。 
//
// 文件名：RedisSentinelClient
// 文件功能描述：
//
// 
// 创建者：名字 AESCR
// 时间：2021/2/26 11:41:30
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Aescr.Redis
{
    public class RedisMonitor
    {
        public bool IsNodes => Master==null;
        public RedisMonitor Master { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
    }
    public class RedisMonitorClient
    {
        private readonly List<RedisMonitor> _monitors;
        /// <summary>
        /// 有效的配置
        /// </summary>
        private  List<string> _runingMaster = new List<string>();

        private bool _isPing = false;
        public RedisMonitorClient()
        {
            _monitors = new List<RedisMonitor>();
        }
        /// <summary>
        /// 添加要监听Redis
        /// </summary>
        /// <param name="host"></param>
        /// <param name="password">密码</param>
        public void AddMonitor(string host,string password="")
        {
            if (_monitors.Exists(x=>x.Host==host)==false)
            {
                _monitors.Add(new RedisMonitor(){Host = host,Password = password});
            }
        }
        /// <summary>
        /// 随机切换
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public string SwitchMonitor(string host)
        {
            return "";
        }
        /// <summary>
        /// 监测所有有效配置
        /// </summary>
        public void PingAll()
        {
            if (_isPing)
            {
                return;
            }
            _isPing = true;
            List<string> runing = new List<string>();
            var aps= _monitors.AsParallel();
            aps.ForAll(ap =>
            {
                var rs= RedisSocketFactory.GetRedisSocket(ap.Host,ap.Password);
                if (!rs.Ping())
                {
                    if (ap.IsNodes)
                    {
                        
                    }
                    return;
                };
                lock (runing)
                {
                    runing.Add(ap.Host);
                }
            });
            _runingMaster = new List<string>(runing);
            _isPing = false;
        }

        public List<string> GetNodes()
        {
            return _runingMaster;
        }
    }
}
