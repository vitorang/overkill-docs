using System.Text.RegularExpressions;

namespace OverkillDocs.Tests.Integration.Helpers;

public static partial class StringHelper
{
    [GeneratedRegex(@"[^a-z0-9]")]
    private static partial Regex NonAlphaNumericRegex();

    public static string SanitizeUsername(string username)
    {
        return NonAlphaNumericRegex().Replace(username.ToLower(), string.Empty).Truncate(15);
    }

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
