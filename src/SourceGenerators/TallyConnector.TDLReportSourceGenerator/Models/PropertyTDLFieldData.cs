namespace TallyConnector.TDLReportSourceGenerator.Models;

public class PropertyTDLFieldData
{
    public string Set { get; internal set; } = string.Empty;
    public bool ExcludeInFetch { get; internal set; }
    public string? Use { get; internal set; }
    public string? TallyType { get; internal set; }
    public string? Format { get; internal set; }
    public string? Invisible { get; internal set; }
    public string? FetchText { get; internal set; }
}

