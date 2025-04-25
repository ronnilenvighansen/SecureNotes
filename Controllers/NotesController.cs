using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureNotes.Models;
using SecureNotes.Services;

namespace SecureNotes.Controllers;

[ApiController]
[Route("[controller]")]
// Fake
//[Authorize] 
public class NotesController : ControllerBase
{
    private readonly NoteService _noteService;

    public NotesController(NoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var username = "testuser";//User.Identity?.Name;
        //if (username == null) return Unauthorized();

        var notes = await _noteService.GetNotesAsync(username);
        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Note note)
    {
        var username = "testuser";//User.Identity?.Name;
        //if (username == null) return Unauthorized();

        var createdNote = await _noteService.AddNoteAsync(note.Content, username);
        return CreatedAtAction(nameof(Get), new { id = note.Id }, createdNote);
    }
}
