using System.ComponentModel.DataAnnotations;

public class UserKey
{
    [Key]
    public string Username { get; set; } = null!;
    public byte[] Salt { get; set; } = Array.Empty<byte>();
}
