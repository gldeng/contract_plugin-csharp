using System.Text;

namespace ContractGenerator;

public class IndentPrinter
{
    private readonly StringBuilder _stringBuilder = new();
    private int _indents;

    public void Indent()
    {
        _indents++;
    }

    public void Outdent()
    {
        if (_indents == 0) throw new Exception("nothing left to outdent");

        _indents--;
    }

    public void Print(string str)
    {
        for (var i = 0; i < _indents; i++) _stringBuilder.Append("  ");
        _stringBuilder.AppendLine(str);
    }

    public string PrintOut()
    {
        return _stringBuilder.ToString();
    }
}
