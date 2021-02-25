using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace RedisClient
{
   

    public class RedisSocket : IDisposable
    {
        /// <summary>
        /// 第一次连接
        /// </summary>
        private bool _fristConnect = true;

        private const string Crlf = "\r\n";
        private Socket _socket;
        private BufferedStream _bstream;
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool IsConnected => _socket?.Connected ?? false;
        private readonly string _password;
        private readonly object _lockObject = new object();

        public RedisSocket(string host, int port, string password)
        {
            Host = host;
            Port = port;
            _password = password;
        }

        public RedisSocket(string host, int port) : this(host, port, "")
        {
        }

        /// <summary>
        /// Redis服务器进行连接
        /// </summary>
        /// <returns>连接状态</returns>
        public bool Connect()
        {
            if (IsConnected) return IsConnected;
            if (!_fristConnect)
            {
                throw new Exception("redis 已断开连接");
            }
            lock (_lockObject)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true,
                };
                _socket.Connect(Host, Port);
                _bstream = new BufferedStream(new NetworkStream(_socket), 16 * 1024);
                if (Auth(_password))
                {
                    _fristConnect = false;
                }
                return IsConnected;
            }
        }

        public bool Auth(string password)
        {
            if (!Connect()) return IsConnected;
            if (string.IsNullOrWhiteSpace(password) != false) return IsConnected;
            if (SendExpectedOk("Auth", password)) return IsConnected;
            _fristConnect = false;
            throw new Exception("redis 密码认证失败");
        }

        private byte[] GenerateCommandData(string cmd, params string[] args)
        {
            string resp = "*" + (1 + args.Length) + Crlf;
            resp += "$" + cmd.Length + Crlf + cmd + Crlf;
            foreach (string arg in args)
            {
                string argStr = arg;
                int argStrLength = Encoding.UTF8.GetByteCount(argStr);
                resp += "$" + argStrLength + Crlf + argStr + Crlf;
            }
            byte[] r = Encoding.UTF8.GetBytes(resp);
            return r;
        }

        private bool Send(byte[] data)
        {
            try
            {
                _bstream.Write(data, 0, data.Length);
                _bstream.Flush();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public RedisAnswer SendCommand(string cmd, params string[] args)
        {
            var data = GenerateCommandData(cmd, args);
            Connect();
            if (Send(data))
            {
                return Parse();
            }
            throw new Exception("SendCommand失败！");
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
            return Convert.ToInt32(resp);
        }

        /// <summary>
        /// 响应结果预期整数
        /// </summary>
        /// <returns></returns>
        public string SendExpectedString(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            var result = resp.ToString();
            if (result == "nil")
            {
                return null;
            }
            return result;
        }

        public string[] SendExpectedArray(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return resp.Type== '*' ? JsonSerializer.Deserialize<string[]>(resp.ToString()) : new string[] { resp.ToString() };
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
                    byte[] bytes = new byte[len];
                    _bstream.Read(bytes, 0, bytes.Length);
                    redisAnswer.Analysis = Encoding.UTF8.GetString(bytes);
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
            _socket?.Close();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _socket?.Dispose();
                _bstream?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}