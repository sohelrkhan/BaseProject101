namespace SadaqaAccounting.Application.Configurations.Convertion
{
    public static class NumberToWordsConverterGlobal
    {
        public static string NumberToWordsWithDecimalUpper(decimal number)
        {
            if (number == 0)
                return "ZERO";

            long wholePart = (long)Math.Floor(number);
            int decimalPart = (int)((number - wholePart) * 100);

            string result = "";

            if (wholePart > 0)
                result += NumberToWords((int)wholePart);

            if (decimalPart > 0)
                result += " AND " + NumberToWords(decimalPart);

            return result.ToUpper(); // Convert entire string to uppercase
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "") words += "and ";

                string[] unitsMap = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                    "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen",
                    "seventeen", "eighteen", "nineteen" };

                string[] tensMap = { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty",
                    "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }
    }
}
