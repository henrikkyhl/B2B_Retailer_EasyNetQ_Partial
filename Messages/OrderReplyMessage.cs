namespace Messages
{
    public class OrderReplyMessage
    {
        public int WarehouseId { get; set; }
        public int OrderId { get; set; }
        public int ItemsInStock { get; set; }
        public int DaysForDelivery { get; set; }
        public decimal ShippingCharge { get; set; }
    }
}
