using System.Text;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using ContractGenerator;

namespace ContractGenerator.Tests;

public class ProtoUtilsTests
{
    private static readonly ExtensionRegistry _extensionRegistry = new ExtensionRegistry();

    static ProtoUtilsTests()
    {
        _extensionRegistry.Add(AElf.OptionsExtensions.Identity);
        _extensionRegistry.Add(AElf.OptionsExtensions.Base);
        _extensionRegistry.Add(AElf.OptionsExtensions.CsharpState);
        _extensionRegistry.Add(AElf.OptionsExtensions.IsView);
        _extensionRegistry.Add(AElf.OptionsExtensions.IsEvent);
        _extensionRegistry.Add(AElf.OptionsExtensions.IsIndexed);
    }

    private FileDescriptorSet GetFileDescriptorSet(string testcaseName)
    {
        var descriptor = File.ReadAllBytes($@"testcases/{testcaseName}/descriptor.bin");
        return FileDescriptorSet.Parser.WithExtensionRegistry(_extensionRegistry).ParseFrom(descriptor);
    }

    [Theory]
    [InlineData("foo_bar.baz", "FooBar.Baz")]
    public void Test_UnderscoresToCamelCase_CapNextLetter_And_PreservingPeriod(string input, string output)
    {
        var o = ProtoUtils.UnderscoresToCamelCase(input, true, true);
        Assert.Equal(output, o);
    }

    [Theory]
    [InlineData("foo_bar.baz", "FooBarBaz")]
    public void Test_UnderscoresToCamelCase_CapNextLetter_And_NotPreservingPeriod(string input, string output)
    {
        var o = ProtoUtils.UnderscoresToCamelCase(input, true, false);
        Assert.Equal(output, o);
    }

    [Theory]
    [InlineData("foo_bar.baz", "FooBarBaz")]
    public void Test_UnderscoresToPascalCase_CapNextLetter_And_NotPreservingPeriod(string input, string output)
    {
        var o = ProtoUtils.UnderscoresToPascalCase(input);
        Assert.Equal(output, o);
    }

    [Fact]
    public void GetClassName_ReturnsCorrectClassName()
    {
        // Arrange: Create a DescriptorBase with a known FullName and File
        var fds = GetFileDescriptorSet("helloworld");
        var byteStrings = fds.File.Select(f => f.ToByteString());
        var fileDescriptors = FileDescriptor.BuildFromByteStrings(byteStrings, _extensionRegistry);
        var svc = fileDescriptors.Last().Services.Last();

        var opt = svc.GetOptions();
        var state = opt.GetExtension(AElf.OptionsExtensions.CsharpState);

        // Act: Call the GetClassName method
        var className = ProtoUtils.GetClassName(svc);

        // Assert: Verify the expected result
        Assert.Equal("global::AElf.Contracts.HelloWorld.HelloWorld", className);
    }
}
