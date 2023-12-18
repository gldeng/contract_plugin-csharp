using ContractGenerator;
using Shouldly;

namespace ContractPlugin.Tests;

public class ContractPluginTests
{
    [Fact]
    public void ParseTests()
    {
        GeneratorOptions options = ParameterParser.Parse("stub,nocontract");
        options.GenerateContract.ShouldBe(false);
    }
}
