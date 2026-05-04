using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Text.Json;

namespace OnlineBookstore.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly string _conString =
            "Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=OnlineBookstore;Integrated Security=True";

        public List<CartItem> CartItems { get; set; } = new();
        public decimal GrandTotal => CartItems.Sum(x => x.Price * x.Quantity);
        public bool OrderPlaced { get; set; }

        public void OnGet()
        {
            CartItems = GetCart();
        }

        public IActionResult OnPost()
        {
            CartItems = GetCart();

            if (!CartItems.Any())
            {
                return RedirectToPage("/Cart");
            }

            string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdValue))
            {
                return RedirectToPage("/Login");
            }

            int userId = int.Parse(userIdValue);

            using SqlConnection con = new SqlConnection(_conString);
            con.Open();

            using SqlTransaction transaction = con.BeginTransaction();

            try
            {
                string orderQuery = @"
                    INSERT INTO [Order] (UserID)
                    OUTPUT INSERTED.OrderID
                    VALUES (@UserID)";

                int orderId;
                using (SqlCommand orderCmd = new SqlCommand(orderQuery, con, transaction))
                {
                    orderCmd.Parameters.AddWithValue("@UserID", userId);
                    orderId = (int)orderCmd.ExecuteScalar();
                }

                string orderLineQuery = @"
                    INSERT INTO OrderLine (OrderID, BookID, Quantity, UnitPrice)
                    VALUES (@OrderID, @BookID, @Quantity, @UnitPrice)";

                foreach (CartItem item in CartItems)
                {
                    using SqlCommand lineCmd = new SqlCommand(orderLineQuery, con, transaction);
                    lineCmd.Parameters.AddWithValue("@OrderID", orderId);
                    lineCmd.Parameters.AddWithValue("@BookID", item.BookID);
                    lineCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    lineCmd.Parameters.AddWithValue("@UnitPrice", item.Price);
                    lineCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            HttpContext.Session.Remove("Cart");

            return RedirectToPage("/Orders");
        }

        private List<CartItem> GetCart()
        {
            string? cartJson = HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }
    }
}