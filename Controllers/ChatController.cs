using DELLight.Models;
using DELLight.Services;
using Microsoft.AspNetCore.Mvc;

namespace DELLight.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ChatController(ChatService service) : ControllerBase
{
    private readonly ChatService service = service;

    [HttpPost]
    public async Task<ActionResult> Ask()
    {
        HttpResponseMessage? response = await service.GenerateAnswerAsync(
            new ChatHistory
            {
                UserId = "1234",
                SystemLanguage = "English",
                History = []
            }
        );
        if (response is null)
            return BadRequest();
        return Ok();
    }
}
