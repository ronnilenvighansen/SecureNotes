using System.Security.Cryptography;
using System.Text;

namespace SecureNotes.Services
{
    public class EncryptionService
    {
        public string Encrypt(string plainText, string username)
        {
            var key = GetKey(username);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            var iv = aes.IV;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var combinedBytes = new byte[iv.Length + cipherBytes.Length];
            Buffer.BlockCopy(iv, 0, combinedBytes, 0, iv.Length);
            Buffer.BlockCopy(cipherBytes, 0, combinedBytes, iv.Length, cipherBytes.Length);

            return Convert.ToBase64String(combinedBytes);
        }

        public string Decrypt(string encryptedText, string username)
        {
            var key = GetKey(username);

            var combinedBytes = Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();
            aes.Key = key;

            var iv = new byte[aes.BlockSize / 8];
            Buffer.BlockCopy(combinedBytes, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = new byte[combinedBytes.Length - iv.Length];
            Buffer.BlockCopy(combinedBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }

        private byte[] GetKey(string username)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(username));
        }
    }
}
