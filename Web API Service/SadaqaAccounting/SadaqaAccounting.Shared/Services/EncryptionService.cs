namespace SadaqaAccounting.Shared.Services
{
    public static class EncryptionService 
    {
        // Static key and IV fields
        private static readonly byte[] _key = Encoding.UTF8.GetBytes("Wm5wrq9*8{bs@?ks?I!P5@g1W'O,Fmoe");
        private static readonly byte[] _iv = Encoding.UTF8.GetBytes("]e}VX8O4eh,gpy&x");

        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    byte[] encryptedBytes = msEncrypt.ToArray();
                    return BitConverter.ToString(encryptedBytes).Replace("-", "");
                }
            }
        }

        public static string Decrypt(string hexCipherText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hexCipherText))
                    return hexCipherText;

                // If it's a plain number string like "12345", just return its integer form as string
                if (int.TryParse(hexCipherText, out int number))
                {
                    return number.ToString();
                }

                // If it's not a valid hex string or length is odd, assume it's not encrypted
                if (hexCipherText.Length % 2 != 0 || !IsHexString(hexCipherText))
                {
                    return hexCipherText;
                }

                byte[] cipherBytes = ConvertHexToBytes(hexCipherText);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _key;
                    aesAlg.IV = _iv;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Decryption failed: {ex.Message}";
            }
        }

        private static byte[] ConvertHexToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                string byteValue = hex.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(byteValue, 16);
            }
            return bytes;
        }

        private static bool IsHexString(string input)
        {
            foreach (char c in input)
            {
                bool isHexDigit = (c >= '0' && c <= '9') ||
                                  (c >= 'A' && c <= 'F') ||
                                  (c >= 'a' && c <= 'f');
                if (!isHexDigit)
                    return false;
            }
            return true;
        }
    }
}