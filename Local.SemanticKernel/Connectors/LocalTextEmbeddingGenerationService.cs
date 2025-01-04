using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Local.SemanticKernel.Connectors.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
#pragma warning disable SKEXP0001

namespace Local.SemanticKernel.Connectors;

/// <summary>
/// A text embedding generation service that talks to LocalAI
/// </summary>
public class LocalTextEmbeddingGenerationService : ITextEmbeddingGenerationService
{
    private readonly string _modelId;
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly string _urlPath;

    /// <summary>
    /// Creates a new instance of the LocalTextEmbeddingGenerationService
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="apiKey"></param>
    /// <param name="httpClient"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="urlPath"></param>
    public LocalTextEmbeddingGenerationService(
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
        _urlPath = urlPath ?? "v1/embeddings";
    }
    
    /// <summary>
    /// Creates a new instance of the LocalTextEmbeddingGenerationService
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="apiKey"></param>
    /// <param name="baseUrl"></param>
    /// <param name="loggerFactory"></param>
    public LocalTextEmbeddingGenerationService(
        string modelId, 
        string apiKey = "API_KEY", 
        string baseUrl = "http://localhost:8080",
        ILoggerFactory? loggerFactory = null) : this(modelId, apiKey, LocalBuilder.BuildHttpClient(baseUrl), loggerFactory)
    {
        
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

    /// <inheritdoc />
    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(
        IList<string> data, 
        Kernel? kernel = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _urlPath))
        {
            var embeddingRequest = new EmbeddingRequest();

            if (data.Count == 1)
            {
                embeddingRequest.Input = data[0];
            }
            else
            {
                var writer = new StringWriter();

                for (var i = 0; i < data.Count; i++)
                {
                    if (i > 0)
                        await writer.WriteAsync("\n").ConfigureAwait(false);
                    
                    var message = data[i];
                    await writer.WriteAsync(message).ConfigureAwait(false);
                }

                embeddingRequest.Input = writer.ToString();
            }
            
            if (!string.IsNullOrEmpty(_modelId))
                embeddingRequest.Model = _modelId;

            // generate the json string from the root object
            var jsonString = JsonSerializer.Serialize(embeddingRequest);
            httpRequestMessage.Content = new StringContent(jsonString);
            httpRequestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var httpResponse = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);

            // get the response content
            var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            // deserialize the response content into a ChatResponse object
            var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(responseContent);

            if (embeddingResponse == null)
                throw new Exception("Failed to deserialize response content into EmbeddingResponse object");

            var response = new List<ReadOnlyMemory<float>>();
            foreach (var embeddingData in embeddingResponse.Data)
            {
                response.Add(embeddingData.Embedding);
            }
            
            return response;
        }
    }
}