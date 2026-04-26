using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace OnlineBookstore.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public List<Book> _books = new List<Book>();

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            string conString = "Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=OnlineBookstore;Integrated Security=True";
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "SELECT * FROM Book";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Book b = new Book();
                        b.ID = reader.GetInt32("BookID");
                        Console.WriteLine(b.ID);
                        b.ISBN = reader.GetString("ISBN");
                        b.AuthorID = reader.GetInt32("AuthorID");
                        b.StoreID = reader.GetInt32("StoreID");
                        b.GenreID = reader.GetInt32("GenreID");
                        b.Title = reader.GetString("Title");
                        b.Price = reader.GetDecimal("Price");
                        b.PublicationYear = reader.GetInt32("PublicationYear");
                        b.Condition = reader.GetString("Condition");
                        b.CoverType = reader.GetString("CoverType");
                        b.ImagePath = reader.IsDBNull(10) ? null : reader.GetString(10);

                        _books.Add(b);
                    }
                }
            }
            ViewData["Books"] = _books;
        }
    }

}
