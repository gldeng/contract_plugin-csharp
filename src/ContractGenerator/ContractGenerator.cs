using AElf;
using Google.Protobuf.Compiler;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public class FlagConstants
{
    public const byte GenerateContract = 0x01; // hex for 0000 0001
    public const byte GenerateStub = 0x02; // hex for 0000 0010
    public const byte GenerateReference = 0x04; // hex for 0000 0100
    public const byte GenerateEvent = 0x08; // hex for 0000 1000
    public const byte InternalAccess = 0x80; // hex for 1000 0000

    public const byte GenerateContractWithEvent = GenerateContract | GenerateEvent;
    public const byte GenerateStubWithEvent = GenerateStub | GenerateEvent;
}

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
    ///     Determines if the proto-message is of EventType based on Aelf.options
    /// </summary>
    private static bool IsEventMessageType(MessageDescriptor message)
    {
        return message.GetOptions().GetExtension(OptionsExtensions.IsEvent);
    }

    /// <summary>
    ///     Determines if the proto-message is of IndexedType based on Aelf.options
    /// </summary>
    private static bool IsIndexedField(FieldDescriptor field)
    {
        return field.GetOptions() != null && field.GetOptions().GetExtension(OptionsExtensions.IsIndexed);
    }

    public static void GenerateEvent(ref IndentPrinter printer, MessageDescriptor message, byte flags)
    {
        if (!IsEventMessageType(message)) return;
        printer.Print(
            $"{ProtoUtils.GetAccessLevel(flags)} partial class {message.Name} : aelf::IEvent<{message.Name}>");
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
                    if (field == null) continue;
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
                        if (field == null) continue;
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
