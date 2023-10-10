using ContractGenerator;

namespace ContractPlugin;

public static class ParameterParser
{
    internal static GeneratorOptions Parse(string parameter)
    {
        var options = new GeneratorOptions()
        {
            GenerateEvent = true,
            GenerateContract = true
        };
        var pairs = new List<Tuple<string, string>>();
        if (parameter != "") ProtoUtils.ParseGeneratorParameter(parameter, pairs);
        foreach (var (key, _) in pairs)
            switch (key)
            {
                case "stub":
                    options.GenerateStub = true;
                    options.GenerateEvent = true;
                    options.GenerateContract = false;
                    break;
                case "reference":
                    // Reference doesn't require an event
                    options.GenerateReference = true;
                    options.GenerateEvent = false;
                    options.GenerateContract = false;
                    break;
                case "nocontract":
                    options.GenerateContract = false;
                    break;
                case "noevent":
                    options.GenerateEvent = false;
                    break;
                case "internal_access":
                    options.InternalAccess = true;
                    break;
                case "":
                    break;
                default:
                    throw new Exception("Unknown parameter key: " + key);
            }

        return options;
    }
}
