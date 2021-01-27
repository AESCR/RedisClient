using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace XRedis
{
    internal class RedisSocket
    {
        Socket socket;
        BufferedStream bstream;
        private string Host;
        private int Port;
        private int SendTimeout;

        void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.SendTimeout = SendTimeout;
            socket.Connect(Host, Port);
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
        public bool SendCommand(string cmd, params object[] args)
        {
            if (socket==null)
            {
                Connect();
            }
            if (socket == null)
                return false;
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
                return false;
            }
            return true;
        }

        /// <summary>
        /// 解析redis回类型
        /// </summary>
        public void ParseResponse()
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
        }

        public void Close()
        {
            if (socket!=null)
            {
                socket.Close();
                socket = null;
            }
        }
    }
}
