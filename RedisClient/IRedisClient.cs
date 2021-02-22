using System;
using System.Collections.Generic;

namespace RedisClient
{
    public interface IRedisClient : IDisposable
    {
        #region Redis 键(key) 命令

        /// <summary>
        /// 返回 key 所储存的值的类型。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        string Type(string key);

        /// <summary>
        /// 设置 key 的过期时间亿以毫秒计。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="milliseconds">毫秒计</param>
        /// <returns>设置成功返回 1 。 当 key 不存在或者不能为 key 设置过期时间时(比如在低于 2.1.3 版本的 Redis 中你尝试更新 key 的过期时间)返回 0 。</returns>
        int PExpire(string key, long milliseconds);

        /// <summary>
        /// 设置 key 过期时间的时间戳(unix timestamp) 以毫秒计
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="timestamp">UNIX 时间戳</param>
        /// <returns>设置成功返回 1 。 当 key 不存在或者不能为 key 设置过期时间时(比如在低于 2.1.3 版本的 Redis 中你尝试更新 key 的过期时间)返回 0 。</returns>
        int PExpireAt(string key, long timestamp);

        /// <summary>
        /// 修改 key 的名称
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="newKey">修改后的键</param>
        /// <returns>改名成功时提示 OK ，失败时候返回一个错误。</returns>
        string Rename(string key, string newKey);

        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>当过期时间移除成功时，返回 1 。 如果 key 不存在或 key 没有设置过期时间，返回 0 。</returns>
        int Persist(string key);

        /// <summary>
        /// 将当前数据库的 key 移动到给定的数据库 db 当中。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="dbIndex">数据库</param>
        /// <returns>移动成功返回 1 ，失败则返回 0 。</returns>
        int Move(string key, int dbIndex);

        /// <summary>
        /// 从当前数据库中随机返回一个 key 。
        /// </summary>
        /// <returns>当数据库不为空时，返回一个 key 。 当数据库为空时，返回 nil 。</returns>
        string RandomKey();

        /// <summary>
        /// 序列化给定 key ，并返回被序列化的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>如果 key 不存在，那么返回 nil 。 否则，返回序列化之后的值。</returns>
        string Dump(string key);

        /// <summary>
        /// 以秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>当 key 不存在时，返回 -2 。 当 key 存在但没有设置剩余生存时间时，返回 -1 。 否则，以毫秒为单位，返回 key 的剩余生存时间。</returns>
        int Ttl(string key);

        /// <summary>
        /// 设置 key 的过期时间。key 过期后将不再可用。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="second">秒</param>
        /// <returns>设置成功返回 1 。 当 key 不存在或者不能为 key 设置过期时间时(比如在低于 2.1.3 版本的 Redis 中你尝试更新 key 的过期时间)返回 0 </returns>
        int Expire(string key, int second);

        /// <summary>
        /// 用于删除已存在的键。不存在的 key 会被忽略。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>被删除 key 的数量。</returns>
        int Del(string key);

        /// <summary>
        /// 以毫秒为单位返回 key 的剩余的过期时间。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>当 key 不存在时，返回 -2 。 当 key 存在但没有设置剩余生存时间时，返回 -1 。 否则，以毫秒为单位，返回 key 的剩余生存时间。</returns>
        int PTtl(string key);

        /// <summary>
        /// 用于在新的 key 不存在时修改 key 的名称 。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="newKey">新键</param>
        /// <returns>修改成功时，返回 1 。 如果 NEW_KEY_NAME 已经存在，返回 0 。</returns>
        int RenameNx(string key, string newKey);

        /// <summary>
        /// 用于检查给定 key 是否存在。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>若 key 存在返回 1 ，否则返回 0 。</returns>
        bool Exists(string key);

        /// <summary>
        /// 以 UNIX 时间戳(unix timestamp)格式设置 key 的过期时间。key 过期后将不再可用。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="second">UNIX 时间戳</param>
        /// <returns>设置成功返回 1 。 当 key 不存在或者不能为 key 设置过期时间时(比如在低于 2.1.3 版本的 Redis 中你尝试更新 key 的过期时间)返回 0 。</returns>
        int ExpireAt(string key, long timestamp);

        /// <summary>
        /// 用于查找所有符合给定模式 pattern 的 key 。。
        /// </summary>
        /// <param name="pattern">匹配规则</param>
        /// <returns>符合给定模式的 key 列表 (Array)。</returns>
        string[] Keys(string pattern);

        #endregion Redis 键(key) 命令

        #region Redis 字符串(String) 命令

        /// <summary>
        /// 在指定的 key 不存在时，为 key 设置指定的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>设置成功，返回 1 。 设置失败，返回 0 。</returns>
        int SetNx(string key, string value);

        /// <summary>
        /// 用于获取存储在指定 key 中字符串的子字符串。字符串的截取范围由 start 和 end 两个偏移量决定(包括 start 和 end 在内)。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="start">默认等于0 从头开始</param>
        /// <param name="end">默认等于-1 结束位置</param>
        /// <returns>截取得到的子字符串。</returns>
        string GetRange(string key, int start = 0, int end = -1);

        /// <summary>
        /// 用于同时设置一个或多个 key-value 对。
        /// </summary>
        /// <returns>总是返回 OK 。</returns>
        string MSet(Dictionary<string, string> kv);

        /// <summary>
        /// 指定的 key 设置值及其过期时间。如果 key 已经存在， SETEX 命令将会替换旧的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间</param>
        /// <returns>设置成功时返回 OK 。</returns>
        string SetEx(string key, string value, int timeout);

        /// <summary>
        /// 用于设置给定 key 的值。如果 key 已经存储其他值， SET 就覆写旧值，且无视类型。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>
        /// 在 Redis 2.6.12 以前版本， SET 命令总是返回 OK 。
        /// 从 Redis 2.6.12 版本开始， SET 在设置操作成功完成时，才返回 OK 。
        /// </returns>
        bool Set(string key, string value);

        /// <summary>
        /// 用于获取指定 key 的值。如果 key 不存在，返回 nil 。如果key 储存的值不是字符串类型，返回一个错误。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回 key 的值，如果 key 不存在时，返回 nil。 如果 key 不是字符串类型，那么返回一个错误。</returns>
        string Get(string key);

        /// <summary>
        /// 用于对 key 所储存的字符串值，获取指定偏移量上的位(bit)。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="offset">偏移量</param>
        /// <returns>
        /// 字符串值指定偏移量上的位(bit)。
        /// 当偏移量 OFFSET 比字符串值的长度大，或者 key 不存在时，返回 0 。
        /// </returns>
        int GetBit(string key, int offset);

        /// <summary>
        /// 用于对 key 所储存的字符串值，设置或清除指定偏移量上的位(bit)。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="offset">偏移量</param>
        /// <param name="value">位(bit)</param>
        /// <returns>指定偏移量原来储存的位。</returns>
        int SetBit(string key, int offset, int value);

        /// <summary>
        /// 将 key 中储存的数字值减一。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>
        /// 执行命令之后 key 的值。
        /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
        /// </returns>
        string Decr(string key);

        /// <summary>
        ///  将 key 所储存的值减去指定的减量值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="num">减量值</param>
        /// <returns>
        /// 减去指定减量值之后， key 的值。
        /// 如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECRBY 操作。
        /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
        /// 本操作的值限制在 64 位(bit)有符号数字表示之内。
        /// </returns>
        string DecrBy(string key, int num);

        /// <summary>
        /// 用于获取指定 key 所储存的字符串值的长度。当 key 储存的不是字符串值时，返回一个错误。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>字符串值的长度。 当 key 不存在时，返回 0。</returns>
        int StrLen(string key);

        /// <summary>
        /// 用于所有给定 key 都不存在时，同时设置一个或多个 key-value 对。
        /// </summary>
        /// <param name="kv">键值对</param>
        /// <returns>当所有 key 都成功设置，返回 1 。 如果所有给定 key 都设置失败(至少有一个 key 已经存在)，那么返回 0 。</returns>
        int MSetNx(Dictionary<string, string> kv);

        /// <summary>
        /// 将 key 中储存的数字加上指定的增量值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="num">增量值</param>
        /// <returns>加上指定的增量值之后， key 的值。</returns>
        string IncrBy(string key, int num);

        /// <summary>
        /// 为 key 中所储存的值加上指定的浮点数增量值。
        /// </summary>
        /// <param name="key">键 如果 key 不存在，那么 INCRBYFLOAT 会先将 key 的值设为 0 ，再执行加法操作。</param>
        /// <param name="fraction">浮点数增量值</param>
        /// <returns>执行命令之后 key 的值。</returns>
        string IncrByFloat(string key, float fraction);

        /// <summary>
        /// 用指定的字符串覆盖给定 key 所储存的字符串值，覆盖的位置从偏移量 offset 开始。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="offset">偏移量</param>
        /// <param name="value">值</param>
        /// <returns>被修改后的字符串长度。</returns>
        int SetRange(string key, int offset, string value);

        /// <summary>
        ///这个命令和 SETEX 命令相似，但它以毫秒为单位设置 key 的生存时间，而不是像 SETEX 命令那样，以秒为单位。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="milliseconds">毫秒</param>
        /// <returns>设置成功时返回 OK 。</returns>
        string PSetEx(string key, string value, int milliseconds);

        /// <summary>
        /// 用于为指定的 key 追加值。
        /// </summary>
        /// <param name="key">
        /// 如果 key 已经存在并且是一个字符串， APPEND 命令将 value 追加到 key 原来的值的末尾。
        /// 如果 key 不存在， APPEND 就简单地将给定 key 设为 value ，就像执行 SET key value 一样。
        /// </param>
        /// <param name="value">值</param>
        /// <returns>追加指定值之后， key 中字符串的长度。</returns>
        int Append(string key, string value);

        /// <summary>
        /// 用于设置指定 key 的值，并返回 key 旧的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>
        /// 返回给定 key 的旧值。 当 key 没有旧值时，即 key 不存在时，返回 nil 。
        /// 当 key 存在但不是字符串类型时，返回一个错误。
        /// </returns>
        string GetSet(string key, string value);

        /// <summary>
        /// 返回所有(一个或多个)给定 key 的值。 如果给定的 key 里面，有某个 key 不存在，那么这个 key 返回特殊值 nil
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <returns>一个包含所有给定 key 的值的列表。</returns>
        string[] MGet(params string[] keys);

        /// <summary>
        /// 将 key 中储存的数字值增一。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>执行 INCR 命令之后 key 的值。</returns>
        string Incr(string key);

        #endregion Redis 字符串(String) 命令

        #region Redis 列表(List) 命令

        /// <summary>
        /// 通过索引获取列表中的元素。你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns>列表中下标为指定索引值的元素。 如果指定索引值不在列表的区间范围内，返回 nil 。</returns>
        string LIndex(string key, int index);

        /// <summary>
        /// 用于将一个或多个值插入到列表的尾部(最右边)。 在 Redis 2.4 版本以前的 RPUSH 命令，都只接受单个 value 值。
        /// </summary>
        /// <param name="key">如果列表不存在，一个空列表会被创建并执行 RPUSH 操作。 当列表存在但不是列表类型时，返回一个错误。</param>
        /// <param name="values">一个或多个值</param>
        /// <returns>执行 RPUSH 操作后，列表的长度。</returns>
        int RPush(string key, params string[] values);

        /// <summary>
        /// 返回列表中指定区间内的元素，区间以偏移量 START 和 END 指定。 其中 0 表示列表的第一个元素， 1 表示列表的第二个元素，以此类推。 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="start">开始</param>
        /// <param name="end">结束</param>
        /// <returns>一个列表，包含指定区间内的元素。</returns>
        string[] LRange(string key, int start = 0, int end = -1);

        /// <summary>
        /// 用于移除列表的最后一个元素，并将该元素添加到另一个列表并返回。
        /// </summary>
        /// <param name="key">移除列表的最后一个元素</param>
        /// <param name="newKey">移除元素添加到另一个列表</param>
        /// <returns>返回移除列表</returns>
        string[] RPopLPush(string key, string newKey);

        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="timeout">等待超时</param>
        /// <param name="key">被弹出列表所属的 key</param>
        /// <returns>如果列表为空，返回一个 nil 。 否则，返回一个含有两个元素的列表，第一个元素是被弹出元素所属的 key ，第二个元素是被弹出元素的值。</returns>
        string[] BlPop(string[] key, int timeout);

        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="value">被弹出元素的值</param>
        /// <param name="timeout">等待时长</param>
        /// <param name="keys">被弹出元素所属的 key </param>
        /// <returns>假如在指定时间内没有任何元素被弹出，则返回一个 nil 和等待时长。 反之，返回一个含有两个元素的列表</returns>
        string[] BrPop(string[] keys, int timeout);

        /// <summary>
        /// 从列表中弹出一个值，将弹出的元素插入到另外一个列表中并返回它； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="newKey">另外一个列表</param>
        /// <param name="timeout">等待时长</param>
        /// <returns>假如在指定时间内没有任何元素被弹出，则返回一个 nil 和等待时长。 反之，返回一个含有两个元素的列表</returns>
        string[] BrPopLPush(string key, string newKey, int timeout);

        /// <summary>
        /// 移除列表元素
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="count">
        /// count > 0 : 从表头开始向表尾搜索，移除与 VALUE 相等的元素，数量为 COUNT 。
        /// count< 0 : 从表尾开始向表头搜索，移除与 VALUE 相等的元素，数量为 COUNT 的绝对值。
        /// count = 0 : 移除表中所有与 VALUE 相等的值。
        /// </param>
        /// <param name="value">值</param>
        /// <returns>被移除元素的数量。 列表不存在时返回 0 。</returns>
        int LRem(string key, string value, int count = 0);

        /// <summary>
        /// 用于返回列表的长度。 如果列表 key 不存在，则 key 被解释为一个空列表，返回 0 。 如果 key 不是列表类型，返回一个错误。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>列表的长度。</returns>
        int LLen(string key);

        /// <summary>
        /// 对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="start">下标 0 表示列表的第一个元素，以 1 表示列表的第二个元素</param>
        /// <param name="end">可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素</param>
        /// <returns>命令执行成功时，返回 ok 。</returns>
        string LTrim(string key, int start = 0, int end = -1);

        /// <summary>
        /// 用于移除并返回列表的第一个元素。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>列表的第一个元素。 当列表 key 不存在时，返回 nil 。</returns>
        string LPop(string key);

        /// <summary>
        /// 一个或多个值插入到已存在的列表头部，列表不存在时操作无效。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="values">值</param>
        /// <returns>LPUSHX 命令执行之后，列表的长度</returns>
        int LPushX(string key, params string[] values);

        /// <summary>
        /// 在列表的元素前或者后插入元素
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">新值</param>
        /// <param name="existValue">存在的值</param>
        /// <param name="before">在存在的值之前</param>
        /// <returns></returns>
        int LInsert(string key, string value, string existValue, bool before = true);

        /// <summary>
        /// 用于移除并返回列表的最后一个元素。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>列表的最后一个元素。 当列表不存在时，返回 nil 。</returns>
        string RPop(string key);

        /// <summary>
        /// 通过索引设置列表元素的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="index">当索引参数超出范围，或对一个空列表进行 LSET 时，返回一个错误。</param>
        /// <returns>操作成功返回 ok ，否则返回错误信息。</returns>
        string LSet(string key, string value, int index);

        /// <summary>
        /// 一个或多个值插入到列表头部。
        /// </summary>
        /// <param name="key">
        /// 如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。 当 key 存在但不是列表类型时，返回一个错误。
        /// 在Redis 2.4版本以前的 LPUSH 命令，都只接受单个 value 值。
        /// </param>
        /// <param name="values">一个或多个值</param>
        /// <returns>执行 LPUSH 命令后，列表的长度。</returns>
        int LPush(string key, params string[] values);

        /// <summary>
        /// 将一个或多个值插入到已存在的列表尾部(最右边)
        /// </summary>
        /// <param name="key">如果列表不存在，操作无效</param>
        /// <param name="values">一个或多个值</param>
        /// <returns>执行 Rpushx 操作后，列表的长度。</returns>
        int RPushX(string key, params string[] values);

        #endregion Redis 列表(List) 命令

        #region Redis 哈希(Hash) 命令

        /// <summary>
        /// 用于同时将多个 field-value (字段-值)对设置到哈希表中。 此命令会覆盖哈希表中已存在的字段。
        /// </summary>
        /// <param name="key">键 如果哈希表不存在，会创建一个空哈希表，并执行 HMSET 操作。</param>
        /// <param name="kV">字段-值</param>
        /// <returns></returns>
        string HmSet(string key, Dictionary<string, string> kV);

        /// <summary>
        /// 用于返回哈希表中，一个或多个给定字段的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="fields">如果指定的字段不存在于哈希表，那么返回一个 nil 值。</param>
        /// <returns>一个包含多个给定字段关联值的表，表值的排列顺序和指定字段的请求顺序一样。</returns>
        string[] HmGet(string key, params string[] fields);

        /// <summary>
        /// 用于为哈希表中的字段赋值
        /// </summary>
        /// <param name="key">
        /// 如果哈希表不存在，一个新的哈希表被创建并进行 HSET 操作。
        /// 如果字段已经存在于哈希表中，旧值将被覆盖。
        /// </param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>如果字段是哈希表中的一个新建字段，并且值设置成功，返回 1 。 如果哈希表中域字段已经存在且旧值已被新值覆盖，返回 0 。</returns>
        int HSet(string key, string field, string value);

        /// <summary>
        /// 用于返回哈希表中，所有的字段和值。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        string[] HGetAll(string key);

        /// <summary>
        /// 用于返回哈希表中指定字段的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <returns>返回给定字段的值。如果给定的字段或 key 不存在时，返回 nil 。</returns>
        string HGet(string key, string field);

        /// <summary>
        /// 用于查看哈希表的指定字段是否存在。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <returns>如果哈希表含有给定字段，返回 1 。 如果哈希表不含有给定字段，或 key 不存在，返回 0 。</returns>
        int HExists(string key, string field);

        /// <summary>
        /// 用于为哈希表中的字段值加上指定增量值。
        /// </summary>
        /// <param name="key">如果哈希表的 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。</param>
        /// <param name="field">如果指定的字段不存在，那么在执行命令前，字段的值被初始化为 0 。</param>
        /// <param name="number">增量也可以为负数，相当于对指定字段进行减法操作。</param>
        /// <returns>执行 HINCRBY 命令之后，哈希表中字段的值。</returns>
        string HinCrBy(string key, string field, int number);

        /// <summary>
        /// 用于获取哈希表中字段的数量。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>哈希表中字段的数量。 当 key 不存在时，返回 0 。</returns>
        int HLen(string key);

        /// <summary>
        /// 用于删除哈希表 key 中的一个或多个指定字段，不存在的字段将被忽略。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="fields">一个或多个指定字段 不存在的字段将被忽略</param>
        /// <returns>被成功删除字段的数量，不包括被忽略的字段。</returns>
        int HDel(string key, params string[] fields);

        /// <summary>
        /// 返回哈希表所有字段的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>一个包含哈希表中所有值的表。 当 key 不存在时，返回一个空表。</returns>
        string[] HVals(string key);

        /// <summary>
        /// 用于为哈希表中的字段值加上指定浮点数增量值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">如果指定的字段不存在，那么在执行命令前，字段的值被初始化为 0 。</param>
        /// <param name="fraction">浮点数增量值。</param>
        /// <returns>执行 Hincrbyfloat 命令之后，哈希表中字段的值。</returns>
        string HinCrByFloat(string key, string field, float fraction);

        /// <summary>
        /// 用于获取哈希表中的所有字段名。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>包含哈希表中所有字段的列表。 当 key 不存在时，返回一个空列表。</returns>
        string[] HKeys(string key);

        /// <summary>
        /// 用于为哈希表中不存在的的字段赋值 。
        /// </summary>
        /// <param name="key">如果哈希表不存在，一个新的哈希表被创建并进行 HSET 操作。</param>
        /// <param name="field">如果字段已经存在于哈希表中，操作无效。</param>
        /// <param name="value">值</param>
        /// <returns>设置成功，返回 1 。 如果给定字段已经存在且没有操作被执行，返回 0 。</returns>
        int HSetNx(string key, string field, string value);

        #endregion Redis 哈希(Hash) 命令

        #region Redis 集合(Set) 命令

        /// <summary>
        /// 返回给定集合的并集。不存在的集合 key 被视为空集。
        /// </summary>
        /// <param name="keys">集合键</param>
        /// <returns>并集成员的列表。</returns>
        string[] SUnion(params string[] keys);

        /// <summary>
        /// 返回集合中元素的数量。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>集合的数量。 当集合 key 不存在时，返回 0 。</returns>
        int SCard(string key);

        /// <summary>
        /// 用于返回集合中的一个随机元素。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="count">
        /// 从 Redis 2.6 版本开始， Srandmember 命令接受可选的 count 参数：
        /// 如果 count 为正数，且小于集合基数，那么命令返回一个包含 count 个元素的数组，数组中的元素各不相同。如果 count 大于等于集合基数，那么返回整个集合。
        /// 如果 count 为负数，那么命令返回一个数组，数组中的元素可能会重复出现多次，而数组的长度为 count 的绝对值。
        /// 该操作和 SPOP 相似，但 SPOP 将随机元素从集合中移除并返回，而 Srandmember 则仅仅返回随机元素，而不对集合进行任何改动。
        /// </param>
        /// <returns></returns>
        string[] SRandMember(string key, int count);

        /// <summary>
        /// 用于返回集合中的一个随机元素。
        /// </summary>
        /// <param name="key">键</param>
        string SRandMember(string key);

        /// <summary>
        /// 返回集合中的所有的成员。 不存在的集合 key 被视为空集合。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>集合中的所有成员。</returns>
        string[] SMembers(string key);

        /// <summary>
        /// 返回给定所有给定集合的交集。 不存在的集合 key 被视为空集。 当给定集合当中有一个空集时，结果也为空集
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns>交集成员的列表。</returns>
        string[] SInter(params string[] keys);

        /// <summary>
        /// 用于移除集合中的一个或多个成员元素，不存在的成员元素会被忽略。
        /// </summary>
        /// <param name="key">当 key 不是集合类型，返回一个错误</param>
        /// <param name="member">在 Redis 2.4 版本以前， SREM 只接受单个成员值。</param>
        /// <returns>被成功移除的元素的数量，不包括被忽略的元素。</returns>
        int SRem(string key, params string[] member);

        /// <summary>
        /// 指定成员 member 元素从 source 集合移动到 destination 集合。
        /// </summary>
        /// <param name="source">source 集合键</param>
        /// <param name="destination">destination 集合键</param>
        /// <param name="moveMember">SMOVE 是原子性操作</param>
        /// <returns>如果成员元素被成功移除，返回 1 。 如果成员元素不是 source 集合的成员，并且没有任何操作对 destination 集合执行，那么返回 0 。</returns>
        int SMove(string source, string destination, string moveMember);

        /// <summary>
        /// 将一个或多个成员元素加入到集合中，已经存在于集合的成员元素将被忽略。
        /// </summary>
        /// <param name="key">假如集合 key 不存在，则创建一个只包含添加的元素作成员的集合。当集合 key 不是集合类型时，返回一个错误。</param>
        /// <param name="values">注意：在Redis2.4版本以前， SADD 只接受单个成员值</param>
        /// <returns>被添加到集合中的新元素的数量，不包括被忽略的元素。</returns>
        int SAdd(string key, params string[] values);

        /// <summary>
        /// 判断成员元素是否是集合的成员。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">要判断的成员</param>
        /// <returns>如果成员元素是集合的成员，返回 1 。 如果成员元素不是集合的成员，或 key 不存在，返回 0 。</returns>
        int SIsMember(string key, string value);

        /// <summary>
        /// 将给定集合之间的差集存储在指定的集合中。
        /// </summary>
        /// <param name="destination">存储在指定的集合。如果指定的集合 key 已存在，则会被覆盖</param>
        /// <param name="keys">指定的差集</param>
        /// <returns>结果集中的元素数量。</returns>
        int SDiffStore(string destination, params string[] keys);

        /// <summary>
        /// 返回给定集合之间的差集。不存在的集合 key 将视为空集。
        /// </summary>
        /// <param name="keys">定集合之间的差集</param>
        /// <returns></returns>
        string[] SDiff(params string[] keys);

        /// <summary>
        /// 用于迭代集合键中的元素。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="cursor">游标</param>
        /// <param name="pattern">匹配的模式</param>
        /// <param name="count">指定从数据集里返回多少元素，默认值为 10</param>
        /// <returns>数组列表。</returns>
        string[] SScan(string key, int cursor, string pattern, int count = 10);

        /// <summary>
        /// 给定集合之间的交集存储在指定的集合中。如果指定的集合已经存在，则将其覆盖。
        /// </summary>
        /// <param name="destination">存储在指定的集合</param>
        /// <param name="keys">集合之间的交集</param>
        /// <returns>交集成员的列表。</returns>
        string[] SInterStore(string destination, params string[] keys);

        /// <summary>
        /// 将给定集合的并集存储在指定的集合 destination 中。
        /// </summary>
        /// <param name="destination">存储在指定的集合</param>
        /// <param name="keys">集合</param>
        /// <returns>结果集中的元素数量。</returns>
        int SUnionStore(string destination, params string[] keys);

        /// <summary>
        /// 用于移除并返回集合中的一个随机元素。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>被移除的随机元素。 当集合不存在或是空集时，返回 nil 。</returns>
        string SPop(string key);

        #endregion Redis 集合(Set) 命令

        #region Redis 有序集合(sorted set) 命令

        /// <summary>
        /// 返回有序集中成员的排名。其中有序集成员按分数值递减(从大到小)排序。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="member">成员</param>
        /// <returns>如果成员是有序集 key 的成员，返回成员的排名。 如果成员不是有序集 key 的成员，返回 nil 。</returns>
        string ZRevRank(string key, string member);

        /// <summary>
        /// 在计算有序集合中指定字典区间内成员数量。
        /// </summary>
        /// <param name="key">有序集合键名称</param>
        /// <param name="min">在有序集合中分数排名较小的成员</param>
        /// <param name="max">在有序集合中分数排名较大的成员</param>
        /// <returns>有序集合中成员名称 min 和 max 之间的成员数量; Integer类型。</returns>
        int ZLexCount(string key, string min, string max);

        /// <summary>
        /// 用于移除有序集中，指定排名(rank)区间内的所有成员。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns>被移除成员的数量。</returns>
        int ZRemRangeByRank(string key, int start, int stop);

        /// <summary>
        /// 用于计算集合中元素的数量
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>当 key 存在且是有序集类型时，返回有序集的基数。 当 key 不存在时，返回 0 。</returns>
        int ZCard(string key);

        /// <summary>
        /// 用于移除有序集中的一个或多个成员，不存在的成员将被忽略
        /// </summary>
        /// <param name="key">当 key 存在但不是有序集类型时，返回一个错误。</param>
        /// <param name="member"> 在 Redis 2.4 版本以前， ZREM 每次只能删除一个元素。</param>
        /// <returns>被成功移除的成员的数量，不包括被忽略的成员</returns>
        int ZRem(string key, params string[] member);

        /// <summary>
        /// 返回有序集中指定成员的排名。其中有序集成员按分数值递增(从小到大)顺序排列。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="member"></param>
        /// <returns>如果成员是有序集 key 的成员，返回 member 的排名。 如果成员不是有序集 key 的成员，返回 nil 。</returns>
        int ZRank(string key, string member);

        /// <summary>
        /// 对有序集合中指定成员的分数加上增量 increment
        /// </summary>
        /// <param name="key"></param>
        /// <param name="increment">增量  递一个负数值 increment ，让分数减去相应的值</param>
        /// <param name="member"></param>
        /// <returns>member 成员的新分数值，以字符串形式表示。</returns>
        string ZIncrBy(string key, int increment, string member);

        //TODO Redis Zunionstore 命令 Redis Zinterstore 命令 ZRangeByScore

        #endregion Redis 有序集合(sorted set) 命令

        #region Redis 连接 命令

        /// <summary>
        /// 用于打印给定的字符串。
        /// </summary>
        /// <returns>返回字符串本身。</returns>
        string Echo(string message);

        /// <summary>
        /// 用于切换到指定的数据库，数据库索引号 index 用数字值指定，以 0 作为起始索引值。
        /// </summary>
        /// <param name="index"></param>
        /// <returns>总是返回 OK 。</returns>
        bool Select(int index);

        /// <summary>
        /// 使用客户端向 Redis 服务器发送一个 PING ，如果服务器运作正常的话，会返回一个 PONG 。
        /// 通常用于测试与服务器的连接是否仍然生效，或者用于测量延迟值。
        /// </summary>
        /// <returns>如果连接正常就返回一个 PONG ，否则返回一个连接错误。</returns>
        string Ping();

        /// <summary>
        /// 用于关闭与当前客户端与redis服务的连接。 一旦所有等待中的回复(如果有的话)顺利写入到客户端，连接就会被关闭。
        /// </summary>
        /// <returns>总是返回 OK 。</returns>
        string Quit();

        /// <summary>
        /// 用于检测给定的密码和配置文件中的密码是否相符
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        string Auth(string password);

        /// <summary>
        /// 于阻塞客户端命令一段时间（以毫秒计）。
        /// </summary>
        /// <param name="timeout">毫秒</param>
        /// <returns>返回 OK。如果 timeout 参数是非法的返回错误。</returns>
        string Pause(long timeout);

        /// <summary>
        /// 获取 key 的调试信息  命令是一个调试命令，它不应被客户端所使用。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>当 key 存在时，返回有关信息。 当 key 不存在时，返回一个错误。</returns>
        string DebugObject(string key);

        /// <summary>
        /// 用于清空当前数据库中的所有 key
        /// </summary>
        /// <returns>总是返回 OK 。</returns>
        string FlushDb();

        /// <summary>
        /// 执行一个同步保存操作，将当前 Redis 实例的所有数据快照(snapshot)以 RDB 文件的形式保存到硬盘。
        /// </summary>
        /// <returns>保存成功时返回 OK 。</returns>
        string Save();

        /// <summary>
        /// 返回最近一次 Redis 成功将数据保存到磁盘上的时间，以 UNIX 时间戳格式表示。
        /// </summary>
        /// <returns>字符串，文本行的集合。</returns>
        string LastSave();

        /// <summary>
        /// 用于获取 redis 服务的配置参数。
        /// </summary>
        /// <param name="parameters">给定配置参数</param>
        /// <returns></returns>
        Dictionary<string, string> ConfigGet(string parameters);

        /// <summary>
        /// 在最新的 Redis 2.6 版本中，所有配置参数都已经可以用 CONFIG GET 访问了。
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> ConfigGet();

        /// <summary>
        /// 用于返回所有的Redis命令的详细信息，以数组形式展示。
        /// </summary>
        /// <returns>嵌套的Redis命令的详细信息列表。列表顺序是随机的。</returns>
        string[] Command();

        /// <summary>
        /// 可以将当前服务器转变为指定服务器的从属服务器(slave server)
        /// 如果当前服务器已经是某个主服务器(master server)的从属服务器，那么执行 SLAVEOF host port 将使当前服务器停止对旧主服务器的同步，丢弃旧数据集，转而开始对新主服务器进行同步。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        /// <returns>总是返回 OK 。</returns>
        bool SlaveOf(string host, int port, string password = "");

        /// <summary>
        /// 一个从属服务器执行命令 SLAVEOF NO ONE 将使得这个从属服务器关闭复制功能，并从从属服务器转变回主服务器，原来同步所得的数据集不会被丢弃。
        /// </summary>
        /// <returns>总是返回 OK 。</returns>
        bool SlaveOf();

        /// <summary>
        /// 执行一个非法的内存访问从而让 Redis 崩溃，仅在开发时用于 BUG 调试。
        /// </summary>
        void DebugSegfault();

        /// <summary>
        /// 用于清空整个 Redis 服务器的数据(删除所有数据库的所有 key )。
        /// </summary>
        /// <returns></returns>
        string FlushAll();

        /// <summary>
        /// 用于返回当前数据库的 key 的数量。
        /// </summary>
        /// <returns>当前数据库的 key 的数量。</returns>
        int DbSize();

        /// <summary>
        /// 用于异步执行一个 AOF（AppendOnly File） 文件重写操作。重写会创建一个当前 AOF 文件的体积优化版本。
        /// 即使 Bgrewriteaof 执行失败，也不会有任何数据丢失，因为旧的 AOF 文件在 Bgrewriteaof 成功之前不会被修改。
        /// 注意：从 Redis 2.4 开始， AOF 重写由 Redis 自行触发， BGREWRITEAOF 仅仅用于手动触发重写操作。
        /// </summary>
        /// <returns>反馈信息。</returns>
        string BgReWriteAof();

        /// <summary>
        /// 用于当前的集群状态，以数组形式展示。
        /// </summary>
        /// <returns>IP/端口嵌套的列表数组。</returns>
        string[] ClusterSlots();

        /// <summary>
        /// 可以动态地调整 Redis 服务器的配置(configuration)而无须重启。
        /// </summary>
        /// <param name="parameter">配置参数</param>
        /// <param name="value">值</param>
        /// <returns>当设置成功时返回 OK ，否则返回一个错误。</returns>
        bool ConfigSet(string parameter, string value);

        /// <summary>
        /// 用于获取 redis 命令的描述信息。
        /// </summary>
        /// <param name="commands">命令名称</param>
        /// <returns>命令描述信息的嵌套列表。</returns>
        string[] CommandInfo(params string[] commands);

        /// <summary>
        /// 停止所有客户端
        /// 如果有至少一个保存点在等待，执行 SAVE 命令
        /// 如果 AOF 选项被打开，更新 AOF 文件
        /// 关闭 redis 服务器(server)
        /// </summary>
        /// <returns>执行失败时返回错误。 执行成功时不返回任何信息，服务器和客户端的连接断开，客户端自动退出。</returns>
        string ShutDown();

        /// <summary>
        /// 用于同步主从服务器。
        /// </summary>
        /// <returns>不明确。</returns>
        string Sync();

        /// <summary>
        /// 用于关闭客户端连接。
        /// </summary>
        /// <returns>成功关闭时，返回 OK 。</returns>
        string ClientKill(string host, int port);

        /// <summary>
        /// 查看主从实例所属的角色，角色有master, slave, sentinel。
        /// </summary>
        /// <returns>返回一个数组：第一个参数是 master, slave, sentinel 三个中的一个。</returns>
        string[] Role();

        /// <summary>
        /// 用于实时打印出 Redis 服务器接收到的命令，调试用。
        /// </summary>
        /// <returns>总是返回 OK 。</returns>
        string Monitor();

        /// <summary>
        /// 用于获取所有 key。
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns>key 的列表。</returns>
        string[] CommandGetKeys(params string[] parameters);

        /// <summary>
        /// 用于返回 CLIENT SETNAME 命令为连接设置的名字
        /// </summary>
        /// <returns> 因为新创建的连接默认是没有名字的， 对于没有名字的连接， CLIENT GETNAME 返回空白回复。</returns>
        string ClientGetName();

        /// <summary>
        /// 用于重置 INFO 命令中的某些统计数据
        /// Keyspace hits (键空间命中次数)
        /// Keyspace misses (键空间不命中次数)
        /// Number of commands processed (执行命令的次数)
        /// Number of connections received (连接服务器的次数)
        /// Number of expired keys (过期key的数量)
        /// Number of rejected connections (被拒绝的连接数量)
        /// Latest fork(2) time(最后执行 fork(2) 的时间)
        /// The aof_delayed_fsync counter(aof_delayed_fsync 计数器的值)
        /// </summary>
        /// <returns>总是返回 OK 。</returns>
        string ConfigResetStat();

        /// <summary>
        /// 用于统计 redis 命令的个数。
        /// </summary>
        /// <returns></returns>
        int CommandCount();

        /// <summary>
        /// 用于返回当前服务器时间。
        /// </summary>
        /// <returns>一个包含两个字符串的列表： 第一个字符串是当前时间(以 UNIX 时间戳格式表示)，而第二个字符串是当前这一秒钟已经逝去的微秒数。</returns>
        string[] Time();

        /// <summary>
        /// 以一种易于理解和阅读的格式，返回关于 Redis 服务器的各种信息和统计数值。
        /// </summary>
        /// <returns>字符串，文本行的集合。</returns>
        string[] Info();

        /// <summary>
        ///  以一种易于理解和阅读的格式，返回关于 Redis 服务器的各种信息和统计数值。
        /// </summary>
        /// <param name="section">给定可选的参数 section ，可以让命令只返回某一部分的信息：</param>
        /// <returns>字符串，文本行的集合。</returns>
        string[] Info(string section);

        /// <summary>
        /// 对启动 Redis 服务器时所指定的 redis.conf 配置文件进行改写
        /// </summary>
        /// <returns>一个状态值：如果配置重写成功则返回 OK ，失败则返回一个错误。</returns>
        string ConfigRewrite();

        /// <summary>
        /// 用于返回所有连接到服务器的客户端信息和统计数据
        /// </summary>
        /// <returns>
        /// 命令返回多行字符串，这些字符串按以下形式被格式化：
        ///
        /// 每个已连接客户端对应一行（以 LF 分割）
        /// 每行字符串由一系列 属性 = 值 形式的域组成，每个域之间以空格分开
        ///     以下是域的含义：
        ///
        /// addr ： 客户端的地址和端口
        ///     fd ： 套接字所使用的文件描述符
        ///     age ： 以秒计算的已连接时长
        ///     idle ： 以秒计算的空闲时长
        ///     flags ： 客户端 flag
        /// db ： 该客户端正在使用的数据库 ID
        /// sub ： 已订阅频道的数量
        ///     psub ： 已订阅模式的数量
        ///     multi ： 在事务中被执行的命令数量
        ///     qbuf ： 查询缓冲区的长度（字节为单位， 0 表示没有分配查询缓冲区）
        /// qbuf-free ： 查询缓冲区剩余空间的长度（字节为单位， 0 表示没有剩余空间）
        /// obl ： 输出缓冲区的长度（字节为单位， 0 表示没有分配输出缓冲区）
        /// oll ： 输出列表包含的对象数量（当输出缓冲区没有剩余空间时，命令回复会以字符串对象的形式被入队到这个队列里）
        /// omem ： 输出缓冲区和输出列表占用的内存总量
        ///     events ： 文件描述符事件
        ///     cmd ： 最近一次执行的命令
        ///     客户端 flag 可以由以下部分组成：
        ///
        /// O ： 客户端是 MONITOR 模式下的附属节点（slave）
        /// S ： 客户端是一般模式下（normal）的附属节点
        ///     M ： 客户端是主节点（master）
        /// x ： 客户端正在执行事务
        ///     b ： 客户端正在等待阻塞事件
        ///     i ： 客户端正在等待 VM I/O 操作（已废弃）
        /// d ： 一个受监视（watched）的键已被修改， EXEC 命令将失败
        /// c : 在将回复完整地写出之后，关闭链接
        ///     u : 客户端未被阻塞（unblocked）
        /// A : 尽可能快地关闭连接
        ///     N : 未设置任何 flag
        /// 文件描述符事件可以是：
        ///
        /// r : 客户端套接字（在事件 loop 中）是可读的（readable）
        /// w : 客户端套接字（在事件 loop 中）是可写的（writeable）
        /// </returns>
        string ClientList();

        /// <summary>
        /// 用于指定当前连接的名称。
        /// </summary>
        /// <returns></returns>
        string ClientSetName(string name);

        /// <summary>
        /// 用于在后台异步保存当前数据库的数据到磁盘。
        /// </summary>
        /// <returns>反馈信息。</returns>
        string BgSave();

        #endregion Redis 连接 命令

        #region Redis 脚本 命令

        /// <summary>
        /// 命令用于杀死当前正在运行的 Lua 脚本，当且仅当这个脚本没有执行过任何写操作时，这个命令才生效
        /// </summary>
        /// <returns></returns>
        string ScriptKill();

        /// <summary>
        /// 用于将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本。
        /// EVAL 命令也会将脚本添加到脚本缓存中，但是它会立即对输入的脚本进行求值。
        /// 如果给定的脚本已经在缓存里面了，那么不执行任何操作。
        /// 在脚本被加入到缓存之后，通过 EVALSHA 命令，可以使用脚本的 SHA1 校验和来调用这个脚本。
        /// 脚本可以在缓存中保留无限长的时间，直到执行 SCRIPT FLUSH 为止。
        /// 关于使用 Redis 对 Lua 脚本进行求值的更多信息，请参见 EVAL 命令。
        /// </summary>
        /// <returns>给定脚本的 SHA1 校验和</returns>
        string ScriptLoad(string script);

        /// <summary>
        /// 使用 Lua 解释器执行脚本。
        /// </summary>
        /// <param name="script">参数是一段 Lua 5.1 脚本程序。脚本不必(也不应该)定义为一个 Lua 函数。</param>
        /// <param name="numkeys">用于指定键名参数的个数。</param>
        /// <param name="keys">键名</param>
        /// <param name="args"> 附加参数，在 Lua 中通过全局变量 ARGV 数组访问，访问的形式和 KEYS 变量类似( ARGV[1] 、 ARGV[2] ，诸如此类)。</param>
        /// <returns></returns>
        string[] Eval(string script, int numkeys, string[] keys, string[] args = null);

        /// <summary>
        /// 根据给定的 sha1 校验码，执行缓存在服务器中的脚本。
        /// </summary>
        /// <param name="sha1"> SHA1 校验和</param>
        /// <param name="numkeys">用于指定键名参数的个数。</param>
        /// <param name="keys">键名</param>
        /// <param name="args"> 附加参数，在 Lua 中通过全局变量 ARGV 数组访问，访问的形式和 KEYS 变量类似( ARGV[1] 、 ARGV[2] ，诸如此类)。</param>
        /// <returns></returns>
        string[] EvalSha(string sha1, int numkeys, string[] keys, string[] args = null);

        /// <summary>
        /// 用于校验指定的脚本是否已经被保存在缓存当中。
        /// </summary>
        /// <param name="sha1"> SHA1 校验和</param>
        /// <param name="numkeys">用于指定键名参数的个数。</param>
        /// <param name="keys">键名</param>
        /// <param name="args"> 附加参数，在 Lua 中通过全局变量 ARGV 数组访问，访问的形式和 KEYS 变量类似( ARGV[1] 、 ARGV[2] ，诸如此类)。</param>
        /// <returns>一个列表，包含 0 和 1 ，前者表示脚本不存在于缓存，后者表示脚本已经在缓存里面了。</returns>
        string[] ScriptExists(string sha1, int numkeys, string[] keys, string[] args = null);

        /// <summary>
        /// 用于清除所有 Lua 脚本缓存。
        /// </summary>
        /// <returns>总是返回 OK</returns>
        string ScriptFlush();

        /// <summary>
        /// 用于执行所有事务块内的命令
        /// </summary>
        /// <returns>事务块内所有命令的返回值，按命令执行的先后顺序排列。 当操作被打断时，返回空值 nil 。</returns>
        string[] Exec();

        /// <summary>
        /// 用于监视一个(或多个) key ，如果在事务执行之前这个(或这些) key 被其他命令所改动，那么事务将被打断
        /// </summary>
        /// <returns>总是返回 OK 。</returns>
        string Watch(params string[] keys);

        /// <summary>
        /// 用于取消事务，放弃执行事务块内的所有命令
        /// </summary>
        /// <returns>总是返回 OK </returns>
        string Discard();

        /// <summary>
        /// 用于取消 WATCH 命令对所有 key 的监视。
        /// </summary>
        /// <returns>总是返回 OK 。</returns>
        string UnWatch();

        /// <summary>
        /// 用于标记一个事务块的开始。 事务块内的多条命令会按照先后顺序被放进一个队列当中，最后由 EXEC 命令原子性(atomic)地执行
        /// </summary>
        /// <returns>总是返回 OK </returns>
        string Multi();

        #endregion Redis 脚本 命令

        #region Redis HyperLogLog 命令

        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog ，合并后的 HyperLogLog 的基数估算值是通过对所有 给定 HyperLogLog 进行并集计算得出的。
        /// </summary>
        /// <param name="destKey">合并后的键</param>
        /// <param name="sourceKey">要合并的键</param>
        /// <returns>返回 OK。</returns>
        string PgMerge(string destKey, params string[] sourceKey);

        /// <summary>
        /// 将所有元素参数添加到 HyperLogLog 数据结构中。
        /// </summary>
        /// <param name="key">存储的位置</param>
        /// <param name="element">要存储的元素</param>
        /// <returns>整型，如果至少有个元素被添加返回 1， 否则返回 0。</returns>
        int PfAdd(string key, params string[] element);

        /// <summary>
        /// 返回给定 HyperLogLog 的基数估算值。
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>整数，返回给定 HyperLogLog 的基数值，如果多个 HyperLogLog 则返回基数估值之和。</returns>
        int PfCount(params string[] keys);

        #endregion Redis HyperLogLog 命令

        #region Redis 发布订阅 命令

        /// <summary>
        /// 用于退订给定的一个或多个频道的信息。
        /// </summary>
        /// <param name="channel">频道</param>
        /// <returns>这个命令在不同的客户端中有不同的表现。</returns>
        string Unsubscribe(params string[] channel);

        /// <summary>
        /// 用于订阅给定的一个或多个频道的信息。
        /// </summary>
        /// <param name="channel">频道</param>
        /// <returns>接收到的信息</returns>
        string Subscribe(params string[] channel);

        /// <summary>
        /// 用于查看订阅与发布系统状态，它由数个不同格式的子命令组成。
        /// </summary>
        /// <returns>由活跃频道组成的列表。</returns>
        string PubSub(string subCommand, params string[] argument);

        /// <summary>
        /// 用于退订所有给定模式的频道。
        /// </summary>
        /// <param name="pattern">频道</param>
        /// <returns>这个命令在不同的客户端中有不同的表现。</returns>
        string PunSubscribe(string pattern);

        /// <summary>
        /// 用于将信息发送到指定的频道。
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="message">信息</param>
        /// <returns>接收到信息的订阅者数量。</returns>
        int Publish(string channel, string message);

        /// <summary>
        /// 订阅一个或多个符合给定模式的频道。
        /// </summary>
        /// <param name="pattern">每个模式以 * 作为匹配符，比如 it* 匹配所有以 it 开头的频道( it.news 、 it.blog 、 it.tweets 等等)。 news.* 匹配所有以 news. 开头的频道( news.it 、 news.global.today 等等)，诸如此类。</param>
        /// <returns>接收到的信息。</returns>
        string PSubscribe(params string[] pattern);

        #endregion Redis 发布订阅 命令

        #region Redis 地理位置(geo) 命令

        /// <summary>
        ///  返回一个或多个位置元素的 Geohash 表示
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns>一个数组， 数组的每个项都是一个 geohash 。 命令返回的 geohash 的位置与用户给定的位置元素的位置一一对应。</returns>
        string[] GeoHash(params string[] keys);

        /// <summary>
        /// 从key里返回所有给定位置元素的位置（经度和纬度）。
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns>
        /// 返回一个数组， 数组中的每个项都由两个元素组成： 第一个元素为给定位置元素的经度， 而第二个元素则为给定位置元素的纬度。
        /// 当给定的位置元素不存在时， 对应的数组项为空值。
        /// </returns>
        string[] GeoPos(params string[] keys);

        /// <summary>
        /// 返回两个给定位置之间的距离 命令在计算距离时会假设地球为完美的球形， 在极限情况下， 这一假设最大会造成 0.5% 的误差。
        /// </summary>
        /// <param name="keys">键</param>
        /// <param name="unit">
        /// m 表示单位为米。
        /// km 表示单位为千米。
        /// mi 表示单位为英里。
        /// ft 表示单位为英尺。
        /// </param>
        /// <returns>计算出的距离会以双精度浮点数的形式被返回。 如果给定的位置元素不存在， 那么命令返回空值。</returns>
        string GeoDist(string[] keys, string unit = "km");

        /// <summary>
        /// 以给定的经纬度为中心， 找出某一半径内的元素  GEORADIUS Sicily 15 37 200 km WITHCOORD WITHDIST
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">半径</param>
        /// <param name="unit">
        /// 单位
        /// m 表示单位为米。
        /// km 表示单位为千米。
        /// mi 表示单位为英里。
        /// ft 表示单位为英尺。
        /// </param>
        /// <param name="key">键</param>
        /// <param name="withCooRd">将位置元素的经度和维度也一并返回。</param>
        /// <param name="withDist"></param>
        /// <param name="withHash">以 52 位有符号整数的形式， 返回位置元素经过原始 geohash 编码的有序集合分值。 这个选项主要用于底层应用或者调试， 实际中的作用并不大。</param>
        /// <param name="count">默认返回全部，减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。 </param>
        /// <param name="sort">
        /// 命令默认返回未排序的位置元素
        /// ASC: 根据中心的位置， 按照从近到远的方式返回位置元素。
        /// DESC: 根据中心的位置， 按照从远到近的方式返回位置元素。</param>
        /// <returns></returns>
        string GeoRadius(string key, decimal longitude, decimal latitude, long radius, string unit = "km", bool withCooRd = false, bool withDist = false, bool withHash = false, int count = -1, int sort = -1);

        /// <summary>
        /// 将指定的地理空间位置（纬度、经度、名称）添加到指定的key中
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="member">位置名称</param>
        /// <returns></returns>
        string GeoAdd(string key, string member, decimal longitude, decimal latitude);

        /// <summary>
        /// 找出位于指定范围内的元素，中心点是由给定的位置元素决定
        /// </summary>
        /// <param name="member">元素</param>
        /// <param name="radius">半径</param>
        /// <param name="unit">
        /// 单位
        /// m 表示单位为米。
        /// km 表示单位为千米。
        /// mi 表示单位为英里。
        /// ft 表示单位为英尺。
        /// </param>
        /// <param name="key">键</param>
        /// <param name="withCooRd">将位置元素的经度和维度也一并返回。</param>
        /// <param name="withDist"></param>
        /// <param name="withHash">以 52 位有符号整数的形式， 返回位置元素经过原始 geohash 编码的有序集合分值。 这个选项主要用于底层应用或者调试， 实际中的作用并不大。</param>
        /// <param name="count">默认返回全部，减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。 </param>
        /// <param name="sort">
        /// 命令默认返回未排序的位置元素
        /// ASC: 根据中心的位置， 按照从近到远的方式返回位置元素。
        /// DESC: 根据中心的位置， 按照从远到近的方式返回位置元素。</param>
        /// <returns></returns>
        string GeoRadiusByMember(string key, string member, long radius, string unit = "km", bool withCooRd = false, bool withDist = false, bool withHash = false, int count = -1, int sort = -1);

        #endregion Redis 地理位置(geo) 命令

        #region 迁移数据

        /// <summary>
        /// 将 key 原子性地从当前实例传送到目标实例的指定数据库上，一旦传送成功， key 保证会出现在目标实例上，而当前实例上的 key 会被删除。
        /// </summary>
        /// <param name="password"></param>
        /// <param name="key">迁移多个键</param>
        /// <param name="host">目标redis的IP地址</param>
        /// <param name="port">目标redis的端口号</param>
        /// <param name="db">目标数据库索引</param>
        /// <param name="timeout">迁移的超时时间（单位毫秒）</param>
        /// <param name="copy"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        bool Migrate(string[] key, string host, int port, string password, int db, int timeout, bool copy = false, bool replace = true);

        #endregion 迁移数据
    }
}