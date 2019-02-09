using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntitityFramework
{
    public class ShoppingContext : DbContext
    {
        static readonly ILoggerFactory loggerFactory =
            LoggerFactory.Create(builder => builder.AddConsole());

        #nullable disable
        public DbSet<Customer> Customers { get; private set; }

        public DbSet<Order> Orders { get; private set; }
        #nullable enable
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(loggerFactory)
                .UseSqlServer("Data Source=(localdb)\\ProjectsV13;Initial Catalog=ShoppingDB;Integrated Security=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
            .HasOne(typeof(Customer))
            .WithMany(nameof(Customer.Orders))
            .IsRequired();

        }
    }


    public class Customer
    {
        readonly IList<Order> orders = new List<Order>();

        public int Id { get; private set; }

        public string? CustomerName { get; set; }

        public IList<Order> Orders => this.orders;
    }

    public class Order
    {
        public int Id { get; private set; }
        public string? Item { get; set; }

        public decimal Price { get; set; }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            await AddOrders();

            await QueryOrders();

            UpdateOrders();
            DeleteCustomersViaTrx();

        }

        private static async Task QueryOrders()
        {
            using (var db = new ShoppingContext())
            {

                var customersWithNoOrders =
                    await db.Customers.Where(c => c.Orders.Count == 0).ToListAsync();

                foreach (var customer in customersWithNoOrders)
                {
                    Console.WriteLine(customer.Id);
                }

                customersWithNoOrders =
                   await db.Customers.Where(c => !c.Orders.Any()).ToListAsync();

                foreach (var customer in customersWithNoOrders)
                {
                    Console.WriteLine(customer.Id);
                }
                customersWithNoOrders = await

                    (from customer in db.Customers
                     join order in db.Orders on customer.Id equals EF.Property<int>(order, "CustomerId") into j
                     from m in j.DefaultIfEmpty()
                     where m == null
                     select customer).ToListAsync();

                foreach (var customer in customersWithNoOrders)
                {
                    Console.WriteLine(customer.Id);
                }
            }
        }

        private static async Task AddOrders()
        {

            try
            {
                //some code
            }
            finally
            {
                //call Dispose method.
            }
            using (var db = new ShoppingContext())
            {
                var customer1 = new Customer() { CustomerName = "Onur" };
                var customer2 = new Customer() { CustomerName = "Frisk" };

                customer1.Orders.Add(new Order() { Item = "Beginning ASP.NET", Price = 35.55M });
                db.Customers.Add(customer1);
                db.Customers.Add(customer2);
                await db.SaveChangesAsync();
            }
        }

        private static void UpdateOrders()
        {
            using (var db = new ShoppingContext())
            {
                var customer = db.Customers.First();
                customer.CustomerName = "John";
                int rowsChanged = db.SaveChanges();
                Console.WriteLine(rowsChanged);

            }
        }

        static void DeleteCustomersViaTrx()

        {

            using (var db = new ShoppingContext())

            using (var transaction = db.Database.BeginTransaction())

            {
                var customers = db.Customers.Include(b => b.Orders).ToList();
                foreach (var customer in customers)
                {

                    db.Entry(customer).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

                    int recordsDeleted = db.SaveChanges();

                    Console.WriteLine("Number of records deleted:" + recordsDeleted);


                }

                transaction.Commit();

            }

        }
    }
}
