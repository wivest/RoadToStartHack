using System.Text;
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
    [HttpGet]
    public ActionResult<List<FlutterMessage>> Messages()
    {
        Credentials? credentials = Authorizer.GetCredentials(Request);
        if (credentials is null)
            return Unauthorized();

        ChatHistory? history = service.GetChatHistory(credentials.Email);
        if (history is null)
            return BadRequest();

        return ChatService.CastHistory(history.History);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<FlutterMessage>> Messages([FromBody] FlutterMessage message)
    {
        Credentials? credentials = Authorizer.GetCredentials(Request);
        if (credentials is null)
            return Unauthorized();

        ChatHistory? history = service.GetChatHistory(credentials.Email);
        if (history is null)
            return BadRequest();
        history.History.Add((Message)message);
        service.UpdateChatHistory(credentials.Email, history);

        HttpResponseMessage response = await service.GenerateAnswerAsync(history);

        Stream stream = response.Content.ReadAsStream();
        GeneratedMessage? generated = JsonSerializer.Deserialize<GeneratedMessage>(stream);
        if (generated == null || !generated.Success)
            return BadRequest();
        history.History.Add((Message)generated);
        service.UpdateChatHistory(credentials.Email, history);

        return (FlutterMessage)(Message)generated;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<FlutterMessage>> Transcript([FromForm] IFormFile file)
    {
        Credentials? credentials = Authorizer.GetCredentials(Request);
        if (credentials is null)
            return Unauthorized();

        ChatHistory? history = service.GetChatHistory(credentials.Email);
        if (history is null)
            return BadRequest();

        HttpResponseMessage response = await service.TranscriptAudio(file);

        Stream stream = response.Content.ReadAsStream();
        GeneratedMessage? generated = JsonSerializer.Deserialize<GeneratedMessage>(stream);
        if (generated == null || !generated.Success)
            return BadRequest();
        history.History.Add((Message)generated);
        service.UpdateChatHistory(credentials.Email, history);

        return (FlutterMessage)(Message)generated;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Translate([FromBody] FlutterMessage message)
    {
        HttpResponseMessage response = await service.TranslateMessage(message.Content);
        Stream stream = response.Content.ReadAsStream();
        GeneratedMessage? generated = JsonSerializer.Deserialize<GeneratedMessage>(stream);
        if (generated == null || !generated.Success)
            return BadRequest();
        return new FileContentResult(Convert.FromBase64String(generated.Content), "audio/wav");
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Visualize([FromBody] FlutterMessage message)
    {
        HttpResponseMessage response = await service.VisualizeMessage(message.Content);
        Stream stream = response.Content.ReadAsStream();
        GeneratedMessage? generated = JsonSerializer.Deserialize<GeneratedMessage>(stream);
        if (generated == null || !generated.Success)
            return BadRequest();
        return new FileContentResult(Convert.FromBase64String(generated.Content), "video/mp4");
    }
}
