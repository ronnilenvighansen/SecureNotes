using SecureNotes.Models;
using SecureNotes.Data;
using Microsoft.EntityFrameworkCore;

namespace SecureNotes.Services;

public class NoteService
{
    private readonly ApplicationDbContext _context;
    private readonly EncryptionService _encryptionService;

    public NoteService(ApplicationDbContext context, EncryptionService encryptionService)
    {
        _context = context;
        _encryptionService = encryptionService;
    }

    public async Task<IEnumerable<NoteDto>> GetNotesAsync(string owner)
    {
        var encryptedNotes = await _context.Notes.Where(n => n.Owner == owner).ToListAsync();
        
        var result = new List<NoteDto>();

        foreach (var note in encryptedNotes)
        {
            var decryptedContent = _encryptionService.Decrypt(note.Content, owner);
            result.Add(new NoteDto
            {
                Id = note.Id,
                Content = decryptedContent,
                Owner = note.Owner
            });
        }

        return result;
    }

    public async Task AddNoteAsync(string content, string owner)
    {
        var encryptedContent = _encryptionService.Encrypt(content, owner);        
        
        var hmac = _encryptionService.GenerateHmac(encryptedContent, owner);

        var note = new Note 
        { 
            Content = encryptedContent, 
            Hmac = hmac,
            Owner = owner 
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
    }

    public async Task ClearNotesForUserAsync(string owner)
    {
        var userNotes = _context.Notes.Where(n => n.Owner == owner);
        _context.Notes.RemoveRange(userNotes);
        await _context.SaveChangesAsync();
    }
}
