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
        public void SendCommand(string cmd, params object[] args)
        {
            if (socket == null)
                throw new NullReferenceException(nameof(socket));
            string resp= "*" + (1 + args.Length)+Environment.NewLine;
            resp += "$" + cmd.Length + Environment.NewLine + cmd + Environment.NewLine;
            foreach (object arg in args)
            {
                string argStr = arg.ToString();
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

        public string SendCommandAnswer(string cmd, params string[] args)
        {
             SendCommand(cmd, args);
             return ParseResponse();
        }

        public bool SendCommandAnswerOk(string cmd, params string[] args)
        {
            SendCommand(cmd, args);
            return ParseResponse()=="OK";
        }
        public int SendCommandAnswerInt(string cmd, params string[] args)
        {
            SendCommand(cmd, args);
            var c= bstream.ReadByte();
            return Convert.ToInt32(ReadLine());
        }
        /// <summary>
        /// 解析redis回类型
        /// </summary>
        public string ParseResponse()
        {
            if (IsConnected)
            {
                int c = bstream.ReadByte();
                switch (c)
                {
                    // 状态回复
                    case '+':
                        break;
                    // 错误回复
                    case '-':
                        break;
                    // 整数回复
                    case ':':
                        break;
                    // 批量回复
                    case '$': // $后面跟数据字节数(长度)
                        break;
                    // 多条批量回复
                    case '*': // *表示后面有多少个参数
                        break;
                }
                return ReadLine();
            }
            return string.Empty;
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
