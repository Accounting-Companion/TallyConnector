namespace TallyConnector.Core.Models.Masters;
public class BaseTallyObject : IBaseTallyObject
{
    [XmlElement(ElementName = "GUID")]
    [IgnoreForCreateDTO]
    [TDLField(ExcludeInFetch = true)]
    public string GUID { get; set; }

    [XmlElement(ElementName = "REMOTEALTGUID")]
    public string RemoteId { get; set; }
}
public class TallyObject : BaseTallyObject, ITallyObject
{
    [XmlElement(ElementName = "MASTERID")]
    [IgnoreForCreateDTO]
    [TDLField(ExcludeInFetch = true)]
    public int MasterId { get; set; }

    [IgnoreForCreateDTO]
    [XmlElement(ElementName = "ALTERID")]
    public int AlterId { get; set; }

}
