using System.Text.Json.Serialization;

namespace LocalSemanticKernel.Connectors.Model;

public class EmbeddingData
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    
    [JsonPropertyName("object")]
    public string Object { get; set; } = null!;
    
    [JsonPropertyName("embedding")]
    public float[] Embedding { get; set; } = null!;
}