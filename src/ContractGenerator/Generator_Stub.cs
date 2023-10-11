namespace ContractGenerator;

public partial class Generator
{
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L484
    private void GenerateStubClass()
    {
        indentPrinter.PrintLine($"public class {GetStubClassName()} : aelf::ContractStubBase");
        indentPrinter.PrintLine("{");
        {
            indentPrinter.Indent();
            var methods = GetFullMethod();
            foreach (var method in methods)
            {
                indentPrinter.PrintLine(
                    $"public aelf::IMethodStub<{ProtoUtils.GetClassName(method.InputType)}, {ProtoUtils.GetClassName(method.OutputType)}> {method.Name}");
                indentPrinter.PrintLine("{");
                {
                    indentPrinter.Indent();
                    indentPrinter.PrintLine($"get {{ return __factory.Create({GetMethodFieldName(method)}); }}");
                    indentPrinter.Outdent();
                }
                indentPrinter.PrintLine("}");
                indentPrinter.PrintLine();
            }

            indentPrinter.Outdent();
        }
        indentPrinter.PrintLine("}");
    }

    private string GetStubClassName()
    {
        return _serviceDescriptor.Name + "Stub";
    }
}
