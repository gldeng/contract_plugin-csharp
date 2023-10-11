using Google.Protobuf.Reflection;

namespace ContractGenerator;

public partial class Generator
{
    #region Methods

    private void Methods()
    {
        InRegion("Methods", () =>
        {
            foreach (var method in GetFullMethod()) GenerateStaticMethodField(method);
        });
    }

    /// <summary>
    ///     Generates instantiations for static readonly aelf::Method fields based on the proto
    /// </summary>
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L349
    private void GenerateStaticMethodField(MethodDescriptor methodDescriptor)
    {
        var request = ProtoUtils.GetClassName(methodDescriptor.InputType);
        var response = ProtoUtils.GetClassName(methodDescriptor.OutputType);
        indentPrinter.PrintLine(
            $"static readonly aelf::Method<{request}, $response$> {GetMethodFieldName(methodDescriptor)} = new " +
            $"aelf::Method<{request}, {response}>(");
        indentPrinter.Indent();
        indentPrinter.Indent();
        indentPrinter.PrintLine($"{GetCSharpMethodType(methodDescriptor)},");
        indentPrinter.PrintLine($"{ServiceFieldName},");
        indentPrinter.PrintLine($"\"{methodDescriptor.Name}\",");
        indentPrinter.PrintLine($"{GetMarshallerFieldName(methodDescriptor.InputType)},");
        indentPrinter.PrintLine($"{GetMarshallerFieldName(methodDescriptor.OutputType)});");
        indentPrinter.PrintLine();
        indentPrinter.Outdent();
        indentPrinter.Outdent();
    }

    #endregion Methods

    #region Descriptors

    private void Descriptors()
    {
        InRegion("Descriptors", () =>
        {
            GenerateServiceDescriptorProperty();
            indentPrinter.PrintLine();
            GenerateAllServiceDescriptorsProperty();
        });
    }

    #endregion Descriptors
}
