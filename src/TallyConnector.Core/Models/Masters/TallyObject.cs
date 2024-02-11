namespace TallyConnector.Core.Models.Masters;
public class BaseTallyObject : IBaseTallyObject
{
    [XmlElement("GUID")]
    [IgnoreForCreateDTO]
    public string GUID { get; set; }
}
public class TallyObject : BaseTallyObject, ITallyObject
{
    [XmlElement("MASTERID")]
    [IgnoreForCreateDTO]
    public int MasterId { get; set; }

    [IgnoreForCreateDTO]
    [XmlElement("ALTERID")]
    public int AlterId { get; set; }

    [XmlElement("REMOTEALTGUID")]
    public string RemoteId { get; set; }
}
