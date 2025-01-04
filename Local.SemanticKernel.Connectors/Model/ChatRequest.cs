using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Local.SemanticKernel.Connectors.Model;

/// <summary>
/// The request to send to the chat API
/// </summary>
public class ChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "";

    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; } = [];

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.7f;

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 4096;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;
}