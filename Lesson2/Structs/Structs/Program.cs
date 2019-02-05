using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Structs
{
    internal class ReadOnlyEnumerator
    {
        readonly List<int>.Enumerator _enumerator;
        public ReadOnlyEnumerator(List<int> list) => _enumerator = list.GetEnumerator();
        public void PrintTheFirstElement()
        {
            _enumerator.MoveNext();
            Console.WriteLine(_enumerator.Current);
        }
    }


    readonly struct MyStruct
    {
        public void SomeMethod()
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var roe = new ReadOnlyEnumerator(new List<int> { 1, 2 });
            roe.PrintTheFirstElement();
        }
      

    }
}
