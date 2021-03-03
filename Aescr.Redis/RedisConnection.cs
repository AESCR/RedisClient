#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。 
//
// 文件名：ConnectionStringBuilder
// 文件功能描述：
//
// 
// 创建者：名字 AESCR
// 时间：2021/2/26 11:42:29
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
using System.Text;
using System.Text.RegularExpressions;

namespace Aescr.Redis
{
    public class RedisConnection: WeightedRoundRobinServer
    {
        public string Host { get; set; } = "127.0.0.1:6379";
        public string Role { get; set; } 
        public bool Ssl { get; set; } = false;
        public string User { get; set; }
        public string Password { get; set; }
        public int Database { get; set; } = 0;
        public string Prefix { get; set; }
        public string ClientName { get; set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromSeconds(20);
        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan ReceiveTimeout { get; set; } = TimeSpan.FromSeconds(20);
        public TimeSpan SendTimeout { get; set; } = TimeSpan.FromSeconds(20);
        public int MaxPoolSize { get; set; } = 100;
        public int MinPoolSize { get; set; } = 1;
        public int Retry { get; set; } = 0;
        public static implicit operator RedisConnection(string connectionString) => Parse(connectionString);
        public static implicit operator string(RedisConnection connectionString) => connectionString.ToString();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.IsNullOrWhiteSpace(Host) ? "127.0.0.1:6379" : Host);
            if (Ssl) sb.Append(",ssl=true");
            if (!string.IsNullOrWhiteSpace(User)) sb.Append(",user=").Append(User);
            if (!string.IsNullOrEmpty(Password)) sb.Append(",password=").Append(Password);
            if (Database > 0) sb.Append(",database=").Append(Database);

            if (!string.IsNullOrWhiteSpace(Prefix)) sb.Append(",prefix=").Append(Prefix);
            if (!string.IsNullOrWhiteSpace(ClientName)) sb.Append(",client name=").Append(ClientName);
            if (!Equals(Encoding, Encoding.UTF8)) sb.Append(",encoding=").Append(Encoding.BodyName);

            if (IdleTimeout != TimeSpan.FromSeconds(20)) sb.Append(",idle timeout=").Append((long)IdleTimeout.TotalMilliseconds);
            if (ConnectTimeout != TimeSpan.FromSeconds(10)) sb.Append(",connect timeout=").Append((long)ConnectTimeout.TotalMilliseconds);
            if (ReceiveTimeout != TimeSpan.FromSeconds(10)) sb.Append(",receive timeout=").Append((long)ReceiveTimeout.TotalMilliseconds);
            if (SendTimeout != TimeSpan.FromSeconds(10)) sb.Append(",send timeout=").Append((long)SendTimeout.TotalMilliseconds);
            if (MaxPoolSize != 100) sb.Append(",max pool size=").Append(MaxPoolSize);
            if (MinPoolSize != 1) sb.Append(",min pool size=").Append(MinPoolSize);
            if (Retry != 0) sb.Append(",retry=").Append(Retry);
            if (!string.IsNullOrWhiteSpace(Role)) sb.Append(",role=").Append(Role);
            return sb.ToString();
        }

        public static RedisConnection Parse(string connectionString)
        {
            var ret = new RedisConnection();
            if (string.IsNullOrEmpty(connectionString)) return ret;

            //支持密码中带有逗号，将原有 split(',') 改成以下处理方式
            var vs = Regex.Split(connectionString, @"\,([\w \t\r\n]+)=", RegexOptions.Multiline);
            ret.Host = vs[0].Trim();

            for (var a = 1; a < vs.Length; a += 2)
            {
                var kv = new[] { Regex.Replace(vs[a].ToLower().Trim(), @"[ \t\r\n]", ""), vs[a + 1] };
                switch (kv[0])
                {
                    case "ssl": if (kv.Length > 1 && kv[1].ToLower().Trim() == "true") ret.Ssl = true; break;
                    case "userid":
                    case "user": if (kv.Length > 1) ret.User = kv[1].Trim(); break;
                    case "password": if (kv.Length > 1) ret.Password = kv[1]; break;
                    case "database":
                    case "defaultdatabase": if (kv.Length > 1 && int.TryParse(kv[1].Trim(), out var database) && database > 0) ret.Database = database; break;

                    case "prefix": if (kv.Length > 1) ret.Prefix = kv[1].Trim(); break;
                    case "name":
                    case "clientname": if (kv.Length > 1) ret.ClientName = kv[1].Trim(); break;
                    case "encoding": if (kv.Length > 1) ret.Encoding = Encoding.GetEncoding(kv[1].Trim()); break;

                    case "idletimeout": if (kv.Length > 1 && long.TryParse(kv[1].Trim(), out var idleTimeout) && idleTimeout > 0) ret.IdleTimeout = TimeSpan.FromMilliseconds(idleTimeout); break;
                    case "connecttimeout": if (kv.Length > 1 && long.TryParse(kv[1].Trim(), out var connectTimeout) && connectTimeout > 0) ret.ConnectTimeout = TimeSpan.FromMilliseconds(connectTimeout); break;
                    case "receivetimeout": if (kv.Length > 1 && long.TryParse(kv[1].Trim(), out var receiveTimeout) && receiveTimeout > 0) ret.ReceiveTimeout = TimeSpan.FromMilliseconds(receiveTimeout); break;
                    case "sendtimeout": if (kv.Length > 1 && long.TryParse(kv[1].Trim(), out var sendTimeout) && sendTimeout > 0) ret.SendTimeout = TimeSpan.FromMilliseconds(sendTimeout); break;

                    case "poolsize":
                    case "maxpoolsize": if (kv.Length > 1 && int.TryParse(kv[1].Trim(), out var maxPoolSize) && maxPoolSize > 0) ret.MaxPoolSize = maxPoolSize; break;
                    case "minpoolsize": if (kv.Length > 1 && int.TryParse(kv[1].Trim(), out var minPoolSize) && minPoolSize > 0) ret.MinPoolSize = minPoolSize; break;
                    case "retry": if (kv.Length > 1 && int.TryParse(kv[1].Trim(), out var retry) && retry > 0) ret.Retry = retry; break;
                    case "role":
                        if (kv.Length > 1) ret.Role = kv[1].Trim();
                        break;
                }
            }
            return ret;
        }
    }
}
