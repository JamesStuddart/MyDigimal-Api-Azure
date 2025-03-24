namespace MyDigimal.Api.Azure.Models.Authentication
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        internal bool IsValid => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Username);
    }
}