using Google.Protobuf.Reflection;
using Xunit.Abstractions;

namespace ContractGenerator.Tests;

public class ContractGeneratorTests
{
    private readonly ITestOutputHelper _output;

    public ContractGeneratorTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private static FileDescriptorSet GetFileDescriptorSet(string testcaseName)
    {
        var descriptor = File.ReadAllBytes($@"testcases/{testcaseName}/descriptor.bin");
        return FileDescriptorSet.Parser.ParseFrom(descriptor);
    }

    [Fact]
    private void Test()
    {
        var fds = GetFileDescriptorSet("helloworld");
        var filenames = string.Join("\n", fds.File.Select(f => f.Name));
        _output.WriteLine($"Got files:\n {filenames}");
    }
}
