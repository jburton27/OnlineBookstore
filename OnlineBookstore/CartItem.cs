namespace OnlineBookstore
{
    public class CartItem
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public int Quantity { get; set; }
    }
}
