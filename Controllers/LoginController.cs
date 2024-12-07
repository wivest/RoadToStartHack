using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DELLight.Models;
using DELLight.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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

    [Authorize]
    [HttpPost]
    public ActionResult RefreshToken()
    {
        Credentials? credentials = Authorizer.GetCredentials(Request);
        if (credentials is null)
            return Unauthorized();

        return Login(
            new Credentials { Email = credentials.Email, Password = credentials.Password }
        );
    }
}
