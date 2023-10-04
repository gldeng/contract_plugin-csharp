using AElf;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

// Generates the overall "container" for the generated C# contract
public class ContractContainerGenerator
{
    /// <summary>
    ///     Generate will produce a chunk of C# code BaseClass for the AElf Contract. based on C++ original
    ///     https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L422
    /// </summary>
    protected internal static void GenerateContractBaseClass(IndentPrinter indentPrinter, ServiceDescriptor service)
    {
        var serverClassName = GetServerClassName(service);
        indentPrinter.Print(
            $"/// <summary>Base class for the contract of {serverClassName}</summary>");
        indentPrinter.Print(
            $"public abstract partial class {serverClassName} : AElf.Sdk.CSharp.CSharpSmartContract<{GetStateTypeName(service)}>");
        indentPrinter.Print("{");
        indentPrinter.Indent();
        var methods = GetFullMethod(service);
        foreach (var method in methods)
            indentPrinter.Print(
                $"public abstract {GetMethodReturnTypeServer(method)} {method.Name}({GetMethodRequestParamServer(method)}{GetMethodResponseStreamMaybe(method)});");
        indentPrinter.Outdent();
        indentPrinter.Print("}\n");
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

    /// <summary>
    ///     Generates instantiations for static readonly aelf::Method fields based on the proto
    /// </summary>
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L349
    private static string GenerateStaticMethodField(IndentPrinter indentPrinter,MethodDescriptor methodDescriptor)
    {
        throw new NotImplementedException();
    }

    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L484
    private static string GenerateStubClass(ServiceDescriptor serviceDescriptor)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Generates the Class for the ReferenceState as part of the aelf contract
    /// </summary>
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L514
    private static string GenerateReferenceClass(ServiceDescriptor serviceDescriptor, byte flags)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Generates the IReadOnlyList of ServiceDescriptors as part of the contract
    /// </summary>
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L386
    private static string GenerateAllServiceDescriptorsProperty(ServiceDescriptor serviceDescriptor)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     GetMarshallerFieldName formats and returns a marshaller-fieldname based on the original C++ logic
    ///     found here https://github.com/AElfProject/contract-plugin/blob/de625fcb79f83603e29d201c8488f101b40f573c/src/contract_csharp_generator.cc#L242
    /// </summary>
    private static string GetMarshallerFieldName(IDescriptor message)
    {
        var msgFullName = message.FullName;
        return "__Marshaller_" + msgFullName.Replace(".", "_");
    }

    /// <summary>
    ///     GetUsedMessages extracts messages from Proto ServiceDescriptor based on the original C++ logic
    ///     found here https://github.com/AElfProject/contract-plugin/blob/de625fcb79f83603e29d201c8488f101b40f573c/src/contract_csharp_generator.cc#L312
    /// </summary>
    private static List<IDescriptor> GetUsedMessages(ServiceDescriptor service)
    {
        var descriptorSet = new HashSet<IDescriptor>();
        var result = new List<IDescriptor>();

        var methods = GetFullMethod(service);
        foreach (var method in methods)
        {
            if (!descriptorSet.Contains(method.InputType))
            {
                descriptorSet.Add(method.InputType);
                result.Add(method.InputType);
            }

            if (descriptorSet.Contains(method.OutputType)) continue;
            descriptorSet.Add(method.OutputType);
            result.Add(method.OutputType);
        }

        return result;
    }


    /// <summary>
    ///     Generates a section of instantiated aelf Marshallers as part of the contract
    /// </summary>
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L332
    private static void GenerateMarshallerFields(IndentPrinter indentPrinter,ServiceDescriptor serviceDescriptor)
    {
        indentPrinter.Print("#region Marshallers");
        var usedMessages = GetUsedMessages(serviceDescriptor);
        foreach(var usedMessage in usedMessages)
        {
            var type = ProtoUtils.GetClassName(usedMessage);
            indentPrinter.Print(
                $"static readonly aelf::Marshaller<{type}> {GetMarshallerFieldName(usedMessage)} = "+
            "aelf::Marshallers.Create((arg) => "+
            "global::Google.Protobuf.MessageExtensions.ToByteArray(arg), "+
            $"{type}.Parser.ParseFrom);");
        }
        indentPrinter.Print("#endregion\n");
    }

    /// <summary>
    ///     Generates the header comments for the C# Container of the AElf Contract.
    /// </summary>
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L106
    private static string GenerateDocCommentBody(SourceCodeInfo sourceCodeInfo)
    {
        // request.ProtoFile[0].SourceCodeInfo.Location[0] this object can be extracted from CodeGeneratorRequest
        throw new NotImplementedException();
    }

    private static string GetServerClassName(IDescriptor service)
    {
        return service.Name + "Base";
    }

    private static string GetStateTypeName(ServiceDescriptor service)
    {
        return service.GetOptions().GetExtension(OptionsExtensions.CsharpState);
    }

    //TODO Implementation following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L60
    private static string GenerateDocCommentBody(ServiceDescriptor service)
    {
        throw new NotImplementedException();
    }

    //TODO Implement https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L115
    private static string GetServiceContainerClassName(IDescriptor service)
    {
        //TODO service null check
        return $"{service.Name}Container";
    }

    //TODO Implement https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L204
    private static IEnumerable<ServiceDescriptor> GetFullService(ServiceDescriptor service)
    {
        var allDependedServices = new List<ServiceDescriptor>();
        var seen = new SortedSet<ServiceDescriptor>();
        DepthFirstSearch(service, ref allDependedServices, ref seen);
        var services = allDependedServices.ToDictionary(dependedService => dependedService.File.Name);
        var result = new List<ServiceDescriptor>();
        var bases = new List<string>();
        var seenBases = new SortedSet<string>();

        DepthFirstSearchForBase(service, ref bases, ref seenBases, services);
        foreach (var baseItem in bases)
        {
            var lastIndex = result.Count;
            result.Insert(lastIndex, services[baseItem]); //push to back of list
        }

        return result;
    }

    private static int GetServiceBaseCount(ServiceDescriptor service)
    {
        if (service.GetOptions().GetExtension(OptionsExtensions.Base) == null) return 0;
        return service.GetOptions().GetExtension(OptionsExtensions.Base).Count == 0
            ? 0
            : service.GetOptions().GetExtension(OptionsExtensions.Base).Count;
    }

    private static string GetServiceBase(ServiceDescriptor service, int index)
    {
        return service.GetOptions().GetExtension(OptionsExtensions.Base)[index];
    }

    private static void DepthFirstSearchForBase(ServiceDescriptor service, ref List<string> list,
        ref SortedSet<string> seen, IReadOnlyDictionary<string, ServiceDescriptor> allServices)
    {
        if (!seen.Add(service.File.Name)) return;

        var baseCount = GetServiceBaseCount(service);
        // const FileDescriptor* file = service->file();
        // Add all dependencies.
        for (var i = 0; i < baseCount; i++)
        {
            var baseName = GetServiceBase(service, i);
            if (!allServices.ContainsKey(baseName))
                //TODO Make this an exception instead?
                Console.WriteLine($"Can't find specified base {baseName}, did you forget to import it?");
            var baseService = allServices[baseName];
            DepthFirstSearchForBase(baseService, ref list, ref seen, allServices);
        }

        // Add this file.
        list.Add(service.File.Name);
    }

    private static void DepthFirstSearch(ServiceDescriptor service, ref List<ServiceDescriptor> list,
        ref SortedSet<ServiceDescriptor> seen)
    {
        if (!seen.Add(service)) return;

        var file = service.File;

        foreach (var dependancy in file.Dependencies)
        {
            switch (dependancy.Services.Count)
            {
                case 0:
                    continue;
                case > 1:
                    Console.WriteLine($"{dependancy.Name}: File contains more than one service.");
                    break;
            }

            DepthFirstSearch(dependancy.Services[0], ref list, ref seen);
        }

        // Add this file.
        list.Add(service);
    }

    //TODO Implement https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L222
    private static List<MethodDescriptor> GetFullMethod(ServiceDescriptor service)
    {
        var services = GetFullService(service);
        return services.SelectMany(serviceItem => serviceItem.Methods).ToList();
    }

    private static string GetServiceNameFieldName(ServiceDescriptor serviceDescriptor)
    {
        return "__ServiceName";
    }

    /// <summary>
    ///     Generate will produce a chunk of C# code that serves as the container class of the AElf Contract.
    /// </summary>
    //TODO Implement following https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L612
    public static string Generate(IndentPrinter indentPrinter,ServiceDescriptor serviceDescriptor, byte flags)
    {
        // GenerateDocCommentBody(serviceDescriptor,)
        indentPrinter.Print($"{ProtoUtils.GetAccessLevel(flags)} static partial class {GetServiceContainerClassName(serviceDescriptor)}");
            indentPrinter.Print("{");
            indentPrinter.Indent();
            indentPrinter.Print($"static readonly string {GetServiceNameFieldName(serviceDescriptor)} = \"{serviceDescriptor.FullName}\";\n");

        GenerateMarshallerFields(indentPrinter, serviceDescriptor);
            indentPrinter.Print("#region Methods\n");
        var methods = GetFullMethod(serviceDescriptor);
        foreach(var method in methods) {
            GenerateStaticMethodField(indentPrinter, method);
        }
        indentPrinter.Print("#endregion\n");
            indentPrinter.Print("\n");

            indentPrinter.Print("#region Descriptors\n");
        GenerateServiceDescriptorProperty(indentPrinter, serviceDescriptor);
            indentPrinter.Print("\n");
        GenerateAllServiceDescriptorsProperty(indentPrinter, serviceDescriptor);
            indentPrinter.Print("#endregion\n");
            indentPrinter.Print("\n");

        if (NeedContract(flags)) {
            GenerateContractBaseClass(indentPrinter, serviceDescriptor);
            GenerateBindServiceMethod(indentPrinter, serviceDescriptor);
        }

        if(NeedStub(flags)) {
            GenerateStubClass(indentPrinter, serviceDescriptor);
        }

        if(NeedReference(flags)){
            GenerateReferenceClass(indentPrinter, serviceDescriptor, flags);
        }
        indentPrinter.Outdent();
            indentPrinter.Print("}\n");
    }

    private enum MethodType
    {
        MethodtypeNoStreaming,
        MethodtypeClientStreaming,
        MethodtypeServerStreaming,
        MethodtypeBidiStreaming
    }
}
