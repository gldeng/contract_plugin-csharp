using System.Text;
using Google.Protobuf;
using Google.Protobuf.Compiler;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public class FlagConstants
{
    public const byte GenerateContract = 0x01; // hex for 0000 0001
    public const byte GenerateStub = 0x02; // hex for 0000 0010
    public const byte GenerateReference = 0x04; // hex for 0000 0100
    public const byte GenerateEvent = 0x08; // hex for 0000 1000
    public const byte InternalAccess = 0x80; // hex for 1000 0000

    public const byte GenerateContractWithEvent = GenerateContract | GenerateEvent;
    public const byte GenerateStubWithEvent = GenerateStub | GenerateEvent;
}

//This is the main entry-point into this project and is exposed to external users
public class ContractGenerator
{
    /// <summary>
    ///     ServicesFilename generates Services FileName based on the FileDescriptor
    ///     Its based on the C++ original
    ///     https://github.com/AElfProject/contract-plugin/blob/de625fcb79f83603e29d201c8488f101b40f573c/src/contract_csharp_generator_helpers.h#L27
    /// </summary>
    private static string ServicesFilename(IDescriptor file)
    {
        return FileNameInUpperCamel(file, false) + ".c.cs";
    }

    private static string LowerUnderscoreToUpperCamel(string str)
    {
        var tokens = str.Split('_');
        var result = new StringBuilder();

        foreach (var token in tokens) result.Append(CapitalizeFirstLetter(token));

        return result.ToString();
    }

    private static string FileNameInUpperCamel(IDescriptor file, bool includePackagePath)
    {
        var tokens = StripProto(file.Name).Split('/');
        var result = new StringBuilder();

        if (includePackagePath)
            for (var i = 0; i < tokens.Length - 1; i++)
                result.Append(tokens[i] + "/");

        result.Append(
            LowerUnderscoreToUpperCamel(tokens[^1])); // Using the "end index" operator to get the last element

        return result.ToString();
    }

    private static string CapitalizeFirstLetter(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;

        var chars = str.ToCharArray();
        chars[0] = char.ToUpper(chars[0]);
        return new string(chars);
    }

    private static string StripProto(string fileName)
    {
        return fileName.EndsWith(".proto") ? fileName[..^".proto".Length] : fileName;
    }

    /// <summary>
    ///     Generates a set of C# files from the input stream containing the proto source. This is the primary entry-point into
    ///     the ContractPlugin.
    /// </summary>
    public static CodeGeneratorResponse Generate(Stream stdin)
    {
        var response = new CodeGeneratorResponse();
        CodeGeneratorRequest request;
        IReadOnlyList<FileDescriptor> fileDescriptors;

        using (stdin)
        {
            request = Deserialize<CodeGeneratorRequest>(stdin);
            // need to rewind the stream before we can read again
            stdin.Seek(0, SeekOrigin.Begin);
            fileDescriptors = FileDescriptorSetLoader.Load(stdin);
        }

        var flags = FlagConstants.GenerateContractWithEvent;
        var options = new List<Tuple<string, string>>();
        ProtoUtils.ParseGeneratorParameter(request.Parameter, options);
        foreach (var option in options)
            switch (option.Item1)
            {
                case "stub":
                    flags |= FlagConstants.GenerateStubWithEvent;
                    flags &= FlagConstants.GenerateContract;
                    break;
                case "reference":
                    // Reference doesn't require an event
                    flags |= FlagConstants.GenerateReference;
                    flags &= FlagConstants.GenerateContract;
                    break;
                case "nocontract":
                    flags &= FlagConstants.GenerateContract;
                    break;
                case "noevent":
                    flags &= FlagConstants.GenerateEvent;
                    break;
                case "internal_access":
                    flags |= FlagConstants.InternalAccess;
                    break;
                default:
                    Console.WriteLine("Unknown generator option: " + option);
                    break;
            }

        foreach (var file in fileDescriptors)
        {
            var generatedCsCodeBody = GetServices(file, flags);
            if (generatedCsCodeBody.Length == 0)
                // don't generate a file if there are no services
                continue;

            // Get output file name.
            var fileName = ServicesFilename(file);
            if (fileName == "") return response;

            response.File.Add(
                new CodeGeneratorResponse.Types.File
                {
                    Name = fileName,
                    Content = generatedCsCodeBody
                }
            );
        }

        return response;
    }

    private static bool NeedEvent(byte flags)
    {
        return (flags & FlagConstants.GenerateEvent) != 0;
    }

    private static bool NeedContract(byte flags)
    {
        return (flags & FlagConstants.GenerateContract) != 0;
    }

    private static bool NeedStub(byte flags)
    {
        return (flags & FlagConstants.GenerateStub) != 0;
    }

    private static bool NeedReference(byte flags)
    {
        return (flags & FlagConstants.GenerateReference) != 0;
    }

    private static bool NeedContainer(byte flags)
    {
        return NeedContract(flags) | NeedStub(flags) | NeedReference(flags);
    }

    private static bool NeedOnlyEvent(byte flags)
    {
        return NeedEvent(flags) & !NeedContract(flags) & !NeedReference(flags) & !NeedStub(flags);
    }

    private static bool HasEvent(FileDescriptor file)
    {
        return file.MessageTypes.Any(EventTypeGenerator.IsEventMessageType);
    }


    /// <summary>
    ///     Iterates over Services included in File Proto and generates the corresponding C# code.
    ///     Based off the C++ original
    ///     https://github.com/AElfProject/contract-plugin/blob/de625fcb79f83603e29d201c8488f101b40f573c/src/contract_csharp_generator.cc#L652
    /// </summary>
    private static string GetServices(FileDescriptor file, byte flags)
    {
        // Scope the output stream so it closes and finalizes output to the string.

        var indentPrinter = new IndentPrinter();

        switch (file.Services.Count)
        {
            // Don't write out any output if there no services, to avoid empty service
            // files being generated for proto files that don't declare any.
            case 0:
                return "";
            case > 1:
                throw new Exception(file.Name + ": File contains more than one service.");
        }

        // Don't write out any output if there no event for event-only generation
        // scenario, this is usually for base contracts
        if (NeedOnlyEvent(flags) && !HasEvent(file)) return "";

        // Write out a file header.
        indentPrinter.Print("// <auto-generated>\n");
        indentPrinter.Print(
            "//     Generated by the protocol buffer compiler.  DO NOT EDIT!");
        indentPrinter.Print(
            $"//     source: {file.Name}");
        indentPrinter.Print("// </auto-generated>");

        // use C++ style as there are no file-level XML comments in .NET
        // string leadingComments = GetCsharpComments(file, true);  TODO uncomment once PR merged
        // if (!leadingComments.empty()) {
        //     indentPrinter.Print("// Original file comments:\n");
        //         indentPrinter.PrintRaw(leading_comments.c_str());
        // }

        indentPrinter.Print("#pragma warning disable 0414, 1591");

        indentPrinter.Print("#region Designer generated code\n");
        indentPrinter.Print("using System.Collections.Generic;");
        indentPrinter.Print("using aelf = global::AElf.CSharp.Core;\n");

        var fileNameSpace = ProtoUtils.GetFileNamespace(file);
        if (fileNameSpace != "")
        {
            indentPrinter.Print($"namespace {fileNameSpace} {{");
            indentPrinter.Indent();
        }

        if (NeedEvent(flags))
        {
            // Events are not needed for contract reference
            indentPrinter.Print("\n#region Events");
            foreach (var msg in file.MessageTypes) EventTypeGenerator.GenerateEvent(indentPrinter, msg, flags);
            indentPrinter.Print("#endregion\n");
        }

        if (NeedContainer(flags))
            foreach (var svc in file.Services)
                ContractContainerGenerator.Generate(indentPrinter, svc, flags);

        if (fileNameSpace != "")
        {
            indentPrinter.Outdent();
            indentPrinter.Print("}");
        }

        indentPrinter.Print("#endregion\n");
        return indentPrinter.PrintOut();
    }

    private static T Deserialize<T>(Stream stream) where T : IMessage<T>, new()
    {
        return new MessageParser<T>(() => new T()).ParseFrom(stream);
    }
}
