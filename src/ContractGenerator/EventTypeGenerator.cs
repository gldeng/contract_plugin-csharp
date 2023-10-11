using AElf;
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

    #region Helper Methods

    /// <summary>
    ///     Determines if the proto-message is of IndexedType based on Aelf.options
    /// </summary>
    private static bool IsIndexedField(FieldDescriptor field)
    {
        return field.GetOptions() != null && field.GetOptions().GetExtension(OptionsExtensions.IsIndexed);
    }

    /// <summary>
    ///     Determines if the proto-message is of EventType based on Aelf.options
    /// </summary>
    public static bool IsEventMessageType(MessageDescriptor message)
    {
        return message.GetOptions().GetExtension(OptionsExtensions.IsEvent);
    }

    #endregion

    public string? Generate()
    {
        if (!IsEventMessageType(_messageDescriptor)) return null;
        indentPrinter.PrintLine(
            $"{ProtoUtils.GetAccessLevel(_options)} partial class {_messageDescriptor.Name} : aelf::IEvent<{_messageDescriptor.Name}>");
        indentPrinter.PrintLine("{");
        {
            indentPrinter.Indent();
            // GetIndexed
            indentPrinter.PrintLine(
                $"public global::System.Collections.Generic.IEnumerable<{_messageDescriptor.Name}> GetIndexed()");
            indentPrinter.PrintLine("{");
            {
                indentPrinter.Indent();
                indentPrinter.PrintLine($"return new List<{_messageDescriptor.Name}>");
                indentPrinter.PrintLine("{");
                var fields = _messageDescriptor.Fields.InFieldNumberOrder();
                foreach (var field in fields)
                {
                    if (field == null) continue;
                    if (!IsIndexedField(field)) continue;
                    indentPrinter.PrintLine($"new {_messageDescriptor.Name}");
                    indentPrinter.PrintLine("{");
                    {
                        indentPrinter.Indent();
                        var propertyName = ProtoUtils.GetPropertyName(field);
                        indentPrinter.PrintLine($"{propertyName} = {propertyName}");
                        indentPrinter.Outdent();
                    }
                    indentPrinter.PrintLine("},");
                }

                indentPrinter.PrintLine("};");
                indentPrinter.Outdent();
            }
            indentPrinter.PrintLine("}"); // end GetIndexed
            indentPrinter.PrintLine();

            // GetNonIndexed
            indentPrinter.PrintLine($"public {_messageDescriptor.Name} GetNonIndexed()");
            indentPrinter.PrintLine("{");
            {
                indentPrinter.Indent();
                indentPrinter.PrintLine($"return new {_messageDescriptor.Name}");
                indentPrinter.PrintLine("{");
                {
                    indentPrinter.Indent();
                    var fields = _messageDescriptor.Fields.InFieldNumberOrder();
                    foreach (var field in fields)
                    {
                        if (field == null) continue;
                        if (IsIndexedField(field)) continue;
                        var propertyName = ProtoUtils.GetPropertyName(field);
                        indentPrinter.PrintLine($"{propertyName} = {propertyName},");
                    }

                    indentPrinter.Outdent();
                }
                indentPrinter.PrintLine("};");
                indentPrinter.Outdent();
            }
            indentPrinter.PrintLine("}");
            indentPrinter.Outdent();
        }

        indentPrinter.PrintLine("}");
        return indentPrinter.ToString();
    }
}
