namespace TallyConnector.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TDLXMLSetAttribute : Attribute
{
    private string? _set;

    public string Set
    {
        get { return _set ?? string.Empty; }
        set { _set = value; }
    }

    private bool _includeInFetch;

    public bool IncludeInFetch
    {
        get { return _includeInFetch; }
        set { _includeInFetch = value; }
    }

    public TDLXMLSetAttribute(string set, bool includeInFetch = false)
    {
        IncludeInFetch = includeInFetch;
        Set = set;
    }

    public TDLXMLSetAttribute()
    {
    }
}
