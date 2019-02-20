using System;
using System.Threading.Tasks;

namespace Customer
{
    class CustomerProgram
    {
        static void Main(string[] args)
        {
            // Create three customers running in separate threads.

            // Because I have a warehouse in Denmark (DK) with product number 1 in
            // stock, this order request will be fulfilled by the local
            // warehouse.
            Task.Factory.StartNew(() => new Customer(1, 1, "DK").Start());

            // I have no warehouse in Sweeden (SE), so this order request will
            // be fulfilled by a warehouse in another country. 
            Task.Factory.StartNew(() => new Customer(2, 1, "SE").Start());

            // A customer placing an order for a product which is not in stock.
            Task.Factory.StartNew(() => new Customer(3, 100, "DK").Start());

            // Block this thread so that the customer program will not exit.
            Console.ReadLine();
        }
    }
}
