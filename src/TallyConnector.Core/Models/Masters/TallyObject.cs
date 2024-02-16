namespace TallyConnector.Core.Models.Masters;
public class BaseTallyObject : IBaseTallyObject
{
    [XmlElement("GUID")]
    [IgnoreForCreateDTO]
    [TDLField(IncludeInFetch = true)]
    public string GUID { get; set; }
}
public class TallyObject : BaseTallyObject, ITallyObject
{
    [XmlElement("MASTERID")]
    [IgnoreForCreateDTO]
    [TDLField(IncludeInFetch = true)]
    public int MasterId { get; set; }

    [IgnoreForCreateDTO]
    [XmlElement("ALTERID")]
    [TDLField(IncludeInFetch = true)]
    public int AlterId { get; set; }

    [XmlElement("REMOTEALTGUID")]
    [TDLField(IncludeInFetch = true)]
    public string RemoteId { get; set; }
}
