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
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Aescr.Redis
{
    /// <summary>
    /// 实体
    /// </summary>
    public class RedisAnswer
    {
        public RedisAnswer(char type)
        {
            Type = type;
        }

        public char Type { get; }
        public object Analysis { get; set; }

        private string Response
        {
            get
            {
                if (Analysis == null)
                {
                    return null;
                }

                if (Type!='*')
                {
                    return Analysis.ToString();
                }
                List<string> result = new List<string>();
                if (Analysis is RedisAnswer[] redisAnswers)
                {
                    foreach (RedisAnswer answer in redisAnswers)
                    {
                        var temp = answer.ToString();
                        result.Add(temp);
                    }
                }
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                return JsonSerializer.Serialize(result, options);
            }
        }

        public override string ToString()
        {
            return Response;
        }
    }

    public class RedisCommand
    {
        public string Cmd { get; set; }

        public string[] Args { get; set; }
    }
}
