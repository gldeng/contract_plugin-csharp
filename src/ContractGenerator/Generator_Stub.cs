using ContractGenerator.Primitives;

namespace ContractGenerator;

public partial class Generator
{
    private void GenerateStubClass()
    {
        _($"public class {GetStubClassName()} : aelf::ContractStubBase");
        InBlock(() =>
        {
            foreach (var method in FullMethods)
            {
                _(
                    $"public aelf::IMethodStub<{method.InputType.GetFullTypeName()}, {method.OutputType.GetFullTypeName()}> {method.Name}");
                InBlock(() => { _($"get {{ return __factory.Create({GetMethodFieldName(method)}); }}"); });
                ___EmptyLine___();
            }
        });
    }

    private string GetStubClassName()
    {
        return _serviceDescriptor.Name + "Stub";
    }
}
