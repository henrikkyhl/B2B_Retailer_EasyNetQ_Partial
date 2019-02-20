using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Warehouse
{
    class WarehouseProgram
    {
        static void Main(string[] args)
        {
            List<Product> dkProducts = new List<Product>{
                new Product { ProductId = 1, ItemsInStock = 10 }
            };

            List<Product> frProducts = new List<Product>{
                new Product { ProductId = 1, ItemsInStock = 10 },
                new Product { ProductId = 2, ItemsInStock = 2 }
            };
            List<Product> usProducts = new List<Product>{
                new Product { ProductId = 1, ItemsInStock = 10 },
                new Product { ProductId = 2, ItemsInStock = 2 },
                new Product { ProductId = 3, ItemsInStock = 5 }
            };

            // Create three warehouses running in separate threads.
            Task.Factory.StartNew(() => new Warehouse(1, "DK", dkProducts).Start());
            Task.Factory.StartNew(() => new Warehouse(2, "FR", frProducts).Start());
            Task.Factory.StartNew(() => new Warehouse(3, "US", usProducts).Start());

            // Block this thread so that the warehouse program will not exit.
            Console.ReadLine();
        }

    }
}
