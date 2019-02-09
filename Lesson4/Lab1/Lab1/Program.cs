using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1
{
    [SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 10)]
    public class Test
    {
        static int[] items;

        [GlobalSetup]
        public void GlobalSetup()
        {
            items = Enumerable.Range(1, 1000000).Reverse().ToArray();
        }
        [Benchmark]
        public static void QuickSortBenchmark()
        {
            var currentItems = items;
            Test.Quicksort(items, 0, items.Length - 1);
        }
        [Benchmark]
        public static void QuickSortParallelBenchmark()
        {
            var currentItems = items;
            Test.QuicksortParallel(items, 0, items.Length - 1);
        }
        [Benchmark(Baseline = true)]
        public static void ArraySortBenchmark()
        {
            var currentItems = items;
            Array.Sort(currentItems);
        }
        private static void Swap<T>(T[] arr, int i, int j)
        {
            var tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static int Partition<T>(T[] arr, int low, int high)
    where T : IComparable<T>
        {
            var pivotPos = (high + low) / 2;
            var pivot = arr[pivotPos];
            Swap(arr, low, pivotPos);

            var left = low;

            var span = arr.AsSpan(low+1, high - low);
            for (var i = 0 ; i < span.Length; i++)
            {
                if (span[i].CompareTo(pivot) >= 0) continue;
                left++;
                Swap(arr, i+low+1, left);
            }

            Swap(arr, low, left);
            return left;
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static void Quicksort<T>(T[] arr, int left, int right) where T : IComparable<T>
        {
            if (right <= left) return;
            var pivot = Partition(arr, left, right);
            Quicksort(arr, left, pivot - 1);
            Quicksort(arr, pivot + 1, right);
        }
        private static void QuicksortParallel<T>(T[] arr, int left, int right)
            where T : IComparable<T>
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Test>();

        }
    }
}
