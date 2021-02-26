using RedisClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XLibrary.Random;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<long> testList=new List<long>();
            while (true)
            {
               
                Task.Run((() =>
                {
                   
                    var x=new int[10000].AsParallel();
                    x.ForAll(c =>
                    {
                        Snowflake snowflake = new Snowflake();
                        long t = snowflake.GetId();
                        Console.WriteLine(t);
                        if (testList.Exists(x => x.Equals(t)))
                        {
                            throw new Exception();
                        }
                        testList.Add(t);
                    });
                 
                }));
             
            }
            Console.ReadKey();
        }
    }
}