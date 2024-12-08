using System.Text.Json.Serialization;

namespace DELLight.Models;

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

    public static explicit operator Message(FlutterMessage message)
    {
        DateTime sentAt = DateTimeOffset.FromUnixTimeMilliseconds(message.Timestamp).DateTime;
        string role = message.Role == 0 ? "user" : "assistant";
        return new Message
        {
            SentAt = sentAt,
            Role = role,
            Language = message.Locale,
            Content = message.Content
        };
    }

    public static explicit operator Message(GeneratedMessage message)
    {
        return new Message
        {
            SentAt = DateTime.UtcNow,
            Role = "assistant",
            Language = "en", //TODO: replace hardcoded
            Content = message.Content
        };
    }
}

public record GeneratedMessage
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("generated_message")]
    public string Content { get; set; } = null!;
}

public record FlutterMessage
{
    public long Timestamp { get; set; }
    public string Locale { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int Role { get; set; }

    public static explicit operator FlutterMessage(Message message)
    {
        long timestamp = ((DateTimeOffset)message.SentAt).ToUnixTimeMilliseconds();
        int role = message.Role == "user" ? 0 : 1;
        return new FlutterMessage
        {
            Timestamp = timestamp,
            Locale = message.Language,
            Content = message.Content,
            Role = role
        };
    }
}
