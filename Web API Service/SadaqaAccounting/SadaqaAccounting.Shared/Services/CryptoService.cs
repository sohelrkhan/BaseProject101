namespace SadaqaAccounting.Shared.Services
{
    public class CryptoService
    {
        private readonly string _encryptionKey = "Your_Secret_Key_32_Char"; // Store securely in appsettings or secrets

        public string Encrypt(string text)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
            aes.GenerateIV();
            var iv = aes.IV;

            using var encryptor = aes.CreateEncryptor();
            var textBytes = Encoding.UTF8.GetBytes(text);
            var encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

            var result = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string encryptedText)
        {
            var fullBytes = Convert.FromBase64String(encryptedText);
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);

            var iv = fullBytes.Take(16).ToArray();
            var cipherText = fullBytes.Skip(16).ToArray();

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
