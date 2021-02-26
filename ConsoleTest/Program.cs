
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            JsonConvert.SerializeObject(args);
            List<long> testList=new List<long>();
           
            Console.ReadKey();
        }
    }
}