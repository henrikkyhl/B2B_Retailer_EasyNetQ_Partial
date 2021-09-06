namespace Messages
{
    public class OrderRequestMessage
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string Country { get; set; }
    }
}
