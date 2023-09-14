using Google.Protobuf;
using Google.Protobuf.Compiler;
using Google.Protobuf.Reflection;

namespace contract_plugin_csharp
{
    // assume current directory is the output directory, and it contains protoc cli.
    // protoc --plugin=protoc-gen-contract_plugin_csharp --contract_plugin_csharp_out=./ --proto_path=%userprofile%\.nuget\packages\google.protobuf.tools\3.21.1\tools --proto_path=./ chat.proto

    internal class Program
    {
        static void Main(string[] args)
        {
            // you can attach debugger
            // System.Diagnostics.Debugger.Launch();

            // get request from standard input
            CodeGeneratorRequest request;
            using (var stdin = Console.OpenStandardInput())
            {
                request = Deserialize<CodeGeneratorRequest>(stdin);
            }

            var response = new CodeGeneratorResponse();


            CSharpContainer csharpContainer = new CSharpContainer();
            response = csharpContainer.Generate(request);

            // set result to standard output
            using (var stdout = Console.OpenStandardOutput())
            {
                response.WriteTo(stdout);
            }
        }

        static T Deserialize<T>(Stream stream) where T : IMessage<T>, new()
            => new MessageParser<T>(() => new T()).ParseFrom(stream);
    }
}