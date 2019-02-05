using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.IO.File;
namespace HistoryOfCSharp
{
    class Product
    {
        // C# v2
        // Auto properties
        public string Name { get; set; }

        public void CalculateTax()
        {
            // Generics
            List<Product> products = new List<Product>();
            //using keyword
            using (Stream stream = new MemoryStream())
            {
                //as keyword
                if (stream as MemoryStream != null)
                {

                }
            }
        }
        //C# v3
        public  IEnumerable<string> GetProductsByname(string s)
        {
            // type inference
            var products = new List<Product>();
            //linq
            return from p in products where p.Name == s select s;
        }

        //C# v4
        // dynamic
        public IEnumerable<string> GetProductsByname(dynamic s)
        {
          
            var products = new List<Product>();
            return from p in products where p.Name == s select (string)s;
        }
        //C# v5
        public async Task<IEnumerable<string>> GetProductsBynameAsync()
        {
            // async await
            return await File.ReadAllLinesAsync("Products");
        }
        //C# v6
        public decimal Price { get; } = 5M; // or from constructor

       // Expression-bodied function members
        public  Task<string[]> GetProductsBynameAsync2() 
            => ReadAllLinesAsync("Products");
        public void ChangeName(Product p)
            => this.Name = p?.Name;

        //String interpolation
        public string Description => $"{this.Name}";
        //Exception filters
        public decimal CalculateTax2()
        {
            try
            {
                return this.Price * 0.1M;
            }

            // nameof
            catch (Exception e) when (e.Message.Contains(nameof(Product)))
            {

            }
            return this.Price;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
        }
    }
}
