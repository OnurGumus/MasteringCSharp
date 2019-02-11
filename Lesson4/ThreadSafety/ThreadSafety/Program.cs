using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSafety
{
    class Program
    {
        static Random r = new Random();
        static object locker = new object();
        static object locker2 = new object();
        static void Main(string[] args)
        {

            Task.Run(Loop1);
            Task.Run(Loop2);
            Console.ReadKey();
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        static void Loop1()
        {
            while (true)
            {
                int x = 0;
            //    lock (locker)
                {
                    x = r.Next();
                    if (x == 0)
                    {
                        Console.WriteLine(x);
                    }
                }

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        static void Loop2()
        {
            while (true)
            {
                int x = 0;
              //  lock (locker)
                {
                    x = r.Next();
                    if(x == 0)
                    {
                        Console.WriteLine(x);
                    }
                }
            }
        }

    }
}
