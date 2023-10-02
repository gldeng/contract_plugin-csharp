namespace ContractGenerator.Tests;

public class IndentPrinterTests
{
    [Fact]
    public void Test_Indent_With_Print_And_PrintOut()
    {
        var indentPrinter = new IndentPrinter();
        indentPrinter.Indent();
        indentPrinter.Print("test func()");
        var outputCode = indentPrinter.PrintOut();
        Assert.Equal(" test func()\n", outputCode);
    }

    [Fact]
    public void Test_2Indent_And_2Outdent_With_Print_And_PrintOut()
    {
        var indentPrinter = new IndentPrinter();
        indentPrinter.Indent();
        indentPrinter.Print("test func(){");
        indentPrinter.Indent();
        indentPrinter.Print("var someFields = new SomeField();");
        indentPrinter.Outdent();
        indentPrinter.Print("}");
        indentPrinter.Outdent();
        indentPrinter.Print("//done");
        var outputCode = indentPrinter.PrintOut();
        Assert.Equal(" test func(){\n  var someFields = new SomeField();\n }\n//done\n", outputCode);
    }

    [Fact]
    public void Test_1Indent_With_2Outdents_Returns_Exception()
    {
        // Arrange: Set up your test scenario
        var indentPrinter = new IndentPrinter();
        indentPrinter.Indent();
        indentPrinter.Print("test func()");
        indentPrinter.Outdent();
        var action = () => indentPrinter.Outdent();
        // Act & Assert: Use Assert.Throws to assert that an exception is thrown
        var exception = Assert.Throws<Exception>(action);

        Assert.Equal("nothing left to outdent", exception.Message);
    }
}
