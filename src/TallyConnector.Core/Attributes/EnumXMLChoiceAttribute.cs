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

    public string Choice { get; set; } = null!;
    public string[] Versions { get; set; } = [];
    public string[] Terminology { get; set; } = [];
}


