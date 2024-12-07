using System.Text.Json;
using DELLight.Models;
using DELLight.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DELLight.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ChatController(ChatService service) : ControllerBase
{
    private readonly ChatService service = service;

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Messages([FromBody] NewMessage message)
    {
        Credentials? credentials = Authorizer.GetCredentials(Request);
        if (credentials is null)
            return Unauthorized();

        ChatHistory? history = service.GetChatHistory(credentials.Email);
        if (history is null)
            return BadRequest();
        history.History.Add((Message)message);

        HttpResponseMessage? response = await service.GenerateAnswerAsync(history);
        if (response is null)
            return NotFound();

        Stream stream = response.Content.ReadAsStream();
        GeneratedMessage? generated = JsonSerializer.Deserialize<GeneratedMessage>(stream);
        if (generated == null || !generated.Success)
            return NotFound();
        return new JsonResult(generated);
    }
}
