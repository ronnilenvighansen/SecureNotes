using Microsoft.EntityFrameworkCore;
using SecureNotes.Models;

namespace SecureNotes.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Note> Notes { get; set; }
    public DbSet<UserKey> UserKeys { get; set; }
}
