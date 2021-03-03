using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
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
        public bool Ssl => _connection.Ssl;
        public int Database => _connection.Database;
        public Encoding Encoding => _connection.Encoding;
        private readonly object _lockObject = new object();
        public event EventHandler<EventArgs> Connected;
        private readonly RedisConnection _connection;
        public RedisConnection RedisConnection => _connection;
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
            _socket ??= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                ReceiveTimeout = (int) _connection.ReceiveTimeout.TotalMilliseconds,
                SendTimeout = (int) _connection.SendTimeout.TotalMilliseconds,
            };
        }
        /// <summary>
        /// Redis服务器进行连接
        /// </summary>
        /// <returns>连接状态</returns>
        public bool Connect()
        {
            if (IsConnected)
            {
                return IsConnected;
            }
            var hostPort = SplitHost(_connection.Host);
            lock (_lockObject)
            {
                try
                {
                    InitSocket();
                    _socket.Connect(hostPort.Key, hostPort.Value);
                    _bstream = new BufferedStream(new NetworkStream(_socket), 16 * 1024);
                    OnConnected();
                }
                catch
                {
                    return false;
                }
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
        public RedisResult SendCommand(RedisCommand command)
        {
            return SendCommand(command.Cmd, command.Args);
        }
        public RedisResult SendCommand(string cmd, params string[] args)
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
            if (_exit == false)
            {
                Connect();
            }
            lock (_lockObject)
            {
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
          
        }
        public string[] SendMultipleCommands(params RedisCommand[] command)
        {
            lock (_lockObject)
            {
                if (!SendExpectedOk("Multi")) throw new Exception("SendMultipleCommands Multi返回预期值错误！");
                foreach (var c in command)
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
            return resp.Value?.ToUpper() == "OK";
        }

        /// <summary>
        /// 响应结果预期整数
        /// </summary>
        /// <returns></returns>
        public int SendExpectedInteger(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return Convert.ToInt32(resp.Value);
        }
        /// <summary>
        /// 响应结果预期整数64
        /// </summary>
        /// <returns></returns>
        public long SendExpectedInteger64(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return Convert.ToInt64(resp.Value);
        }
        /// <summary>
        /// 响应结果预期整数
        /// </summary>
        /// <returns></returns>
        public string SendExpectedString(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            var result = resp.Value;
            return result;
        }

        public string[] SendExpectedArray(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return JsonSerializer.Deserialize<string[]>(resp.Value);
        }

        public void SendExpectedQueued(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            if (resp.Value?.ToUpper()== "QUEUED")
            {
                return ;
            }
            throw new Exception("预期值应该为Queued");
        }

    
        public RedisResult Parse()
        {
            var c = (char)_bstream.ReadByte();
            var redisAnswer = new RedisResult(c);
            switch (c)
            {
                case '+':
                    redisAnswer.Value = ReadLine();
                    break;
                case ':':
                    redisAnswer.Value = ReadLine();
                    break;
                case '-':
                    redisAnswer.Value = ReadLine();
                    break;
                case '$':
                    var lenStr = ReadLine();
                    var len = Convert.ToInt32(lenStr);
                    redisAnswer.AppendLine(lenStr);
                    if (len==-1)
                    {
                        redisAnswer.Value = String.Empty;
                        break;
                    }
                    byte[] bytes = new byte[len];
                    _bstream.Read(bytes, 0, bytes.Length);
                    redisAnswer.Value = Encoding.GetString(bytes);
                    redisAnswer.AppendLine(redisAnswer.Value);
                    SkipLine();
                    break;

                case '*':
                    var parameterlenStr = ReadLine();
                    var parameterLen = Convert.ToInt32(parameterlenStr);
                    redisAnswer.AppendLine(parameterlenStr);
                    RedisResult[] redisAnswers = new RedisResult[parameterLen];
                    for (int i = 0; i < parameterLen; i++)
                    {
                        redisAnswers[i] = Parse();
                        redisAnswer.AppendLine(redisAnswers[i].Source);
                    }

                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
                    jsonSerializerOptions.Encoder=JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    redisAnswer.Value = JsonSerializer.Serialize(redisAnswers.Select(x => x.Value), jsonSerializerOptions);
                    redisAnswer.NestedValue = redisAnswers;
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

        public bool Quit()
        {
            if (IsConnected)
            {
                SendExpectedOk("Quit");
            }
            _socket?.Dispose();
            _bstream?.Dispose();
            _socket = null;
            _exit = true;
            return IsConnected;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Quit();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Auth()
        {
            string auth = "ok";
            if (!string.IsNullOrEmpty(_connection.Password))
            {
                auth = SendExpectedString("Auth", _connection.Password);
            }
            if (auth.ToLower()!="ok")
            {
                throw new Exception($"Redis认证失败！{auth}");
            }
        }

        private void InitConnect()
        {
            Auth();
            Select(_connection.Database);
        }

        public bool Select(int index)
        {
            if (IsConnected==false)
            {
                _connection.Database = index;
                return true;
            }
            var result = SendExpectedOk("Select", index.ToString());
            if (result)
            {
                _connection.Database = index;
            }
            return result;
        }
        protected virtual void OnConnected()
        {
             InitConnect();
             Connected?.Invoke(this, EventArgs.Empty);
        }
    }
}