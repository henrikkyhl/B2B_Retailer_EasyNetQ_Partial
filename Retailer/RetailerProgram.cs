﻿using EasyNetQ;
using Messages;

string connectionStr = "host=hare.rmq.cloudamqp.com;virtualHost=npaprqop;username=npaprqop;password=<type your password here>";

Object lockObject = new object();
IBus bus = null;
int timeoutInterval = 5000;
int orderId = 0;
// This list keeps track of the order reply messages from warehouses, which
// have not yet been processed by the retailer.
List<OrderReplyMessage> allReplies = new List<OrderReplyMessage>();


using (bus = RabbitHutch.CreateBus(connectionStr))
{
    Console.WriteLine("Retailer is running.");

    // Listen for order request messages from customers
    bus.SendReceive.Receive<OrderRequestMessage>("customerToRetailerQueue", 
        message => HandleOrderRequest(message));

    // Listen for order reply messages from warehouses (use a point-to-point channel).
    // WRITE CODE HERE!

    // Block this thread so that the retailer program will not exit.
    Console.ReadLine();
}


void HandleOrderRequest(OrderRequestMessage message)
{
    message.OrderId = ++orderId;

    // Send an order request message to all warehouses (publish-subscribe channel).
    // WRITE CODE HERE!

    lock (lockObject)
    {
        Console.WriteLine("Order request message from customer " + message.CustomerId + " published");
    }

    // Create a timer to wait for a specified amount of time for replies from
    // warehouses. When the timeout occurs, the specified event handler
    // (Timeout_Elapsed) will process the replies from warehouses and reply
    // to the customer who created the order request.
    // Beware that a timer runs in a separate thread.
    Timer timer = new Timer(Timeout_Elapsed, message, timeoutInterval, 
        Timeout.Infinite);
}


void HandleOrderReply(OrderReplyMessage message)
{
    lock (lockObject)
    {
        // Add the order reply message the list of all replies.
        allReplies.Add(message);
    }
}


void Timeout_Elapsed(object message)
{
    OrderRequestMessage m = message as OrderRequestMessage;

    lock (lockObject)
    {
        Console.WriteLine("Processing replies for request from customer " + m.CustomerId + ".");

        // Get all replies from warehouses for this specific order.
        var repliesForThisOrder = allReplies.FindAll(r => r.OrderId == m.OrderId);

        if (repliesForThisOrder.Count > 0)
        {
            // Get a reply from a local warehouse (DaysForDelivery == 2) if possible.
            var reply = 
                repliesForThisOrder.FirstOrDefault(r => r.DaysForDelivery == 2);
            if (reply == null)
            {
                // Otherwise, accept the first reply from one of the other warehouses.
                reply = repliesForThisOrder.First();
            }

            // Remove the replies for this specific order from the "allReplies" list,
            // because they have already been processed.
            allReplies.RemoveAll(r => r.OrderId == m.OrderId);

            // Uses Topic Based Routing to send the reply to a customer.
            // The topic ís the CustomerId for the outstanding request.
            bus.PubSub.Publish<OrderReplyMessage>(reply, m.CustomerId.ToString());


            Console.WriteLine("Order: " + reply.OrderId);
            Console.WriteLine("Warehouse: " + reply.WarehouseId);
            Console.WriteLine("Items: " + reply.ItemsInStock);
            Console.WriteLine("Shipping charge: " + reply.ShippingCharge);
            Console.WriteLine("Days for delivery: " + reply.DaysForDelivery);

        }
        else  // if there was no reply from any warehouse
        {
            Console.WriteLine("No items in stock for this product.");
            OrderReplyMessage replyNoItems = new OrderReplyMessage();
            replyNoItems.OrderId = m.OrderId;
            bus.PubSub.Publish<OrderReplyMessage>(replyNoItems, m.CustomerId.ToString());
        }
    }
}
