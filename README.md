# LocalSemanticKernel

## Overview
`LocalSemanticKernel` allows you to easily use models running in LocalAI with SemanticKernel and DotNet.

This package currently provides support for the following services:
* [IChatCompletionService](https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.chatcompletion.ichatcompletionservice?view=semantic-kernel-dotnet)
* [ITextGenerationService](https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.textgeneration.itextgenerationservice?view=semantic-kernel-dotnet)
* [ITextEmbeddingGenerationService](https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.embeddings.itextembeddinggenerationservice?view=semantic-kernel-dotnet)

## Installation

To install the package, you can use the following command from the directory that contains your project file:

```bash
dotnet add package LocalSemanticKernel
```

## Usage

To use `LocalSemanticKernel` within an ASP.net web application, you need to add the following code to your `Program.cs` file:

```csharp
builder
    .Services
    .AddKernel()
    .AddLocalChatCompletion(
        modelId: "llama-3.2-3b-instruct:q4_k_m");
```

To use `LocalSemanticKernel` with Kernel Memory for RAG scenarios, you need to add the following code to your `Program.cs` file:

```csharp
builder.Services.AddKernelMemory<MemoryServerless>(memoryBuilder =>
{
    memoryBuilder
        .WithPostgresMemoryDb(
            new PostgresConfig()
            {
                ConnectionString = builder.Configuration.GetConnectionString("rag-db")!
            }
        )
        .WithSemanticKernelTextGenerationService(
            new LocalChatCompletionService(
                modelId: "llama-3.2-3b-instruct:q4_k_m"),
            new SemanticKernelConfig()
        )
        .WithSemanticKernelTextEmbeddingGenerationService(
            new LocalTextEmbeddingGenerationService(
                modelId: "bert-embeddings"),
            new SemanticKernelConfig()
        );
});
```

**Note:** You need to replace the `modelId` with the id of the actual model you want to use.

To use `LocalSemanticKernel` in a console application, you could do the following:

```csharp
var builder = Kernel.CreateBuilder()
                .AddLocalChatCompletion("llama-3.2-3b-instruct:q4_k_m");
            
var kernel = builder.Build();
var chatService = kernel.Services.GetRequiredService<IChatCompletionService>();

// Use the chat service
            
```

## License
    
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
