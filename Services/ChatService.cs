using System.Text;
using System.Text.Json;
using DELLight.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DELLight.Services;

public class ChatService
{
    private readonly IMongoCollection<ChatHistory> collection;
    private readonly string mlEndpoint;

    public ChatService(IOptions<ChatDatabaseSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
        collection = mongoDatabase.GetCollection<ChatHistory>(settings.Value.ChatCollectionName);
        mlEndpoint = settings.Value.MLEndpoint;
    }

    public ChatHistory? GetChatHistory(string userId)
    {
        return collection.Find(history => history.UserId == userId).FirstOrDefault();
    }

    public void UpdateChatHistory(string userId, ChatHistory newHistory)
    {
        collection.ReplaceOne(history => history.UserId == userId, newHistory);
    }

    public async Task<HttpResponseMessage?> GenerateAnswerAsync(ChatHistory history)
    {
        using var client = new HttpClient();
        string json = JsonSerializer.Serialize(history);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(mlEndpoint, content);
        if (response.IsSuccessStatusCode)
            return response;
        else
            return response;
    }

    public static List<FlutterMessage> CastHistory(List<Message> messages)
    {
        var casted = new List<FlutterMessage>();

        foreach (Message message in messages)
            casted.Add((FlutterMessage)message);

        return casted;
    }
}
