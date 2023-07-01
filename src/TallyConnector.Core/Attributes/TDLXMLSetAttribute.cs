namespace TallyConnector.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class TDLXMLSetAttribute : Attribute
{
    public string Set { get; set; }

    public bool ExcludeInFetch { get; set; }

    public string? Use { get; set; }
    public string? TallyType { get; set; }
    public string? Format { get; set; }
    public TDLXMLSetAttribute(string set)
    {
        Set = set;
    }

    public TDLXMLSetAttribute()
    {
    }
}
