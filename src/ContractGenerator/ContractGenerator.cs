using Google.Protobuf.Compiler;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

//This is the main entry-point into this project and is exposed to external users
public class ContractGenerator
{
    /// <summary>
    ///     Generates a set of C# files from the input stream containing the proto source. This is the primary entry-point into
    ///     the ContractPlugin.
    /// </summary>
    public CodeGeneratorResponse Generate(Stream stdin)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Determines if the proto-message is of EventType based on Aelf.options
    /// </summary>
    private static bool IsEventMessageType(MessageDescriptor message)
    {
        return message.GetOptions().GetExtension(AElf.OptionsExtensions.IsEvent);
    }

    /// <summary>
    /// Determines if the proto-message is of IndexedType based on Aelf.options
    /// </summary>
    private static bool IsIndexedField(FieldDescriptor field)
    {
        return field.GetOptions().GetExtension(AElf.OptionsExtensions.IsIndexed);
    }

    private static string GetAccessLevel(byte flags)
    {
        throw new NotImplementedException();
    }
    public static void GenerateEvent(ref IndentPrinter printer, MessageDescriptor message, byte flags)
    {
        if (!IsEventMessageType(message)) return;
        printer.Print($"{GetAccessLevel(flags)} partial class {message.Name} : aelf::IEvent<{message.Name}>");
        printer.Print("{");
        {
            printer.Indent();
            // GetIndexed
            printer.Print($"public global::System.Collections.Generic.IEnumerable<{message.Name}> GetIndexed()");
            printer.Print("{");
            {
                printer.Indent();
                printer.Print($"return new List<{message.Name}>");
                printer.Print("{");
                var fields = message.Fields.InFieldNumberOrder();
                foreach (var field in fields)
                {
                    if (!IsIndexedField(field)) continue;
                    printer.Print($"new {message.Name}");
                    printer.Print("{");
                    {
                        printer.Indent();
                        var propertyName = ProtoUtils.GetPropertyName(field);
                        printer.Print($"{propertyName} = {propertyName}");
                        printer.Outdent();
                    }
                    printer.Print("},");
                }

                printer.Print("};");
                printer.Outdent();
            }
            printer.Print("}\n"); // end GetIndexed

            // GetNonIndexed
            printer.Print($"public {message.Name} GetNonIndexed()");
            printer.Print("{");
            {
                printer.Indent();
                printer.Print($"return new {message.Name}");
                printer.Print("{");
                {
                    printer.Indent();
                    var fields = message.Fields.InFieldNumberOrder();
                    foreach (var field in fields)
                    {
                        if (IsIndexedField(field)) continue;
                        var propertyName = ProtoUtils.GetPropertyName(field);
                        printer.Print($"{propertyName} = {propertyName},");
                    }

                    printer.Outdent();
                }
                printer.Print("};");
                printer.Outdent();
            }
            printer.Print("}");
            printer.Outdent();
        }

        printer.Print("}\n");
    }
}
