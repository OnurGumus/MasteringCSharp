using System;
using System.Threading;
using System.Threading.Tasks;
namespace Volatile
{
    class Program
    {
        static int x, y, a, b;
        static void Main()
        {
            MemoryBarrierTest();
        }
       


        static void MemoryBarrierTest()
        {
            while (true)
            {
                var t1 = Task.Run(MemoryBarrierTest1);
                var t2 = Task.Run(MemoryBarrierTest2);
                Task.WaitAll(t1, t2);
                if (a == 0 && b == 0)
                {
                    Console.WriteLine("{0}, {1}", a, b);
                }
                x = y = a = b = 0;
            }
        }

        static void MemoryBarrierTest1()
        {
            x = 1;
            a = y;
            //read 1
            //write to x
            //read y
            //write to a
        }

        static void MemoryBarrierTest2()
        {
            y = 2;
            b = x;
        }
    }
}
