using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace SIMD
{
    [SimpleJob(launchCount: 1, warmupCount: 2, targetCount: 10)]
    [DisassemblyDiagnoser]
    public class TestSuite
    {
        static int[] arr1 = new int[64 * 1024];
        static int[] arr2 = new int[64 * 1024];

        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static int[] ArrayAdd()
        {
            var lhs = arr1;
            var rhs = arr2;
            var result = new int[lhs.Length];
            for (var i = 0; i < lhs.Length; i++)
            {
                result[i] = lhs[i] + rhs[i];
            }
            return result;
        }
        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static int[] SIMDArrayAddition3()
        {
            var lhs = arr1;
            var rhs = arr2;
            var result = new int[lhs.Length];
            var vlhs = MemoryMarshal.Cast<int, Vector<int>>(lhs);
            var vrhs = MemoryMarshal.Cast<int, Vector<int>>(rhs);
            var vres = MemoryMarshal.Cast<int, Vector<int>>(result);
            for (var i = 0; i < vlhs.Length; i++)
                vres[i] = vlhs[i] + vrhs[i];
            return result;
        }
        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public unsafe static int[] SIMDArrayAddition4()
        {
            var lhs = arr1;
            var rhs = arr2;
            var result = new int[lhs.Length];
            fixed (int* plhs = &lhs[0])
            fixed (int* prhs = &rhs[0])
            fixed (int* pres = &result[0])
            {
                for (var i = 0; i < lhs.Length; i += 32)
                {
                    Avx2.Store(pres + i, Avx2.Add(Avx.LoadVector256(plhs + i), Avx2.LoadVector256(prhs + i)));
                    Avx2.Store(pres + i + 8, Avx2.Add(Avx.LoadVector256(plhs + i + 8), Avx2.LoadVector256(prhs + i + 8)));
                    Avx2.Store(pres + i + 16, Avx2.Add(Avx.LoadVector256(plhs + i + 16), Avx2.LoadVector256(prhs + i + 16)));
                    Avx2.Store(pres + i + 24, Avx2.Add(Avx.LoadVector256(plhs + i + 24), Avx2.LoadVector256(prhs + i + 24)));
                }
            }
            return result;
        }
        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static int[] SIMDArrayAddition2()
        {
            var lhs = arr1;
            var rhs = arr2;


            var result = new int[lhs.Length];

            ref var resultStart = ref result[0];
            ref var lhsStart = ref lhs[0];
            ref var rhsStart = ref rhs[0];

            for (var i = 0; i <= (result.Length - Vector<int>.Count); i += Vector<int>.Count)
            {
                var va = Unsafe.ReadUnaligned<Vector<int>>(ref Unsafe.As<int, byte>(ref Unsafe.Add(ref lhsStart, i)));
                var vb = Unsafe.ReadUnaligned<Vector<int>>(ref Unsafe.As<int, byte>(ref Unsafe.Add(ref rhsStart, i)));
                ref var r = ref Unsafe.As<int, Vector<int>>(ref Unsafe.Add(ref resultStart, i));
                r = (va + vb);
            }
            return result;
        }
        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static int[] SIMDArrayAddition()
        {
            var lhs = arr1;
            var rhs = arr2;
            var simdLength = Vector<int>.Count * 4;
            var l = lhs.Length - simdLength;
            var result = new int[lhs.Length];
            for (var i = 0; i <= l; i += simdLength)
            {
                var va = new Vector<int>(lhs, i);
                var vb = new Vector<int>(rhs, i);
                var va2 = new Vector<int>(lhs, i + 8);
                var vb2 = new Vector<int>(rhs, i + 8);
                var va3 = new Vector<int>(lhs, i + 16);
                var vb3 = new Vector<int>(rhs, i + 16);
                var va4 = new Vector<int>(lhs, i + 24);
                var vb4 = new Vector<int>(rhs, i + 24);
                (va + vb).CopyTo(result, i);
                (va2 + vb2).CopyTo(result, i + 8);
                (va3 + vb3).CopyTo(result, i + 16);
                (va4 + vb4).CopyTo(result, i + 24);
            }
            return result;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {

            var summary = BenchmarkRunner.Run<TestSuite>();
        }
    }
}
