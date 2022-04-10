namespace TallyConnector.Models;

class ObjEnv
{
}

[XmlRoot(ElementName = "ENVELOPE")]
public class ObjEnvelope : TallyXmlJson
{
    [XmlElement(ElementName = "HEADER")]
    public ObjHeader? Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public ObjBody Body { get; set; } = new();

}

[XmlRoot(ElementName = "HEADER")]
public class ObjHeader
{
    public ObjHeader(string subtype, string ObjID)
    {
        this._request = "Export";
        this._type = "Object";
        this.SubType = subtype;
        ID.Text = ObjID;
    }
    public ObjHeader() { }
    private int _version = 1;
    private string? _request;
    private string? _type;
    [XmlElement(ElementName = "VERSION")]
    public int Version { get { return _version; } set { _version = value; } }

    [XmlElement(ElementName = "TALLYREQUEST")]
    public string? Request { get { return _request; } set { _request = value; } }

    [XmlElement(ElementName = "TYPE")]
    public string? Type { get { return _type; } set { _type = value; } }

    [XmlElement(ElementName = "SUBTYPE")]
    public string? SubType { get; set; }

    [XmlElement(ElementName = "ID")]
    public ID ID { get; set; } = new();


}

[XmlRoot(ElementName = "ID")]
public class ID
{

    [XmlAttribute(AttributeName = "TYPE")]
    public string Type { get { return "Name"; } set { } }

    [XmlText]
    public string? Text { get; set; }
}
[XmlRoot(ElementName = "BODY")]
public class ObjBody
{
    [XmlElement(ElementName = "DESC")]
    public ObjDescription Desc { get; set; } = new();

}

[XmlRoot(ElementName = "DESC")]
public class ObjDescription
{
    [XmlElement(ElementName = "STATICVARIABLES")]
    public StaticVariables StaticVariables { get; set; } = new();

    [XmlElement(ElementName = "FETCHLIST")]
    public FetchList? FetchList { get; set; }

}
[XmlRoot(ElementName = "FETCHLIST")]
public class FetchList
{
    public FetchList()
    {
        LFetch.Add("*");
    }

    public FetchList(List<string> FList)
    {
        LFetch = FList;
    }
    [XmlElement(ElementName = "FETCH")]
    public List<string> LFetch { get; set; } = new();
}
