using System;
using System.Collections;
using System.Collections.Generic;
using Aescr.Redis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace TestBenchmarkDotNet
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    public class DicVsHash
    {
        private Dictionary<int, RedisClient> _dictionary = new Dictionary<int, RedisClient>();
        private Hashtable _hashtable = new Hashtable();
        private Random _random;
        [Params(1000, 10000)]
        public int N;
        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < N; i++)
            {
                _dictionary.Add(i,new RedisClient());
                _hashtable.Add(i,new RedisClient());
            }

            _random = new Random();
        }
        [Benchmark]
        public RedisClient GetRedisClientByDictionary()
        {
            var rNext = _random.Next(0,N);
            return _hashtable[rNext] as RedisClient;
        }
        [Benchmark]
        public RedisClient GetRedisClientByHashtable()
        {
            var rNext = _random.Next(0,N);
            return _dictionary[rNext];
        }
    }
    
}