namespace TallyConnector.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TDLXMLSetAttribute : Attribute
{
    private string? _set;

    public string? Set
    {
        get { return _set; }
        set { _set = value; }
    }

}
