using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{

   
    class Program
    {
        static void Main(string[] args)
        {
            var a = new byte[8];
            Span<byte> bytes = a;
            bytes[0] = 1;
            // a[0] == 1;
            var slice = bytes.Slice(4, 3);
            slice[0] = 2;
            //bytes[4] == 2;
            //a[4] == 2;

            Memory<int> x = stackalloc int[10];
             

        }

        //async Task Work (Span<char> v)
        //{

        //}
        async Task Work(Memory<char> v)
        {

        }
    }
}
