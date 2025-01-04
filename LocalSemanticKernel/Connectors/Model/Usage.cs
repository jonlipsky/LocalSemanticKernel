using System.Text.Json.Serialization;

namespace LocalSemanticKernel.Connectors.Model;

// ReSharper disable once ClassNeverInstantiated.Global
public class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }
    
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }
    
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}