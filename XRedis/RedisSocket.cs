using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace XRedis
{
    internal class RedisSocket:IDisposable
    {
        Socket socket;
        BufferedStream bstream;
        public string Host { get; private set; }
        public int Port { get; private set; }
        private readonly string _password;
        public int SendTimeout
        {
            get => _sendTimeout;
            set
            {
                _sendTimeout = value;
                if (IsConnected)
                {
                    socket.SendTimeout = _sendTimeout;
                }
            }
        }

        private int _sendTimeout  = 1000;
        public bool IsConnected => socket?.Connected??false;
        public RedisSocket(string host, int port, string password)
        {
            Host = host;
            Port = port;
            _password = password;
        }
        void Connect()
        {
            if (IsConnected) return;
            Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true, SendTimeout = _sendTimeout
            };
            socket.Connect(Host, Port);
            if (!socket.Connected)
            {
                socket.Close();
                socket = null;
                return;
            }

            bstream = new BufferedStream(new NetworkStream(socket), 16 * 1024);
            if (string.IsNullOrEmpty(_password) != false) return;
            var result = SendCommandString("AUTH", _password);
            if (result != "OK")
            {
                throw new Exception("redis 密码错误！");
            }
        }
        /// <summary>
        /// redis命令发送格式 https://segmentfault.com/a/1190000011145207
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        void SendCommand(string cmd, params string[] args)
        {
            Connect();
            if (socket == null)
                throw new NullReferenceException(nameof(socket));
            string resp= "*" + (1 + args.Length)+Environment.NewLine;
            resp += "$" + cmd.Length + Environment.NewLine + cmd + Environment.NewLine;
            foreach (string arg in args)
            {
                string argStr = arg;
                int argStrLength = Encoding.UTF8.GetByteCount(argStr);
                resp += "$" + argStrLength + Environment.NewLine + argStr + Environment.NewLine;
            }
            byte[] r = Encoding.UTF8.GetBytes(resp);
            try
            {
                socket.Send(r);
            }
            catch
            {
                socket.Close();
                socket = null;
                throw new Exception("发送Send命令失败！");
            }
        }
        public string SendCommandString(string cmd, params string[] args)
        {
             SendCommand(cmd, args);
             var resp= ParseResp();
             if (resp is string result)
             {
                 return result;
             }
             throw new Exception("返回意料之外的值");
        }
        public string[] SendCommandArray(string cmd, params string[] args)
        {
            SendCommand(cmd, args);
            var resp = ParseResp();
            if (resp is string[] result)
            {
                return result;
            }
            throw new Exception("返回意料之外的值");
        }
        public int SendCommandInt(string cmd, params string[] args)
        {
            SendCommand(cmd, args);
            var resp = ParseResp();
            if (resp is int result)
            {
                return result;
            }
            throw new Exception("返回意料之外的值");
        }
        
        public void Close()
        {
            socket?.Close();
            socket?.Dispose();
            bstream?.Dispose();
            socket = null;
            bstream = null;
        }
        public void Dispose()
        {
            socket?.Close();
            socket?.Dispose();
            bstream?.Dispose();
            socket = null;
            bstream = null;
        }

        string Read(int len)
        {
            if (len<0)
            {
                return String.Empty;
            }
            byte[] bytes = new byte[len];
            bstream.Read(bytes, 0, bytes.Length);
            var nL=Environment.NewLine.Length;
            byte[] newline=new byte[nL];
            bstream.Read(newline, 0, newline.Length);
            if (Encoding.UTF8.GetString(newline)== Environment.NewLine)
            {
                var result = Encoding.UTF8.GetString(bytes);
                if (result== "nil")
                {
                    return String.Empty;
                }
                return result;
            }
            throw new Exception("批量读取长度异常！");
        }
        string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            int c;
            while ((c = bstream.ReadByte()) != -1)
            {
                if (c == '\r')
                    continue;
                if (c == '\n')
                    break;
                sb.Append((char)c);
            }
            return sb.ToString();
        }

        object ParseResp()
        {
            if (IsConnected)
            {
                int c = bstream.ReadByte();
                switch (c)
                {
                    case '+':
                        return ReadLine();
                    case '-':
                        return ReadLine();
                    case ':':
                        return Convert.ToInt32(ReadLine());
                    case '$':
                        return ParseBulkReply();
                    case '*':
                        return ParseMultiBulkReply();
                }
            }
            throw new Exception("redis 已断开连接，不可读取响应信息");
        }
        private string[] ParseMultiBulkReply()
        {
            int r = Convert.ToInt32(ReadLine());
            string[] result = new string[r];
            for (int i = 0; i < r; i++)
            {
                int c = bstream.ReadByte();
                if (c == '$')
                {
                    result[i] = ParseBulkReply();
                    continue;
                }
                throw new Exception("批量回复预期返回值错误");
            }
            return result;
        }
        private string ParseBulkReply()
        {
            int r = Convert.ToInt32(ReadLine());
            return Read(r);
        }
    }
}
