using System.Text.RegularExpressions;

namespace OverkillDocs.Tests.Integration.Helpers
{
    public static partial class StringHelper
    {
        [GeneratedRegex(@"[^a-z0-9]")]
        private static partial Regex NonAlphaNumericRegex();

        public static string SanitizeUsername(string username)
        {
            return NonAlphaNumericRegex().Replace(username.ToLower(), string.Empty);
        }
    }
}
