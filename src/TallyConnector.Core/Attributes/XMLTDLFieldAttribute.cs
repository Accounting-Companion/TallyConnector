namespace TallyConnector.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class XMLTDLFieldAttribute : Attribute
{
    private string? _set;

    public string Set
    {
        get { return _set ?? string.Empty; }
        set { _set = value; }
    }
    private string _dataType;

    public string DataType
    {
        get { return _dataType; }
        set { _dataType = value; }
    }

    private bool _excludeInFetch;

    public bool ExcludeInFetch
    {
        get { return _excludeInFetch; }
        set { _excludeInFetch = value; }
    }

    public XMLTDLFieldAttribute(string set, bool excludeInFetch = false)
    {
        ExcludeInFetch = excludeInFetch;
        Set = set;
    }

    public XMLTDLFieldAttribute()
    {

    }
}
