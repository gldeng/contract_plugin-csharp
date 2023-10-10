using ContractGenerator;
using Google.Protobuf;
using Google.Protobuf.Compiler;
using Google.Protobuf.Reflection;

namespace ContractPlugin;

// assume current directory is the output directory, and it contains protoc cli.
// protoc --plugin=protoc-gen-contract_plugin_csharp --contract_plugin_csharp_out=./ --proto_path=%userprofile%\.nuget\packages\google.protobuf.tools\3.21.1\tools --proto_path=./ chat.proto

internal class Program
{
    private static void Main()
    {
        // you can attach debugger
        // System.Diagnostics.Debugger.Launch();

        using var stream = Console.OpenStandardInput();

        var request = CodeGeneratorRequest.Parser.ParseFrom(stream);
        var fileDescriptors = FileDescriptorSetLoader.Load(request.ProtoFile);

        var options = new List<Tuple<string, string>>();
        if (request.Parameter != "") ProtoUtils.ParseGeneratorParameter(request.Parameter, options);
        var response = ContractGenerator.ContractGenerator.Generate(fileDescriptors, options);

        // set result to standard output
        using var stdout = Console.OpenStandardOutput();
        response.WriteTo(stdout);
    }
}
