using System.Text.Json.Serialization;

namespace LocalSemanticKernel.Connectors.Model;

// ReSharper disable once ClassNeverInstantiated.Global
public class Choice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    
    [JsonPropertyName("message")]
    public Message Message { get; set; } = null!;
    
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = null!;
}