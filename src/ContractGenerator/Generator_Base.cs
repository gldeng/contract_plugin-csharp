using AElf;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public partial class Generator
{
    /// <summary>
    ///     Generate will produce a chunk of C# code BaseClass for the AElf Contract. based on C++ original
    ///     https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L422
    /// </summary>
    protected internal void GenerateContractBaseClass()
    {
        var serverClassName = GetServerClassName();
        indentPrinter.PrintLine(
            $"/// <summary>Base class for the contract of {serverClassName}</summary>");
        indentPrinter.PrintLine(
            $"public abstract partial class {serverClassName} : AElf.Sdk.CSharp.CSharpSmartContract<{GetStateTypeName()}>");
        indentPrinter.PrintLine("{");
        indentPrinter.Indent();
        var methods = GetFullMethod();
        foreach (var method in methods)
            indentPrinter.PrintLine(
                $"public abstract {GetMethodReturnTypeServer(method)} {method.Name}({GetMethodRequestParamServer(method)}{GetMethodResponseStreamMaybe(method)});");
        indentPrinter.Outdent();
        indentPrinter.PrintLine("}");
    }

    private string GetStateTypeName()
    {
        return _serviceDescriptor.GetOptions().GetExtension(OptionsExtensions.CsharpState);
    }


    private static string GetMethodReturnTypeServer(MethodDescriptor method)
    {
        return ProtoUtils.GetClassName(method.OutputType);
    }

    private static string GetMethodRequestParamServer(MethodDescriptor method)
    {
        switch (GetMethodType(method))
        {
            case MethodType.MethodtypeNoStreaming:
            case MethodType.MethodtypeServerStreaming:
                return ProtoUtils.GetClassName(method.InputType) + " input";
            case MethodType.MethodtypeClientStreaming:
            case MethodType.MethodtypeBidiStreaming:
                return "grpc::IAsyncStreamReader<" + ProtoUtils.GetClassName(method.InputType) +
                       "> requestStream";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static string GetMethodResponseStreamMaybe(MethodDescriptor method)
    {
        switch (GetMethodType(method))
        {
            case MethodType.MethodtypeNoStreaming:
            case MethodType.MethodtypeClientStreaming:
                return "";
            case MethodType.MethodtypeServerStreaming:
            case MethodType.MethodtypeBidiStreaming:
                return ", grpc::IServerStreamWriter<" +
                       ProtoUtils.GetClassName(method.OutputType) + "> responseStream";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private static MethodType GetMethodType(MethodDescriptor method)
    {
        if (method.IsClientStreaming)
            return method.IsServerStreaming ? MethodType.MethodtypeBidiStreaming : MethodType.MethodtypeClientStreaming;
        return method.IsServerStreaming ? MethodType.MethodtypeServerStreaming : MethodType.MethodtypeNoStreaming;
    }


    private void GenerateBindServiceMethod()
    {
        indentPrinter.PrintLine(
            $"public static aelf::ServerServiceDefinition BindService({GetServerClassName()} serviceImpl)");
        indentPrinter.PrintLine("{");
        indentPrinter.Indent();
        indentPrinter.PrintLine("return aelf::ServerServiceDefinition.CreateBuilder()");
        indentPrinter.Indent();
        indentPrinter.Indent();
        indentPrinter.PrintLine(".AddDescriptors(Descriptors)");
        var methods = GetFullMethod();
        foreach (var method in methods)
            indentPrinter.PrintLine($".AddMethod({GetMethodFieldName(method)}, serviceImpl.{method.Name}).Build();");
        indentPrinter.Outdent();
        indentPrinter.Outdent();

        indentPrinter.Outdent();
        indentPrinter.PrintLine("}");
        indentPrinter.PrintLine();
    }
}
