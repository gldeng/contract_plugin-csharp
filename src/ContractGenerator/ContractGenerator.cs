using Google.Protobuf.Compiler;

namespace ContractGenerator;

//This is the main entry-point into this project and is exposed to external users
public class ContractGenerator
{
    /// <summary>
    /// Generates a set of C# files from the input stream containing the proto source. This is the primary entry-point into the ContractPlugin.
    /// </summary>
    public CodeGeneratorResponse Generate(Stream stdin)
    {
        throw new NotImplementedException();
    }
}
