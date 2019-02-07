using System;
using System.Collections.Generic;
using System.Linq;
using EasyNetQ;
using Messages;

namespace Warehouse
{
    class WarehouseProgram
    {
        static string country;
        static int warehouseId;
        static List<Product> products = null;
        static IBus bus = null;

        static void Main(string[] args)
        {
            products = new List<Product>{
                new Product { ProductId = 1, ItemsInStock = 10 },
                new Product { ProductId = 2, ItemsInStock = 2 },
            };

            Console.WriteLine("Enter warehouse Id:");
            warehouseId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter warehouse country code:");
            country = Console.ReadLine();

            using (bus = RabbitHutch.CreateBus("host=localhost;persistentMessages=false"))
            {
                bus.Subscribe<OrderRequestMessage>("warehouse" + warehouseId.ToString(), HandleOrderRequestMessage);
                Console.WriteLine("Listening for order requests\n");
                Console.ReadLine();
            }
        }

        static void HandleOrderRequestMessage(OrderRequestMessage message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Order request received:");
            Console.WriteLine("Order Id: " + message.OrderId);
            Console.WriteLine("Product Id: " + message.ProductId);
            Console.WriteLine("Country: " + message.Country);
            Console.ResetColor();

            int daysForDelivery = country == message.Country ? 2 : 10;
            decimal shippingCharge = country == message.Country ? 5 : 10;

            Product requestedProduct =
                products.Single(p => p.ProductId == message.ProductId);

            OrderReplyMessage replyMessage = new OrderReplyMessage
            {
                WarehouseId = warehouseId,
                OrderId = message.OrderId,
                ItemsInStock = requestedProduct.ItemsInStock,
                DaysForDelivery = daysForDelivery,
                ShippingCharge = shippingCharge
            };

            bus.Send(message.ReplyTo, replyMessage);
            Console.WriteLine("Reply sent back to retailer");
        }

    }
}
