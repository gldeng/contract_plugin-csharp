using Google.Protobuf.Reflection;

namespace ContractGenerator.Tests;

public class ContractContainerGeneratorTests
{
    //TODO Replace with TestBase once rebased
    protected static IReadOnlyList<FileDescriptor> GetFileDescriptors(string testcaseName)
    {
        using var stream = File.Open($"testcases/{testcaseName}/descriptor.bin", FileMode.Open);

        return FileDescriptorSetLoader.Load(stream);
    }

    [Fact]
    public void TestGenerateContractBaseClass_NoErrors()
    {
        var indentPrinter = new IndentPrinter();
        var fileDescriptors = GetFileDescriptors("helloworld");
        var svc = fileDescriptors[^1].Services.Last();

        ContractContainerGenerator.GenerateContractBaseClass(indentPrinter, svc);
        var contractBaseCodeStr = indentPrinter.PrintOut();
        const string expectedCodeStr =
            @"/// <summary>Base class for the contract of HelloWorldBase</summary>
public abstract partial class HelloWorldBase : AElf.Sdk.CSharp.CSharpSmartContract<AElf.Contracts.HelloWorld.HelloWorldState>
{
  public abstract global::Google.Protobuf.WellKnownTypes.Empty Update(global::Google.Protobuf.WellKnownTypes.StringValue input);
  public abstract global::Google.Protobuf.WellKnownTypes.StringValue Read(global::Google.Protobuf.WellKnownTypes.Empty input);
}

";
        Assert.Equal(expectedCodeStr, contractBaseCodeStr);
    }
}
