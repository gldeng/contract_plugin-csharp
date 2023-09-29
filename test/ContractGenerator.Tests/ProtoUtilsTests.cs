using System.Text;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using ContractGenerator;

namespace ContractGenerator.Tests;

public class ProtoUtilsTests
{
    private FileDescriptorSet GetFileDescriptorSet(string testcaseName)
    {
        var descriptor = File.ReadAllBytes($@"testcases/{testcaseName}/descriptor.bin");
        return FileDescriptorSet.Parser.ParseFrom(descriptor);
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

        // List<string> dependencys = new List<string>();
        // foreach (var dependency in fds.File[0].Dependency)
        // {
        //     Console.WriteLine("dependancy:"+dependency);
        //     dependencys.Add(dependency);
        // }
        //

        var aelfOptionsDescriptor = AElf.OptionsReflection.Descriptor.ToProto().ToByteString();
        var emptyDescRef = EmptyReflection.Descriptor.ToProto().ToByteString();
        var wrappersDescRef = WrappersReflection.Descriptor.ToProto().ToByteString();
        var descriptorRef = DescriptorReflection.Descriptor.ToProto().ToByteString();

        var byteStrings = fds.File.Select(f => f.ToByteString());
        var fileDescriptors = FileDescriptor.BuildFromByteStrings(new []{
            aelfOptionsDescriptor,emptyDescRef,wrappersDescRef,descriptorRef,fds.File[0].ToByteString()
        });
        // Act: Call the GetClassName method
        var className = ProtoUtils.GetClassName(fileDescriptors[0]);

        // Assert: Verify the expected result
        Assert.Equal("HelloWorld", className);
    }
}
