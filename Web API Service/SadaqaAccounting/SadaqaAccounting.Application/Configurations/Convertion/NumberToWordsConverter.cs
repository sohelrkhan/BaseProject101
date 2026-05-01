namespace SadaqaAccounting.Application.Configurations.Convertion
{
    public static class NumberToWordsConverter
    {
        private static readonly string[] Units =
            { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
          "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
          "Eighteen", "Nineteen" };

        private static readonly string[] Tens =
            { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public static string ConvertAmountToWords(decimal amount)
        {
            if (amount == 0)
                return "Zero Taka";

            long rupees = (long)amount;
            int paise = (int)((amount - rupees) * 100);

            string rupeesInWords = ConvertNumberToWords(rupees);
            string paiseInWords = paise > 0 ? $" and {ConvertNumberToWords(paise)} Paisa" : "";

            return $"{rupeesInWords} Taka {paiseInWords}";
        }

        private static string ConvertNumberToWords(long number)
        {
            if (number == 0) return "Zero";

            if (number < 0) return "Minus " + ConvertNumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += ConvertNumberToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += ConvertNumberToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertNumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertNumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "") words += "and ";

                if (number < 20)
                    words += Units[number];
                else
                {
                    words += Tens[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + Units[number % 10];
                }
            }

            return words.Trim();
        }
    }
}