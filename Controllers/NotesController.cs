using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureNotes.Models;
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

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        var notes = await _noteService.GetNotesAsync(username);
        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Note note)
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        var csrfTokenFromHeader = Request.Headers["X-CSRF-TOKEN"].FirstOrDefault();
        if (string.IsNullOrEmpty(csrfTokenFromHeader))
        {
            return BadRequest("Missing CSRF token.");
        }

        var csrfTokenFromCookie = Request.Cookies["X-CSRF-TOKEN"];
        if (csrfTokenFromCookie != csrfTokenFromHeader)
        {
            return BadRequest("Invalid CSRF token.");
        }

        await _noteService.AddNoteAsync(note.Content, username);
        return CreatedAtAction(nameof(Get), new { id = note.Id }, "Note added successfully.");
    }

    [HttpPost("clear")]
    public async Task<IActionResult> ClearNotes()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var csrfTokenFromHeader = Request.Headers["X-CSRF-TOKEN"].FirstOrDefault();
        if (string.IsNullOrEmpty(csrfTokenFromHeader))
        {
            return BadRequest("Missing CSRF token.");
        }

        var csrfTokenFromCookie = Request.Cookies["X-CSRF-TOKEN"];
        if (csrfTokenFromCookie != csrfTokenFromHeader)
        {
            return BadRequest("Invalid CSRF token.");
        }

        await _noteService.ClearNotesForUserAsync(username);
        return Ok($"All notes cleared for {username}.");
    }
}
