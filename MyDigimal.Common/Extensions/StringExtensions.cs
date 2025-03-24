using System.Text.RegularExpressions;

namespace MyDigimal.Common.Extensions
{
    public static class StringExtensions
    {
        public static string MaskedEmail(this string value)
        {
            if (!value.Contains("@"))
                return new string('*', value.Length);
            var pattern = @"(?<=[\w]{1})[\w-\._\+%\\]*(?=[\w]{1}@)|(?<=@[\w]{1})[\w-_\+%]*(?=\.)";
            return value.Split('@')[0].Length < 4 ? @"*@*.*" : Regex.Replace(value, pattern, m => new string('*', m.Length));
        }
    }
}