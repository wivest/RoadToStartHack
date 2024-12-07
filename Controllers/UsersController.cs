using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DELLight.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DELLight.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UsersController : ControllerBase
{
    [Authorize]
    [HttpPost]
    public ActionResult Me()
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
        string? claimEmail = null;
        foreach (Claim claim in claims.Claims)
            if (claim.Type == "login")
            {
                claimEmail = claim.Value;
                break;
            }
        if (claimEmail is null)
            return BadRequest();
        return new JsonResult(new { email = claimEmail });
    }
}
