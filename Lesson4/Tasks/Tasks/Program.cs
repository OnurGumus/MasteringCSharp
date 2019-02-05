using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks
{
    class Program
    {
        static void Main(string[] args)
        {
            //Only compiles with C# 7.3+
            //Creates a background thread;
            Task.Run(PrintTime);

            var s = new CancellationTokenSource();
            var t = Task.Run(() => PrintTimeWithCancellation(s.Token), s.Token);
            s.CancelAfter(3000);
            Thread.Sleep(5000);
            Console.WriteLine(t.IsCanceled);

            //without wait the exception won't be thrown
            Task.Run(ThrowingTask).Wait();

            //since the threads are background 
            //we need below otherwise applicaiton will quit.
            Console.ReadKey();
        }

        static void PrintTime()
        {
            while (true)
            {
                Console.WriteLine(DateTime.Now);
                Thread.Sleep(1000);

            }
        }
        //in general avoid void
        static async void PrintTimeAsync()
        {
            while (true)
            {
                Console.WriteLine(DateTime.Now);
                //no threads waiting here.
                await Task.Delay(1000);
            }
        }
        static void ThrowingTask()
        {
            throw new Exception();
        }

        Task<int> RandomNumber()
        {
            //Chosen with fair dice roll!
            return Task.FromResult(4);
        }
        ValueTask<int> RandomNumberValue()
        {
            //no allocation
            return new ValueTask<int>(4);
        }

        Task<int> RandomNumber2()
        {
            var t = new TaskCompletionSource<int>();
            Task.Run(() => { Thread.Sleep(1000); t.SetResult(4); });
            //Chosen with fair dice roll!
            return t.Task;
        }
        static void PrintTimeWithCancellation(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine(DateTime.Now);
                Thread.Sleep(1000);
            }
            token.ThrowIfCancellationRequested();
        }
    }
}
