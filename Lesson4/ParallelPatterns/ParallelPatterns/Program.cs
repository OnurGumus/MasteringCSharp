using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ParallelPatterns
{
    public class Product
    {
        public decimal UnitPrice { get; set; }
        public decimal Weight { get; set; }
    }
    class ShoppingCart
    {
        public IList<Product> Items { get; } = new List<Product>();
        public decimal Total { get; set; }
    }

    class Program
    {
        static IList<ShoppingCart> PrepareShoppingCards()
        {
            var shoppingCarts = new List<ShoppingCart>();
            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var shoppingCart = new ShoppingCart();
                shoppingCarts.Add(shoppingCart);
                for (var j = 0; j < 10; j++)
                {
                    var p = new Product
                    {
                        UnitPrice = (decimal)(random.NextDouble() * 100.0),
                        Weight = (decimal)(random.NextDouble() * 50.0)
                    };
                    shoppingCart.Items.Add(p);
                }
            }
            return shoppingCarts;
        }

        static void Main(string[] args)
        {
            var shoppingCarts = PrepareShoppingCards();
            foreach (var cart in shoppingCarts)
            {
                cart.Total =
                    cart
                    .Items
                    .Select(item => item.UnitPrice * item.Weight)
                    .Sum();
            }
            // ParallelForeach(shoppingCarts);

            // Partition(shoppingCarts);
            //  PLLinqMerge(shoppingCarts);
            IO(shoppingCarts);
            Parallel.Invoke(() => { }, () => { });
        }
        static void ProcessFile(string inputPath, string outputPath)
        {
            var concurrentBag = new ConcurrentBag<string>();
            var concurrentQueue = new ConcurrentQueue<string>();
            var concurrentStack = new ConcurrentStack<string>();
            var inputLines = new BlockingCollection<string>(concurrentBag);
            var processedLines = new BlockingCollection<string>(concurrentQueue,20);

            // Stage #1     
            var readLines = Task.Run(() =>
            {
                try
                {
                    foreach (var line in File.ReadLines(inputPath))
                        inputLines.Add(line);
                }
                finally { inputLines.CompleteAdding(); }
            });

            // Stage #2     
            var processLines = Task.Run(() =>
            {
                try
                {
                    foreach (var line in inputLines.GetConsumingEnumerable().Select(line => Regex.Replace(line, @"\s+", ", ")))
                    {
                        processedLines.Add(line);
                    }
                }
                finally { processedLines.CompleteAdding(); }
            });

            // Stage #3     
            var writeLines = Task.Run(() =>
            {
                File.WriteAllLines(outputPath, processedLines.GetConsumingEnumerable());
            });

            Task.WaitAll(readLines, processLines, writeLines);
        }

        private static void TPLDataFlow(string inputPath, string outputPath)
        {
        
            var bufferBlock = new BufferBlock<string>();
            var processLines = new TransformBlock<string, string>(line => Regex.Replace(line, @"\s+", ", "));
            var batchBlock = new BatchBlock<string>(50);
            var writeLines = new ActionBlock<string[]>(async lines =>  await File.AppendAllLinesAsync(outputPath, lines));
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            bufferBlock.LinkTo(processLines, linkOptions);
            processLines.LinkTo(batchBlock, linkOptions);
            batchBlock.LinkTo(writeLines, linkOptions);
            foreach (var line in File.ReadLines(inputPath))
                bufferBlock.Post(line);
            bufferBlock.Complete();
        }
        private static void IO(IList<ShoppingCart> shoppingCarts)
        {
            var httpClient = new HttpClient();
            var carts = (from cart in shoppingCarts select httpClient.PostAsJsonAsync("http://httpbin.org/post", cart)).ToArray();
            Task.WaitAll(carts);
            foreach (var cart in carts)
                Console.WriteLine("{0}: {1}", cart.Result.StatusCode, cart.Result.Content.ReadAsStringAsync().Result);
            Console.ReadKey();
        }

        private static void PLLinqMerge(IList<ShoppingCart> shoppingCarts)
        {
            var q = from cart in shoppingCarts.AsParallel() select cart;
            Parallel.ForEach(q, item => { /* Process item. */ });
            q.ForAll(p => { });
        }

        private static void Partition(IList<ShoppingCart> shoppingCarts)
        {
            Parallel.ForEach(Partitioner.Create(0, shoppingCarts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Console.WriteLine($"{ range.Item1}-{range.Item2}");
                }
            });
        }

        private static void ParallelForeach(IList<ShoppingCart> shoppingCarts)
        {
            var cts = new CancellationTokenSource();
            var taskSchedulerPair = new ConcurrentExclusiveSchedulerPair();
            var taskFactory = new TaskFactory(taskSchedulerPair.ExclusiveScheduler);

            Task.Run(() =>
            {
                while (true)
                {
                    taskFactory.StartNew(() =>
                    {
                        shoppingCarts.Add(new ShoppingCart());
                        Thread.Sleep(1000);
                        Console.WriteLine("Added");
                    });
                    Thread.Sleep(2000);
                }
            });
            var options = new ParallelOptions { CancellationToken = cts.Token, MaxDegreeOfParallelism = 1, TaskScheduler = taskSchedulerPair.ConcurrentScheduler };
            Parallel.ForEach(shoppingCarts, options, cart =>
            {
                cart.Total = cart.Items.Select(p => p.UnitPrice * p.Weight).Sum();
                Console.WriteLine(cart.Total.ToString("C"));
            });
            Thread.Sleep(5000);
            Parallel.ForEach(shoppingCarts, options, cart =>
            {
                cart.Total = cart.Items.Select(p => p.UnitPrice * p.Weight).Sum();
                Console.WriteLine(cart.Total.ToString("C"));
            });
        }
    }
}
