using System.Text;
using ContractGenerator.Primitives;
using Google.Protobuf.Compiler;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

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
    public static CodeGeneratorResponse Generate(IEnumerable<FileDescriptor> fileDescriptors, GeneratorOptions options)
    {
        var response = new CodeGeneratorResponse();


        foreach (var file in fileDescriptors)
        {
            var generatedCsCodeBody = GetServices(file, options);
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

    private static bool HasEvent(FileDescriptor file)
    {
        return file.MessageTypes.Any(m => m.IsEventMessageType());
    }


    /// <summary>
    ///     Iterates over Services included in File Proto and generates the corresponding C# code.
    ///     Based off the C++ original
    ///     https://github.com/AElfProject/contract-plugin/blob/de625fcb79f83603e29d201c8488f101b40f573c/src/contract_csharp_generator.cc#L652
    /// </summary>
    private static string GetServices(FileDescriptor file, GeneratorOptions options)
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
        if (options.GenerateEventOnly && !HasEvent(file)) return "";

        // Write out a file header.
        indentPrinter.PrintLine("// <auto-generated>");
        indentPrinter.PrintLine(
            "//     Generated by the protocol buffer compiler.  DO NOT EDIT!");
        indentPrinter.PrintLine(
            $"//     source: {file.Name}");
        indentPrinter.PrintLine("// </auto-generated>");

        // use C++ style as there are no file-level XML comments in .NET
        // string leadingComments = GetCsharpComments(file, true);  TODO uncomment once PR merged
        // if (!leadingComments.empty()) {
        //     indentPrinter.Print("// Original file comments:\n");
        //         indentPrinter.PrintRaw(leading_comments.c_str());
        // }

        indentPrinter.PrintLine("#pragma warning disable 0414, 1591");

        indentPrinter.PrintLine("#region Designer generated code");
        indentPrinter.PrintLine();
        indentPrinter.PrintLine("using System.Collections.Generic;");
        indentPrinter.PrintLine("using aelf = global::AElf.CSharp.Core;");
        indentPrinter.PrintLine();

        var fileNameSpace = ProtoUtils.GetFileNamespace(file);
        if (fileNameSpace != "")
        {
            indentPrinter.PrintLine($"namespace {fileNameSpace} {{");
            indentPrinter.Indent();
        }

        if (options.GenerateEvent)
        {
            // Events are not needed for contract reference
            indentPrinter.PrintLine();
            indentPrinter.PrintLine("#region Events");
            foreach (var msg in file.MessageTypes)
                indentPrinter.Print(new EventTypeGenerator(msg, options).Generate() ?? "");
            indentPrinter.PrintLine("#endregion");
            indentPrinter.PrintLine();
        }

        if (options.GenerateContainer)
            foreach (var svc in file.Services)
            {
                indentPrinter.Print(new Generator(svc, options).Generate() ?? "");
            }

        if (fileNameSpace != "")
        {
            indentPrinter.Outdent();
            indentPrinter.PrintLine("}");
        }

        indentPrinter.PrintLine("#endregion");
        indentPrinter.PrintLine();
        return indentPrinter.PrintOut();
    }
}
