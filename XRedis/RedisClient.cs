using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace XRedis
{
    public class RedisClient:ICommand
    {
        private readonly RedisSocket _redisSocket;
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool IsConnected => _redisSocket?.IsConnected ?? false;
        public RedisClient(string host, int port):this(host, port,"")
        {
        }
        public RedisClient(string host, int port,string password)
        {
            Host = host;
            Port = port;
            _redisSocket=new RedisSocket(Host,port,password);
        }
        public string Type(string key)
        {
            return _redisSocket.SendCommandString("Type", key);
        }

        public int PExpire(string key, int milliseconds)
        {
            throw new NotImplementedException();
        }

        public int PExpireAt(string key, long timestamp)
        {
            return _redisSocket.SendCommandInt("PExpireAt", key, timestamp.ToString());
        }

        public string Rename(string key, string newKey)
        {
            return _redisSocket.SendCommandString("Rename", key, newKey);
        }

        public int Persist(string key)
        {
            return _redisSocket.SendCommandInt("Persist", key);
        }

        public int Move(string key, int dbIndex)
        {
            return _redisSocket.SendCommandInt("Move", key, dbIndex.ToString());
        }

        public string RandomKey()
        {
            return _redisSocket.SendCommandString("RandomKey");
        }

        public string Dump(string key)
        {
            return _redisSocket.SendCommandString("Dump", key);
        }

        public int Ttl(string key)
        {
            return _redisSocket.SendCommandInt("Ttl", key);
        }

        public int Expire(string key, int second)
        {
            return _redisSocket.SendCommandInt("Expire", key, second.ToString());
        }

        public int Del(string key)
        {
            return _redisSocket.SendCommandInt("Del", key);
        }

        public int PTtl(string key)
        {
            return _redisSocket.SendCommandInt("PTtl", key);
        }

        public int RenameNx(string key, string newKey)
        {
            return _redisSocket.SendCommandInt("RenameNx", key, newKey);
        }

        public int Exists(string key)
        {
            return _redisSocket.SendCommandInt("Exists", key);
        }

        public int ExpireAt(string key, long timestamp)
        {
            return _redisSocket.SendCommandInt("ExpireAt", key, timestamp.ToString());
        }

        public string[] Keys(string pattern)
        {
            return _redisSocket.SendCommandArray("Keys", pattern);
        }

        public int SetNx(string key, string value)
        {
            return _redisSocket.SendCommandInt("SetNx", key, value);
        }

        public string GetRange(string key, int start = 0, int end = -1)
        {
            return _redisSocket.SendCommandString("GetRange", key, start.ToString(),end.ToString());
        }

        public string MSet(Dictionary<string, string> kv)
        {
            string[] result=new string[kv.Count*2];
            int index = 0;
            foreach (string key in kv.Keys)
            {
                result[index] = key;
                result[index+1] = kv[key];
                index=index+2;
            }
            return _redisSocket.SendCommandString("MSet",  result);
        }

        public string SetEx(string key, string value, int timeout)
        {
            return _redisSocket.SendCommandString("SetEx", timeout.ToString(), value);
        }

        public string Set(string key, string value)
        {
            return _redisSocket.SendCommandString("Set", key, value);
        }

        public string Get(string key)
        {
            return _redisSocket.SendCommandString("Get", key);
        }

        public int GetBit(string key, int offset)
        {
            return _redisSocket.SendCommandInt("GetBit", key,offset.ToString());
        }

        public int SetBit(string key, int offset, int value)
        {
            return _redisSocket.SendCommandInt("SetBit", key, offset.ToString(),value.ToString());
        }

        public string Decr(string key)
        {
            return _redisSocket.SendCommandString("Decr", key);
        }

        public string DecrBy(string key, int num)
        {
            return _redisSocket.SendCommandString("DecrBy", key, num.ToString());
        }

        public int StrLen(string key)
        {
            return _redisSocket.SendCommandInt("StrLen", key);
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
            return _redisSocket.SendCommandInt("MSetNx", result);
        }

        public string IncrBy(string key, int num)
        {
            return _redisSocket.SendCommandString("IncrBy", key,num.ToString());
        }

        public string IncrByFloat(string key, float fraction)
        {
            return _redisSocket.SendCommandString("IncrByFloat", key, fraction.ToString(CultureInfo.InvariantCulture));
        }

        public int SetRange(string key, int offset, string value)
        {
            return _redisSocket.SendCommandInt("SetRange", key, offset.ToString(), value);
        }

        public string PSetEx(string key, string value, int milliseconds)
        {
            return _redisSocket.SendCommandString("PSetEx", key, milliseconds.ToString(), value);
        }

        public int Append(string key, string value)
        {
            return _redisSocket.SendCommandInt("Append", key, value);
        }

        public string GetSet(string key, string value)
        {
            return _redisSocket.SendCommandString("GetSet", key, value);
        }

        public string[] MGet(params string[] keys)
        {
            return _redisSocket.SendCommandArray("MGet", keys);
        }

        public string Incr(string key)
        {
            return _redisSocket.SendCommandString("Incr", key);
        }

        public string LIndex(string key, int index)
        {
            return _redisSocket.SendCommandString("LIndex", key,index.ToString());
        }

        public int RPush(string key, params string[] values)
        {
            string[] args=new string[values.Length+1];
            args[0] = key;
            int index = 1;
            foreach (var value in values)
            {
                args[index] = value;
                index++;
            }
            return _redisSocket.SendCommandInt("RPush", args);
        }

        public string[] LRange(string key, int start = 0, int end = -1)
        {
            return _redisSocket.SendCommandArray("LRange", key, start.ToString(),end.ToString());
        }

        public string[] RPopLPush(string key, string newKey)
        {
            return _redisSocket.SendCommandArray("RPopLPush", key, newKey);
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
            return _redisSocket.SendCommandArray("BlPop", args);
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
            return _redisSocket.SendCommandArray("BrPop", args);
        }

        public string[] BrPopLPush(string key, string newKey, int timeout)
        {
            return _redisSocket.SendCommandArray("BrPopLPush", key,newKey,timeout.ToString());
        }

        public int LRem(string key, string value, int count = 0)
        {
            return _redisSocket.SendCommandInt("LRem", key, count.ToString(), value);
        }

        public int LLen(string key)
        {
            return _redisSocket.SendCommandInt("LLen", key);
        }

        public string LTrim(string key, int start = 0, int end = -1)
        {
            return _redisSocket.SendCommandString("LTrim", key, start.ToString(),end.ToString());
        }

        public string LPop(string key)
        {
            return _redisSocket.SendCommandString("LPop", key);
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
            return _redisSocket.SendCommandInt("LPushX", args);
        }

        public int LInsert(string key, string value, string existValue, bool before = true)
        {
            string[] args = new string[4];
            args[0] = key;
            args[1] = before? "BEFORE": "AFTER ";
            args[2] = existValue;
            args[3] = value;
            return _redisSocket.SendCommandInt("LInsert", args);
        }

        public string RPop(string key)
        {
            return _redisSocket.SendCommandString("RPop", key);
        }

        public string LSet(string key, string value, int index)
        {
            return _redisSocket.SendCommandString("LSet", key,index.ToString(),value);
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
            return _redisSocket.SendCommandInt("LPush", args);
        }

        public int RPushX(string key, params string[] values)
        {
            throw new NotImplementedException();
        }

        public string HmSet(string key, Dictionary<string, string> kV)
        {
            throw new NotImplementedException();
        }

        public string[] HmGet(string key, params string[] fields)
        {
            throw new NotImplementedException();
        }

        public int HSet(string key, string field, string value)
        {
            throw new NotImplementedException();
        }

        public string[] HGetAll(string key)
        {
            throw new NotImplementedException();
        }

        public string HGet(string key, string field)
        {
            throw new NotImplementedException();
        }

        public int HExists(string key, string field)
        {
            throw new NotImplementedException();
        }

        public string HinCrBy(string key, string field, int number)
        {
            throw new NotImplementedException();
        }

        public int HLen(string key)
        {
            throw new NotImplementedException();
        }

        public int HDel(string key, params string[] fields)
        {
            throw new NotImplementedException();
        }

        public string[] HVals(string key)
        {
            throw new NotImplementedException();
        }

        public string HinCrByFloat(string key, string field, float fraction)
        {
            throw new NotImplementedException();
        }

        public string[] HKeys(string key)
        {
            throw new NotImplementedException();
        }

        public int HSetNx(string key, string field, string value)
        {
            throw new NotImplementedException();
        }

        public string[] SUnion(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public int SCard(string key)
        {
            throw new NotImplementedException();
        }

        public string[] SRandMember(string key, int count = 1)
        {
            throw new NotImplementedException();
        }

        public string[] SMembers(string key)
        {
            throw new NotImplementedException();
        }

        public string[] SInter(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public int SRem(string key, params string[] member)
        {
            throw new NotImplementedException();
        }

        public int SMove(string source, string destination, string moveMember)
        {
            throw new NotImplementedException();
        }

        public int SAdd(string kye, params string[] values)
        {
            throw new NotImplementedException();
        }

        public int SIsMember(string key, string value)
        {
            throw new NotImplementedException();
        }

        public int SDiffStore(string destination, params string[] keys)
        {
            throw new NotImplementedException();
        }

        public string[] SDiff(params string[] keys)
        {
            throw new NotImplementedException();
        }

        public string[] SScan(string key, int cursor, string pattern, int count = 10)
        {
            throw new NotImplementedException();
        }

        public string[] SInterStore(string destination, params string[] keys)
        {
            throw new NotImplementedException();
        }

        public int SUnionStore(string destination, params string[] keys)
        {
            throw new NotImplementedException();
        }

        public string SPop(string key)
        {
            throw new NotImplementedException();
        }

        public string ZRevRank(string key, string member)
        {
            throw new NotImplementedException();
        }

        public int ZLexCount(string key, string min, string max)
        {
            throw new NotImplementedException();
        }

        public int ZRemRangeByRank(string key, int start, int stop)
        {
            throw new NotImplementedException();
        }

        public int ZCard(string key)
        {
            throw new NotImplementedException();
        }

        public int ZRem(string key, params string[] member)
        {
            throw new NotImplementedException();
        }

        public int ZRank(string key, string member)
        {
            throw new NotImplementedException();
        }

        public string ZIncrBy(string key, int increment, string member)
        {
            throw new NotImplementedException();
        }

        public string Echo(string message)
        {
            throw new NotImplementedException();
        }

        public string Select(int index)
        {
            return _redisSocket.SendCommandString("Select",index.ToString());
        }

        public string Ping()
        {
            throw new NotImplementedException();
        }

        public string Quit()
        {
            throw new NotImplementedException();
        }

        public string Auth(string password)
        {
            throw new NotImplementedException();
        }

        public string Pause(int timeout)
        {
            throw new NotImplementedException();
        }

        public string DebugObject(string key)
        {
            throw new NotImplementedException();
        }

        public string FlushDb()
        {
            throw new NotImplementedException();
        }

        public string Save()
        {
            throw new NotImplementedException();
        }

        public string ShowLog(params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public string LastSave()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> ConfigGet(string parameters)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> ConfigGet()
        {
            throw new NotImplementedException();
        }

        public string[] Command()
        {
            throw new NotImplementedException();
        }

        public string SlaveOf(string host, int port)
        {
            throw new NotImplementedException();
        }

        public string SlaveOf()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            if (disposing)
            {
                _redisSocket?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
