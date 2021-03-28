using System.ComponentModel;

namespace Aescr.Redis
{
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
}