using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DistributionService
{
    public static class Nuran
    {
        private static readonly object balanceLock = new object();

        public  static bool  IsLocked = true;
        public static void Do()
        {

           
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(2000);
                    Console.WriteLine(i);
                }

                IsLocked = true;
                             
        }
    }
}
