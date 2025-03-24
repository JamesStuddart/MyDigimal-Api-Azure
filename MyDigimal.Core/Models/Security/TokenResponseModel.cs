using System;

namespace MyDigimal.Core.Models.Security
{
    public class TokenResponseModel
    {
        public string Token { get; set; }
        public string TokenType { get; set; }
        public DateTime Expiry { get; set; }
    }
}