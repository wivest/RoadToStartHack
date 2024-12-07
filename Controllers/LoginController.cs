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
        string? token = Request.Headers["Authorization"];
        if (token is null)
            return Unauthorized();
        token = token.Replace("Bearer ", "");

        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            IssuerSigningKey = Authorizer.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };

        var claims = handler.ValidateToken(token, parameters, out var _);
        string? login = null;
        string? password = null;
        foreach (Claim claim in claims.Claims)
        {
            if (claim.Type == "login")
                login = claim.Value;
            if (claim.Type == "password")
                password = claim.Value;
        }
        if (login is null || password is null)
            return BadRequest();

        return Login(new Credentials { Email = login, Password = password });
    }
}

public record Credentials
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
