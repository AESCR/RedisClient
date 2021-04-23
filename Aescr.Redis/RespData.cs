using System;
using System.Collections.Generic;
using System.Text;

namespace Aescr.Redis
{
    /// <summary>
    /// RESP 协议
    /// </summary>
    public class RespData
    {
        public  static Encoding Encoding { get; private set; } = Encoding.UTF8;
        private const string Crlf = "\r\n";
        private  char _responseType;
        private  string _respStrings;
        public bool Succeed
        {
            get
            {
                return _responseType switch
                {
                    '-' => false,
                    _ => true
                };
            }
        }

        public string GetRespType()
        {
            return _responseType switch
            {
                '+' => "简单字符串",
                ':' => "错误信息",
                '-' => "整数",
                '$' => "大容量字符串",
                '*' => "数组",
                _ => "未知类型匹配！"
            };
        }
        public RespData()
        {
        }
        public void SetResponse(char responseType,string respString)
        {
            _responseType = responseType;
            _respStrings = respString;
        }

        public static string GetCommand(params string[] args)
        {
            string resp = string.Empty;
            foreach (string arg in args)
            {
                string argStr = arg.Trim();
                resp += " "+argStr;
            }
            return resp;
        }
        public static string GetRequest(params string[] args)
        {
            string resp = "*" + args.Length + Crlf;
            foreach (string arg in args)
            {
                string argStr = arg.Trim();
                int argStrLength =argStr.Length;
                resp += "$" + argStrLength + Crlf + argStr + Crlf;
            }
            return resp;
        }
        public bool ResponseOk()
        {
            return _respStrings?.ToUpper() == "OK";
        }
        public string ResponseString()
        {
            return _respStrings;
        }
        public string[] ResponseArray()
        {
            return _respStrings.Trim('\n','\r').Split("\r\n");
        }
        public int ResponseInt()
        {
            if (_responseType == ':')
            {
                return Convert.ToInt32(_respStrings);
            }
            throw new Exception($"预期返回整形，但获取到意料之外的值:{_respStrings}");
        }
        public long ResponseInt64()
        {
            if (_responseType==':')
            {
                return Convert.ToInt64(_respStrings);
            }
            throw new Exception($"预期返回整形，但获取到意料之外的值:{_respStrings}");
        }
    }
}