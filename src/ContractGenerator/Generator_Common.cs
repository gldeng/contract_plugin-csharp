using ContractGenerator.Primitives;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public partial class Generator
{
    #region Methods

    private void Methods()
    {
        InRegion("Methods", () =>
        {
            foreach (var method in FullMethods) GenerateStaticMethodField(method);
        });
    }

    /// <summary>
    ///     Generates instantiations for static readonly aelf::Method fields based on the proto
    /// </summary>
    private void GenerateStaticMethodField(MethodDescriptor methodDescriptor)
    {
        var request = methodDescriptor.InputType.GetFullTypeName();
        var response = methodDescriptor.OutputType.GetFullTypeName();
        _(
            $"static readonly aelf::Method<{request}, {response}> {GetMethodFieldName(methodDescriptor)} = new aelf::Method<{request}, {response}>(");
        DoubleIndented(() =>
        {
            _($"{GetCSharpMethodType(methodDescriptor)},");
            _($"{ServiceFieldName},");
            _($"\"{methodDescriptor.Name}\",");
            _($"{GetMarshallerFieldName(methodDescriptor.InputType)},");
            _($"{GetMarshallerFieldName(methodDescriptor.OutputType)});");
        });
        ___EmptyLine___();
    }

    #endregion Methods

    #region Descriptors

    private void Descriptors()
    {
        InRegion("Descriptors", () =>
        {
            GenerateServiceDescriptorProperty();
            ___EmptyLine___();
            GenerateAllServiceDescriptorsProperty();
        });
    }

    #endregion Descriptors
}
