namespace Test.Local.SemanticKernel.Connectors;

[TestClass]
public sealed class TestChatCompletionServices : AbstractConnectorTest
{
    [TestMethod]
    public async Task TestYesNoPrompt()
    {
        var result = await ChatAsync("If red is a color respond yes, otherwise respond no");
        Assert.AreEqual("Yes", result);
    }
}