using System;
using Messages;
using System.Threading;
using EasyNetQ;
using System.Text;

namespace Customer
{
    public class Customer
    {
        private int customerID;
        private int productID;
        private string country;

        public Customer(int customerID, int productID, string country)
        {
            this.customerID = customerID;
            this.productID = productID;
            this.country = country;
        }

        public void Start()
        {
            lock (this)
            {
                Console.WriteLine("Customer " + customerID + " is running. Waiting for a reply.");
            }

            OrderRequestMessage request = new OrderRequestMessage
            {
                CustomerId = customerID,
                ProductId = productID,
                Country = country
            };

            using (IBus bus = RabbitHutch.CreateBus("host=goose.rmq2.cloudamqp.com;virtualHost=mldigrlk;username=mldigrlk;password=b4bT92Z_sBkWhRoIP1ZAUe_BH_8hpTcv;persistentMessages=false"))
            {
                // SOLUTION #1 - Listen to reply messages from the Retailer (use Topic Based Routing).
                bus.PubSub.Subscribe<OrderReplyMessage>("warehouseToRetailerQueue", HandleOrderEvent, x => x.WithTopic(customerID.ToString()));
                // SOLUTION #2 - Send an order request message to the Retailer (use a point-to-point channel).
                bus.SendReceive.Send("customerToRetailerQueue", request);
                // Block this thread so that the customer instance will not exit.
                Console.ReadLine();
            }
        }
        private void HandleOrderEvent(OrderReplyMessage message)
        {
            StringBuilder reply = new StringBuilder();
            reply.Append("Order reply received by customer: " + customerID + "\n");
            reply.Append("Warehouse ID: " + message.WarehouseId + "\n");
            reply.Append("Items in stock: " + message.ItemsInStock + "\n");
            reply.Append("Shipping charge: " + message.ShippingCharge + "\n");
            reply.Append("Days for delivery: " + message.DaysForDelivery + "\n");
            string SummaryMessage = message.WarehouseId == 0 ? "Your order count not be fulfilled.\n" : "Your Order will cost " + message.ShippingCharge + "$ and be delivered in " + message.DaysForDelivery + " days\n";
            reply.Append(SummaryMessage);
            lock (this)
            {
                Console.WriteLine(reply.ToString());
            }
        }

    }
}
