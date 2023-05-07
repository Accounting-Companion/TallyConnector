namespace TallyConnector.Core.Models.Common.Request;
[XmlRoot("ENVELOPE")]
public class RequestEnvelope
{
    public RequestHeader Header { get; set; }
    public RequestBody Body { get; set; }
}

public class RequestHeader
{
    public string Version { get; set; }
    public string TallyRequest { get; set; }
    public string Type { get; set; }
    public string Id { get; set; }
}
public class RequestBody
{
    [XmlElement("DESC")]
    public RequestDescription Description { get; set; }
}
public class RequestDescription
{
    public StaticVariables StaticVariables { get; set; }
    public TDL TDL { get; set; }
}
public class StaticVariables
{
    [XmlElement("SVEXPORTFORMAT")]
    public string ExportFormat { get; set; }

    [XmlElement("SVCURRENTCOMPANY")]
    public string Company { get; set; }

    [XmlElement("SVFROMDATE")]
    public string FromDate { get; set; }

    [XmlElement("SVTODATE")]
    public string ToDate { get; set; }
}

public class TDL
{
    public List<Report> Reports { get; set; }
    public List<Form> Forms { get; set; }
    public List<Line> Lines { get; set; }
    public List<Field> Fields { get; set; }
}
public class Report : TallyObjectAttributes
{

}
public class Form : TallyObjectAttributes
{

}
public class Line : TallyObjectAttributes
{

}
public class Field : TallyObjectAttributes
{

}