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
