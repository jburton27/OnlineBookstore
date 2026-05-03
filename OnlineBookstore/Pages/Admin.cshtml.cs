using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace OnlineBookstore.Pages
{
    public class AdminModel : PageModel
    {
        private readonly string _conString =
            "Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=OnlineBookstore;Integrated Security=True";

        [BindProperty]
        public string ISBN { get; set; } = "";

        [BindProperty]
        public int AuthorID { get; set; }

        [BindProperty]
        public int StoreID { get; set; }

        [BindProperty]
        public int GenreID { get; set; }

        [BindProperty]
        public string Title { get; set; } = "";

        [BindProperty]
        public decimal Price { get; set; }

        [BindProperty]
        public int? PublicationYear { get; set; }

        [BindProperty]
        public string? Condition { get; set; }

        [BindProperty]
        public string? CoverType { get; set; }

        [BindProperty]
        public string? ImagePath { get; set; }

        public string Message { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            using SqlConnection con = new SqlConnection(_conString);
            con.Open();

            string query = @"
                INSERT INTO Book
                (
                    ISBN,
                    AuthorID,
                    StoreID,
                    GenreID,
                    Title,
                    Price,
                    PublicationYear,
                    [Condition],
                    CoverType,
                    ImagePath
                )
                VALUES
                (
                    @ISBN,
                    @AuthorID,
                    @StoreID,
                    @GenreID,
                    @Title,
                    @Price,
                    @PublicationYear,
                    @Condition,
                    @CoverType,
                    @ImagePath
                )";

            using SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@ISBN", ISBN);
            cmd.Parameters.AddWithValue("@AuthorID", AuthorID);
            cmd.Parameters.AddWithValue("@StoreID", StoreID);
            cmd.Parameters.AddWithValue("@GenreID", GenreID);
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@Price", Price);
            cmd.Parameters.AddWithValue("@PublicationYear", (object?)PublicationYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Condition", (object?)Condition ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CoverType", (object?)CoverType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ImagePath", (object?)ImagePath ?? DBNull.Value);

            cmd.ExecuteNonQuery();

            Message = "Book added successfully.";

            return Page();
        }
    }
}
