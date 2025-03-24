namespace MyDigimal.Core.Authentication.Models
{
    public class Auth0Settings
    {
        public string AuthorityEndpoint { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string ActionSecret { get; set; }
    }
}