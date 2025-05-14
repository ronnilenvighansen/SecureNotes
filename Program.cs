using SecureNotes.Data;
using Microsoft.EntityFrameworkCore;
using SecureNotes.Services;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Data Source=SecureNotes.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<EncryptionService>();

builder.Services.AddScoped<NoteService>();

builder.Services.AddScoped<UserKeyService>();

builder.Services.AddControllers();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:8443/realms/SecureNotes";
        options.Audience = "secure-notes-client";
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, 
            NameClaimType = "preferred_username"
        };
    });


builder.Services.AddAuthorization();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7084, listenOptions =>
    {
        listenOptions.UseHttps("certs/localhost.pfx", "password");
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
