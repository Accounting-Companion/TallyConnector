﻿namespace TallyConnector.Core.Models.Response;


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

[XmlRoot(ElementName = "RESULT")]
public class PostResult
{

    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; } 

    [XmlElement(ElementName = "MASTERID")]
    public int MasterId { get; set; }

    [XmlElement(ElementName = "OBJECTTYPE")]
    public string? ObjectType { get; set; }

    [XmlElement(ElementName = "GUID")]
    public string? GUID { get; set; }

    [XmlElement(ElementName = "REMOTEID")]
    public string? RemoteId { get; set; }

    [XmlElement(ElementName = "ACTION")]
    public string? Action { get; set; }

    [XmlElement(ElementName = "ERROR")]
    public string? Error { get; set; }
}

[XmlRoot(ElementName = "RESULTS")]
public class PostResults
{

    [XmlElement(ElementName = "RESULT")]
    public List<PostResult> Results { get; set; } = [];
}

[XmlRoot("ENVELOPE")]
public class ReportResponseEnvelope<T> where T : ITallyRequestableObject
{
    public static Type TypeInfo = typeof(ReportResponseEnvelope<T>);
    public List<T> Objects { get; set; } = [];

    [System.Xml.Serialization.XmlElementAttribute(ElementName = "TC_TOTALCOUNT")]
    public int? TotalCount { get; set; }
}