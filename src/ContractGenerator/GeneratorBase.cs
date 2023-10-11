namespace ContractGenerator;

public class GeneratorBase
{
    protected IndentPrinter indentPrinter;

    public GeneratorBase(IndentPrinter? printer)
    {
        indentPrinter = printer ?? new IndentPrinter();
    }

    protected void InRegion(string name, Action a)
    {
        indentPrinter.PrintLine($"#region {name}");
        a();
        indentPrinter.PrintLine($"#endregion {name}");
    }

    protected void InBlock(Action a)
    {
        indentPrinter.PrintLine("{");
        indentPrinter.Indent();
        a();
        indentPrinter.Outdent();
        indentPrinter.PrintLine("}");
    }

    protected void InBlockWithSemicolon(Action a)
    {
        indentPrinter.PrintLine("{");
        indentPrinter.Indent();
        a();
        indentPrinter.Outdent();
        indentPrinter.PrintLine("};");
    }
    protected void InBlockWithComma(Action a)
    {
        indentPrinter.PrintLine("{");
        indentPrinter.Indent();
        a();
        indentPrinter.Outdent();
        indentPrinter.PrintLine("};");
    }
}
