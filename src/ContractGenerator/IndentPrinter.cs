using System.Text;

namespace ContractGenerator;

public class IndentPrinter
{
    private int _indents;
    private readonly StringBuilder _stringBuilder = new();

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
        var indentedStr = "";
        for (var i = 0; i < _indents; i++) indentedStr += " ";
        _stringBuilder.AppendLine(indentedStr + str);
    }

    public string PrintOut()
    {
        return _stringBuilder.ToString();
    }
}
