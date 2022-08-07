namespace TallyConnector.Core.Models;


[XmlRoot(ElementName = "ENVELOPE")]
public class ResponseEnvelope
{

    [XmlElement(ElementName = "HEADER")]
    public RHeader? Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public RBody? Body { get; set; }
}


[XmlRoot(ElementName = "HEADER")]
public class RHeader
{

    [XmlElement(ElementName = "VERSION")]
    public int Version { get; set; }

    [XmlElement(ElementName = "STATUS")]
    public int Status { get; set; }
}

[XmlRoot(ElementName = "BODY")]
public class RBody
{

    [XmlElement(ElementName = "DATA")]
    public Rdata? Data { get; set; }

    //[XmlElement(ElementName = "DESC")]
    //public Desc Desc { get; set; }
}

[XmlRoot(ElementName = "DATA")]
public class Rdata
{
    [XmlElement(ElementName = "LINEERROR")]
    public string? LineError { get; set; }

    [XmlElement(ElementName = "IMPORTRESULT")]
    public ImportResult? ImportResult { get; set; }
}

[XmlRoot(ElementName = "IMPORTRESULT")]
public class ImportResult
{


    [XmlElement(ElementName = "CREATED")]
    public int? Created { get; set; }

    [XmlElement(ElementName = "ALTERED")]
    public int? Altered { get; set; }

    [XmlElement(ElementName = "DELETED")]
    public int? Deleted { get; set; }

    [XmlElement(ElementName = "LASTVCHID")]
    public int? LastVchId { get; set; }

    [XmlElement(ElementName = "LASTMID")]
    public int? LastMID { get; set; }

    [XmlElement(ElementName = "COMBINED")]
    public int? Combined { get; set; }

    [XmlElement(ElementName = "IGNORED")]
    public int? Ignored { get; set; }

    [XmlElement(ElementName = "ERRORS")]
    public int? Errors { get; set; }

    [XmlElement(ElementName = "CANCELLED")]
    public int? Cacelled { get; set; }
}

