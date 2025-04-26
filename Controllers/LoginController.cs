using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SecureNotes.Controllers;

[ApiController]
public class LoginController : ControllerBase
{
    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required.");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };

        var identity = new ClaimsIdentity(claims, "MyCookie");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("MyCookie", principal);

        return Ok($"Logged in as {username}");
    }

    [Authorize]
    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MyCookie");
        return Ok();
    }
}
