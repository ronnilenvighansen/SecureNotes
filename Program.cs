using SecureNotes.Data;
using Microsoft.EntityFrameworkCore;
using SecureNotes.Services;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Data Source=SecureNotes.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<NoteService>();

builder.Services.AddControllers();

// Fake
builder.Services.AddAuthentication("MyCookie")
    .AddCookie("MyCookie", options =>
    {
        options.LoginPath = "/login"; 
    });

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


// Fake
app.MapPost("/login", (HttpContext http, string username) =>
{
    var claims = new[] { new System.Security.Claims.Claim("name", username) };
    var identity = new System.Security.Claims.ClaimsIdentity(claims, "MyCookie");
    var principal = new System.Security.Claims.ClaimsPrincipal(identity);
    return http.SignInAsync("MyCookie", principal);
});

app.MapPost("/logout", (HttpContext http) =>
{
    return http.SignOutAsync("MyCookie");
});

app.Run();
