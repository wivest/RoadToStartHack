using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DELLight.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DELLight.Services;

public class ChatService
{
    private readonly IMongoCollection<ChatHistory> collection;
    private readonly string generateEndpoint;
    private readonly string transcriptEndpoint;
    private readonly string translateEndpoint;
    private readonly string visualizeEndpoint;

    public ChatService(IOptions<ChatDatabaseSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        collection = mongoDatabase.GetCollection<ChatHistory>(settings.Value.ChatCollectionName);
        generateEndpoint = settings.Value.GenerateEndpoint;
        transcriptEndpoint = settings.Value.TranscriptEndpoint;
        translateEndpoint = settings.Value.TranslateEndpoint;
        visualizeEndpoint = settings.Value.VisualizeEndpoint;
    }

    public ChatHistory? GetChatHistory(string userId)
    {
        return collection.Find(history => history.UserId == userId).FirstOrDefault();
    }

    public void UpdateChatHistory(string userId, ChatHistory newHistory)
    {
        collection.ReplaceOne(history => history.UserId == userId, newHistory);
    }

    public async Task<HttpResponseMessage> GenerateAnswerAsync(ChatHistory history)
    {
        using var client = new HttpClient();
        string json = JsonSerializer.Serialize(history);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await client.PostAsync(generateEndpoint, content);
    }

    public async Task<HttpResponseMessage> TranscriptAudio(IFormFile file)
    {
        using var client = new HttpClient();
        var content = new MultipartFormDataContent();

        Stream stream = file.OpenReadStream();
        var byteArrayContent = new StreamContent(stream);
        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        content.Add(byteArrayContent, "file", "15.wav");

        return await client.PostAsync(transcriptEndpoint, content);
    }

    public async Task<HttpResponseMessage> TranslateMessage(string message)
    {
        using var client = new HttpClient();
        string json = JsonSerializer.Serialize(new { text = message, language = "en" }); // TODO: replace language
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await client.PostAsync(translateEndpoint, content);
    }

    public async Task<HttpResponseMessage> VisualizeMessage(string message)
    {
        using var client = new HttpClient();
        string json = JsonSerializer.Serialize(new { text = message, language = "en" }); // TODO: replace language
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await client.PostAsync(visualizeEndpoint, content);
    }

    public static List<FlutterMessage> CastHistory(List<Message> messages)
    {
        var casted = new List<FlutterMessage>();

        foreach (Message message in messages)
            casted.Add((FlutterMessage)message);

        return casted;
    }
}
