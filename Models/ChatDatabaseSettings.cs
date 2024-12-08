namespace DELLight.Models;

public class ChatDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string ChatCollectionName { get; set; } = null!;
    public string GenerateEndpoint { get; set; } = null!;
    public string TranscriptEndpoint { get; set; } = null!;
    public string TranslateEndpoint { get; set; } = null!;
}
