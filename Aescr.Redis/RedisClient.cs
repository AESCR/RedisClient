using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Aescr.Redis
{
    public class RedisClient : IRedisClient
    {
        private static readonly Hashtable Hashtable = Hashtable.Synchronized(new Hashtable());
        public bool IsConnected => _redisSocket?.IsConnected ?? false;
        public string Prefix => _connection?.Prefix;
        public string Host => _connection?.Host?? "127.0.0.1";
        private  RedisSocket _redisSocket;
        private bool _disposedValue;
        public int Database => _connection?.Database??0;
        private RedisConnection _connection;
        public int Weight { get; set; } = 1;
        public event EventHandler<RedisMessage> Recieved
        {
            add => _redisSocket.Recieved += value;
            remove => _redisSocket.Recieved -= value;
        }

        public event EventHandler Connected
        {
            add => _redisSocket.Connected += value;
            remove => _redisSocket.Connected -= value;
        }
        public static RedisClient GetSingle(string host)
        {
            RedisConnection rc = host;
            if (!Hashtable.ContainsKey(rc.Host))
            {
                RedisClient redisClient = new RedisClient(rc);
                Hashtable.Add(redisClient.Host, redisClient);
                return redisClient;
            }
            else
            {
                return Hashtable[rc.Host] as RedisClient;
            }
        }
        public RedisClient(string connectionStr)
        {
            Init(connectionStr);
        }
        public RedisClient(string ip, int port, string password)
        {
            _connection = new RedisConnection { Host = $"{ip}:{port}", Password = password };
            Init(_connection);
        }
        public RedisClient() : this("127.0.0.1", 6379, "")
        {

        }
        private void Init(string connectionStr)
        {
            _connection = connectionStr;
            _redisSocket = new RedisSocket(_connection.Host, _connection.Ssl, _connection.Encoding);
            _redisSocket.Connected += redisSocket_Connected;
        }
        private void redisSocket_Connected(object sender, EventArgs e)
        {
            Auth(_connection.Password);
            Select(_connection.Database);
        }
        /*public void AutoMasterSlave()
        {
            SlaveOf();
            var par = _slaveClients.AsParallel();
            par.ForAll(@this =>
            {
                @this.SlaveOf(Host,_connection.Password);
            });
        }*/
        public bool Connect()
        {
            return _redisSocket.Connect();
        }
        public void SetPrefix(string prefix)
        {
            _connection.Prefix = prefix;
        }
        public string GetPrefixKey(string key)
        {
            if (string.IsNullOrWhiteSpace(Prefix) ==false)
            {
                if (key.IndexOf(Prefix, StringComparison.Ordinal)==0)
                {
                    return key;
                }
                return _connection.Prefix + "_" + key;
            }
            return key;
        }

        public string GetRandomKey()
        {
            var key= Snowflake.Instance().GetId().ToString();
            return GetPrefixKey(key);
        }
        public string[] GetPrefixKey(string[] key)
        {
            if (string.IsNullOrWhiteSpace(Prefix) == false)
            {
                string[] str=new string[key.Length];
                for (int i = 0; i < key.Length; i++)
                {
                    str[i] = _connection.Prefix +"_"+ key[i];
                }
                return str;
            }
            return key;
        }
        public string Add(string value, TimeSpan expiresIn)
        {
            var key = GetRandomKey();
            if (!SetNx(key, value)) throw new Exception($"添加Add失败！Key:{key}");
            if (expiresIn != TimeSpan.Zero)
            {
                Expire(key, expiresIn.Seconds);
            }
            return key;
        }
        public string Add(string value)
        {
            var key = GetRandomKey();
            if (!SetNx(key, value)) throw new Exception($"添加Add失败！Key:{key}");
            return key;
        }
        public bool SetPassword(string newPassword)
        {
            return ConfigSet("requirepass", newPassword);
        }
        public string Type(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendCommand(new []{ "Type", prefixKey });
        }

        public int PExpire(string key, long milliseconds)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("PExpire", prefixKey, milliseconds.ToString());
        }

        public int PExpire(string key, TimeSpan timeSpan)
        {
            var milliseconds = (long)timeSpan.TotalMilliseconds;
            return PExpire(key, milliseconds);
        }
        public int PExpireAt(string key, long timestamp)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("PExpireAt", prefixKey, timestamp.ToString());
        }

        public string Rename(string key, string newKey)
        {
            var prefixKey = GetPrefixKey(key);
            var prefixNewKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("Rename", prefixKey, prefixNewKey);
        }

        public int Persist(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("Persist", prefixKey);
        }

        public int Move(string key, int dbIndex)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("Move", prefixKey, dbIndex.ToString());
        }

        public string RandomKey()
        {
            return _redisSocket.SendExpectedString("RandomKey");
        }

        public string Dump(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("Dump", prefixKey);
        }

        public int Ttl(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("Ttl", prefixKey);
        }

        public int Expire(string key, int second)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("Expire", prefixKey, second.ToString());
        }

        public int Del(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("Del", prefixKey);
        }

        public long PTtl(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInt64("PTtl", prefixKey);
        }

        public int RenameNx(string key, string newKey)
        {
            var prefixKey = GetPrefixKey(key);
            var prefixNewKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("RenameNx", prefixKey, prefixNewKey);
        }

        public bool Exists(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("Exists", prefixKey) == 1;
        }

        public int ExpireAt(string key, long timestamp)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("ExpireAt", prefixKey, timestamp.ToString());
        }

        public string[] Keys(string pattern)
        {
            return _redisSocket.SendExpectedArray("Keys", pattern);
        }

        public bool SetNx(string key, string value)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("SetNx", prefixKey, value)==1;
        }

        public string GetRange(string key, int start = 0, int end = -1)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("GetRange", prefixKey, start.ToString(), end.ToString());
        }

        public string MSet(Dictionary<string, string> kv)
        {
            string[] result = new string[kv.Count * 2];
            int index = 0;
            foreach (string key in kv.Keys)
            {
                var prefixKey = GetPrefixKey(key);
                result[index] = prefixKey;
                result[index + 1] = kv[key];
                index = index + 2;
            }
            return _redisSocket.SendExpectedString("MSet", result);
        }

        public string SetEx(string key, string value, int timeout)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("SetEx", prefixKey, timeout.ToString(), value);
        }

        public bool Set(string key, string value)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedOk("Set", prefixKey, value);
        }

        public string Get(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("Get", prefixKey);
        }

        public int GetBit(string key, int offset)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("GetBit", prefixKey, offset.ToString());
        }

        public int SetBit(string key, int offset, int value)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("SetBit", prefixKey, offset.ToString(), value.ToString());
        }

        public string Decr(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("Decr", prefixKey);
        }

        public string DecrBy(string key, int num)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("DecrBy", prefixKey, num.ToString());
        }

        public int StrLen(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("StrLen", prefixKey);
        }

        public int MSetNx(Dictionary<string, string> kv)
        {
            string[] result = new string[kv.Count * 2];
            int index = 0;
            foreach (string key in kv.Keys)
            {
                var prefixKey = GetPrefixKey(key);
                result[index] = prefixKey;
                result[index + 1] = kv[key];
                index = index + 2;
            }
            return _redisSocket.SendExpectedInteger("MSetNx", result);
        }

        public string IncrBy(string key, int num)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("IncrBy", prefixKey, num.ToString());
        }

        public string IncrByFloat(string key, float fraction)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("IncrByFloat", prefixKey, fraction.ToString(CultureInfo.InvariantCulture));
        }

        public int SetRange(string key, int offset, string value)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("SetRange", prefixKey, offset.ToString(), value);
        }

        public string PSetEx(string key, string value, int milliseconds)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("PSetEx", prefixKey, milliseconds.ToString(), value);
        }

        public int Append(string key, string value)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("Append", prefixKey, value);
        }

        public string GetSet(string key, string value)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("GetSet", prefixKey, value);
        }

        public string[] MGet(params string[] keys)
        {
            var prefixKey = GetPrefixKey(keys);
            return _redisSocket.SendExpectedArray("MGet", prefixKey);
        }

        public string Incr(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("Incr", prefixKey);
        }

        public string LIndex(string key, int index)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("LIndex", prefixKey, index.ToString());
        }

        public int RPush(string key, params string[] values)
        {
            string[] args = new string[values.Length + 1];
            var prefixKey = GetPrefixKey(key);
            args[0] = prefixKey;
            int index = 1;
            foreach (var value in values)
            {
                args[index] = value;
                index++;
            }
            return _redisSocket.SendExpectedInteger("RPush", args);
        }

        public string[] LRange(string key, int start = 0, int end = -1)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedArray("LRange", prefixKey, start.ToString(), end.ToString());
        }

        public string[] RPopLPush(string key, string newKey)
        {
            var prefixKey = GetPrefixKey(key);
            var prefixNewKey = GetPrefixKey(newKey);
            return _redisSocket.SendExpectedArray("RPopLPush", prefixKey, prefixNewKey);
        }

        public string[] BlPop(string[] key, int timeout)
        {
            string[] args = new string[key.Length + 1];
            int index = 0;
            foreach (var value in key)
            {
                var prefixNewKey = GetPrefixKey(value);
                args[index] = prefixNewKey;
                index++;
            }
            args[index] = timeout.ToString();
            return _redisSocket.SendExpectedArray("BlPop", args);
        }

        public string[] BrPop(string[] keys, int timeout)
        {
            string[] args = new string[keys.Length + 1];
            int index = 0;
            foreach (var value in keys)
            {
                var prefixNewKey = GetPrefixKey(value);
                args[index] = prefixNewKey;
                index++;
            }
            args[index] = timeout.ToString();
            return _redisSocket.SendExpectedArray("BrPop", args);
        }

        public string[] BrPopLPush(string key, string newKey, int timeout)
        {
            var prefixKey = GetPrefixKey(key);
            var prefixNewKey = GetPrefixKey(newKey);
            return _redisSocket.SendExpectedArray("BrPopLPush", prefixKey, prefixNewKey, timeout.ToString());
        }

        public int LRem(string key, string value, int count = 0)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("LRem", prefixKey, count.ToString(), value);
        }

        public int LLen(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("LLen", prefixKey);
        }

        public string LTrim(string key, int start = 0, int end = -1)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("LTrim", prefixKey, start.ToString(), end.ToString());
        }

        public string LPop(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("LPop", prefixKey);
        }

        public int LPushX(string key, params string[] values)
        {
            string[] args = new string[values.Length + 1];
            var prefixKey = GetPrefixKey(key);
            args[0] = prefixKey;
            int index = 1;
            foreach (var value in values)
            {
                args[index] = value;
                index++;
            }
            return _redisSocket.SendExpectedInteger("LPushX", args);
        }

        public int LInsert(string key, string value, string existValue, bool before = true)
        {
            string[] args = new string[4];
            var prefixKey = GetPrefixKey(key);
            args[0] = prefixKey;
            args[1] = before ? "BEFORE" : "AFTER ";
            args[2] = existValue;
            args[3] = value;
            return _redisSocket.SendExpectedInteger("LInsert", args);
        }

        public string RPop(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("RPop", prefixKey);
        }

        public string LSet(string key, string value, int index)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("LSet", prefixKey, index.ToString(), value);
        }

        public int LPush(string key, params string[] values)
        {
            string[] args = new string[values.Length + 1];
            var prefixKey = GetPrefixKey(key);
            args[0] = prefixKey;
            int index = 1;
            foreach (var value in values)
            {
                args[index] = value;
                index++;
            }
            return _redisSocket.SendExpectedInteger("LPush", args);
        }

        public int RPushX(string key, params string[] values)
        {
            string[] args = new string[values.Length + 1];
            var prefixKey = GetPrefixKey(key);
            args[0] = prefixKey;
            int index = 1;
            foreach (var value in values)
            {
                args[index] = value;
                index++;
            }
            return _redisSocket.SendExpectedInteger("RPushX", args);
        }

        public string HmSet(string key, Dictionary<string, string> kV)
        {
            string[] args = new string[kV.Count * 2 + 1];
            var prefixKey = GetPrefixKey(key);
            args[0] = prefixKey;
            int index = 1;
            foreach (var k in kV.Keys)
            {
                args[index] = k;
                index++;
                args[index] = kV[k];
                index++;
            }
            return _redisSocket.SendExpectedString("HmSet", args);
        }

        public string[] HmGet(string key, params string[] fields)
        {
            string[] args = new string[fields.Length + 1];
            var prefixKey = GetPrefixKey(key);
            args[0] = prefixKey;
            int index = 1;
            foreach (var field in fields)
            {
                args[index] = field;
                index++;
            }
            return _redisSocket.SendExpectedArray("HmGet", args);
        }

        public int HSet(string key, string field, string value)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("HSet", prefixKey, field, value);
        }

        public string[] HGetAll(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedArray("HGetAll", prefixKey);
        }

        public string HGet(string key, string field)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("HSet", prefixKey, field);
        }

        public int HExists(string key, string field)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("HExists", prefixKey, field);
        }

        public string HinCrBy(string key, string field, int number)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("HinCrBy", prefixKey, field, number.ToString());
        }

        public int HLen(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("HLen", prefixKey);
        }

        public int HDel(string key, params string[] fields)
        {
            var prefixKey = GetPrefixKey(key);
            string[] args = new string[fields.Length + 1];
            args[0] = prefixKey;
            int index = 1;
            foreach (var field in fields)
            {
                args[index] = field;
                index++;
            }
            return _redisSocket.SendExpectedInteger("HDel", args);
        }

        public string[] HVals(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedArray("HVals", prefixKey);
        }

        public string HinCrByFloat(string key, string field, float fraction)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("HinCrByFloat", prefixKey, field, fraction.ToString(CultureInfo.InvariantCulture));
        }

        public string[] HKeys(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedArray("HKeys", prefixKey);
        }

        public int HSetNx(string key, string field, string value)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("HKeys", prefixKey);
        }

        public string[] SUnion(params string[] keys)
        {
            var prefixKey = GetPrefixKey(keys);
            return _redisSocket.SendExpectedArray("SUnion", prefixKey);
        }

        public int SCard(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("SCard", prefixKey);
        }

        public string[] SRandMember(string key, int count)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedArray("SRandMember", prefixKey, count.ToString());
        }

        public string SRandMember(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("SRandMember", prefixKey);
        }

        public string[] SMembers(string key)
        {
            var prefixKey = GetPrefixKey(key);
            return _redisSocket.SendExpectedArray("SMembers", prefixKey);
        }

        public string[] SInter(params string[] keys)
        {
            var prefixKey = GetPrefixKey(keys);
            return _redisSocket.SendExpectedArray("SInter", prefixKey);
        }

        public int SRem(string key, params string[] member)
        {
            string[] args = new string[member.Length + 1];
            var prefixKey = GetPrefixKey(key);
            args[0] = prefixKey;
            int index = 1;
            foreach (var field in member)
            {
                args[index] = field;
                index++;
            }
            return _redisSocket.SendExpectedInteger("SRem", args);
        }

        public int SMove(string source, string destination, string moveMember)
        {
            return _redisSocket.SendExpectedInteger("SMove", source, destination, moveMember);
        }

        public int SAdd(string key, params string[] values)
        {
            var prefixKey = GetPrefixKey(key);
            string[] args = new string[values.Length + 1];
            args[0] = prefixKey;
            int index = 1;
            foreach (var field in values)
            {
                args[index] = field;
                index++;
            }
            return _redisSocket.SendExpectedInteger("SAdd", args);
        }

        public int SIsMember(string key, string value)
        {
            key = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("SAdd", key, value);
        }

        public int SDiffStore(string destination, params string[] keys)
        {
            keys = GetPrefixKey(keys);
            string[] args = new string[keys.Length + 1];
            args[0] = destination;
            int index = 1;
            foreach (var k in keys)
            {
                args[index] = k;
                index++;
            }
            return _redisSocket.SendExpectedInteger("SDiffStore", args);
        }

        public string[] SDiff(params string[] keys)
        {
            keys = GetPrefixKey(keys);
            return _redisSocket.SendExpectedArray("SDiff", keys);
        }

        public string[] SScan(string key, int cursor, string pattern, int count = 10)
        {
            key = GetPrefixKey(key);
            string[] args = new string[4];
            args[0] = key;
            args[1] = cursor.ToString();
            args[2] = pattern;
            args[3] = count.ToString();
            return _redisSocket.SendExpectedArray("SScan", args);
        }

        public string[] SInterStore(string destination, params string[] keys)
        {
            keys = GetPrefixKey(keys);
            string[] args = new string[keys.Length + 1];
            args[0] = destination;
            int index = 1;
            foreach (var k in keys)
            {
                args[index] = k;
                index++;
            }
            return _redisSocket.SendExpectedArray("SInterStore", args);
        }

        public int SUnionStore(string destination, params string[] keys)
        {
            keys = GetPrefixKey(keys);
            string[] args = new string[keys.Length + 1];
            args[0] = destination;
            int index = 1;
            foreach (var k in keys)
            {
                args[index] = k;
                index++;
            }
            return _redisSocket.SendExpectedInteger("SInterStore", args);
        }

        public string SPop(string key)
        {
            key = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("SPop", key);
        }

        public string ZRevRank(string key, string member)
        {
            key = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("ZRevRank", key, member);
        }

        public int ZLexCount(string key, string min, string max)
        {
            key = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("ZLexCount", key, min, max);
        }

        public int ZRemRangeByRank(string key, int start, int stop)
        {
            key = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("ZREMRANGEBYRANK", key, start.ToString(), stop.ToString());
        }

        public int ZCard(string key)
        {
            key = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("ZCard", key);
        }

        public int ZRem(string key, params string[] member)
        {
            key = GetPrefixKey(key);
            string[] args = new string[member.Length + 1];
            args[0] = key;
            int index = 1;
            foreach (var k in member)
            {
                args[index] = k;
                index++;
            }
            return _redisSocket.SendExpectedInteger("ZRem", args);
        }

        public int ZRank(string key, string member)
        {
            key = GetPrefixKey(key);
            return _redisSocket.SendExpectedInteger("ZRank", key, member);
        }

        public string ZIncrBy(string key, int increment, string member)
        {
            key = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("ZRank", key, increment.ToString(), member);
        }

        public string Echo(string message)
        {
            return _redisSocket.SendExpectedString("Echo", message);
        }

        public bool Select(int index)
        {
            var result = _redisSocket.SendExpectedOk("Select",index.ToString());
            if (result)
            {
                _connection.Database = index;
            }
            return result;
        }
        public bool Ping()
        {
            var resp = _redisSocket.SendExpectedString("Ping").ToUpper().Trim();
            return resp == "PONG";
        }
        public string Ping(string text)
        {
            var resp= _redisSocket.SendExpectedString("Ping", text);
            if (resp.Contains(text))
            {
                return text;
            }
            else
            {
                return String.Empty;
            }
        }
        public bool Quit()
        {
            _redisSocket.SendExpectedOk("Quit");
            return _redisSocket.Close();
        }

        public bool Auth(string password)
        {
            if (string.IsNullOrWhiteSpace(password)==false)
            {
                var auth = _redisSocket.SendExpectedOk("Auth", password);
                return !auth;
            }
            return _redisSocket.SendExpectedOk("Auth", "Aescr"); ;
        }

        public string Pause(long timeout)
        {
            return _redisSocket.SendExpectedString("Pause", timeout.ToString());
        }

        public string DebugObject(string key)
        {
            key = GetPrefixKey(key);
            return _redisSocket.SendExpectedString("DEBUG OBJECT", key);
        }

        public string FlushDb()
        {
            return _redisSocket.SendExpectedString("FlushDb");
        }

        public string Save()
        {
            return _redisSocket.SendExpectedString("Save");
        }

        public string LastSave()
        {
            return _redisSocket.SendExpectedString("LastSave");
        }

        public Dictionary<string, string> ConfigGet(string parameters)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var result = _redisSocket.SendExpectedArray("Config", "Get", parameters);
            for (int i = 0; i < result.Length;)
            {
                dic.Add(result[i], result[i + 1]);
                i = i + 2;
            }
            return dic;
        }

        public Dictionary<string, string> ConfigGet()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var result = _redisSocket.SendExpectedArray("CONFIG", "GET", "*");
            for (int i = 0; i < result.Length;)
            {
                dic.Add(result[i], result[i + 1]);
                i += 2;
            }
            return dic;
        }

        public string[] Command()
        {
            return _redisSocket.SendExpectedArray("Command");
        }
        public bool SlaveOf(string host, string password = "")
        {
            var (ip,port) = RedisSocket.SplitHost(host);
            return SlaveOf(ip,port,password);
        }
        public bool SlaveOf(string ip, int port, string password = "")
        {
            var status = ConfigGet("slaveof");
            var hostPort = status["slaveof"];
            if (hostPort == $"{ip} {port}") return true;
            var result = _redisSocket.SendExpectedOk("SLAVEOF", ip, port.ToString());
            //设置向redis主同步的密码
            if (string.IsNullOrWhiteSpace(password) == false)
            {
                ConfigSet("masterauth", password);
            }
            return result;
        }

        public bool SlaveOf()
        {
            return _redisSocket.SendExpectedOk("SLAVEOF", "NO", "ONE");
        }

        public void DebugSegfault()
        {
            _redisSocket.SendCommand("DEBUG SEGFAULT");
        }

        public string FlushAll()
        {
            return _redisSocket.SendExpectedString("FlushAll");
        }

        public int DbSize()
        {
            return _redisSocket.SendExpectedInteger("DbSize");
        }

        public string BgReWriteAof()
        {
            return _redisSocket.SendExpectedString("BGREWRITEAOF ");
        }

        public string[] ClusterSlots()
        {
            return _redisSocket.SendExpectedArray("CLUSTER SLOTS");
        }

        public bool ConfigSet(string parameter, string value)
        {
            return _redisSocket.SendExpectedOk("Config", "Set", parameter, value);
        }

        public string[] CommandInfo(params string[] commands)
        {
            string[] command=new string[commands.Length+1];
            command[0] = "INFO";
            for (int i = 1; i < commands.Length+1; i++)
            {
                command[i] = commands[i-1];
            }
            return _redisSocket.SendExpectedArray("COMMAND", command);
        }

        public string ShutDown()
        {
            return _redisSocket.SendExpectedString("SHUTDOWN");
        }

        public string Sync()
        {
            return _redisSocket.SendExpectedString("SYNC");
        }

        public void PSync()
        {
            _redisSocket.SendExpectedString("PSync");
        }

        public string ClientKill(string host, int port)
        {
            return _redisSocket.SendExpectedString("CLIENT KILL", $"{host}:{port}");
        }

        public string[] Role()
        {
            return _redisSocket.SendExpectedArray("ROLE");
        }

        public string Monitor()
        {
            return _redisSocket.SendExpectedString("MONITOR");
        }

        public string[] CommandGetKeys(params string[] parameters)
        {
            string[] temp=new string[parameters.Length+1];
            temp[0] = "GETKEYS";
            for (int i = 0; i < parameters.Length; i++)
            {
                temp[i + 1] = parameters[i];
            }
            return _redisSocket.SendExpectedArray("COMMAND", temp);
        }

        public string ClientGetName()
        {
            return _redisSocket.SendExpectedString("CLIENT", "GETNAME");
        }

        public string ConfigResetStat()
        {
            return _redisSocket.SendExpectedString("CONFIG", "RESETSTAT");
        }

        public int CommandCount()
        {
            return _redisSocket.SendExpectedInteger("COMMAND", "COUNT");
        }

        public string[] Time()
        {
            return _redisSocket.SendExpectedArray("TIME");
        }

        public string Info()
        {
            return _redisSocket.SendExpectedString("INFO");
        }

        public string Info(string section)
        {
            return _redisSocket.SendExpectedString("INFO", section);
        }

        public string ConfigRewrite()
        {
            return _redisSocket.SendExpectedString("CONFIG", "REWRITE");
        }

        public string ClientList()
        {
            return _redisSocket.SendExpectedString("CLIENT", "LIST");
        }

        public string ClientSetName(string name)
        {
            return _redisSocket.SendExpectedString("CLIENT", "SETNAME", name);
        }

        public string BgSave()
        {
            return _redisSocket.SendExpectedString("BGSAVE");
        }

        public string ScriptKill()
        {
            return _redisSocket.SendExpectedString("SCRIPT KILL");
        }

        public string ScriptLoad(string script)
        {
            return _redisSocket.SendExpectedString("SCRIPT LOAD", script);
        }

        public string[] Exec()
        {
            return _redisSocket.SendExpectedArray("Exec");
        }

        public bool Watch(params string[] keys)
        {
            return _redisSocket.SendExpectedOk("Watch",keys);
        }

        public bool Discard()
        {
            return _redisSocket.SendExpectedOk("Discard");
        }

        public bool UnWatch()
        {
            return _redisSocket.SendExpectedOk("UnWatch");
        }

        public bool Multi()
        {
            return _redisSocket.SendExpectedOk("Multi");
        }

        public bool PgMerge(string destKey, params string[] sourceKey)
        {
            string[] p=new string[sourceKey.Length+1];
            p[0] = destKey;
            for (int i = 0; i < sourceKey.Length; i++)
            {
                p[i+1] = sourceKey[i];
            }
            return _redisSocket.SendExpectedOk("PgMerge",p);
        }

        public bool PfAdd(string key, params string[] element)
        {
            key = GetPrefixKey(key);
            string[] p = new string[element.Length + 1];
            p[0] = key;
            for (int i = 0; i < element.Length; i++)
            {
                p[i + 1] = element[i];
            }
            return _redisSocket.SendExpectedInteger("PFADD", p) ==1;
        }

        public int PfCount(params string[] keys)
        {
            keys = GetPrefixKey(keys);
            return _redisSocket.SendExpectedInteger("PFCOUNT", keys);
        }
        public string Unsubscribe(params string[] channel)
        {
            return _redisSocket.SendCommand("Unsubscribe", channel);
        }
        public RedisSubscribe Subscribe(params string[] channel)
        {
            //var r= _redisSocket.SendExpectedString("Subscribe", channel);
            string conn = _connection;
            return new RedisSubscribe(conn);
        }

        public string[] PubSub(string subCommand, params string[] argument)
        {
            return _redisSocket.SendExpectedArray("PUBSUB", argument);
        }

        public string PunSubscribe(string pattern)
        {
            return _redisSocket.SendCommand("PunSubscribe", pattern);
        }

        public int Publish(string channel, string message)
        {
            return _redisSocket.SendExpectedInteger("Publish", channel, message);
        }

        public string[] PSubscribe(params string[] pattern)
        {
            return _redisSocket.SendExpectedArray("PSubscribe", pattern);
        }

        public string[] GeoHash(params string[] keys)
        {
            keys = GetPrefixKey(keys);
            return _redisSocket.SendExpectedArray("GeoHash", keys);
        }

        public string[] GeoPos(params string[] keys)
        {
            keys = GetPrefixKey(keys);
            return _redisSocket.SendExpectedArray("GeoPos", keys);
        }

        public string GeoDist(string[] keys, string unit = "km")
        {
            keys = GetPrefixKey(keys);
            throw new NotImplementedException();
        }

        public string GeoRadius(string key, decimal longitude, decimal latitude, long radius, string unit = "km",
            bool withCooRd = false, bool withDist = false, bool withHash = false, int count = -1, int sort = -1)
        {
            throw new NotImplementedException();
        }

        public string GeoAdd(string key, string member, decimal longitude, decimal latitude)
        {
            key = GetPrefixKey(key);
            throw new NotImplementedException();
        }

        public string GeoRadiusByMember(string key, string member, long radius, string unit = "km", bool withCooRd = false,
            bool withDist = false, bool withHash = false, int count = -1, int sort = -1)
        {
            key = GetPrefixKey(key);
            throw new NotImplementedException();
        }

        public bool Migrate(string[] key, string host, int port = 6379, string password = "", int db = 0, int timeout = 5000, bool copy = false,
            bool replace = true)
        {
            key = GetPrefixKey(key);
            var param = new List<string>
            {
                host,
                port.ToString(),
                password,
                db.ToString(),
                timeout.ToString()
            };
            if (copy)
            {
                param.Add("copy");
            }
            if (replace)
            {
                param.Add("replace");
            }
            param.Add("keys");
            param.AddRange(key);
            return _redisSocket.SendExpectedOk("migrate", param.ToArray()); ;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    _redisSocket?.Dispose();
                }
                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                _disposedValue = true;
            }
        }

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~RedisClient()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}