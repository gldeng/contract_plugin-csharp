using ContractGenerator.Primitives;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public class EventTypeGenerator : AbstractGenerator
{
    private string PublicOrInternal => Options.InternalAccess ? "internal" : "public";
    private readonly MessageDescriptor _messageDescriptor;
    private string TypeName => _messageDescriptor.Name;

    private IEnumerable<FieldDescriptor> IndexedFields =>
        _messageDescriptor.Fields.InFieldNumberOrder().Where(f => f.IndexedField()).ToList();

    private IEnumerable<FieldDescriptor> NonIndexedFields =>
        _messageDescriptor.Fields.InFieldNumberOrder().Where(f => f.NonIndexedField()).ToList();

    public EventTypeGenerator(MessageDescriptor message, GeneratorOptions options) : base(options)
    {
        _messageDescriptor = message;
    }

    public override string? Generate()
    {
        if (!_messageDescriptor.IsEventMessageType()) return null;
        _($"{PublicOrInternal} partial class {TypeName} : aelf::IEvent<{TypeName}>");
        InBlock(() =>
            {
                GetIndexed();
                ___EmptyLine___();
                GetNonIndexed();
            }
        );
        return PrintOut();
    }

    private void GetIndexed()
    {
        _(
            $"public global::System.Collections.Generic.IEnumerable<{TypeName}> GetIndexed()");
        InBlock(() =>
        {
            _($"return new List<{TypeName}>");
            InBlockWithSemicolon(() =>
            {
                foreach (var field in IndexedFields)
                {
                    _($"new {TypeName}");
                    InBlockWithComma(() =>
                    {
                        var propertyName = field.GetPropertyName();
                        _($"{propertyName} = {propertyName}");
                    });
                }
            });
        });
    }

    private void GetNonIndexed()
    {
        _($"public {TypeName} GetNonIndexed()");
        InBlock(() =>
        {
            _($"return new {TypeName}");
            InBlockWithSemicolon(() =>
            {
                foreach (var field in NonIndexedFields)
                {
                    var propertyName = field.GetPropertyName();
                    _($"{propertyName} = {propertyName},");
                }
            });
        });
    }
}
