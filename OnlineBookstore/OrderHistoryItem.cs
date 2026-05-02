namespace OnlineBookstore
{
    public class OrderHistoryItem
    {
        public int BookID { get; set; }
        public string Title { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
