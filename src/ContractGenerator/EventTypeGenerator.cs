using AElf;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public static class EventTypeGenerator
{
    public static void GenerateEvent(IndentPrinter printer, MessageDescriptor message, GeneratorOptions options)
    {
        if (!IsEventMessageType(message)) return;
        printer.PrintLine(
            $"{ProtoUtils.GetAccessLevel(options)} partial class {message.Name} : aelf::IEvent<{message.Name}>");
        printer.PrintLine("{");
        {
            printer.Indent();
            // GetIndexed
            printer.PrintLine($"public global::System.Collections.Generic.IEnumerable<{message.Name}> GetIndexed()");
            printer.PrintLine("{");
            {
                printer.Indent();
                printer.PrintLine($"return new List<{message.Name}>");
                printer.PrintLine("{");
                var fields = message.Fields.InFieldNumberOrder();
                foreach (var field in fields)
                {
                    if (field == null) continue;
                    if (!IsIndexedField(field)) continue;
                    printer.PrintLine($"new {message.Name}");
                    printer.PrintLine("{");
                    {
                        printer.Indent();
                        var propertyName = ProtoUtils.GetPropertyName(field);
                        printer.PrintLine($"{propertyName} = {propertyName}");
                        printer.Outdent();
                    }
                    printer.PrintLine("},");
                }

                printer.PrintLine("};");
                printer.Outdent();
            }
            printer.PrintLine("}"); // end GetIndexed
            printer.PrintLine();

            // GetNonIndexed
            printer.PrintLine($"public {message.Name} GetNonIndexed()");
            printer.PrintLine("{");
            {
                printer.Indent();
                printer.PrintLine($"return new {message.Name}");
                printer.PrintLine("{");
                {
                    printer.Indent();
                    var fields = message.Fields.InFieldNumberOrder();
                    foreach (var field in fields)
                    {
                        if (field == null) continue;
                        if (IsIndexedField(field)) continue;
                        var propertyName = ProtoUtils.GetPropertyName(field);
                        printer.PrintLine($"{propertyName} = {propertyName},");
                    }

                    printer.Outdent();
                }
                printer.PrintLine("};");
                printer.Outdent();
            }
            printer.PrintLine("}");
            printer.Outdent();
        }

        printer.PrintLine("}");
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
}
