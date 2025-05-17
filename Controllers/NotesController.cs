using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureNotes.Services;

namespace SecureNotes.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize] 
public class NotesController : ControllerBase
{
    private readonly NoteService _noteService;    

    public NotesController(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    [Authorize(Roles = "viewer")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        var notes = await _noteService.GetNotesAsync(username);
        return Ok(notes);
    }
    
    [Authorize(Roles = "writer")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateNoteDto note)
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        await _noteService.AddNoteAsync(note.Content, username);
        return CreatedAtAction(nameof(Get), new { }, "Note added successfully.");
    }

    [Authorize(Roles = "deleter")]
    [HttpPost("clear")]
    public async Task<IActionResult> ClearNotes()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        await _noteService.ClearNotesForUserAsync(username);
        return Ok($"All notes cleared for {username}.");
    }
}
