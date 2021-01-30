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

        public bool IsConnected => socket?.Connected??false;

        public void Connect(string host, int port, int sendTimeout)
        {
            if (socket!=null)
            {
                Close();
            }
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.SendTimeout = sendTimeout;
            socket.Connect(host, port);
            if (!socket.Connected)
            {
                socket.Close();
                socket = null;
                return;
            }
            bstream = new BufferedStream(new NetworkStream(socket), 16 * 1024);
        }
        /// <summary>
        /// redis命令发送格式 https://segmentfault.com/a/1190000011145207
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public void SendCommand(string cmd, params string[] args)
        {
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
        public string SendCommandBulkReply(string cmd, params string[] args)
        {
             SendCommand(cmd, args);
             return BulkReply();
        }
        public string[] SendCommandMultiBulkReply(string cmd, params string[] args)
        {
            SendCommand(cmd, args);
            return MultiBulkReply();
        }
        public bool SendCommandStatusReply(string cmd, params string[] args)
        {
            SendCommand(cmd, args);
            return StatusReply()=="OK";
        }
        public string SendCommandStringReply(string cmd, params string[] args)
        {
            SendCommand(cmd, args);
            return StatusReply();
        }
        public int SendCommandIntegerReply(string cmd, params string[] args)
        {
            SendCommand(cmd, args);
            return IntegerReply();
        }
        /// <summary>
        /// 状态回复（status reply）的第一个字节是 "+"，例如+OK\r\n
        /// </summary>
        /// <returns></returns>
        private string StatusReply()
        {
            if (IsConnected)
            {
                int c = bstream.ReadByte();
                if (c== '+')
                {
                    return ReadLine();
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 错误回复（error reply）的第一个字节是 "-"，例如-No such key\r\n
        /// </summary>
        /// <returns></returns>
        public string ErrorReply()
        {
            if (IsConnected)
            {
                int c = bstream.ReadByte();
                if (c == '-')
                {
                    return ReadLine();
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 整数回复（integer reply）的第一个字节是 ":"，例如:1\r\n
        /// </summary>
        /// <returns></returns>
        private int IntegerReply()
        {
            if (IsConnected)
            {
                int c = bstream.ReadByte();
                if (c == ':')
                {
                    return Convert.ToInt32(ReadLine());
                }
            }
            return 0;
        }

        public string BulkReply()
        {
            if (IsConnected)
            {
                int c = bstream.ReadByte();
                if (c == '$')
                {
                    return ParseBulkReply();
                }
            }
            return string.Empty;
        }

        public string[] MultiBulkReply()
        {
            if (IsConnected)
            {
                int c = bstream.ReadByte();
                if (c == '*')
                {
                    return ParseMultiBulkReply();
                }
            }
            return new string[0];
        }
        private string[] ParseMultiBulkReply()
        {
            int r = Convert.ToInt32(ReadLine());
            string[] result=new string[r];
            for (int i = 0; i < r; i++)
            {
                int c = bstream.ReadByte();
                if (c=='$')
                {
                    result[i]=ParseBulkReply();
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
            byte[] bytes = new byte[len];
            bstream.Read(bytes, 0, bytes.Length);
            var nL=Environment.NewLine.Length;
            byte[] newline=new byte[nL];
            bstream.Read(newline, 0, newline.Length);
            if (Encoding.UTF8.GetString(newline)== Environment.NewLine)
            {
                var result = Encoding.UTF8.GetString(bytes);
                if (result== "nil ")
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
    }
}
