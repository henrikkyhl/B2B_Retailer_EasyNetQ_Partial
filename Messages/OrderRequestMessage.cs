namespace Messages
{
    public class OrderRequestMessage
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Country { get; set; }
        public string ReplyTo { get; set; }
    }
}
