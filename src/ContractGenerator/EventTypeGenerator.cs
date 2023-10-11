using AElf;
using ContractGenerator.Primitives;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public class EventTypeGenerator : GeneratorBase
{
    private MessageDescriptor _messageDescriptor;
    private GeneratorOptions _options;

    public EventTypeGenerator(MessageDescriptor message, GeneratorOptions options, IndentPrinter? printer) :
        base(printer)
    {
        _messageDescriptor = message;
        _options = options;
    }

    public string? Generate()
    {
        if (!_messageDescriptor.IsEventMessageType()) return null;
        indentPrinter.PrintLine(
            $"{ProtoUtils.GetAccessLevel(_options)} partial class {_messageDescriptor.Name} : aelf::IEvent<{_messageDescriptor.Name}>");
        InBlock(() =>
            {
                GetIndexed();
                indentPrinter.PrintLine();
                GetNonIndexed();
            }
        );
        return indentPrinter.ToString();
    }

    private void GetIndexed()
    {
        indentPrinter.PrintLine(
            $"public global::System.Collections.Generic.IEnumerable<{_messageDescriptor.Name}> GetIndexed()");
        InBlock(() =>
        {
            indentPrinter.PrintLine($"return new List<{_messageDescriptor.Name}>");
            InBlockWithSemicolon(() =>
            {
                var fields = _messageDescriptor.Fields.InFieldNumberOrder();
                foreach (var field in fields.Where(f => f.IndexedField()))
                {
                    indentPrinter.PrintLine($"new {_messageDescriptor.Name}");
                    InBlockWithComma(() =>
                    {
                        var propertyName = field.GetPropertyName();
                        indentPrinter.PrintLine($"{propertyName} = {propertyName}");
                    });
                }
            });
        });
    }

    private void GetNonIndexed()
    {
        indentPrinter.PrintLine($"public {_messageDescriptor.Name} GetNonIndexed()");
        InBlock(() =>
        {
            indentPrinter.PrintLine($"return new {_messageDescriptor.Name}");
            InBlockWithSemicolon(() =>
            {
                var fields = _messageDescriptor.Fields.InFieldNumberOrder();
                foreach (var field in fields.Where(f => f.NonIndexedField()))
                {
                    var propertyName = field.GetPropertyName();
                    indentPrinter.PrintLine($"{propertyName} = {propertyName},");
                }
            });
        });
    }
}
