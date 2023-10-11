namespace ContractGenerator;

public partial class Generator
{
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L484
    private void GenerateStubClass()
    {
        PrintLine($"public class {GetStubClassName()} : aelf::ContractStubBase");
        PrintLine("{");
        {
            Indent();
            var methods = GetFullMethod();
            foreach (var method in methods)
            {
                PrintLine(
                    $"public aelf::IMethodStub<{ProtoUtils.GetClassName(method.InputType)}, {ProtoUtils.GetClassName(method.OutputType)}> {method.Name}");
                PrintLine("{");
                {
                    Indent();
                    PrintLine($"get {{ return __factory.Create({GetMethodFieldName(method)}); }}");
                    Outdent();
                }
                PrintLine("}");
                PrintLine();
            }

            Outdent();
        }
        PrintLine("}");
    }

    private string GetStubClassName()
    {
        return _serviceDescriptor.Name + "Stub";
    }
}
