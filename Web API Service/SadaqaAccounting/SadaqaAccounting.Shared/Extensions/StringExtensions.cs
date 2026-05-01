namespace SadaqaAccounting.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string ToSentenceCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var result = System.Text.RegularExpressions.Regex
                .Replace(input, "([A-Z])", " $1") 
                .Trim(); 

            return char.ToUpper(result[0]) + result.Substring(1); 
        }
    }
}