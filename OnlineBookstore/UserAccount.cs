namespace OnlineBookstore
{
    public class UserAccount
    {
        public int UserID { get; set; }
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
    }
}
