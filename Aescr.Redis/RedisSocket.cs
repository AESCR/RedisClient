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
        private Socket _socket;
        private BufferedStream _bstream;
        public bool IsConnected => _socket?.Connected ?? false;
        public bool Ssl { get; private set; } = false;
        public string Ip { get; private set; }
        public int Port { get; private set; }
        public Encoding Encoding { get; private set; }
        private readonly object _lockSocket = new object();
        public event EventHandler Connected;
        public event EventHandler<string> Message;
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
                if (IsConnected) return IsConnected;
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
        public RespData SendMultiCommand(params string[] cmds)
        {
            lock (_lockSocket)
            {
                if (!SendExpectedOk("MULTI")) throw new Exception("事务开启失败！");
                foreach (string cmd in cmds)
                {
                    SendExpectedString(cmd);
                }
                var result = SendCommand("Exec");
                return result;
            }
        }
        public RespData SendCommand(string cmd)
        {
            var cSplit= cmd.Trim().Split(' ',StringSplitOptions.RemoveEmptyEntries);
            return SendCommand(cSplit);
        }
        public RespData SendCommand(string cmd,params string[] args)
        {
            string[] argStrings = new string[args.Length + 1];
            argStrings[0] = cmd;
            for (int i = 0; i < args.Length; i++)
            {
                argStrings[i+1] = args[i];
            }
            return SendCommand(argStrings);
        }
        public RespData SendCommand(string[] args)
        {
            string cmd = JoinCommand(args);
            string resp = RespCommand(args);
            byte[] r = Encoding.GetBytes(resp);
            RespData result;
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
                result= RespParse();
            }
            Message?.Invoke(this, $"发送命令{cmd},接受类型{result.GetType()},响应内容:{result.ResponseString()}");
            return result;
        }
        public static string JoinCommand(params string[] args)
        {
            string resp = string.Empty;
            foreach (string arg in args)
            {
                string argStr = arg.Trim();
                resp += " "+argStr;
            }
            return resp;
        }
        public static string RespCommand(params string[] args)
        {
            string resp = "*" + args.Length + RespData.Crlf;
            foreach (string arg in args)
            {
                string argStr = arg.Trim();
                int argStrLength =argStr.Length;
                resp += "$" + argStrLength + RespData.Crlf + argStr + RespData.Crlf;
            }
            return resp;
        }
        public bool SendExpectedOk(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return resp.ResponseOk();
        }
        public string SendExpectedString(string cmd, params string[] args)
        {
            return SendCommand(cmd, args).ResponseString();
        }
        public int SendExpectedInteger(string cmd, params string[] args)
        {
            return SendCommand(cmd, args).ResponseInt();
        }
        public long SendExpectedInt64(string cmd, params string[] args)
        {
            return SendCommand(cmd, args).ResponseInt64();
        }
        public string[] SendExpectedArray(string cmd, params string[] args)
        {
            return SendCommand(cmd, args).ResponseArray();
        }
        private RespData RespParse()
        {
            var c = (char)_bstream.ReadByte();
            var respData= new RespData(c);
            switch (c)
            {
                case '+':
                    respData.RespValue = ParseSimpleString();
                    break;
                case ':':
                    respData.RespValue =ParseInteger().ToString();
                    break;
                case '-':
                    respData.RespValue = ParseError();
                    break;
                case '$':
                    respData.RespValue  =ParseBulkString();
                    break;
                case '*':
                    respData.RespValue=ParseArray();
                    break;
                default:
                    throw new Exception("未知类型匹配！");
            }
            return respData;
        }

        private string ReadToRn()
        {
            StringBuilder sb = new StringBuilder();
            int currentByte;
            while ((currentByte = _bstream.ReadByte()) != '\r' && currentByte != -1)  
            {  
                sb.Append((char)currentByte);  
            }  
            // 验证下一个字节是否为 '\n' 严格检查协议
            if (currentByte == '\r' && _bstream.ReadByte() != '\n')  
            {  
                throw new InvalidOperationException("Expected '\\n' after '\\r' for simple string termination");  
            }  
            return sb.ToString();
        }
        private string ParseSimpleString()
        {
            return ReadToRn();
        }

        private string ParseError()
        {
            return ReadToRn();
        }

        private long ParseInteger()
        {
            var i= ReadToRn();
            return Convert.ToInt64(i);
        }

        private string ParseBulkString()
        {
            var len = ParseInteger();
            if (len==-1)
                return null;
            byte[] bytes = new byte[len];
            _= _bstream.Read(bytes, 0, bytes.Length);
            ReadToRn();
            var resp = Encoding.GetString(bytes);
            return resp;
        }

        private string ParseArray()
        {
            var parameterLen = ParseInteger();
            StringBuilder redisAnswers = new StringBuilder();
            for (int i = 0; i < parameterLen; i++)
            {
                var r = RespParse();
                redisAnswers.Append(r.RespValue+RespData.Crlf);
            }
            return redisAnswers.ToString();
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