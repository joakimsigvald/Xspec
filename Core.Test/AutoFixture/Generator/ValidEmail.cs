using System.Text.RegularExpressions;

namespace Xspec.Test.AutoFixture.Primitives;

public static partial class ValidEmail
{
    private static readonly Regex _emailRegex = EmailRegex2();
    public static bool Verify(string email) => _emailRegex.IsMatch(email);

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex2();
}