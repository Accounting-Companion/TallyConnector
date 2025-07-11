
using TallyConnector.TDLReportSourceGenerator.Services;

namespace TallyConnector.TDLReportSourceGenerator.Models;

public class XMLData
{
    public string? XmlTag { get; set; }

    public List<EnumChoiceData> EnumChoices { get; set; } = [];

    public INamedTypeSymbol? Symbol { get; set; }

    public ClassData? ClassData { get; set; }
    public bool IsAttribute { get; set; }
    public bool Exclude { get; internal set; }
    public string? FieldName { get; internal set; }
    public string? CollectionPrefix { get; internal set; }
}

public class EnumChoiceData
{
    public EnumChoiceData(string choice, string[]? versions = null)
    {
        Choice = choice;
        Versions = versions ?? [];
    }

    public string Choice { get; }

    public string[] Versions { get; }
}