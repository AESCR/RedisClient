#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 单位 运管家
// 版权所有。 
//
// 文件名：RedisAnswer
// 文件功能描述：
//
// 
// 创建者：名字 AESCR
// 时间：2021/2/25 15:28:46
//
// 修改人：
// 时间：
// 修改说明：
//
// 修改人：
// 时间：
// 修改说明：
//
// 版本：V1.0.0
//----------------------------------------------------------------*/
#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Aescr.Redis
{
    public class RedisCommand
    {
        public string Cmd { get; set; }

        public string[] Args { get; set; }
    }

    public enum RedisKeyType
    {
        [Description("key不存在")]
        None,
        [Description("字符串")]
        String,
        [Description("列表")]
        List,
        [Description("集合")]
        Set,
        [Description("有序集")]
        ZSet,
        [Description("哈希表")]
        Hash
    }
    public class RedisResult
    {
        public RedisResult(char type)
        {
            _type = type;
            _source.AppendLine(_type.ToString());
        }
        private char _type { get; }
        public string Source
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_source.ToString()))
                {
                    return _type + "\r\n"+Value;
                }
                else
                {
                    return _source.ToString();
                }
            }
        }
        public string Value{get;set; }
        public RedisResult[] NestedValue { get; set; }
        private StringBuilder _source = new StringBuilder();
        public void AppendLine(string val)
        {
            _source.AppendLine(val);
        }
        public char GetType()
        {
            return _type;
        }
    }
}
