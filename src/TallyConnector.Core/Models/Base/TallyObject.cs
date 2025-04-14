using TallyConnector.Core.Models.Interfaces;

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
    public int MasterId { get; set; }

    [IgnoreForCreateDTO]
    [XmlElement(ElementName = "ALTERID")]
    public int AlterId { get; set; }

}
