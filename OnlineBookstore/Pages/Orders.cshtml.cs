using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace OnlineBookstore.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly string conString =
            "Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=OnlineBookstore;Integrated Security=True";

        public List<OrderHistoryViewModel> Orders { get; set; } = new();

        public void OnGet()
        {
            string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdValue))
            {
                return;
            }

            int userId = int.Parse(userIdValue);

            using SqlConnection con = new SqlConnection(conString);
            con.Open();

            string query = @"
                SELECT
                    o.OrderID,
                    o.OrderDate,
                    ol.OrderLineID,
                    ol.BookID,
                    ol.Quantity,
                    ol.UnitPrice,
                    b.Title
                FROM [Order] o
                INNER JOIN OrderLine ol ON o.OrderID = ol.OrderID
                INNER JOIN Book b ON ol.BookID = b.BookID
                WHERE o.UserID = @UserID
                ORDER BY o.OrderDate DESC, o.OrderID DESC";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int orderId = reader.GetInt32(reader.GetOrdinal("OrderID"));

                OrderHistoryViewModel? existingOrder = Orders.FirstOrDefault(o => o.OrderID == orderId);

                if (existingOrder == null)
                {
                    existingOrder = new OrderHistoryViewModel
                    {
                        OrderID = orderId,
                        OrderDate = reader.GetDateTimeOffset(reader.GetOrdinal("OrderDate")),
                        Items = new List<OrderHistoryItem>()
                    };

                    Orders.Add(existingOrder);
                }

                existingOrder.Items.Add(new OrderHistoryItem
                {
                    BookID = reader.GetInt32(reader.GetOrdinal("BookID")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice"))
                });
            }
        }
    }
}