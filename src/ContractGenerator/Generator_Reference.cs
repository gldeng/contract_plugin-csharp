using ContractGenerator.Primitives;

namespace ContractGenerator;

public partial class Generator
{
    private string ReferenceClassName => _serviceDescriptor.Name + "ReferenceState";

    /// <summary>
    ///     Generates the Class for the ReferenceState as part of the aelf contract
    /// </summary>
    protected internal void GenerateReferenceClass()
    {
        _($"public class {ReferenceClassName} : global::AElf.Sdk.CSharp.State.ContractReferenceState");
        InBlock(() =>
        {
            foreach (var method in FullMethods)
            {
                var request = method.InputType.GetFullTypeName();
                var response = method.OutputType.GetFullTypeName();
                _(
                    $"{PublicOrInternal} global::AElf.Sdk.CSharp.State.MethodReference<{request}, {response}> {method.Name} {{ get; set; }}");
            }
        });
    }
}
