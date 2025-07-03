namespace TallyConnector.Abstractions.Models;

public abstract class MetaObject
{
    protected string _pathPrefix;

    public MetaObject(string pathPrefix = "")
    {
        _pathPrefix = pathPrefix;
    }
}
public class Condition
{
    public string Path { get; }
    public string Operator { get; }
    public object Value { get; }

    public Condition(string path, string op, object value)
    {
        Path = path;
        Operator = op;
        Value = value;
    }

    public override string ToString() => $"{Path} {Operator} {Value}";
}


