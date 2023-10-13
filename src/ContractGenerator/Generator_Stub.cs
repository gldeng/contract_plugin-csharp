using ContractGenerator.Primitives;

namespace ContractGenerator;

public partial class Generator
{
    private string StubClassName => _serviceDescriptor.Name + "Stub";

    private void GenerateStubClass()
    {
        _($"public class {StubClassName} : aelf::ContractStubBase");
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
}
