namespace ContractGenerator.Tests;

public class ProtoUtilsTests
{
    [Theory]
    [InlineData("foo_bar.baz", "FooBar.Baz")]
    public void Test_UnderscoresToCamelCase_CapNextLetter_And_PreservingPeriod(string input, string output)
    {
        var o = ProtoUtils.UnderscoresToCamelCase(input, true, true);
        Assert.Equal(output, o);
    }

    [Theory]
    [InlineData("foo_bar.baz", "FooBarBaz")]
    public void Test_UnderscoresToCamelCase_CapNextLetter_And_NotPreservingPeriod(string input, string output)
    {
        var o = ProtoUtils.UnderscoresToCamelCase(input, true, false);
        Assert.Equal(output, o);
    }
}
