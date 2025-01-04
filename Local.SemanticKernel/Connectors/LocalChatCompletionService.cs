using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Local.SemanticKernel.Connectors.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;

namespace Local.SemanticKernel.Connectors;

/// <summary>
/// A chat completion service that talks to LocalAI
/// </summary>
public class LocalChatCompletionService : IChatCompletionService, ITextGenerationService
{
    private readonly string _modelId;
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly string _urlPath;

    /// <summary>
    /// Creates a new instance of the LocalChatCompletionService
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="urlPath"></param>
    public LocalChatCompletionService(
        string modelId, 
        string? apiKey, 
        HttpClient httpClient, 
        ILoggerFactory? loggerFactory = null,
        string? urlPath = null)
    {
        _modelId = modelId;
        _apiKey = apiKey;
        _httpClient = httpClient;
        _loggerFactory = loggerFactory;
        _urlPath = urlPath ?? "v1/chat/completions";
    }
    
    /// <summary>
    /// Creates a new instance of the LocalChatCompletionService
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="apiKey"></param>
    /// <param name="baseUrl"></param>
    /// <param name="loggerFactory"></param>
    public LocalChatCompletionService(
        string modelId, 
        string? apiKey = null, 
        string baseUrl = "http://localhost:8080",
        ILoggerFactory? loggerFactory = null) : this(modelId, apiKey, LocalBuilder.BuildHttpClient(baseUrl), loggerFactory)
    {
        
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
        ChatHistory chatHistory, 
        PromptExecutionSettings? executionSettings = null, 
        Kernel? kernel = null, 
        CancellationToken cancellationToken = default)
    {
        using (var request = new HttpRequestMessage(HttpMethod.Post, _urlPath))
        {
            var root = new ChatRequest();
            foreach (var message in chatHistory)
            {
                var msg = new Message
                {
                    Role = message.Role.ToString().ToLower(),
                    Content = message.Content ?? ""
                };
                root.Messages.Add(msg);
            }

            if (!string.IsNullOrEmpty(_modelId))
                root.Model = _modelId;

            // generate the json string from the root object
            var jsonString = JsonSerializer.Serialize(root);
            request.Content = new StringContent(jsonString);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var httpResponse = await _httpClient.SendAsync(request, cancellationToken);

            // get the response content
            var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            // deserialize the response content into a ChatResponse object
            var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseContent);

            if (chatResponse == null)
                throw new Exception("Failed to deserialize response content into ChatResponse object");
            
            var response = new List<ChatMessageContent>
            {
                new ChatMessageContent(
                    role: AuthorRole.Assistant, 
                    content: chatResponse.Choices[0].Message.Content,
                    modelId: chatResponse.Model,
                    innerContent: null,
                    encoding: null,
                    metadata: null)
            };

            return response;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
        ChatHistory chatHistory, 
        PromptExecutionSettings? executionSettings = null, 
        Kernel? kernel = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var chatMessageContents = await GetChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);
        for (var index = 0; index < chatMessageContents.Count; index++)
        {
            var chatMessageContent = chatMessageContents[index];
            if (!cancellationToken.IsCancellationRequested)
                yield return new StreamingChatMessageContent(
                    chatMessageContent.Role,
                    chatMessageContent.Content,
                    chatMessageContent.InnerContent,
                    index,
                    chatMessageContent.ModelId,
                    chatMessageContent.Encoding,
                    chatMessageContent.Metadata);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(
        string prompt, 
        PromptExecutionSettings? executionSettings = null, 
        Kernel? kernel = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        using (var request = new HttpRequestMessage(HttpMethod.Post, _urlPath))
        {
            var root = new ChatRequest();
            var msg = new Message
            {
                Role = "user",
                Content = prompt
            };
            root.Messages.Add(msg);

            if (executionSettings is LocalPromptExecutionSettings localPromptExecutionSettings)
            {
                root.Temperature = localPromptExecutionSettings.Temperature;
                root.MaxTokens = localPromptExecutionSettings.MaxTokens;
            }
            
            if (!string.IsNullOrEmpty(_modelId))
                root.Model = _modelId;

            // generate the json string from the root object
            var jsonString = JsonSerializer.Serialize(root);
            request.Content = new StringContent(jsonString);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var httpResponse = await _httpClient.SendAsync(request, cancellationToken);

            // get the response content
            var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            // deserialize the response content into a ChatResponse object
            var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseContent);

            if (chatResponse == null)
                throw new Exception("Failed to deserialize response content into ChatResponse object");
            
            var response = new List<TextContent>
            {
                new TextContent()
                {
                    Text = chatResponse.Choices[0].Message.Content
                }
            };

            return response;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(
        string prompt, 
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = new CancellationToken())
    {
        var response = await GetTextContentsAsync(prompt, executionSettings, kernel, cancellationToken);
        for (var index = 0; index < response.Count; index++)
        {
            var textContent = response[index];
            if (!cancellationToken.IsCancellationRequested)
                yield return new StreamingTextContent(
                    textContent.Text,
                    index);
        }
    }
}