namespace SecureNotes.Models;

public class Note
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
}
