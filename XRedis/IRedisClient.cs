using System;
using System.Collections.Generic;
using System.Text;

namespace XRedis
{
    public interface IRedisClient:IDisposable
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
        int PExpire(string key,int milliseconds);
        /// <summary>
        /// 设置 key 过期时间的时间戳(unix timestamp) 以毫秒计
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="milliseconds">UNIX 时间戳</param>
        /// <returns>设置成功返回 1 。 当 key 不存在或者不能为 key 设置过期时间时(比如在低于 2.1.3 版本的 Redis 中你尝试更新 key 的过期时间)返回 0 。</returns>
        int PExpireAt(string key, int milliseconds);
        /// <summary>
        /// 修改 key 的名称
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="newKey">修改后的键</param>
        /// <returns>改名成功时提示 OK ，失败时候返回一个错误。</returns>
        string Rename(string key,string newKey);
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
        int Move(string key,int dbIndex);
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
        int Expire(string key,int second);
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
        int RenameNx(string key,string newKey);
        /// <summary>
        /// 用于检查给定 key 是否存在。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>若 key 存在返回 1 ，否则返回 0 。</returns>
        int Exists(string key);
        /// <summary>
        /// 以 UNIX 时间戳(unix timestamp)格式设置 key 的过期时间。key 过期后将不再可用。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="second">UNIX 时间戳</param>
        /// <returns>设置成功返回 1 。 当 key 不存在或者不能为 key 设置过期时间时(比如在低于 2.1.3 版本的 Redis 中你尝试更新 key 的过期时间)返回 0 。</returns>
        int ExpireAt(string key,int second);
        /// <summary>
        /// 用于查找所有符合给定模式 pattern 的 key 。。
        /// </summary>
        /// <param name="pattern">匹配规则</param>
        /// <returns>符合给定模式的 key 列表 (Array)。</returns>
        string[] Keys(string pattern);

        #endregion


        #region Redis 字符串(String) 命令
        /// <summary>
        /// 在指定的 key 不存在时，为 key 设置指定的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>设置成功，返回 1 。 设置失败，返回 0 。</returns>
        int SetNx(string key,string value);
        /// <summary>
        /// 用于获取存储在指定 key 中字符串的子字符串。字符串的截取范围由 start 和 end 两个偏移量决定(包括 start 和 end 在内)。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="start">默认等于0 从头开始</param>
        /// <param name="end">默认等于-1 结束位置</param>
        /// <returns>截取得到的子字符串。</returns>
        string GetRange(string key,int start=0,int end=-1);
        /// <summary>
        /// 用于同时设置一个或多个 key-value 对。
        /// </summary>
        /// <returns>总是返回 OK 。</returns>
        string MSet(Dictionary<string,string> kv);

        /// <summary>
        /// 指定的 key 设置值及其过期时间。如果 key 已经存在， SETEX 命令将会替换旧的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间</param>
        /// <returns>设置成功时返回 OK 。</returns>
        string SetEx(string key,string value,int timeout);
        /// <summary>
        /// 用于设置给定 key 的值。如果 key 已经存储其他值， SET 就覆写旧值，且无视类型。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>
        /// 在 Redis 2.6.12 以前版本， SET 命令总是返回 OK 。
        /// 从 Redis 2.6.12 版本开始， SET 在设置操作成功完成时，才返回 OK 。
        /// </returns>
        string Set(string key, string value);
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
        int GetBit(string key,int offset);

        /// <summary>
        /// 用于对 key 所储存的字符串值，设置或清除指定偏移量上的位(bit)。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="offset">偏移量</param>
        /// <param name="value">位(bit)</param>
        /// <returns>指定偏移量原来储存的位。</returns>
        int SetBit(string key,int offset,int value);
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
        string DecrBy(string key,int num);
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
        int MSetNx(Dictionary<string,string> kv);
        /// <summary>
        /// 将 key 中储存的数字加上指定的增量值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="num">增量值</param>
        /// <returns>加上指定的增量值之后， key 的值。</returns>
        string IncrBy(string key,int num);
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
        int SetRange(string key, int offset,string value);
        /// <summary>
        ///这个命令和 SETEX 命令相似，但它以毫秒为单位设置 key 的生存时间，而不是像 SETEX 命令那样，以秒为单位。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="milliseconds">毫秒</param>
        /// <returns>设置成功时返回 OK 。</returns>
        string PSetEx(string key,string value,int milliseconds);
        /// <summary>
        /// 用于为指定的 key 追加值。
        /// </summary>
        /// <param name="key">
        /// 如果 key 已经存在并且是一个字符串， APPEND 命令将 value 追加到 key 原来的值的末尾。
        /// 如果 key 不存在， APPEND 就简单地将给定 key 设为 value ，就像执行 SET key value 一样。
        /// </param>
        /// <param name="value">值</param>
        /// <returns>追加指定值之后， key 中字符串的长度。</returns>
        int Append(string key,string value);
        /// <summary>
        /// 用于设置指定 key 的值，并返回 key 旧的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>
        /// 返回给定 key 的旧值。 当 key 没有旧值时，即 key 不存在时，返回 nil 。
        /// 当 key 存在但不是字符串类型时，返回一个错误。
        /// </returns>
        string GetSet(string key,string value);
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

        #endregion

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
        int RPush(string key,params string[] values);
        /// <summary>
        /// 返回列表中指定区间内的元素，区间以偏移量 START 和 END 指定。 其中 0 表示列表的第一个元素， 1 表示列表的第二个元素，以此类推。 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="start">开始</param>
        /// <param name="end">结束</param>
        /// <returns>一个列表，包含指定区间内的元素。</returns>
        string[] LRange(string key,int start=0,int end=-1);
        /// <summary>
        /// 用于移除列表的最后一个元素，并将该元素添加到另一个列表并返回。
        /// </summary>
        /// <param name="key">移除列表的最后一个元素</param>
        /// <param name="newKey">移除元素添加到另一个列表</param>
        /// <returns>返回移除列表</returns>
        string[] RPopLPush(string key,string newKey);

        /// <summary>
        /// 移出并获取列表的第一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="value">被弹出元素的值</param>
        /// <param name="timeout">等待超时</param>
        /// <param name="key">被弹出元素所属的 key</param>
        /// <returns>如果列表为空，返回一个 nil 。 否则，返回一个含有两个元素的列表，第一个元素是被弹出元素所属的 key ，第二个元素是被弹出元素的值。</returns>
        string[] BlPop(string value, int timeout,params string[] key);

        /// <summary>
        /// 移出并获取列表的最后一个元素， 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="value">被弹出元素的值</param>
        /// <param name="timeout">等待时长</param>
        /// <param name="keys">被弹出元素所属的 key </param>
        /// <returns>假如在指定时间内没有任何元素被弹出，则返回一个 nil 和等待时长。 反之，返回一个含有两个元素的列表</returns>
        string[] BrPop(string value,int timeout,params string[] keys);
        /// <summary>
        /// 从列表中弹出一个值，将弹出的元素插入到另外一个列表中并返回它； 如果列表没有元素会阻塞列表直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">被弹出元素的值</param>
        /// <param name="timeout">等待时长</param>
        /// <returns>假如在指定时间内没有任何元素被弹出，则返回一个 nil 和等待时长。 反之，返回一个含有两个元素的列表</returns>
        string[] BrPopLPush(string key,string value,int timeout);
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
        int LRem(string key, string value,int count=0);
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
        string LTrim(string key,int start=0,int end=-1);

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
        int LPushX(string key,params string[] values);

        /// <summary>
        /// 在列表的元素前或者后插入元素
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">新值</param>
        /// <param name="existValue">存在的值</param>
        /// <param name="before">在存在的值之前</param>
        /// <returns></returns>
        int LInsert(string key,string value,string existValue, bool before=true);
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
        string LSet(string key,string value,int index);
        /// <summary>
        /// 一个或多个值插入到列表头部。
        /// </summary>
        /// <param name="key">
        /// 如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。 当 key 存在但不是列表类型时，返回一个错误。
        /// 在Redis 2.4版本以前的 LPUSH 命令，都只接受单个 value 值。
        /// </param>
        /// <param name="values">一个或多个值</param>
        /// <returns>执行 LPUSH 命令后，列表的长度。</returns>
        int LPush(string key,params string[] values);
        /// <summary>
        /// 将一个或多个值插入到已存在的列表尾部(最右边)
        /// </summary>
        /// <param name="key">如果列表不存在，操作无效</param>
        /// <param name="values">一个或多个值</param>
        /// <returns>执行 Rpushx 操作后，列表的长度。</returns>
        int RPushX(string key, params string[] values);

        #endregion

        #region Redis 哈希(Hash) 命令
        /// <summary>
        /// 用于同时将多个 field-value (字段-值)对设置到哈希表中。 此命令会覆盖哈希表中已存在的字段。 
        /// </summary>
        /// <param name="key">键 如果哈希表不存在，会创建一个空哈希表，并执行 HMSET 操作。</param>
        /// <param name="kV">字段-值</param>
        /// <returns></returns>
        string HmSet(string key,Dictionary<string,string> kV);
        /// <summary>
        /// 用于返回哈希表中，一个或多个给定字段的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="fields">如果指定的字段不存在于哈希表，那么返回一个 nil 值。</param>
        /// <returns>一个包含多个给定字段关联值的表，表值的排列顺序和指定字段的请求顺序一样。</returns>
        string[] HmGet(string key,params string[] fields);
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
        int HSet(string key,string field,string value);
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
        string HGet(string key,string field);
        /// <summary>
        /// 用于查看哈希表的指定字段是否存在。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="field">字段</param>
        /// <returns>如果哈希表含有给定字段，返回 1 。 如果哈希表不含有给定字段，或 key 不存在，返回 0 。</returns>
        int HExists(string key,string field);
        /// <summary>
        /// 用于为哈希表中的字段值加上指定增量值。
        /// </summary>
        /// <param name="key">如果哈希表的 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。</param>
        /// <param name="field">如果指定的字段不存在，那么在执行命令前，字段的值被初始化为 0 。</param>
        /// <param name="number">增量也可以为负数，相当于对指定字段进行减法操作。</param>
        /// <returns>执行 HINCRBY 命令之后，哈希表中字段的值。</returns>
        string HinCrBy(string key,string field,int number);
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
        int HDel(string key,params string[] fields);
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
        int HSetNx(string key,string field,string value);

        #endregion

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
        string[] SRandMember(string key,int count=1);
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
        int SRem(string key,params string[] member);
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
        /// <param name="kye">假如集合 key 不存在，则创建一个只包含添加的元素作成员的集合。当集合 key 不是集合类型时，返回一个错误。</param>
        /// <param name="values">注意：在Redis2.4版本以前， SADD 只接受单个成员值</param>
        /// <returns>被添加到集合中的新元素的数量，不包括被忽略的元素。</returns>
        int SAdd(string kye,params string[] values);
        /// <summary>
        /// 判断成员元素是否是集合的成员。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">要判断的成员</param>
        /// <returns>如果成员元素是集合的成员，返回 1 。 如果成员元素不是集合的成员，或 key 不存在，返回 0 。</returns>
        int SIsMember(string key,string value);
        /// <summary>
        /// 将给定集合之间的差集存储在指定的集合中。
        /// </summary>
        /// <param name="destination">存储在指定的集合。如果指定的集合 key 已存在，则会被覆盖</param>
        /// <param name="keys">指定的差集</param>
        /// <returns>结果集中的元素数量。</returns>
        int SDiffStore(string destination,params string[] keys);
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
        string[] SScan(string key,int cursor, string pattern,int count=10);
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

        #endregion

        #region Redis 有序集合(sorted set) 命令


        #endregion
    }
}
