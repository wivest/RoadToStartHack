using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DELLight.Models;

public class ChatHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = null!;

    [JsonPropertyName("systemLanguage")]
    public string SystemLanguage { get; set; } = null!;

    [JsonPropertyName("history")]
    public List<Message> History { get; set; } = null!;
}

public record Message
{
    [JsonPropertyName("timestamp")]
    public DateTime SentAt { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;

    [JsonPropertyName("language")]
    public string Language { get; set; } = null!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}

public record GeneratedMessage
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("generated_message")]
    public string Content { get; set; } = null!;
}
