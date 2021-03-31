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
        public  Encoding Encoding { get; private set; } = Encoding.UTF8;
        private const string Crlf = "\r\n";
        private  char _responseType;
        private  string _respStrings;
        private readonly List<RespData> _bulkRespData=new();
        public void AddBulkResp(RespData respData)
        {
            _bulkRespData.Add(respData);
        }

        public new string GetType()
        {
            switch (_responseType)
            {
                case '+':
                    return "简单字符串";
                case ':':
                    return "错误信息";
                case '-':
                    return "整数";
                case '$':
                    return "大容量字符串";
                case '*':
                    return "数组";
                default:
                    return "未知类型匹配！";
            }
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
        public string Request(params string[] args)
        {
            string resp = "*" + args.Length + Crlf;
            foreach (string arg in args)
            {
                string argStr = arg.Trim();
                int argStrLength = Encoding.GetByteCount(argStr);
                resp += "$" + argStrLength + Crlf + argStr + Crlf;
            }
            return resp;
        }
        public string Response()
        {
            StringBuilder stringBuilder = new StringBuilder(_responseType);
            switch (_responseType)
            {
                case '+':
                case ':':
                case '-':
                    stringBuilder.AppendLine(_respStrings);
                    break;
                case '$':
                    if (_respStrings==null)
                    {
                        stringBuilder.AppendLine("-1");
                        break;
                    }
                    stringBuilder.AppendLine(":" + _respStrings.Length.ToString());
                    stringBuilder.AppendLine(_respStrings);
                    break;
                case '*':
                    var parameter = _respStrings.Split("\r\n");
                    stringBuilder.AppendLine(":" + parameter.Length);;
                    foreach (var respData in _bulkRespData)
                    {
                        stringBuilder.AppendLine(respData.Response());
                    }
                    break;
                default:
                    throw new Exception("未知类型匹配！");
            }
            return stringBuilder.ToString();
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