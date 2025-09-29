using System.Text.RegularExpressions;

namespace Compago.Common
{
    public static class StringExtension
    {
        public static bool IsColorCode(this string value)
        {
            return Regex.Match(value, "^#(?:[0-9a-fA-F]{3}){1,2}$", RegexOptions.IgnoreCase).Success;
        }
    }
}
