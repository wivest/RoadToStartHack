using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DELLight.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DataController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public ActionResult Get()
    {
        return Ok();
    }
}
