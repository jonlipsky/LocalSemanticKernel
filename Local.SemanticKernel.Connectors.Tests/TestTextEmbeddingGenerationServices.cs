namespace Test.Local.SemanticKernel.Connectors;

[TestClass]
public sealed class TestTextEmbeddingGenerationServices : AbstractConnectorTest
{
    [TestMethod]
    public async Task TestQuickBrownFox()
    {
        var result = await GenerateEmbeddingsAsync("The quick brown fox jumps over the lazy dog.");
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);
    }
}