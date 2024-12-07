using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DELLight.Models;

public class ChatHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string UserId { get; set; } = null!;
    public string SystemLanguage { get; set; } = null!;
    public List<Message> History { get; set; } = null!;
}

public record Message
{
    public int MessageId { get; set; }
    public string Role { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string Content { get; set; } = null!;
}
