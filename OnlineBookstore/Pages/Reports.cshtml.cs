using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace OnlineBookstore.Pages
{
    public class ReportsModel : PageModel
    {
        private string _conString = "Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=OnlineBookstore;Integrated Security=True";

        public List<GenreSalesReport> GenreSalesReports { get; set; } = new();
        public List<AuthorPerformanceReport> AuthorReports { get; set; } = new();
        public List<MonthlyCustomerReport> MonthlyReports { get; set; } = new();
        public List<TopCustomerReport> TopCustomerReports { get; set; } = new();

        public void OnGet()
        {
            LoadGenreSalesReport();
            LoadAuthorPerformanceReport();
            LoadMonthlyCustomerReport();
            LoadTopCustomerReport();
        }

        private void LoadGenreSalesReport()
        {
            using SqlConnection con = new SqlConnection(_conString);
            con.Open();

            using SqlCommand cmd = new SqlCommand("GetGenreSalesReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StartDate", "2026-01-01");
            cmd.Parameters.AddWithValue("@EndDate", "2026-12-31");

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                GenreSalesReports.Add(new GenreSalesReport
                {
                    GenreName = reader.GetString(reader.GetOrdinal("GenreName")),
                    TotalSales = reader.GetDecimal(reader.GetOrdinal("TotalSales")),
                    UniqueCustomers = reader.GetInt32(reader.GetOrdinal("UniqueCustomers")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue")),
                    GenreSalesRank = reader.GetInt64(reader.GetOrdinal("GenreSalesRank"))
                });
            }
        }

        private void LoadAuthorPerformanceReport()
        {
            using SqlConnection con = new SqlConnection(_conString);
            con.Open();

            using SqlCommand cmd = new SqlCommand("GetAuthorPerformanceReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MinimumRatings", 1);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                AuthorReports.Add(new AuthorPerformanceReport
                {
                    AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                    AverageBookRating = reader.GetDecimal(reader.GetOrdinal("AverageBookRating")),
                    TotalRatingsReceived = reader.GetInt32(reader.GetOrdinal("TotalRatingsReceived")),
                    TotalBooksSold = reader.GetInt32(reader.GetOrdinal("TotalBooksSold"))
                });
            }
        }

        private void LoadMonthlyCustomerReport()
        {
            using SqlConnection con = new SqlConnection(_conString);
            con.Open();

            using SqlCommand cmd = new SqlCommand("GetMonthlyCustomerBehavior", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StartDate", "2026-01-01");
            cmd.Parameters.AddWithValue("@EndDate", "2026-12-31");

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MonthlyReports.Add(new MonthlyCustomerReport
                {
                    OrderYear = reader.GetInt32(reader.GetOrdinal("OrderYear")),
                    OrderMonth = reader.GetInt32(reader.GetOrdinal("OrderMonth")),
                    NewCustomers = reader.GetInt32(reader.GetOrdinal("NewCustomers")),
                    RepeatCustomers = reader.GetInt32(reader.GetOrdinal("RepeatCustomers")),
                    TotalActiveCustomers = reader.GetInt32(reader.GetOrdinal("TotalActiveCustomers"))
                });
            }
        }

        private void LoadTopCustomerReport()
        {
            using SqlConnection con = new SqlConnection(_conString);
            con.Open();

            using SqlCommand cmd = new SqlCommand("GetTopCustomers", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TopCount", 10);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                TopCustomerReports.Add(new TopCustomerReport
                {
                    CustomerFullName = reader.GetString(reader.GetOrdinal("CustomerFullName")),
                    CustomerEmail = reader.GetString(reader.GetOrdinal("CustomerEmail")),
                    LifetimeSpend = reader.GetDecimal(reader.GetOrdinal("LifetimeSpend")),
                    TotalBooksPurchased = reader.GetInt32(reader.GetOrdinal("TotalBooksPurchased")),
                    FavoriteGenre = reader.GetString(reader.GetOrdinal("FavoriteGenre"))
                });
            }
        }
    }
}


