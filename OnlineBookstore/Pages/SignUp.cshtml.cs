using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace OnlineBookstore.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly string _conString =
            "Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=OnlineBookstore;Integrated Security=True";

        [BindProperty]
        public string FirstName { get; set; } = "";

        [BindProperty]
        public string LastName { get; set; } = "";

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string Message { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            using SqlConnection con = new SqlConnection(_conString);
            con.Open();

            string checkQuery = "SELECT COUNT(*) FROM [User] WHERE Email = @Email";
            using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
            {
                checkCmd.Parameters.AddWithValue("@Email", Email);
                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    Message = "That email already exists.";
                    return Page();
                }
            }

            var hasher = new PasswordHasher<UserAccount>();
            string passwordHash = hasher.HashPassword(new UserAccount(), Password);

            string insertQuery = @"
        INSERT INTO [User] (FirstName, LastName, Email, PasswordHash)
        VALUES (@FirstName, @LastName, @Email, @PasswordHash)";

            using SqlCommand insertCmd = new SqlCommand(insertQuery, con);
            insertCmd.Parameters.AddWithValue("@FirstName", FirstName);
            insertCmd.Parameters.AddWithValue("@LastName", LastName);
            insertCmd.Parameters.AddWithValue("@Email", Email);
            insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            insertCmd.ExecuteNonQuery();

            return RedirectToPage("/Login");
        }
    }
}