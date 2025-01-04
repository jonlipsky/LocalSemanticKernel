using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Local.SemanticKernel.Connectors.Model;

/// <summary>
/// The response received from the embedding API
/// </summary>
public class EmbeddingResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    
    [JsonPropertyName("object")]
    public string Object { get; init; } = null!;
    
    [JsonPropertyName("created")]
    public int Created { get; init; }
    
    [JsonPropertyName("model")]
    public string Model { get; init; } = null!;
    
    [JsonPropertyName("data")]
    public List<EmbeddingData> Data { get; init; } = null!;
    
    [JsonPropertyName("usage")]
    public Usage Usage { get; init; } = null!;
}