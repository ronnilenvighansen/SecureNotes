using SecureNotes.Models;
using SecureNotes.Data;
using Microsoft.EntityFrameworkCore;

namespace SecureNotes.Services;

public class NoteService
{
    private readonly ApplicationDbContext _context;
    private readonly EncryptionService _encryptionService;
    private readonly UserKeyService _userKeyService;


    public NoteService(ApplicationDbContext context, EncryptionService encryptionService, UserKeyService userKeyService)
    {
        _context = context;
        _encryptionService = encryptionService;
        _userKeyService = userKeyService;
    }

    public async Task<IEnumerable<NoteDto>> GetNotesAsync(string owner)
    {
        await _userKeyService.EnsureSaltExistsAsync(owner);
        
        var encryptedNotes = await _context.Notes.Where(n => n.Owner == owner).ToListAsync();
        
        var result = new List<NoteDto>();

        foreach (var note in encryptedNotes)
        {
            if (!await _encryptionService.VerifyHmacAsync(note.Content, note.Hmac, owner))
            {
                continue;
            }

            var decryptedContent = await _encryptionService.DecryptAsync(note.Content, owner);
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
        await _userKeyService.EnsureSaltExistsAsync(owner);

        var encryptedContent = await _encryptionService.EncryptAsync(content, owner);        
        
        var hmac = await _encryptionService.GenerateHmacAsync(encryptedContent, owner);

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
