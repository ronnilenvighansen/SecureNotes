using SecureNotes.Data;
using Microsoft.EntityFrameworkCore;
using SecureNotes.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Data Source=SecureNotes.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddSingleton<EncryptionService>();

builder.Services.AddScoped<NoteService>();

builder.Services.AddControllers();

// Fake
builder.Services.AddAuthentication("MyCookie")
    .AddCookie("MyCookie", options =>
    {
        options.LoginPath = "/login"; 
    });

builder.Services.AddAuthorization();

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
