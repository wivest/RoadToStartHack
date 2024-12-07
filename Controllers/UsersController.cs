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
        Credentials? credentials = Authorizer.GetCredentials(Request);
        if (credentials is null)
            return Unauthorized();
        return new JsonResult(new { email = credentials.Email });
    }
}
