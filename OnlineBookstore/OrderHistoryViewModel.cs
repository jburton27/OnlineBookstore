namespace OnlineBookstore
{
    public class OrderHistoryViewModel
    {
        public int OrderID { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public List<OrderHistoryItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
