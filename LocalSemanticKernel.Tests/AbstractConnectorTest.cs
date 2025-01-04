using LocalSemanticKernel.Connectors;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextGeneration;
#pragma warning disable SKEXP0001

namespace Test.Local.SemanticKernel.Connectors;

public abstract class AbstractConnectorTest
{
    private Kernel? _kernel;
    private PromptExecutionSettings? _promptExecutionSettings;

    protected async Task<string?> ChatAsync(string promptText)
    {
        // Create a history store the conversation
        var history = new ChatHistory();
        
        // Get the initial prompt and add it to the history
        history.AddUserMessage(promptText);
       
        var kernel = GetOrCreateKernel();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        
        // Get the response from the AI
        var result = await ChatCompletionServiceExtensions.GetChatMessageContentAsync(chatCompletionService, history,
            executionSettings: GetOrCreatePromptExecutionSettings(),
            kernel: kernel);

        return result.Content;
    }
    
    protected async Task<string?> GenerateAsync(string promptText)
    {
        var kernel = GetOrCreateKernel();
        var textGenerationService = kernel.GetRequiredService<ITextGenerationService>();

        var result = await textGenerationService.GetTextContentAsync(
            promptText,
            GetOrCreatePromptExecutionSettings(), 
            kernel);

        return result.Text;
    }
    
    protected async Task<IList<ReadOnlyMemory<float>>?> GenerateEmbeddingsAsync(params string[] values)
    {
        var kernel = GetOrCreateKernel();
        var textEmbeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

        var result = await textEmbeddingGenerationService.GenerateEmbeddingsAsync(
            new List<string>(values), 
            kernel);

        return result;
    }

    private PromptExecutionSettings GetOrCreatePromptExecutionSettings()
    {
        return _promptExecutionSettings ??= new PromptExecutionSettings();
    }

    private Kernel GetOrCreateKernel()
    {
        if (_kernel == null)
        {
            var builder = Kernel.CreateBuilder()
                .AddLocalChatCompletion("llama-3.2-3b-instruct:q4_k_m")
                .AddLocalTextEmbeddingGeneration("bert-embeddings");
            
            _kernel = builder.Build();
        }

        return _kernel;
    }
}