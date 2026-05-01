using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace OnlineBookstore.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        private string _conString = "Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=OnlineBookstore;Integrated Security=True";

        public List<Book> _books = new List<Book>();

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        { 
            using (SqlConnection con = new SqlConnection(_conString))
            {
                con.Open();
                string query = @"
                    SELECT 
                        b.BookID,
                        b.ISBN,
                        b.AuthorID,
                        b.StoreID,
                        b.GenreID,
                        b.Title,
                        b.Price,
                        b.PublicationYear,
                        b.Condition,
                        b.CoverType,
                        b.ImagePath,
                        a.FirstName + ' ' + a.LastName AS AuthorName
                    FROM Book b
                    INNER JOIN Author a
                        ON b.AuthorID = a.AuthorID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Book b = new Book();
                        b.ID = reader.GetInt32(reader.GetOrdinal("BookID"));
                        b.ISBN = reader.GetString(reader.GetOrdinal("ISBN"));
                        b.AuthorID = reader.GetInt32(reader.GetOrdinal("AuthorID"));
                        b.StoreID = reader.GetInt32(reader.GetOrdinal("StoreID"));
                        b.GenreID = reader.GetInt32(reader.GetOrdinal("GenreID"));
                        b.Title = reader.GetString(reader.GetOrdinal("Title"));
                        b.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                        b.PublicationYear = reader.GetInt32(reader.GetOrdinal("PublicationYear"));
                        b.Condition = reader.GetString(reader.GetOrdinal("Condition"));
                        b.CoverType = reader.GetString(reader.GetOrdinal("CoverType"));
                        b.ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? null : reader.GetString(reader.GetOrdinal("ImagePath"));
                        b.AuthorName = reader.GetString(reader.GetOrdinal("AuthorName"));

                        _books.Add(b);
                    }
                }
            }
            ViewData["Books"] = _books;
        }

        public IActionResult OnPostAddToCart(int bookId)
        {
            Book? book = GetBookById(bookId);

            if (book == null)
            {
                return RedirectToPage();
            }

            List<CartItem> cart = GetCart();

            CartItem? existingItem = cart.FirstOrDefault(x => x.BookID == bookId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    BookID = book.ID,
                    Title = book.Title,
                    Price = book.Price,
                    ImagePath = book.ImagePath,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));

            return RedirectToPage();
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

        private Book? GetBookById(int bookId)
        {
            using SqlConnection con = new SqlConnection(_conString);
            con.Open();

            string query = @"
                SELECT 
                    b.BookID,
                    b.ISBN,
                    b.AuthorID,
                    b.StoreID,
                    b.GenreID,
                    b.Title,
                    b.Price,
                    b.PublicationYear,
                    b.Condition,
                    b.CoverType,
                    b.ImagePath,
                    a.FirstName + ' ' + a.LastName AS AuthorName
                FROM Book b
                INNER JOIN Author a ON b.AuthorID = a.AuthorID
                WHERE b.BookID = @BookID";

            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@BookID", bookId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new Book
            {
                ID = reader.GetInt32(reader.GetOrdinal("BookID")),
                ISBN = reader.GetString(reader.GetOrdinal("ISBN")),
                AuthorID = reader.GetInt32(reader.GetOrdinal("AuthorID")),
                StoreID = reader.GetInt32(reader.GetOrdinal("StoreID")),
                GenreID = reader.GetInt32(reader.GetOrdinal("GenreID")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                PublicationYear = reader.GetInt32(reader.GetOrdinal("PublicationYear")),
                Condition = reader.GetString(reader.GetOrdinal("Condition")),
                CoverType = reader.GetString(reader.GetOrdinal("CoverType")),
                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ImagePath")),
                AuthorName = reader.GetString(reader.GetOrdinal("AuthorName"))
            };
        }
    }

    public class CartItem
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public int Quantity { get; set; }
    }
}
    
