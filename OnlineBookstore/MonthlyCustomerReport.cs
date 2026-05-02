namespace OnlineBookstore
{
    public class MonthlyCustomerReport
    {
        public int OrderYear { get; set; }
        public int OrderMonth { get; set; }
        public int NewCustomers { get; set; }
        public int RepeatCustomers { get; set; }
        public int TotalActiveCustomers { get; set; }
    }
}
