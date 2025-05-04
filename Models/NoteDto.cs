namespace SecureNotes.Models;

public class NoteDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
}
