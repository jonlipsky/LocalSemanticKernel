using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Local.SemanticKernel.Connectors.Model;

/// <summary>
/// The request to send to the embedding API
/// </summary>
public class EmbeddingRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("input")] 
    public string Input { get; set; } = string.Empty;
}