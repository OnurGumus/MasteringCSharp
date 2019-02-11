using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Arrays
{
    [DisassemblyDiagnoser]
    [SimpleJob(launchCount: 1, warmupCount: 2, targetCount: 10)]
    public class TestSuite
    {
        const int itemCount = 64 * 1024 * 1024;
        static readonly int[] items = new int[itemCount];
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        [Benchmark]
        public void Loop1()
        {
            var items = TestSuite.items;
            for (var i = 0; i < items.Length; i++)
            {
                items[i] *= 3;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        [Benchmark]
        public void Loop2()
        {
            var items = TestSuite.items;
            var count = itemCount - 10;
            for (var i = 0; i < count; i++)
            {
                items[i] *= 3;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        [Benchmark]
        public void Loop21()
        {
            int[] items = TestSuite.items;
            Span<int> s = items.AsSpan().Slice(0, itemCount - 10);
            for (var i = 0; i < s.Length; i++)
                s[i] *= 3;
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        [Benchmark]
        public void Loop22()
        {
            int[] items = TestSuite.items;
            foreach (ref int item in items.AsSpan().Slice(0, itemCount - 10))
                item *= 3;
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        [Benchmark]
        public unsafe void Loop3()
        {
            int[] items = TestSuite.items;
            fixed (int* item = items)
                for (var i = 0; i < itemCount; i++)
                {
                    item[i] *= 3;
                }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        [Benchmark]
        public unsafe void Loop4()
        {
            int[] items = TestSuite.items;
            fixed (int* item = items)
                for (var i = 0; i < itemCount; i+=8)
                {
                    item[i] *= 3;
                }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        [Benchmark]
        public unsafe void Loop5()
        {
            int[] items = TestSuite.items;
            fixed (int* item = items)
                for (var i = 0; i < itemCount; i += 16)
                {
                    item[i] *= 3;
                }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        [Benchmark]
        public unsafe void Loop6()
        {
            int[] items = TestSuite.items;
            fixed (int* item = items)
                for (var i = 0; i < itemCount; i += 32)
                {
                    item[i] *= 3;
                }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        [Benchmark]
        public unsafe void Loop7()
        {
            int[] items = TestSuite.items;
            fixed (int* item = items)
                for (var i = 0; i < itemCount; i += 64)
                {
                    item[i] *= 3;
                }
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TestSuite>();

        }
        public struct Point
        {
            public int X;
            public int y;
        }
        public void ProcessPointsArray(Point [] points)
        {
            for(var i = 0; i < points.Length; i ++)
            {
                points[i].X++;
            }
        }

        public void ProcessPointsList(List<Point> points)
        {
            for (var i = 0; i < points.Count; i++)
            {
               // points[i].X++; won't compile
            }
        }
    }
}
