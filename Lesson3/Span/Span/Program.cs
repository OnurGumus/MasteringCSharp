using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Span
{
    class Program
    {
        static void Main(string[] args)
        {
            var arr = new int[100];
            Span<int> span =  arr.AsSpan();
            DoWork(span);
            DoWorkM(arr.AsMemory(1,2)).Wait();

            Span<int> span2 = stackalloc int[100];
            IntPtr unmanagedHandle = Marshal.AllocHGlobal(256);
            //works in unsafe context
            //Span<byte> unmanaged = new Span<byte>(unmanagedHandle.ToPointer(), 256);
            var streamWriter = new StreamWriter("path");
            var s = "foo".AsSpan();
            streamWriter.Write(s);
        }

        public static void DoWork(Span<int> items)
        {

        }

        //public static async Task DoWork(Span<int> items)
        //{

        //}
        public static async Task DoWorkM(Memory<int> items)
        {

        }
    }
}
