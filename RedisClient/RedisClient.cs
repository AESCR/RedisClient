using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RedisClient
{
    public class RedisClient: IRedisClient
    {
        private readonly Random _random = new Random();
        public bool IsConnected => _redisSocket?.IsConnected ?? false;
        private readonly RedisSocket _redisSocket;
        public string Host=>_redisSocket.Host;
        public int Port => _redisSocket.Port;
        public string Password => _redisSocket.Password;
        public string HostPort => $"{Host}:{Port}";
        private readonly List<RedisClient> _slaveClient = new List<RedisClient>();
        public RedisClient GetRandomSlaveClient()
        {
            var index= _random.Next(0, _slaveClient.Count);
            return _slaveClient[index];
        }
        public void AddSlave(string host, int port= 6379, string password="")
        {
           var exits=  _slaveClient.Exists(x => x.HostPort == $"{host}:{port}");
           if (exits != false) return;
           var slave = new RedisClient(host, port, password);
            _slaveClient.Add(slave);
            var slaveOf= slave.SlaveOf(Host, Port);
            Console.WriteLine($"SlaveOf:{HostPort}----{slaveOf}");
        }
        private bool _disposedValue;

        public RedisClient(string host, string password) : this(host, 6379, password)
        {
        }
        public RedisClient(string host) : this(host, 6379, "")
        {
        }
        public RedisClient(string host, int port) : this(host, port, "")
        {
        }
        public RedisClient(string host, int port, string password)
        {
            _redisSocket = new RedisSocket(host, port, password);
        }

        public object SendCommand(string cmd, params string[] args)
        {
            return _redisSocket.SendCommand(cmd, args);
        }
        public string Type(string key)
        {
            return _redisSocket.SendExpectedString("Type", key);
        }

        public int PExpire(string key, long milliseconds)
        {
            return _redisSocket.SendExpectedInteger("PExpire", key, milliseconds.ToString());
        }

        public int PExpireAt(string key, long timestamp)
        {
            return _redisSocket.SendExpectedInteger("PExpireAt", key, timestamp.ToString());
        }

        public string Rename(string key, string newKey)
        {
            return _redisSocket.SendExpectedString("Rename", key, newKey);
        }

        public int Persist(string key)
        {
            return _redisSocket.SendExpectedInteger("Persist", key);
        }

        public int Move(string key, int dbIndex)
        {
            return _redisSocket.SendExpectedInteger("Move", key, dbIndex.ToString());
        }

        public string RandomKey()
        {
            return _redisSocket.SendExpectedString("RandomKey");
        }

        public string Dump(string key)
        {
            return _redisSocket.SendExpectedString("Dump", key);
        }

        public int Ttl(string key)
        {
            return _redisSocket.SendExpectedInteger("Ttl", key);
        }

        public int Expire(string key, int second)
        {
            return _redisSocket.SendExpectedInteger("Expire", key, second.ToString());
        }

        public int Del(string key)
        {
            return _redisSocket.SendExpectedInteger("Del", key);
        }

        public int PTtl(string key)
        {
            return _redisSocket.SendExpectedInteger("PTtl", key);
        }

        public int RenameNx(string key, string newKey)
        {
            return _redisSocket.SendExpectedInteger("RenameNx", key, newKey);
        }

        public int Exists(string key)
        {
            return _redisSocket.SendExpectedInteger("Exists", key);
        }

        public int ExpireAt(string key, long timestamp)
        {
            return _redisSocket.SendExpectedInteger("ExpireAt", key, timestamp.ToString());
        }

        public string[] Keys(string pattern)
        {
            return _redisSocket.SendExpectedArray("Keys", pattern);
        }

        public int SetNx(string key, string value)
        {
            return _redisSocket.SendExpectedInteger("SetNx", key, value);
        }

        public string GetRange(string key, int start = 0, int end = -1)
        {
            return _redisSocket.SendExpectedString("GetRange", key, start.ToString(), end.ToString());
        }

        public string MSet(Dictionary<string, string> kv)
        {
            string[] result = new string[kv.Count * 2];
            int index = 0;
            foreach (string key in kv.Keys)
            {
                result[index] = key;
                result[index + 1] = kv[key];
                index = index + 2;
            }
            return _redisSocket.SendExpectedString("MSet", result);
        }

        public string SetEx(string key, string value, int timeout)
        {
            return _redisSocket.SendExpectedString("SetEx", timeout.ToString(), value);
        }

        public string Set(string key, string value)
        {
            return _redisSocket.SendExpectedString("Set", key, value);
        }

        public string Get(string key)
        {
            return _redisSocket.SendExpectedString("Get", key);
        }

        public int GetBit(string key, int offset)
        {
            return _redisSocket.SendExpectedInteger("GetBit", key, offset.ToString());
        }

        public int SetBit(string key, int offset, int value)
        {
            return _redisSocket.SendExpectedInteger("SetBit", key, offset.ToString(), value.ToString());
        }

        public string Decr(string key)
        {
            return _redisSocket.SendExpectedString("Decr", key);
        }

        public string DecrBy(string key, int num)
        {
            return _redisSocket.SendExpectedString("DecrBy", key, num.ToString());
        }

        public int StrLen(string key)
        {
            return _redisSocket.SendExpectedInteger("StrLen", key);
        }

        public int MSetNx(Dictionary<string, string> kv)
        {
            string[] result = new string[kv.Count * 2];
            int index = 0;
            foreach (string key in kv.Keys)
            {
                result[index] = key;
                result[index + 1] = kv[key];
                index = index + 2;
            }
            return _redisSocket.SendExpectedInteger("MSetNx", result);
        }

        public string IncrBy(string key, int num)
        {
            return _redisSocket.SendExpectedString("IncrBy", key, num.ToString());
        }

        public string IncrByFloat(string key, float fraction)
        {
            return _redisSocket.SendExpectedString("IncrByFloat", key, fraction.ToString(CultureInfo.InvariantCulture));
        }

        public int SetRange(string key, int offset, string value)
        {
            return _redisSocket.SendExpectedInteger("SetRange", key, offset.ToString(), value);
        }

        public string PSetEx(string key, string value, int milliseconds)
        {
            return _redisSocket.SendExpectedString("PSetEx", key, milliseconds.ToString(), value);
        }

        public int Append(string key, string value)
        {
            return _redisSocket.SendExpectedInteger("Append", key, value);
        }

        public string GetSet(string key, string value)
        {
            return _redisSocket.SendExpectedString("GetSet", key, value);
        }

        public string[] MGet(params string[] keys)
        {
            return _redisSocket.SendExpectedArray("MGet", keys);
        }

        public string Incr(string key)
        {
            return _redisSocket.SendExpectedString("Incr", key);
        }

        public string LIndex(string key, int index)
        {
            return _redisSocket.SendExpectedString("LIndex", key, index.ToString());
        }

        public int RPush(string key, params string[] values)
        {
            string[] args = new string[values.Length + 1];
            args[0] = key;
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
            return _redisSocket.SendExpectedArray("LRange", key, start.ToString(), end.ToString());
        }

        public string[] RPopLPush(string key, string newKey)
        {
            return _redisSocket.SendExpectedArray("RPopLPush", key, newKey);
        }

        public string[] BlPop(string[] key, int timeout)
        {
            string[] args = new string[key.Length + 1];
            int index = 0;
            foreach (var value in key)
            {
                args[index] = value;
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
                args[index] = value;
                index++;
            }
            args[index] = timeout.ToString();
            return _redisSocket.SendExpectedArray("BrPop", args);
        }

        public string[] BrPopLPush(string key, string newKey, int timeout)
        {
            return _redisSocket.SendExpectedArray("BrPopLPush", key, newKey, timeout.ToString());
        }

        public int LRem(string key, string value, int count = 0)
        {
            return _redisSocket.SendExpectedInteger("LRem", key, count.ToString(), value);
        }

        public int LLen(string key)
        {
            return _redisSocket.SendExpectedInteger("LLen", key);
        }

        public string LTrim(string key, int start = 0, int end = -1)
        {
            return _redisSocket.SendExpectedString("LTrim", key, start.ToString(), end.ToString());
        }

        public string LPop(string key)
        {
            return _redisSocket.SendExpectedString("LPop", key);
        }

        public int LPushX(string key, params string[] values)
        {
            string[] args = new string[values.Length + 1];
            args[0] = key;
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
            args[0] = key;
            args[1] = before ? "BEFORE" : "AFTER ";
            args[2] = existValue;
            args[3] = value;
            return _redisSocket.SendExpectedInteger("LInsert", args);
        }

        public string RPop(string key)
        {
            return _redisSocket.SendExpectedString("RPop", key);
        }

        public string LSet(string key, string value, int index)
        {
            return _redisSocket.SendExpectedString("LSet", key, index.ToString(), value);
        }

        public int LPush(string key, params string[] values)
        {
            string[] args = new string[values.Length + 1];
            args[0] = key;
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
            args[0] = key;
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
            args[0] = key;
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
            args[0] = key;
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
            return _redisSocket.SendExpectedInteger("HSet", key, field, value);
        }

        public string[] HGetAll(string key)
        {
            return _redisSocket.SendExpectedArray("HGetAll", key);
        }

        public string HGet(string key, string field)
        {
            return _redisSocket.SendExpectedString("HSet", key, field);
        }

        public int HExists(string key, string field)
        {
            return _redisSocket.SendExpectedInteger("HExists", key, field);
        }

        public string HinCrBy(string key, string field, int number)
        {
            return _redisSocket.SendExpectedString("HinCrBy", key, field, number.ToString());
        }

        public int HLen(string key)
        {
            return _redisSocket.SendExpectedInteger("HLen", key);
        }

        public int HDel(string key, params string[] fields)
        {
            string[] args = new string[fields.Length + 1];
            args[0] = key;
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
            return _redisSocket.SendExpectedArray("HVals", key);
        }

        public string HinCrByFloat(string key, string field, float fraction)
        {
            return _redisSocket.SendExpectedString("HinCrByFloat", key, field, fraction.ToString(CultureInfo.InvariantCulture));
        }

        public string[] HKeys(string key)
        {
            return _redisSocket.SendExpectedArray("HKeys", key);
        }

        public int HSetNx(string key, string field, string value)
        {
            return _redisSocket.SendExpectedInteger("HKeys", key);
        }

        public string[] SUnion(params string[] keys)
        {
            return _redisSocket.SendExpectedArray("SUnion", keys);
        }

        public int SCard(string key)
        {
            return _redisSocket.SendExpectedInteger("SCard", key);
        }

        public string[] SRandMember(string key, int count)
        {
            return _redisSocket.SendExpectedArray("SRandMember", key, count.ToString());
        }
        public string SRandMember(string key)
        {
            return _redisSocket.SendExpectedString("SRandMember", key);
        }
        public string[] SMembers(string key)
        {
            return _redisSocket.SendExpectedArray("SMembers", key);
        }

        public string[] SInter(params string[] keys)
        {
            return _redisSocket.SendExpectedArray("SInter", keys);
        }

        public int SRem(string key, params string[] member)
        {
            string[] args = new string[member.Length + 1];
            args[0] = key;
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
            string[] args = new string[values.Length + 1];
            args[0] = key;
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
            return _redisSocket.SendExpectedInteger("SAdd", key, value);
        }

        public int SDiffStore(string destination, params string[] keys)
        {
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
            return _redisSocket.SendExpectedArray("SDiff", keys);
        }

        public string[] SScan(string key, int cursor, string pattern, int count = 10)
        {
            string[] args = new string[4];
            args[0] = key;
            args[1] = cursor.ToString();
            args[2] = pattern;
            args[3] = count.ToString();
            return _redisSocket.SendExpectedArray("SScan", args);
        }

        public string[] SInterStore(string destination, params string[] keys)
        {
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
            return _redisSocket.SendExpectedString("SPop", key);
        }

        public string ZRevRank(string key, string member)
        {
            return _redisSocket.SendExpectedString("ZRevRank", key, member);
        }

        public int ZLexCount(string key, string min, string max)
        {
            return _redisSocket.SendExpectedInteger("ZLexCount", key, min, max);
        }

        public int ZRemRangeByRank(string key, int start, int stop)
        {
            return _redisSocket.SendExpectedInteger("ZREMRANGEBYRANK", key, start.ToString(), stop.ToString());
        }

        public int ZCard(string key)
        {
            return _redisSocket.SendExpectedInteger("ZCard", key);
        }

        public int ZRem(string key, params string[] member)
        {
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
            return _redisSocket.SendExpectedInteger("ZRank", key, member);
        }

        public string ZIncrBy(string key, int increment, string member)
        {
            return _redisSocket.SendExpectedString("ZRank", key, increment.ToString(), member);
        }

        public string Echo(string message)
        {
            return _redisSocket.SendExpectedString("Echo", message);
        }

        public string Select(int index)
        {
            return _redisSocket.SendExpectedString("Select", index.ToString());
        }

        public string Ping()
        {
            return _redisSocket.SendExpectedString("Ping");
        }

        public string Quit()
        {
            return _redisSocket.SendExpectedString("Quit");
        }

        public string Auth(string password)
        {
            return _redisSocket.SendExpectedString("Auth", password);
        }

        public string Pause(long timeout)
        {
            return _redisSocket.SendExpectedString("Pause", timeout.ToString());
        }

        public string DebugObject(string key)
        {
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
            var result = _redisSocket.SendExpectedArray("Config Get", parameters);
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
            var result = _redisSocket.SendExpectedArray("Config Get");
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

        public string SlaveOf(string host, int port)
        {
            return _redisSocket.SendExpectedString("SLAVEOF", host, port.ToString());
        }

        public string SlaveOf()
        {
            return _redisSocket.SendExpectedString("SLAVEOF", "NO", "ONE");
        }

        public void DebugSegfault()
        {
            throw new NotImplementedException();
        }

        public string FlushAll()
        {
            throw new NotImplementedException();
        }

        public int DbSize()
        {
            throw new NotImplementedException();
        }

        public string BgReWriteAof()
        {
            throw new NotImplementedException();
        }

        public string[] ClusterSlots()
        {
            throw new NotImplementedException();
        }

        public string ConfigSet(string parameter, string value)
        {
            throw new NotImplementedException();
        }

        public string[] CommandInfo(params string[] commands)
        {
            throw new NotImplementedException();
        }

        public string ShutDown()
        {
            throw new NotImplementedException();
        }

        public string Sync()
        {
            return _redisSocket.SendExpectedString("Sync");
        }

        public string ClientKill(string host, int port)
        {
            throw new NotImplementedException();
        }

        public string[] Role()
        {
            throw new NotImplementedException();
        }

        public string Monitor()
        {
            throw new NotImplementedException();
        }

        public string[] CommandGetKeys(params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public string ClientGetName()
        {
            throw new NotImplementedException();
        }

        public string ConfigResetStat()
        {
            throw new NotImplementedException();
        }

        public int CommandCount()
        {
            throw new NotImplementedException();
        }

        public string[] Time()
        {
            throw new NotImplementedException();
        }

        public string[] Info()
        {
            throw new NotImplementedException();
        }

        public string[] Info(string section)
        {
            throw new NotImplementedException();
        }

        public string ConfigRewrite()
        {
            throw new NotImplementedException();
        }

        public string ClientList()
        {
            throw new NotImplementedException();
        }

        public string ClientSetName(string name)
        {
            throw new NotImplementedException();
        }

        public string BgSave()
        {
            throw new NotImplementedException();
        }

        public string ScriptKill()
        {
            throw new NotImplementedException();
        }

        public string ScriptLoad(string script)
        {
            throw new NotImplementedException();
        }

        public string[] Eval(string script, int numkeys, string[] keys, string[] args = null)
        {
            throw new NotImplementedException();
        }

        public string[] EvalSha(string sha1, int numkeys, string[] keys, string[] args = null)
        {
            throw new NotImplementedException();
        }

        public string[] ScriptExists(string sha1, int numkeys, string[] keys, string[] args = null)
        {
            throw new NotImplementedException();
        }

        public string ScriptFlush()
        {
            throw new NotImplementedException();
        }

        public string[] Exec()
        {
            throw new NotImplementedException();
        }

        public string Watch(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public string Discard()
        {
            throw new NotImplementedException();
        }

        public string UnWatch()
        {
            throw new NotImplementedException();
        }

        public string Multi()
        {
            throw new NotImplementedException();
        }

        public string PgMerge(string destKey, params string[] sourceKey)
        {
            throw new NotImplementedException();
        }

        public int PfAdd(string key, params string[] element)
        {
            throw new NotImplementedException();
        }

        public int PfCount(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public string Unsubscribe(params string[] channel)
        {
            throw new NotImplementedException();
        }

        public string Subscribe(params string[] channel)
        {
            throw new NotImplementedException();
        }

        public string PubSub(string subCommand, params string[] argument)
        {
            throw new NotImplementedException();
        }

        public string PunSubscribe(string pattern)
        {
            throw new NotImplementedException();
        }

        public int Publish(string channel, string message)
        {
            throw new NotImplementedException();
        }

        public string PSubscribe(params string[] pattern)
        {
            throw new NotImplementedException();
        }

        public string[] GeoHash(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public string[] GeoPos(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public string GeoDist(string[] keys, string unit = "km")
        {
            throw new NotImplementedException();
        }

        public string GeoRadius(string key, decimal longitude, decimal latitude, long radius, string unit = "km",
            bool withCooRd = false, bool withDist = false, bool withHash = false, int count = -1, int sort = -1)
        {
            throw new NotImplementedException();
        }

        public string GeoAdd(string key, string member, decimal longitude, decimal latitude)
        {
            throw new NotImplementedException();
        }

        public string GeoRadiusByMember(string key, string member, long radius, string unit = "km", bool withCooRd = false,
            bool withDist = false, bool withHash = false, int count = -1, int sort = -1)
        {
            throw new NotImplementedException();
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
                for (int i = 0; i < _slaveClient.Count; i++)
                {
                    var slave = _slaveClient[i];
                    slave.Dispose(disposing);
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
