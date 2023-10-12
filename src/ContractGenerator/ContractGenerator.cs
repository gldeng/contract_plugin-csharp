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
            var generatedCsCodeBody = new ServiceGenerator(file, options).Generate() ?? "";
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
}
