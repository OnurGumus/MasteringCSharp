using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ReactiveStreams
{
    class Program
    {


        static void Main(string[] args)
        {
            var system = ActorSystem.Create("system");
            var mat = system.Materializer();
            var inputPath = "Program.cs";
            var outputPath = "Program.min.cs";
            var files = File.ReadAllLines(inputPath);

            var stream =
                Source.From(files)
                .Async()
                .Select(line => Regex.Replace(line, @"\s|\t|\n|\r", ""))
                .Async()
                .Grouped(5)
                //.To(Sink.ForEach<IEnumerable<string>>(lines => File.AppendAllLines(outputPath, lines)))
                .ToMaterialized(Sink.AggregateAsync<IEnumerable<string>, int>(0, async (batchCount, lines) =>
                     {
                         var text = string.Join("", lines);
                         if (batchCount == 0)
                         {
                             await File.WriteAllTextAsync(outputPath, text);
                         }
                         else
                         {
                             await File.AppendAllTextAsync(outputPath, text);
                         }
                         return ++batchCount;
                     }), Keep.Right)
                     .Run(mat);

            var totalBatchCount = stream.Result;
            Console.WriteLine($"task completed with {totalBatchCount} batches.");
            Console.ReadKey();
        }
    }
}
