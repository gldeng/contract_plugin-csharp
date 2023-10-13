using ContractGenerator.Primitives;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public class ProtoUtils
{
    //TODO Implement https://github.com/protocolbuffers/protobuf/blob/e57166b65a6d1d55fc7b18beaae000565f617f22/src/google/protobuf/compiler/csharp/names.cc#L73


    public static string GetReflectionClassName(FileDescriptor descriptor)
    {
        var result = descriptor.GetNamespace();
        if (result.Length > 0) result += '.';
        result += GetReflectionClassUnqualifiedName(descriptor);
        return "global::" + result;
    }

    private static string GetFileNameBase(IDescriptor descriptor)
    {
        var protoFile = descriptor.Name;
        var lastSlash = protoFile.LastIndexOf('/');
        var stringBase = protoFile[(lastSlash + 1)..];
        return StripDotProto(stringBase).UnderscoresToPascalCase();
    }

    private static string StripDotProto(string protoFile)
    {
        var lastIndex = protoFile.LastIndexOf(".", StringComparison.Ordinal);
        return protoFile[..lastIndex];
    }

    public static string GetReflectionClassUnqualifiedName(FileDescriptor descriptor)
    {
        // TODO: Detect collisions with existing messages,
        // and append an underscore if necessary.
        return GetFileNameBase(descriptor) + "Reflection";
    }
}
