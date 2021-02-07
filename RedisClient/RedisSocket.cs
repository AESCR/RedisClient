using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace RedisClient
{
    public class RedisSocket:IDisposable
    {
        private const string Crlf = "\r\n";
        Socket socket;
        BufferedStream _bstream;
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool IsConnected => socket?.Connected ?? false;
        private readonly string _password;
        public RedisSocket(string host, int port,string password)
        {
            Host = host;
            Port = port;
            _password = password;
        }
        public RedisSocket(string host, int port):this(host,port,"")
        {
        }
        void Connect()
        {
            if (socket != null) return;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
            };
            socket.Connect(Host, Port);
            _bstream = new BufferedStream(new NetworkStream(socket), 16 * 1024);
            if (string.IsNullOrWhiteSpace(_password)) return;
            if (!SendExpectedOk("Auth", _password))
            {
                throw new Exception("redis 密码认证失败");
            }
        }
        public object SendCommand(string cmd, params string[] args)
        {
            Connect();
            string resp = "*" + (1 + args.Length) + Crlf;
            resp += "$" + cmd.Length + Crlf + cmd + Crlf;
            foreach (string arg in args)
            {
                string argStr = arg;
                int argStrLength = Encoding.UTF8.GetByteCount(argStr);
                resp += "$" + argStrLength + Crlf + argStr + Crlf;
            }
            byte[] r = Encoding.UTF8.GetBytes(resp);
            try
            {
                socket.Send(r);
                return ParseResp();
            }
            catch(Exception e)
            {
                throw new Exception("发送Send命令失败！", e);
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
            return (int) resp;
        }
        /// <summary>
        /// 响应结果预期整数
        /// </summary>
        /// <returns></returns>
        public string SendExpectedString(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return resp.ToString();
        }
        public string[] SendExpectedArray(string cmd, params string[] args)
        {
            var resp = SendCommand(cmd, args);
            return resp as string[];
        }
        private object ParseResp()
        {
            if (!IsConnected) throw new Exception("redis 已断开连接，不可读取响应信息");
            var c=_bstream.ReadByte();
            switch (c)
            {
                case '+':
                case ':':
                    return ReadLine();
                case '-':
                    var error= ReadLine();
                    throw new Exception("响应错误"+error);
                case '$':
                    return ParseBulkReply();
                case '*':
                    return ParseMultiBulkReply();
            }
            throw new Exception("redis 已断开连接，不可读取响应信息");
        }
        string ReadLine()
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
        private string[] ParseMultiBulkReply()
        {
            int r = Convert.ToInt32(ReadLine());
            string[] result = new string[r];
            for (int i = 0; i < r; i++)
            {
                int c = _bstream.ReadByte();
                if (c != '$') throw new Exception("批量回复预期返回值错误");
                result[i] = ParseBulkReply();
            }
            return result;
        }
        string Read(int len)
        {
            if (len < 0)
            {
                return String.Empty;
            }
            byte[] bytes = new byte[len];
            _bstream.Read(bytes, 0, bytes.Length);
            byte[] newline = new byte[2];
            _bstream.Read(newline, 0, newline.Length);
            if (Encoding.UTF8.GetString(newline) == Crlf)
            {
                var result = Encoding.UTF8.GetString(bytes);
                return result;
            }
            throw new Exception("批量读取长度异常！");
        }
        private string ParseBulkReply()
        {
            int r = Convert.ToInt32(ReadLine());
            return Read(r);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                socket?.Dispose();
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
