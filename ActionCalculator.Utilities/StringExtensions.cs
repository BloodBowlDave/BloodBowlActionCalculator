using System.Text.RegularExpressions;

namespace ActionCalculator.Utilities
{
    public static class StringExtensions
    {
        public static string PascalCaseToSpaced(this string input) => 
            Regex.Replace(input, "([A-Z])", " $1").TrimStart();
    }
}
