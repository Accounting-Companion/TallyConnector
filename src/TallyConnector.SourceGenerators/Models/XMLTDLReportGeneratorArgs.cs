namespace TallyConnector.SourceGenerators.Models;
public class XMLTDLReportGeneratorArgs
{
    public string NameSpace { get; set; }
    public string ClassName { get; set; }
    public string ReportName { get; set; }

    public List<XMLTDLReportPart> Parts { get; set; } = new();
    public List<XMLTDLLine> Lines { get; set; } = new();
    public List<XMLTDLField> Fields { get; set; } = new();
}

public class XMLTDLReportPart
{
    public string Use { get; set; }
    public string PartName { get; set; }
    public List<string> Lines { get; set; }
    public string Repeat { get; set; }
    public string Scroll { get; set; }
    public string XMLTag { get; set; }
}
public class XMLTDLLine
{
    public string LineName { get; set; }

    public List<string> Fields { get; set; }
    public string XMLTag { get; set; }

    public List<string> Explode { get; set; }
}

public class XMLTDLField
{
    public string FieldName { get; set; }

    public string XMLTag { get; set; }
    public string? Format { get; set; }
    public string Set { get; set; }
    public string? Type { get; set; }
}

public class XMLTDLCollection
{
    public string CollectionName { get; set; }
    public string Type { get; set; }

}


