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
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RedisClient
{
    /// <summary>
    /// 响应实体
    /// </summary>
    public class RedisAnswer
    {
        public RedisAnswer(char type)
        {
            Type = type;
        }

        public char Type { get; }
        public object Analysis { get; set; }

        public override string ToString()
        {
            if (Analysis ==null)
            {
                return null;
            }
            switch (Type)
            {
                case '+':
                case ':':
                case '-':
                case '$':
                    return Analysis.ToString();
                case '*':
                    List<string> result = new List<string>();
                    if (Analysis is RedisAnswer[] redisAnswers)
                    {
                        foreach (RedisAnswer answer in redisAnswers)
                        {
                            var temp = answer.ToString();
                            result.Add(temp);
                        }
                    }
                    return JsonSerializer.Serialize(result);

                default:
                    return JsonSerializer.Serialize(Analysis);
            }
        }
    }

    public class RedisCommand
    {
        public string Cmd { get; set; }

        public string[] Args { get; set; }
    }
}
