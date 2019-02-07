using System;
using EasyNetQ;
using Messages;
using System.Threading;

namespace Retailer
{
    class RetailerProgram
    {
        private static Object lockObject = new Object();
        private static int timeoutInterval = 1000;

        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost;persistentMessages=false"))
            {
                OrderRequestMessage requestMessage = new OrderRequestMessage
                {
                    OrderId = 1,
                    ProductId = 1,
                    Country = "DK",
                    ReplyTo = "replyQueueForOrderRequestMessage"
                };

                bus.Publish<OrderRequestMessage>(requestMessage);

                Console.WriteLine("Order request message published");

                bus.Receive<OrderReplyMessage>(requestMessage.ReplyTo, message => HandleOrderReplyMessage(message));

                Timer timer = new Timer(Timeout_Elapsed, requestMessage, timeoutInterval, Timeout.Infinite);

                Console.ReadLine();
            }
        }

        private static void Timeout_Elapsed(object message)
        {
            OrderRequestMessage m = message as OrderRequestMessage;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Timeout on order request " + m.OrderId + " from " + m.Country + "!");
            Console.ResetColor();
        }

        static void HandleOrderReplyMessage(OrderReplyMessage message)
        {
            lock (lockObject)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reply received from warehouse" + message.WarehouseId);
                Console.WriteLine("Order Id: " + message.OrderId);
                Console.WriteLine("Items in stock: " + message.ItemsInStock);
                Console.WriteLine("Days for delivery: " + message.DaysForDelivery);
                Console.WriteLine("Shipping charge: " + message.ShippingCharge);
                Console.ResetColor();
            }
        }

    }
}
