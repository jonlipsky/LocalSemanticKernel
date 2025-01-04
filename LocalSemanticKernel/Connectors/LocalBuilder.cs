using System;
using System.Collections.Concurrent;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextGeneration;

#pragma warning disable SKEXP0001

namespace LocalSemanticKernel.Connectors;

/// <summary>
/// A class that provides extension methods for adding local services to the kernel.
/// </summary>
public static class LocalBuilder
{
    private static ConcurrentDictionary<string, HttpClient>? _httpClients;

    /// <summary>
    /// Adds a local chat completion service to the kernel.
    /// </summary>
    /// <param name="builder">The kernel builder</param>
    /// <param name="modelId">The id of the model to use</param>
    /// <param name="apiKey">The api key (optional)</param>
    /// <param name="baseUrl">The base URL (defaults to http://localhost:8080)</param>
    /// <param name="serviceId">The service id (optional)</param>
    /// <param name="timeout">The HTTP client timeout (defaults to 10 minutes)</param>
    /// <returns></returns>
    public static IKernelBuilder AddLocalChatCompletion(
        this IKernelBuilder builder,
        string modelId,
        string? apiKey = null,
        string baseUrl = "http://localhost:8080",
        string? serviceId = null,
        TimeSpan? timeout = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelId);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);

        var httpClient = BuildHttpClient(baseUrl, timeout);

        LocalChatCompletionService Factory(IServiceProvider serviceProvider, object? _) =>
            new(modelId, apiKey, httpClient, serviceProvider.GetService<ILoggerFactory>());
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (Func<IServiceProvider, object?, LocalChatCompletionService>)Factory);
        builder.Services.AddKeyedSingleton<ITextGenerationService>(serviceId, (Func<IServiceProvider, object?, LocalChatCompletionService>)Factory);
        
        return builder;
    }
    
    /// <summary>
    /// Adds a local text embedding generation service to the kernel.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="modelId"></param>
    /// <param name="apiKey"></param>
    /// <param name="baseUrl"></param>
    /// <param name="serviceId"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static IKernelBuilder AddLocalTextEmbeddingGeneration(
        this IKernelBuilder builder,
        string modelId,
        string? apiKey = null,
        string baseUrl = "http://localhost:8080",
        string? serviceId = null,
        TimeSpan? timeout = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelId);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);

        var httpClient = BuildHttpClient(baseUrl, timeout);

        LocalTextEmbeddingGenerationService Factory(IServiceProvider serviceProvider, object? _) =>
            new(modelId, apiKey, httpClient, serviceProvider.GetService<ILoggerFactory>());
        builder.Services.AddKeyedSingleton<ITextEmbeddingGenerationService>(serviceId, (Func<IServiceProvider, object?, LocalTextEmbeddingGenerationService>)Factory);
        
        return builder;
    }
    
    internal static HttpClient BuildHttpClient(
        string baseUrl,
        TimeSpan? timeout = null)
    {
        _httpClients ??= new ConcurrentDictionary<string, HttpClient>();

        if (baseUrl.EndsWith('/'))
            baseUrl = baseUrl[..^1];
        
        if (!_httpClients.TryGetValue(baseUrl, out var client))
        {
            client = new HttpClient(new LocalHttpMessageHandler(baseUrl))
            {
                BaseAddress = new Uri(baseUrl),
                Timeout =  timeout ?? TimeSpan.FromMinutes(10)
            };
        }

        _httpClients.TryAdd(baseUrl, client);
        return client;
    }
}