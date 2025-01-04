using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Local.SemanticKernel.Connectors;

/// <summary>
/// A custom HttpMessageHandler that can be used to intercept and modify requests
/// </summary>
public class LocalHttpMessageHandler: HttpClientHandler
{
    private readonly string[] _urls = ["api.openai.com", "openai.azure.com"];
    private readonly string _baseUrl;

    /// <summary>
    /// Creates a new instance of the LocalHttpMessageHandler
    /// </summary>
    /// <param name="baseUrl"></param>
    public LocalHttpMessageHandler(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        // validate if the request.RequestUri is not null and request.RequestUri.Host is in urls
        if (request.RequestUri != null && IsRemoteUrl(request.RequestUri.Host))
        {
            // set request.RequestUri to a new Uri with the LLMUrl and request.RequestUri.PathAndQuery
            request.RequestUri = new Uri($"{_baseUrl}{request.RequestUri.PathAndQuery}");
        }

        return base.SendAsync(request, cancellationToken);
    }
    
    private bool IsRemoteUrl(string url) => Array.IndexOf(_urls, url) != -1;
}
