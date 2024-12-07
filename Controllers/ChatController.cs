using System.Text.Json;
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
            return NotFound();
        Stream stream = response.Content.ReadAsStream();
        GeneratedMessage? generated = JsonSerializer.Deserialize<GeneratedMessage>(stream);
        if (generated == null || !generated.Success)
            return NotFound();
        return new JsonResult(generated);
    }
}
