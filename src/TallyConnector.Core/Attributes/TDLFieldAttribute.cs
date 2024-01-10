namespace TallyConnector.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TDLFieldAttribute : Attribute
{
    public string Set { get; set; }

    public bool IncludeInFetch { get; set; }

    public string? Use { get; set; }
    public string? TallyType { get; set; }
    public string? Format { get; set; }
    public TDLFieldAttribute(string set)
    {
        Set = set;
    }

    public TDLFieldAttribute()
    {
    }
}
