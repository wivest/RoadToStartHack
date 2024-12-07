using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DELLight.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}
