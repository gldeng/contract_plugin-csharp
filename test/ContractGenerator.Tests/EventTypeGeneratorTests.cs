namespace ContractGenerator.Tests;

public class EventTypeGeneratorTests : TestBase
{
    [Fact]
    public void TestGenerateEvent_NoErrors()
    {
        var fileDescriptors = GetFileDescriptors("helloworld");
        var msg = fileDescriptors[^1].MessageTypes.Last();

        var indentPrinter = new EventTypeGenerator(msg, new GeneratorOptions
        {
            GenerateEvent = true
        });
        indentPrinter.Generate();
        var eventCodeStr = indentPrinter.Output();
        const string expectedCodeStr =
            """
            public partial class UpdatedMessage : aelf::IEvent<UpdatedMessage>
            {
              public global::System.Collections.Generic.IEnumerable<UpdatedMessage> GetIndexed()
              {
                return new List<UpdatedMessage>
                {
                };
              }

              public UpdatedMessage GetNonIndexed()
              {
                return new UpdatedMessage
                {
                  Value = Value,
                };
              }
            }

            """;
        Assert.Equal(expectedCodeStr, eventCodeStr);
    }
}
