using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LocalSemanticKernel.Connectors.Model;

/// <summary>
/// The request to send to the chat API
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// The id of the model
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = "";

    /// <summary>
    /// The list of messages
    /// </summary>
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; } = [];

    /// <summary>
    /// The temperature (between 0 and 1) to use
    /// </summary>
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.7f;

    /// <summary>
    /// The max number of tokens to use
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// Indicates if the response should be streamed
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;
}