namespace Test.Local.SemanticKernel.Connectors;

[TestClass]
public sealed class TestTextGenerationServices : AbstractConnectorTest
{
    [TestMethod]
    public async Task TestYesNoPrompt()
    {
        var result = await GenerateAsync("If red is a color respond yes, otherwise respond no");
        Assert.AreEqual("Yes", result);
    }
}