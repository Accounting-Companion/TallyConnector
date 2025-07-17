namespace TallyConnector.Abstractions.Models;

public class Filter
{
    public Filter()
    {
    }

    public Filter(string filterName, string filterFormulae, bool excludeinCollection = false)
    {
        FilterName = filterName;
        FilterFormulae = filterFormulae;
        ExcludeinCollection = excludeinCollection;
    }

    public string FilterName { get; set; }

    public string FilterFormulae { get; set; }
    public bool ExcludeinCollection { get; set; }
}
public abstract class FilterNode
{
    public abstract string ToDebugString(int indent = 0);
}

public class BinaryFilterNode : FilterNode
{
    public string Operator { get; }
    public string PropertyPath { get; }
    public object Value { get; }

    public BinaryFilterNode(string op, string prop, object value)
    {
        Operator = op;
        PropertyPath = prop;
        Value = value;
    }

    public override string ToDebugString(int indent = 0)
    {
        var pad = new string(' ', indent);
        return $"{pad}{PropertyPath} {Operator} {Value}";
    }
}

public class MethodFilterNode : FilterNode
{
    public string Method { get; }
    public string PropertyPath { get; }
    public object Argument { get; }

    public MethodFilterNode(string method, string prop, object arg)
    {
        Method = method;
        PropertyPath = prop;
        Argument = arg;
    }

    public override string ToDebugString(int indent = 0)
    {
        var pad = new string(' ', indent);
        return $"{pad}{PropertyPath}.{Method}({Argument})";
    }
}

public class LogicalFilterNode : FilterNode
{
    public string Operator { get; }
    public FilterNode Left { get; }
    public FilterNode Right { get; }

    public LogicalFilterNode(string op, FilterNode left, FilterNode right)
    {
        Operator = op;
        Left = left;
        Right = right;
    }

    public override string ToDebugString(int indent = 0)
    {
        var pad = new string(' ', indent);
        return $"{pad}(\n{Left.ToDebugString(indent + 2)}\n{pad} {Operator}\n{Right.ToDebugString(indent + 2)}\n{pad})";
    }
}
