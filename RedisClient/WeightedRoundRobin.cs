using System;
using System.Collections.Generic;
using System.Linq;

namespace RedisClient
{
    /// <summary>
    /// 服务器结构
    /// </summary>
    public class Server
    {
        public string IP;
        public int Weight;
    }
    /// <summary>
    /// 权重轮询算法
    /// </summary>
    public  class WeightedRoundRobin
    {
        public WeightedRoundRobin()
        {
           
        }
        public WeightedRoundRobin(List<Server> server)
        {
            s = server.OrderBy(a => a.Weight).ToList(); ;
        }
        public void Load(List<Server> server)
        {
            s = server.OrderBy(a => a.Weight).ToList(); ;
        }
        private  List<Server> s;

        private  int i = -1; //代表上一次选择的服务器
        private  int gcd => GetGcd(s); //表示集合S中所有服务器权值的最大公约数
        private  int cw = 0; //当前调度的权值
        private  int max => GetMaxWeight(s);
        private  int n => s.Count; //服务器个数


        /**
         * 算法流程：
         * 假设有一组服务器 S = {S0, S1, …, Sn-1} ，有相应的权重，变量I表示上次选择的服务器，1每次步长
         * 权值cw初始化为0，i初始化为-1 ，当第一次的时候 权值取最大的那个服务器，
         * 通过权重的不断递减 寻找 适合的服务器返回，直到轮询结束，权值返回为0
         */
        public  Server GetServer()
        {
            while (true)
            {
                i = (i + 1) % n;
                if (i == 0)
                {
                    cw = cw - gcd;
                    if (cw <= 0)
                    {
                        cw = max;
                        if (cw == 0)
                            return null;
                    }
                }

                if (s[i].Weight >= cw)
                {
                    return s[i];
                }
            }
        }


        /// <summary>
        /// 获取服务器所有权值的最大公约数
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        private  int GetGcd(List<Server> servers)
        {
            return 1;
        }

        /// <summary>
        /// 获取最大的权值
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        private  int GetMaxWeight(List<Server> servers)
        {
            int max = 0;
            foreach (var s in servers)
            {
                if (s.Weight > max)
                    max = s.Weight;
            }

            return max;
        }

        public void TestWeightedRoundRobin()
        {
            WeightedRoundRobin weighted = new WeightedRoundRobin(new List<Server>()
            {
                new Server(){IP = "192.168.0.1",Weight = 10},
                new Server(){IP = "192.168.0.2",Weight = 5},
                new Server(){IP = "192.168.0.3",Weight = 50},
                new Server(){IP = "192.168.0.4",Weight = 3},
                new Server(){IP = "192.168.0.5",Weight = 2},
                new Server(){IP = "192.168.0.6",Weight = 1},
            });
            Dictionary<string, int> dic = new Dictionary<string, int>();
            Server s;
            for (int j = 0; j < 1000; j++)
            {
                s = weighted.GetServer();
                Console.WriteLine("{0},weight:{1}", s.IP, s.Weight);

                if (!dic.ContainsKey("服务器" + s.IP + ",权重:" + s.Weight))
                    dic.Add("服务器" + s.IP + ",权重:" + s.Weight, 0);
                dic["服务器" + s.IP + ",权重:" + s.Weight]++;
            }

            foreach (var i1 in dic)
            {
                Console.WriteLine("{0}共处理请求{1}次", i1.Key, i1.Value);
            }
        }
    }

   
   
}