namespace TallyConnector.Core.Models.Common.Request;
[XmlRoot("ENVELOPE")]
public class RequestEnvelope
{
    public RequestHeader Header { get; set; } = new();
    public RequestBody Body { get; set; }
}

public class RequestHeader
{
    public string Version { get; set; } = "1";
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

    public ReqTDL TDL { get; set; } = new();
}
public class StaticVariables
{
    [XmlElement("SVEXPORTFORMAT")]
    public string ExportFormat { get; set; }


    public string sdf { get; set; }

    [XmlElement("SVCURRENTCOMPANY")]
    public string Company { get; set; }

    [XmlElement("SVFROMDATE")]
    public string FromDate { get; set; }

    [XmlElement("SVTODATE")]
    public string ToDate { get; set; }
}
[XmlRoot(ElementName = "TDL")]
public class ReqTDL
{
    public ReqTDL()
    {
    }

    [XmlElement(ElementName = "TDLMESSAGE")]
    public TDLMessage TDLMessage { get; set; } = new();
}
public class TDLMessage
{
    public List<Report> Reports { get; set; }
    public List<Form> Forms { get; set; }
    public List<Part> Parts { get; set; }
    public List<Line> Lines { get; set; }
    public List<Field> Fields { get; set; }
}
public class Report : TallyObjectAttributes
{

    public Report(string name,
                  bool isModify = false,
                  bool isFixed = false,
                  bool isInitialize = false,
                  bool isOption = false,
                  bool isInternal = false) : base(name,
                                                  isModify,
                                                  isFixed,
                                                  isInitialize,
                                                  isOption,
                                                  isInternal)
    {
        Form = name;
    }
    public string Form { get; set; }
}
public class Form : TallyObjectAttributes
{


    public Form(string name,
                bool isModify = false,
                bool isFixed = false,
                bool isInitialize = false,
                bool isOption = false,
                bool isInternal = false) : base(name,
                                                isModify,
                                                isFixed,
                                                isInitialize,
                                                isOption,
                                                isInternal)
    {
        Part = name;
    }

    public string Part { get; set; }

    public string XmlTag { get; set; } = string.Empty;
}
public class Part : TallyObjectAttributes
{

    public Part(string name,
                bool isModify = false,
                bool isFixed = false,
                bool isInitialize = false,
                bool isOption = false,
                bool isInternal = false) : base(name,
                                                isModify,
                                                isFixed,
                                                isInitialize,
                                                isOption,
                                                isInternal)
    {
    }

    public Part(string name, string collectionName,string? xmlTag=null) : base(name)
    {
        Line = name;
        Repeat = $"{name}:{collectionName}";
        Scrolled = "Vertical";
        XmlTag = xmlTag;
    }
    public string Line { get; set; }
    public string? Repeat { get; set; }
    public string? Scrolled { get; set; }
    public string? XmlTag { get; set; }
}
public class Line : TallyObjectAttributes
{


    public Line(string name,
                bool isModify = false,
                bool isFixed = false,
                bool isInitialize = false,
                bool isOption = false,
                bool isInternal = false) : base(name,
                                                isModify,
                                                isFixed,
                                                isInitialize,
                                                isOption,
                                                isInternal)
    {
    }

    public Line(string name, List<string>? fields, string? use, string? xmlTag = null) : base(name,use)
    {
        Fields = fields;
        XmlTag = xmlTag;
    }

    public Line(string name, string? use, string? xmlTag = null) : base(name, use)
    {
        XmlTag = xmlTag;
    }

    public List<string>? Fields { get; set; }
    public string? XmlTag { get; set; }
    [XmlElement("EXPLODE")]
    public List<string> Explodes { get; set; } = new();
}
public class Field : TallyObjectAttributes
{


    public Field(string name,
                string XMLTag,
                 bool isModify = false,
                 bool isFixed = false,
                 bool isInitialize = false,
                 bool isOption = false,
                 bool isInternal = false) : base(name,
                                                 isModify,
                                                 isFixed,
                                                 isInitialize,
                                                 isOption,
                                                 isInternal)
    {
        this.XMLTag = XMLTag;
    }

    public Field(string name, string XMLTag, string? set, string? use, string? tallyType = null) : this(name, XMLTag)
    {
        Set = set;
        TallyType = tallyType;
        Use = use;
    }
    public string? Set { get; set; }
    public string XMLTag { get; set; }
    public string? TallyType { get; set; }
    public string? Format { get; set; }


}
