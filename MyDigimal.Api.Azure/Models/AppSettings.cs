using MyDigimal.Core.Authentication;

namespace MyDigimal.Api.Azure.Models
{
    public class AppSettings
    {
        public IEnumerable<SocialPlatform> AvailableLoginTypes { get; set; } = [];
    }
}