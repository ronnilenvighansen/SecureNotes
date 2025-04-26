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

    public async Task<IEnumerable<Note>> GetNotesAsync(string owner)
    {
        var encryptedNotes = await _context.Notes.Where(n => n.Owner == owner).ToListAsync();
        
        foreach (var note in encryptedNotes)
        {
            note.Content = _encryptionService.Decrypt(note.Content, owner);
        }

        return encryptedNotes;
    }

    public async Task<Note> AddNoteAsync(string content, string owner)
    {
        var encryptedContent = _encryptionService.Encrypt(content, owner);        

        var note = new Note 
        { 
            Content = encryptedContent, 
            Owner = owner 
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public async Task ClearNotesForUserAsync(string owner)
    {
        var userNotes = _context.Notes.Where(n => n.Owner == owner);
        _context.Notes.RemoveRange(userNotes);
        await _context.SaveChangesAsync();
    }
}
