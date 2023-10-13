using ContractGenerator.Primitives;

namespace ContractGenerator;

public partial class Generator
{
    /// <summary>
    ///     Generates the Class for the ReferenceState as part of the aelf contract
    /// </summary>
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L514
    private void GenerateReferenceClass()
    {
        PrintLine($"public class {GetReferenceClassName()} : global::AElf.Sdk.CSharp.State.ContractReferenceState");
        InBlock(() =>
        {
            var methods = GetFullMethod();
            foreach (var method in methods)
            {
                var request = ProtoUtils.GetClassName(method.InputType);
                var response = ProtoUtils.GetClassName(method.OutputType);
                PrintLine(
                    $"{_options.GetAccessLevel()} global::AElf.Sdk.CSharp.State.MethodReference<{request}, {response}> {method.Name} {{ get; set; }}");
            }
        });
    }

    private string GetReferenceClassName()
    {
        return _serviceDescriptor.Name + "ReferenceState";
    }
}
