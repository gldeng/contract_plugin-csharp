using Google.Protobuf.Reflection;

namespace ContractGenerator.Primitives;

public static class FilePrimitives
{
    public static bool ContainsEvent(this FileDescriptor file)
    {
        return file.MessageTypes.Any(m => m.IsEventMessageType());
    }

    public static string GetNamespace(this FileDescriptor fileDescriptor)
    {
        return fileDescriptor.GetOptions().HasCsharpNamespace
            ? fileDescriptor.GetOptions().CsharpNamespace
            : fileDescriptor.Package.UnderscoresToCamelCase(true, true);
    }
}
