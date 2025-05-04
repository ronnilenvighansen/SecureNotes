/*using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SecureNotes.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IAntiforgery _antiForgery;

    public LoginController(IAntiforgery antiForgery)
    {
        _antiForgery = antiForgery;
    }
    
    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = AuthenticateUser(model);
        if (user == null) return Unauthorized("Invalid login.");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Username)
        };

        var identity = new ClaimsIdentity(claims, "MyCookie");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("MyCookie", principal);

        var tokens = _antiForgery.GetAndStoreTokens(HttpContext);
        Response.Cookies.Append("X-CSRF-TOKEN", tokens.RequestToken!, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(30)
        });

        return Ok(new { Message = "Login successful", CsrfToken = tokens.RequestToken });
    }

    [Authorize]
    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        var csrfTokenFromHeader = Request.Headers["X-CSRF-TOKEN"].FirstOrDefault();
        if (string.IsNullOrEmpty(csrfTokenFromHeader))
            return BadRequest("Missing CSRF token.");

        var csrfTokenFromCookie = Request.Cookies["X-CSRF-TOKEN"];
        if (csrfTokenFromCookie != csrfTokenFromHeader)
            return BadRequest("Invalid CSRF token.");
   
        await HttpContext.SignOutAsync("MyCookie");
        return Ok();
    }

    private object AuthenticateUser(LoginModel model)
    {
        return new { Username = model.Username };
    }
}
*/