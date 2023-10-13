using System.Text;

namespace ContractGenerator;

public class IndentPrinter
{
    private readonly StringBuilder _stringBuilder = new();
    private int _indents;

    /// <summary>
    /// Increase the indentation for all content written afterwards.
    /// </summary>
    public void Indent()
    {
        _indents++;
    }

    /// <summary>
    /// Reduce the indentation for all content written afterwards.
    /// </summary>
    /// <exception cref="InvalidOperationException">If there's not current indentation, calling this method will cause
    /// an exception to be thrown.</exception>
    public void Outdent()
    {
        if (_indents == 0) throw new InvalidOperationException("No more indentation to reduce.");

        _indents--;
    }

    /// <summary>
    /// Prints an empty line.
    /// </summary>
    /// <remarks>The name <c>___EmptyLine___</c> is used so that it's easy to see from code.</remarks>
    public void ___EmptyLine___()
    {
        _(String.Empty);
    }

    public void Print(string str)
    {
        if (string.IsNullOrEmpty(str)) return;
        var lines = str.Split(Environment.NewLine);
        foreach (var line in lines.SkipLast(1))
        {
            PrintOneLine(line);
        }

        _stringBuilder.Append(lines.Last());
    }

    public void PrintIgnoreWhitespace(string? str)
    {
        if (string.IsNullOrWhiteSpace(str)) return;
        Print(str);
    }

    /// <summary>
    /// Prints a line.
    /// </summary>
    /// <param name="content">Content to be printed. A new line will be appended at the end.</param>
    /// <remarks>The name <c>_</c> is used so that it won't distract readers from the content being printed.</remarks>
    public void _(string content)
    {
        var lines = content.Split(Environment.NewLine);
        foreach (var line in lines)
        {
            PrintOneLine(line);
        }
    }

    public string PrintOut()
    {
        return _stringBuilder.ToString();
    }

    private void PrintOneLine(string line)
    {
        if (line.Trim().Length == 0)
        {
            _stringBuilder.AppendLine();
            return;
        }

        for (var i = 0; i < _indents; i++) _stringBuilder.Append("  ");
        _stringBuilder.AppendLine(line);
    }
}
