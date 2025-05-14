using System.Security.Cryptography;
using SecureNotes.Data;

public class UserKeyService
{
    private readonly ApplicationDbContext _context;

    public UserKeyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task EnsureSaltExistsAsync(string username)
    {
        var existing = await _context.UserKeys.FindAsync(username);
        if (existing != null) return;

        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[16];
        rng.GetBytes(salt);

        var userKey = new UserKey
        {
            Username = username,
            Salt = salt
        };

        _context.UserKeys.Add(userKey);
        await _context.SaveChangesAsync();
    }

    public async Task<byte[]> GetSaltAsync(string username)
    {
        var key = await _context.UserKeys.FindAsync(username);
        return key?.Salt ?? throw new InvalidOperationException("Salt not found for user.");
    }
}
