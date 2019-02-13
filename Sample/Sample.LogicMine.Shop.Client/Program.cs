using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sample.LogicMine.Shop.Client.Service;

namespace Sample.LogicMine.Shop.Client
{
    class Program
    {
        private const int CreateObjectCount = 5;
        private const string ServiceUrl = "http://localhost:8181/api";
        private const string Username = "eddie";
        private const string Password = "supersecret1";

        public static void Main(string[] args)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    Console.WriteLine("=========== Logging In ===========");
                    var securityToken = new LoginManager(httpClient, ServiceUrl).LoginAsync(Username, Password)
                        .GetAwaiter().GetResult();
                    
                    var customerManager = new ObjectManager<Customer>(httpClient, ServiceUrl, securityToken);
                    var productManager = new ObjectManager<Product>(httpClient, ServiceUrl, securityToken);
                    var purchaseManager = new ObjectManager<Purchase>(httpClient, ServiceUrl, securityToken);
                    var salesSummaryManager = new SalesSummaryManager(httpClient, ServiceUrl, securityToken);

                    Console.WriteLine("=========== Creating Customers ===========");
                    CreateCustomers(customerManager, CreateObjectCount).GetAwaiter().GetResult();

                    Console.WriteLine(Environment.NewLine + "=========== Creating Products ===========");
                    CreateProducts(productManager, CreateObjectCount).GetAwaiter().GetResult();

                    Console.WriteLine(Environment.NewLine + "=========== Creating Purchases ===========");
                    CreatePurchases(purchaseManager, CreateObjectCount).GetAwaiter().GetResult();

                    Console.WriteLine(Environment.NewLine + "=========== Getting Customers By Id===========");
                    var customers = GetObjectsById(customerManager, CreateObjectCount).GetAwaiter().GetResult();
                    foreach (var customer in customers)
                        Console.WriteLine($"Customer: {customer}");

                    Console.WriteLine(Environment.NewLine + "=========== Getting Products By Id ===========");
                    var products = GetObjectsById(productManager, CreateObjectCount).GetAwaiter().GetResult();
                    foreach (var product in products)
                        Console.WriteLine($"Product: {product}");

                    Console.WriteLine(Environment.NewLine + "=========== Getting Filtered Purchases ===========");
                    // pull back only the purchases that have a unit cost between 350 and 550 and a quantity >= 30
                    var purchases = purchaseManager.GetCollectionAsync("UnitPrice range 350..550 and Quantity ge 30")
                        .GetAwaiter().GetResult();

                    foreach (var purchase in purchases)
                        Console.WriteLine($"Purchase: {purchase}");

                    Console.WriteLine(Environment.NewLine + "=========== Sales Summary ===========");
                    var summary = salesSummaryManager.GetAsync().GetAwaiter().GetResult();
                    Console.WriteLine($"Summary: {summary}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred and the application is shutting down: {ex.Message}");
            }
        }

        private static Task CreateCustomers(ObjectManager<Customer> manager, int count)
        {
            var customers = new List<Customer>();
            for (var i = 0; i < count; i++)
            {
                customers.Add(new Customer
                {
                    Forename = $"Jimmy{i}",
                    Surname = $"Riddle{i}",
                    Email = $"jimmy{i}@riddle{i}.com"
                });
            }

            return CreateObjects(manager, customers);
        }

        private static Task CreateProducts(ObjectManager<Product> manager, int count)
        {
            var products = new List<Product>();
            for (var i = 1; i <= count; i++)
            {
                products.Add(new Product
                {
                    Name = $"IPhone {i}",
                    Price = i * 125.25m
                });
            }

            return CreateObjects(manager, products);
        }

        private static Task CreatePurchases(ObjectManager<Purchase> manager, int count)
        {
            var purchases = new List<Purchase>();
            for (var i = 1; i <= count; i++)
            {
                purchases.Add(new Purchase
                {
                    CustomerId = i,
                    ProductId = i,
                    Quantity = i * 10
                });
            }

            return CreateObjects(manager, purchases);
        }

        private static async Task CreateObjects<T>(ObjectManager<T> manager, IEnumerable<T> objs) where T : class
        {
            foreach (var obj in objs)
            {
                try
                {
                    var id = await manager.CreateAsync(obj).ConfigureAwait(false);

                    Console.WriteLine($"Created '{typeof(T).Name}' with Id {id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
            }
        }

        private static async Task<IEnumerable<T>> GetObjectsById<T>(ObjectManager<T> manager, int count)
            where T : class
        {
            var result = new List<T>();
            for (var i = 1; i <= count; i++)
                result.Add(await manager.GetAsync(i).ConfigureAwait(false));

            return result;
        }
    }
}