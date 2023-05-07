namespace TallyConnector.SourceGenerators.Models;
public class TDLReportParams
{
    public TDLReportParams()
    {
        Fields = new();
        Parts = new();
    }

    public string ReportName { get; set; }
    public List<TDLField> Fields { get; set; }
    public List<TDLPart> Parts { get; set; }
}
public class TDLField
{
    public string Name { get; set; }
    public string Set { get; set; }
    public string XmlTag { get; set; }

}
public class TDLPart
{
    public string CollectionName { get; set; }

    public List<TDLField> Fields { get; set; } = new();
    public List<TDLPart> Parts { get; set; } = new();
}
