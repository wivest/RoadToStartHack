using DELLight.Models;
using DELLight.Services;
using Microsoft.AspNetCore.Mvc;

namespace DELLight.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController(AuthService service) : ControllerBase
{
    private readonly AuthService service = service;

    [HttpPost]
    public ActionResult Login([FromBody] Credentials credentials)
    {
        User? user = service.GetUser(credentials.Email, credentials.Password);
        if (user is null)
            return Unauthorized();

        return new JsonResult(
            new
            {
                accessToken = Authorizer.GenerateJWT(credentials.Email, credentials.Password),
                refreshToken = ""
            }
        );
    }
}

public record Credentials
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
