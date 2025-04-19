namespace TallyConnector.Core.Models.Base;
public class BaseTallyObject : IBaseTallyObject
{
    [XmlElement(ElementName = "GUID")]
    [IgnoreForCreateDTO]
    [TDLField(ExcludeInFetch = true)]
    public string GUID { get; set; } = null!;

    [XmlElement(ElementName = "REMOTEALTGUID")]
    public string RemoteId { get; set; } = null!;
}
public class TallyObject : BaseTallyObject, ITallyObject
{
    [XmlElement(ElementName = "MASTERID")]
    [IgnoreForCreateDTO]
    [TDLField(ExcludeInFetch = true)]
    public ulong MasterId { get; set; }

    [IgnoreForCreateDTO]
    [XmlElement(ElementName = "ALTERID")]
    public ulong AlterId { get; set; }

    [IgnoreForCreateDTO]
    [XmlElement(ElementName = "ENTEREDBY")]
    public string? EnteredBy { get; set; }

    [IgnoreForCreateDTO]
    [XmlElement(ElementName = "ALTEREDBY")]
    public string? AlteredBy { get; set; }

}
