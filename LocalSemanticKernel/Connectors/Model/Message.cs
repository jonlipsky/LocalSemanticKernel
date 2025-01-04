using System.Text.Json.Serialization;

namespace LocalSemanticKernel.Connectors.Model;

// ReSharper disable once ClassNeverInstantiated.Global
public class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;
    
    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}