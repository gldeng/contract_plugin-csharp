using AElf;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public static class FileDescriptorSetLoader
{
    private static readonly ExtensionRegistry _extensionRegistry = new();

    static FileDescriptorSetLoader()
    {
        _extensionRegistry.Add(OptionsExtensions.Identity);
        _extensionRegistry.Add(OptionsExtensions.Base);
        _extensionRegistry.Add(OptionsExtensions.CsharpState);
        _extensionRegistry.Add(OptionsExtensions.IsView);
        _extensionRegistry.Add(OptionsExtensions.IsEvent);
        _extensionRegistry.Add(OptionsExtensions.IsIndexed);
    }

    public static IReadOnlyList<FileDescriptor> Load(Stream stream)
    {
        var fds = FileDescriptorSet.Parser.WithExtensionRegistry(_extensionRegistry).ParseFrom(stream);
        return Load(fds.File);
    }

    public static IReadOnlyList<FileDescriptor> Load(IEnumerable<FileDescriptorProto> protos)
    {
        var fileInByteStrings = protos.Select(f => f.ToByteString());
        return FileDescriptor.BuildFromByteStrings(fileInByteStrings, _extensionRegistry);
    }
}
