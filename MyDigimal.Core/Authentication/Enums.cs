using System.ComponentModel;

namespace MyDigimal.Core.Authentication
{
    public enum SocialPlatform
    {
        Undefined,
        [Description("")]
        Facebook,
        [Description("")]
        Instagram,
        [Description("accounts.google.com")]
        Google,
        [Description("")]
        Twitter,
        [Description("")]
        Microsoft,
        [Description("https://dev-whitefishcreative.eu.auth0.com/")]
        Auth0
    }
}