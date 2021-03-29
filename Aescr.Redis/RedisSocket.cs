﻿using System;
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
using System.Threading.Tasks;

namespace Aescr.Redis
{
    public class RedisSocket : IDisposable
    {
        private const string Crlf = "\r\n";
        private Socket _socket;
        private BufferedStream _bstream;
        public bool IsConnected => _socket?.Connected ?? false;
        public bool Ssl { get; private set; } = false;
        public string Ip { get; private set; }
        public int Port { get; private set; }
        public Encoding Encoding { get; private set; }
        private readonly object _lockSocket = new object();
        public event EventHandler Connected;
        public event EventHandler<RedisMessage> Recieved;
        public event EventHandler<string> Subscribe;
        public RedisSocket(string host, bool ssl = false, Encoding encoding = null)
        {
            Encoding = encoding ?? Encoding.UTF8;
            Ssl = ssl;
            var hostPort = SplitHost(host);
            Ip = hostPort.Key;
            Port= hostPort.Value;
        }
        public bool Connect()
        {
            if (IsConnected) return IsConnected;
            lock (_lockSocket)
            {
                _socket ??= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true,
                    ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds,
                    SendTimeout = (int)TimeSpan.FromSeconds(20).TotalMilliseconds,
                };
                if (IsConnected) return IsConnected;
                try
                {
                    _socket.Connect(Ip, Port);
                    _bstream = new BufferedStream(new NetworkStream(_socket), 16 * 1024);
                    Connected?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    throw new Exception("Redis连接失败！", ex);
                }
                return IsConnected;
            }
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
        public bool Auth(string password)
        {
            if (string.IsNullOrWhiteSpace(password) == false)
            {
                var auth = SendExpectedOk("Auth", password);
                return !auth;
            }
            return SendExpectedOk("Auth", "Aescr");
        }
        public string SendCommand(string cmd)
        {
            var cSplit= cmd.Trim().Split(' ',StringSplitOptions.RemoveEmptyEntries);
            return SendCommand(cSplit);
        }
        public string SendCommand(string cmd,params string[] args)
        {
            string[] argStrings = new string[args.Length + 1];
            argStrings[0] = cmd;
            for (int i = 0; i < args.Length; i++)
            {
                argStrings[i+1] = args[i];
            }
            return SendCommand(argStrings);
        }
        public string SendCommand(string[] args)
        {
            string cmd=String.Empty;
            string resp = "*" + args.Length + Crlf;
            foreach (string arg in args)
            {
                string argStr = arg.Trim();
                int argStrLength = Encoding.GetByteCount(argStr);
                resp += "$" + argStrLength + Crlf + argStr + Crlf;
                cmd += " "+argStr;
            }
            cmd = cmd.Trim();
            byte[] r = Encoding.GetBytes(resp);
            lock (_lockSocket)
            {
                Connect();
                try
                {
                    _socket.Send(r);
                }
                catch(Exception ex)
                {
                    throw new Exception($"发送redis命令:{cmd}失败！", ex);
                }

                var rawBuilder = new StringBuilder();
                var result= Parse(ref rawBuilder);
                var rm = new RedisMessage {SendCommand = cmd, ReceiveMessage = result, RawCommand = resp,RawMessage = rawBuilder.ToString()};
                Recieved?.Invoke(this, rm);
                return result;
            }
        }
        public bool SendExpectedOk(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return resp?.ToUpper() == "OK";
        }
        public string SendExpectedString(string cmd, params string[] args)
        {
            return SendCommand(cmd, args);
        }
        public int SendExpectedInteger(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            try
            {
                return Convert.ToInt32(resp);
            }
            catch (Exception e)
            {
                throw new Exception($"返回意料之外的值:{resp}",e);
            }
           
        }
        public long SendExpectedInt64(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            try
            {
                return Convert.ToInt64(resp);
            }
            catch (Exception e)
            {
                throw new Exception($"返回意料之外的值:{resp}", e);
            }
          
        }
        public string[] SendExpectedArray(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return resp.Split("\r\n");
        }
        private string Parse(ref StringBuilder rawBuilder)
        {
            string resp;
            var c = (char)_bstream.ReadByte();
            rawBuilder.Append(c);
            switch (c)
            {
                case '+':
                    resp = ReadLine();
                    rawBuilder.AppendLine(resp);
                    break;
                case ':':
                    resp = ReadLine();
                    rawBuilder.AppendLine(resp);
                    break;
                case '-':
                    resp = ReadLine();
                    rawBuilder.AppendLine(resp);
                    break;
                case '$':
                    var lenStr = ReadLine();
                    var len = Convert.ToInt32(lenStr);
                    rawBuilder.AppendLine(len.ToString());
                    if (len==-1)
                    {
                        rawBuilder.Append("");
                        resp= null;
                        break;
                    }
                    byte[] bytes = new byte[len];
                    _bstream.Read(bytes, 0, bytes.Length);
                    SkipLine();
                    resp= Encoding.GetString(bytes);
                    rawBuilder.AppendLine(resp);
                    break;
                case '*':
                    var parameterlenStr = ReadLine();
                    var parameterLen = Convert.ToInt32(parameterlenStr);
                    rawBuilder.AppendLine(parameterLen.ToString());
                    StringBuilder redisAnswers = new StringBuilder();
                    for (int i = 0; i < parameterLen; i++)
                    {
                        redisAnswers.AppendLine(Parse(ref rawBuilder));
                    }
                    resp = redisAnswers.ToString();
                    break;
                default:
                    throw new Exception("未知类型匹配！");
            }

            return resp;
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

        public void ReceiveSubscribe()
        {
            Connect();
            while (true)
            {
                lock (_lockSocket)
                {
                    StringBuilder sb = new StringBuilder();
                    int c;
                    while ((c = _bstream.ReadByte()) != -1)
                    {
                        if (c == '\r')
                            continue;
                        if (c == '\n')
                        {
                            Subscribe?.Invoke(this, sb.ToString());
                            sb.Clear();
                            break;
                        }

                        sb.Append((char)c);
                    }
                }
            }
        }
        public bool Close()
        {
            _socket?.Close();
            _socket?.Dispose();
            _bstream?.Dispose();
            _bstream = null;
            _socket = null;
            return IsConnected;
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
    }
}