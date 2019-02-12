using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ConsoleApp94
{
    class Details
    {
        public DateTime Date { get; } = DateTime.Now;
        public decimal Price { get; }
    }
    class Account
    {
        public List<Details> Transactions = new List<Details>();
    }
    class Program
    {
        static List<Account> accounts = new List<Account>();
        static void Main(string[] args)
        {
            var random = new Random();
            while (true)
            {
                try
                {
                    DoWork();
                }
                catch
                {

                }
            }
            Console.ReadKey();
        }
        public static void DoWork()
        {
            var account = new Account();
            account.Transactions.Add(new Details());
            accounts.Add(account);
            Thread.Sleep(10);
            if (accounts.Count == 100)
            {
                accounts = null;
            }
        }
    }
}
