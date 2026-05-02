namespace OnlineBookstore
{
    public class GenreSalesReport
    {
        public string GenreName { get; set; }
        public decimal TotalSales { get; set; }
        public int UniqueCustomers { get; set; }
        public decimal AverageOrderValue { get; set; }
        public long GenreSalesRank { get; set; }
    }
}
