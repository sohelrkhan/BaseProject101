namespace SadaqaAccounting.Shared.Behaviours
{
    public static class GenerateUniqueNumber
    {
        private static readonly Random _random = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateUniqueString(this object obj)
        {
            // Generate a random 7-character alphanumeric string
            char[] stringChars = new char[7];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = _chars[_random.Next(_chars.Length)];
            }

            return new string(stringChars);
        }
    }
}