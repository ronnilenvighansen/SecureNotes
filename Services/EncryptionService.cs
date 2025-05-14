using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SecureNotes.Data;

namespace SecureNotes.Services
{
    public class EncryptionService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserKeyService _userKeyService;
        public EncryptionService(ApplicationDbContext context, UserKeyService userKeyService)
        {
            _context = context;
            _userKeyService = userKeyService;
        }

        public async Task<string> EncryptAsync(string plainText, string username)
        {
            await _userKeyService.EnsureSaltExistsAsync(username);
            var key = await GetKeyAsync(username);

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

        public async Task<string> DecryptAsync(string encryptedText, string username)
        {
            await _userKeyService.EnsureSaltExistsAsync(username);
            var key = await GetKeyAsync(username);

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

        /*private byte[] GetKey(string username)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(username));
        }*/

        private async Task<byte[]> GetKeyAsync(string username)
        {
            await _userKeyService.EnsureSaltExistsAsync(username);
            var userKey = await _context.UserKeys.FirstOrDefaultAsync(u => u.Username == username);
            if (userKey == null)
                throw new InvalidOperationException("User salt not found. You must register the user key first.");

            using var pbkdf2 = new Rfc2898DeriveBytes(username, userKey.Salt, 100_000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        public async Task<string> GenerateHmacAsync(string data, string username)
        {
            await _userKeyService.EnsureSaltExistsAsync(username);
            var key = await GetKeyAsync(username); 
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }

        public async Task<bool> VerifyHmacAsync(string data, string expectedHmac, string username)
        {
            await _userKeyService.EnsureSaltExistsAsync(username);
            var actualHmac = await GenerateHmacAsync(data, username);
            return actualHmac == expectedHmac;
        }
    }
}
