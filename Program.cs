using SecureNotes.Data;
using Microsoft.EntityFrameworkCore;
using SecureNotes.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

//var connectionString = "Data Source=SecureNotes.db";
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
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
            ValidateIssuer = true,
            RoleClaimType = "roles",
            NameClaimType = "preferred_username"
        };
        
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var identity = context.Principal?.Identity as ClaimsIdentity;
                var realmAccessClaim = context.Principal?.FindFirst("realm_access");

                if (realmAccessClaim is not null)
                {
                    using var jsonDoc = JsonDocument.Parse(realmAccessClaim.Value);
                    if (jsonDoc.RootElement.TryGetProperty("roles", out var roles))
                    {
                        foreach (var role in roles.EnumerateArray())
                        {
                            identity?.AddClaim(new Claim("roles", role.GetString()!));
                        }
                    }
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7084, listenOptions =>
    {
        var certPassword = builder.Configuration["CertPassword"];
        listenOptions.UseHttps("certs/localhost.pfx", certPassword);
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
