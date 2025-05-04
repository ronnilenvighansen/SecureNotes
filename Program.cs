using SecureNotes.Data;
using Microsoft.EntityFrameworkCore;
using SecureNotes.Services;
using SecureNotes.Controllers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Data Source=SecureNotes.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddSingleton<EncryptionService>();

builder.Services.AddScoped<NoteService>();

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://localhost:8080/realms/SecureNotes";
    options.ClientId = "secure-notes-client";
    options.ClientSecret = "AmEGHjQ7DDfhMM6nVEed5E7nXjhMTint";
    options.ResponseType = "code";

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.TokenValidationParameters.NameClaimType = "preferred_username";

    options.RequireHttpsMetadata = true;
});

builder.Services.AddAuthorization();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "X-CSRF-TOKEN"; 
    options.Cookie.HttpOnly = false; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 

});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
