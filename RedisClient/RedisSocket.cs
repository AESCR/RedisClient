using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Aescr.Redis
{
    public class RedisSocket : IDisposable
    {
        /// <summary>
        /// 断开连接
        /// </summary>
        private bool _exit = false;

        private const string Crlf = "\r\n";
        private Socket _socket;
        private BufferedStream _bstream;
        public bool IsConnected => _socket?.Connected ?? false;
        public int Database => _connection.Database;
        public string Prefix => _connection.Prefix;
        public string ClientName => _connection.ClientName;
        public bool Ssl => _connection.Ssl;
        public Encoding Encoding => _connection.Encoding;
        private readonly object _lockObject = new object();
        public event EventHandler<EventArgs> Connected;
        private readonly RedisConnection _connection;
        public RedisSocket(string connection)
        {
            _connection = connection;
            InitSocket();
        }
        public RedisSocket(string ip, int port, string password)
        {
            _connection = new RedisConnection {Host = $"{ip}:{port}", Password = password};
            InitSocket();
        }

        public RedisSocket(string ip, int port) : this(ip, port, "")
        {
        }

        private void InitSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                ReceiveTimeout = (int)_connection.ReceiveTimeout.TotalMilliseconds,
                SendTimeout = (int)_connection.SendTimeout.TotalMilliseconds,
            };
        }
        /// <summary>
        /// Redis服务器进行连接
        /// </summary>
        /// <returns>连接状态</returns>
        public bool Connect()
        {
            if (IsConnected) return IsConnected;
            if (_exit)
            {
                throw new Exception("redis 已断开连接");
            }
            var hostPort = SplitHost(_connection.Host);
            if (IsConnected) return IsConnected;
            lock (_lockObject)
            {
                if (IsConnected) return IsConnected;
                _socket.Connect(hostPort.Key, hostPort.Value);
                _bstream = new BufferedStream(new NetworkStream(_socket), 16 * 1024);
                if (string.IsNullOrWhiteSpace(_connection.Password) == false)
                {
                    if (SendExpectedOk("Auth", _connection.Password) == false)
                    {
                        throw new Exception("redis 密码认证失败");
                    }
                }
                if (_connection.Database > 0)
                {
                    Select(_connection.Database);
                }
                OnConnected();
            }
            return IsConnected;
        }

        public static KeyValuePair<string, int> SplitHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host?.Trim()))
                return new KeyValuePair<string, int>("127.0.0.1", 6379);

            host = host.Trim();
            var ipv6 = Regex.Match(host, @"^\[([^\]]+)\]\s*(:\s*(\d+))?$");
            if (ipv6.Success) //ipv6+port 格式： [fe80::b164:55b3:4b4f:7ce6%15]:6379
                return new KeyValuePair<string, int>(ipv6.Groups[1].Value.Trim(),
                    int.TryParse(ipv6.Groups[3].Value, out var tryint) && tryint > 0 ? tryint : 6379);

            var spt = (host ?? "").Split(':');
            if (spt.Length == 1) //ipv4 or domain
                return new KeyValuePair<string, int>(string.IsNullOrWhiteSpace(spt[0].Trim()) == false ? spt[0].Trim() : "127.0.0.1", 6379);

            if (spt.Length == 2) //ipv4:port or domain:port
            {
                if (int.TryParse(spt.Last().Trim(), out var testPort2))
                    return new KeyValuePair<string, int>(string.IsNullOrWhiteSpace(spt[0].Trim()) == false ? spt[0].Trim() : "127.0.0.1", testPort2);

                return new KeyValuePair<string, int>(host, 6379);
            }

            if (IPAddress.TryParse(host, out var tryip) && tryip.AddressFamily == AddressFamily.InterNetworkV6) //test ipv6
                return new KeyValuePair<string, int>(host, 6379);

            if (int.TryParse(spt.Last().Trim(), out var testPort)) //test ipv6:port
            {
                var testHost = string.Join(":", spt.Where((a, b) => b < spt.Length - 1));
                if (IPAddress.TryParse(testHost, out tryip) && tryip.AddressFamily == AddressFamily.InterNetworkV6)
                    return new KeyValuePair<string, int>(testHost, 6379);
            }

            return new KeyValuePair<string, int>(host, 6379);
        }
        /// <summary>
        /// 用来测试连接是否存活，或者测试延迟。
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            try
            {
                return SendExpectedString("PING","AESCR")=="AESCR";
            }
            catch
            {
                return false;
            }
        }
        public RedisAnswer SendCommand(RedisCommand command)
        {
            return SendCommand(command.Cmd, command.Args);
        }
        public RedisAnswer SendCommand(string cmd, params string[] args)
        {
            string resp = "*" + (1 + args.Length) + Crlf;
            resp += "$" + cmd.Length + Crlf + cmd + Crlf;
            foreach (string arg in args)
            {
                string argStr = arg.Trim();
                int argStrLength = Encoding.GetByteCount(argStr);
                resp += "$" + argStrLength + Crlf + argStr + Crlf;
            }
            byte[] r = Encoding.GetBytes(resp);
            if (Connect()==false)
            {
                throw new Exception("与Redis服务器连接失败！");
            }
            try
            {
                _bstream.Write(r, 0, r.Length);
                _bstream.Flush();
            }
            catch
            {
                throw new Exception("SendCommand失败！");
            }
            return Parse();
        }
        public string[] SendMultipleCommands(params RedisCommand[] command)
        {
            lock (_lockObject)
            {
                if (!SendExpectedOk("Multi")) throw new Exception("SendMultipleCommands Multi返回预期值错误！");
                foreach (RedisCommand c in command)
                {
                    SendExpectedQueued(c.Cmd, c.Args);
                }
                return SendExpectedArray("Exec");
            }
        }
        /// <summary>
        /// 响应结果预期OK
        /// </summary>
        /// <returns></returns>
        public bool SendExpectedOk(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return resp.ToString()?.ToUpper() == "OK";
        }

        /// <summary>
        /// 响应结果预期整数
        /// </summary>
        /// <returns></returns>
        public int SendExpectedInteger(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return Convert.ToInt32(resp.Analysis);
        }

        /// <summary>
        /// 响应结果预期整数
        /// </summary>
        /// <returns></returns>
        public string SendExpectedString(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            var result = resp.ToString();
            return result;
        }

        public string[] SendExpectedArray(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return resp.Type== '*' ? JsonSerializer.Deserialize<string[]>(resp.ToString()) : new string[] { resp.ToString() };
        }

        public void SendExpectedQueued(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            if (resp.Analysis.ToString()?.ToUpper()== "QUEUED")
            {
                return ;
            }
            throw new Exception("预期值应该为Queued");
        }

    
        public RedisAnswer Parse()
        {
            var c = (char)_bstream.ReadByte();
            var redisAnswer = new RedisAnswer(c);
            switch (c)
            {
                case '+':
                    redisAnswer.Analysis = ReadLine();
                    break;
                case ':':
                    redisAnswer.Analysis = Convert.ToInt32(ReadLine());
                    break;

                case '-':
                    redisAnswer.Analysis = ReadLine();
                    break;

                case '$':
                    var len = Convert.ToInt32(ReadLine());
                    if (len==-1)
                    {
                        redisAnswer.Analysis =null;
                        break;
                    }
                    byte[] bytes = new byte[len];
                    _bstream.Read(bytes, 0, bytes.Length);
                    redisAnswer.Analysis = Encoding.GetString(bytes);
                    SkipLine();
                    break;

                case '*':
                    var parameterLen = Convert.ToInt32(ReadLine());
                    RedisAnswer[] redisAnswers = new RedisAnswer[parameterLen];
                    for (int i = 0; i < parameterLen; i++)
                    {
                        redisAnswers[i] = Parse();
                    }
                    redisAnswer.Analysis = redisAnswers;
                    break;

                default:
                    throw new Exception("未知类型匹配！");
            }

            return redisAnswer;
        }

        private void SkipLine()
        {
            int c;
            while ((c = _bstream.ReadByte()) != -1)
            {
                if (c == '\r')
                    continue;
                if (c == '\n')
                    break;
            }
        }

        private string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            int c;
            while ((c = _bstream.ReadByte()) != -1)
            {
                if (c == '\r')
                    continue;
                if (c == '\n')
                    break;
                sb.Append((char)c);
            }
            return sb.ToString();
        }

        public void Close()
        {
            _socket?.Dispose();
            _bstream?.Dispose();
            _exit = true;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 切换db
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Select(in int index)
        {
            if (!SendExpectedOk("Select", index.ToString()))
            {
                return false;
            };
            _connection.Database = index;
            return true;
        }
    }
}