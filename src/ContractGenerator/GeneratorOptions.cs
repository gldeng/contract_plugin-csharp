namespace ContractGenerator;

public enum GenerateType
{
    None,
    Contract,
    Reference,
    Stub
}

// foreach (var option in options)
//     switch (option.Item1)
//     {
//         case "stub":
//             flags |= FlagConstants.GenerateStubWithEvent;
//             flags &= FlagConstants.GenerateContract;
//             break;
//         case "reference":
//             // Reference doesn't require an event
//             flags |= FlagConstants.GenerateReference;
//             flags &= FlagConstants.GenerateContract;
//             break;
//         case "nocontract":
//             flags &= FlagConstants.GenerateContract;
//             break;
//         case "noevent":
//             flags &= FlagConstants.GenerateEvent;
//             break;
//         case "internal_access":
//             flags |= FlagConstants.InternalAccess;
//             break;
//         case "":
//             break;
//         default:
//             throw new Exception("Unknown generator option: " + option);
//     }

public class GeneratorOptions
{
    public bool InternalAccess { get; set; }
    public bool GenerateContract { get; set; }
    public bool GenerateReference { get; set; }
    public bool GenerateStub { get; set; }
    public bool GenerateEvent { get; set; }

    public bool GenerateEventOnly => GenerateEvent & !GenerateContract & !GenerateReference & !GenerateStub;
    public bool GenerateContainer => GenerateContract | GenerateReference | GenerateStub;
}
