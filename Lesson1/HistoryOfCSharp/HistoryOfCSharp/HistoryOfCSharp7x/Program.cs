using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HistoryOfCSharp7x
{
    class Person
    {
        public string Name { get; set; }
    }
    class Program
    {
        //Async main
        static async Task Main(string[] args)
        {
            var list = await Main();
            var result = list.Select(c => (c.Length, c.First())).First();
            //infer tuple names
            Console.WriteLine(result.Length);
            Console.WriteLine(result.Item2);
            //digit seperators
            var x = 1_000_000;
            //  won't compile
            // ref var r = ref (list != null ? ref list[0] : ref list[1]);
            ConsumeRef();
            Console.WriteLine(People[0].Name);

        }


        public static void ConsumeRef()
        {
            ref var p = ref GetContactInformation("foo", "bar");
            p = People[1];
            p = ref People[2];
            p = People[1];
        }
        // won't work for other collections. Why ?


        //In C#, ref works for:
        //Variables(local or parameters)
        //Fields
        //Array locations
        static readonly Person[] People = new Person[] { new Person() { Name = "Naruto" },  new Person() { Name = "Itachi" }, new Person() { Name = "Kisame" } };
        public static ref Person GetContactInformation(string fname, string lname)
        {
            // ...method implementation...
            return ref People[0];
        }


        ref int AnotherMethod(int[] list)
        {
            var l = new int[1];
            return ref (list != null ? ref list[0] : ref l[1]);
        }

        //default expressions
        static async Task<List<string>> Main(CancellationToken token = default)
        {
            // This could also be replaced with the body
            // DoAsyncWork, including its await expressions:
            await Task.Delay(100);
            return new List<string> { "a", "b", "c" };
        }
        public ValueTask<int> CachedFunc()
        {
            return (cache) ? new ValueTask<int>(cacheResult) : new ValueTask<int>(LoadCache());
        }
        private bool cache = false;
        private int cacheResult;
        private async Task<int> LoadCache()
        {
            // simulate async work:
            await Task.Delay(100);
            cacheResult = 100;
            cache = true;
            return cacheResult;
        }
    }
}
