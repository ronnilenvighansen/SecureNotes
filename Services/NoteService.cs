using SecureNotes.Models;
using SecureNotes.Data;
using Microsoft.EntityFrameworkCore;

namespace SecureNotes.Services;

public class NoteService
{
    private readonly ApplicationDbContext _context;

    public NoteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Note>> GetNotesAsync(string owner)
    {
        return await _context.Notes.Where(n => n.Owner == owner).ToListAsync();
    }

    public async Task<Note> AddNoteAsync(string content, string owner)
    {
        var note = new Note { Content = content, Owner = owner };
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        return note;
    }
}
