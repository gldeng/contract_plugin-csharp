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

        var stream = Console.OpenStandardInput();
        IReadOnlyList<FileDescriptor> fileDescriptors;
        CodeGeneratorRequest request;

        using (stream)
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            //Unfortunately FileDescriptor's proto doesnt have the CLI Parameters equivalent of https://github.com/protocolbuffers/protobuf/blob/main/csharp/src/Google.Protobuf/Compiler/Plugin.pb.cs#L502
            //So we need to parse request to get these params
            request = Deserialize<CodeGeneratorRequest>(memoryStream);
            // need to rewind the stream before we can read again
            memoryStream.Seek(0, SeekOrigin.Begin);
            // request.ProtoFile;
            fileDescriptors = FileDescriptorSetLoader.Load(request.ProtoFile);
        }

        var options = new List<Tuple<string, string>>();
        Console.WriteLine("params:" + request.Parameter);
        ProtoUtils.ParseGeneratorParameter(request.Parameter, options);
        var response = ContractGenerator.ContractGenerator.Generate(fileDescriptors, options);

        // set result to standard output
        using var stdout = Console.OpenStandardOutput();
        response.WriteTo(stdout);
    }

    private static T Deserialize<T>(Stream stream) where T : IMessage<T>, new()
    {
        return new MessageParser<T>(() => new T()).ParseFrom(stream);
    }
}
