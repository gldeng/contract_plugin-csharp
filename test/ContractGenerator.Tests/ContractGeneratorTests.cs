using AElf;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace ContractGenerator.Tests;

public class ContractGeneratorTests
{
    private static readonly ExtensionRegistry _extensionRegistry = new();

    static ContractGeneratorTests()
    {
        _extensionRegistry.Add(OptionsExtensions.Identity);
        _extensionRegistry.Add(OptionsExtensions.Base);
        _extensionRegistry.Add(OptionsExtensions.CsharpState);
        _extensionRegistry.Add(OptionsExtensions.IsView);
        _extensionRegistry.Add(OptionsExtensions.IsEvent);
        _extensionRegistry.Add(OptionsExtensions.IsIndexed);
    }

    private static FileDescriptorSet GetFileDescriptorSet(string testcaseName)
    {
        var descriptor = File.ReadAllBytes($@"testcases/{testcaseName}/descriptor.bin");
        return FileDescriptorSet.Parser.WithExtensionRegistry(_extensionRegistry).ParseFrom(descriptor);
    }

    [Fact]
    public void TestGenerateEvent_NoErrors()
    {
        var indentPrinter = new IndentPrinter();
        var fds = GetFileDescriptorSet("helloworld");
        var byteStrings = fds.File.Select(f => f.ToByteString());
        var fileDescriptors = FileDescriptor.BuildFromByteStrings(byteStrings, _extensionRegistry);
        var msg = fileDescriptors[^1].MessageTypes.Last();

        ContractGenerator.GenerateEvent(ref indentPrinter, msg, FlagConstants.GenerateEvent);
        var eventCodeStr = indentPrinter.PrintOut();
        const string expectedCodeStr =
            "public partial class UpdatedMessage : aelf::IEvent<UpdatedMessage>\n{\n  public global::System.Collections.Generic.IEnumerable<UpdatedMessage> GetIndexed()\n  {\n    return new List<UpdatedMessage>\n    {\n    };\n  }\n\n  public UpdatedMessage GetNonIndexed()\n  {\n    return new UpdatedMessage\n    {\n      Value = Value,\n    };\n  }\n}\n\n";
        Assert.Equal(expectedCodeStr, eventCodeStr);
    }
}
