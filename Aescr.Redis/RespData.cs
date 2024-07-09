using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aescr.Redis
{
    /// <summary>
    /// RESP 协议
    /// </summary>
    public class RespData
    {
        public  static Encoding Encoding { get; private set; } = Encoding.UTF8;
        public const string Crlf = "\r\n";
        public  char RespType { get; }

        public string RespValue { get; set; }

        public RespData(char type)
        {
            RespType = type;
        }
        public bool ResponseOk()
        {
            return RespValue.ToUpper() == "OK";
        }
        public string ResponseString()
        {
            return RespValue;
        }
        public string[] ResponseArray()
        {
            return RespValue.Split(Crlf).ToArray();
        }
        public int ResponseInt()
        {
            if (RespType == ':')
            {
                return Convert.ToInt32(RespValue);
            }
            throw new Exception($"预期返回整形，但获取到意料之外的值:{RespValue}");
        }
        public long ResponseInt64()
        {
            if (RespType==':')
            {
                return Convert.ToInt64(RespValue);
            }
            throw new Exception($"预期返回整形，但获取到意料之外的值:{RespValue}");
        }
        public string GetRespType()
        {
            return RespType switch
            {
                '+' => "简单字符串",
                ':' => "错误信息",
                '-' => "整数",
                '$' => "大容量字符串",
                '*' => "数组",
                _ => "未知类型匹配！"
            };
        }
    }
}