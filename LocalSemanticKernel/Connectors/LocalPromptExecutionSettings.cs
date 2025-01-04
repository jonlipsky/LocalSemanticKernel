using Microsoft.SemanticKernel;

namespace LocalSemanticKernel.Connectors;

/// <summary>
/// The settings for executing a prompt locally
/// </summary>
public class LocalPromptExecutionSettings : PromptExecutionSettings
{
    /// <summary>
    /// Default: 0.7 What sampling temperature to use, between 0.0 and 1.0. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.
    /// </summary>
    public float Temperature { get; set; } = 0.7f;
    
    
    /// <summary>
    /// The token count of your prompt plus max_tokens cannot exceed the model's context length.
    /// </summary>
    public int MaxTokens { get; set; } = 4096;
}