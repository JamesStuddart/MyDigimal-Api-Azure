using System.IdentityModel.Tokens.Jwt;

namespace MyDigimal.Core.Authentication.Models
{
    public class ValidatedToken
    {
        public bool IsValid { get; set; } 
        public SocialPlatform Platform { get; set; } = SocialPlatform.Undefined;
        public string Token { get; set; } = string.Empty;
        
        public JwtSecurityToken DecodedToken { get; set; }
    }
}