using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace OnlineBookstore.Pages
{
    public class LoginModel : PageModel
    {
        private readonly string _conString =
            "Data Source=(localdb)\\MSSQLLocalDb;Initial Catalog=OnlineBookstore;Integrated Security=True";

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string Message { get; set; } = "";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            UserAccount? user = null;

            using SqlConnection con = new SqlConnection(_conString);
            con.Open();

            string query = "SELECT UserID, Email, PasswordHash FROM [User] WHERE Email = @Email";
            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Email", Email);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user = new UserAccount
                {
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                };
            }

            if (user == null)
            {
                Message = "Invalid login.";
                return Page();
            }

            var hasher = new PasswordHasher<UserAccount>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, Password);

            if (result == PasswordVerificationResult.Failed)
            {
                Message = "Invalid login.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Shop");
        }
    }
}