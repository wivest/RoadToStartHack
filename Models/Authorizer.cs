using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DELLight.Models;

public class Authorizer
{
    const string KEY = "mysupersecret_secretsecretsecretkey!!!42";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new(Encoding.UTF8.GetBytes(KEY));

    public static string GenerateJWT(string login, string password)
    {
        var claims = new List<Claim> { new("login", login), new("password", password) };
        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(7)),
            signingCredentials: new SigningCredentials(
                GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public static Credentials? GetCredentials(HttpRequest request)
    {
        string? token = request.Headers["Authorization"];
        if (token is null)
            return null;
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
            return null;

        return new Credentials { Email = login, Password = password };
    }
}

public record Credentials
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
