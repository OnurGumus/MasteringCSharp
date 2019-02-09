using System;
using System.Linq;

namespace Lab3
{
    class Program
    {
        private static void Swap<T>(T[] arr, int i, int j)
        {
            var tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }
        private static int Partition<T>(T[] arr, int low, int high)
 where T : IComparable<T>
        {
            var pivotPos = (high + low) / 2;
            var pivot = arr[pivotPos];
            Swap(arr, low, pivotPos);

            var left = low;

            var span = arr.AsSpan(low + 1, high - low - 1);
            for (var i = 0; i < span.Length; i++)
            {
                if (span[i].CompareTo(pivot) >= 0) continue;
                left++;
                Swap(arr, i + low + 1, left);
            }

            Swap(arr, low, left);
            return left;
        }
        public static void Quicksort<T>(T[] arr, int left, int right) where T : IComparable<T>
        {
            if (right <= left) return;
            var pivot = Partition(arr, left, right);
            Quicksort(arr, left, pivot - 1);
            Quicksort(arr, pivot + 1, right);
        }
        static void Main(string[] args)
        {
            var target = Enumerable.Range(1, 10).Reverse().ToArray();
            Quicksort(target, 0, 9);
            foreach(var item in target)
            {
                Console.WriteLine(item);
            }
        }
    }
}
