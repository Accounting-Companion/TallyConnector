namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class EnumXMLChoiceAttribute : Attribute
{
    public EnumXMLChoiceAttribute()
    {
    }

    public EnumXMLChoiceAttribute(string choice)
    {
        Choice = choice;
    }

    public string Choice { get; set; }
    public string Version { get; set; }
}
