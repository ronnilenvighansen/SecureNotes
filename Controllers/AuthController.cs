using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login()
    {
        var authProps = new AuthenticationProperties
        {
            RedirectUri = "/" // or your frontend route
        };
        return Challenge(authProps, "oidc");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        var authProps = new AuthenticationProperties
        {
            RedirectUri = "/" // or your frontend route
        };
        return SignOut(authProps,
            CookieAuthenticationDefaults.AuthenticationScheme,
            "oidc");
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(User.Identity?.Name);
    }
}
