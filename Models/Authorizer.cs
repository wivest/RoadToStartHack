using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DELLight.Models;

public class Authorizer
{
    public const string ISSUER = "42";
    public const string AUDIENCE = "DELLight";
    const string KEY = "mysupersecret_secretsecretsecretkey!!!42";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new(Encoding.UTF8.GetBytes(KEY));

    public static string GenerateJWT(string login, string password)
    {
        var claims = new List<Claim> { new(login, password) };
        var jwt = new JwtSecurityToken(
            issuer: ISSUER,
            audience: AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(7)),
            signingCredentials: new SigningCredentials(
                GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
