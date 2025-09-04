using TallyConnector.Core.Models;

namespace TallyConnector.Models.Base;
[MaptoDTO<BaseTallyObjectDTO>]
public partial class BaseTallyObject : IBaseTallyObject
{
    [XmlElement(ElementName = "GUID")]
    [IgnoreForCreateDTO]
    [TDLField(ExcludeInFetch = true)]
    public string GUID { get; set; } = null!;

    [XmlElement(ElementName = "REMOTEALTGUID")]
    public string RemoteId { get; set; } = null!;
}
public class BaseTallyObjectDTO
{
    private string? _remoteId;
    [XmlAttribute("ACTION")]
    public Core.Models.Action Action { get; set; }

    [XmlElement(ElementName = "REMOTEALTGUID")]
    public string RemoteId { get { return _remoteId ??= SetRemoteId(suffix:GUIDSuffix); } set { _remoteId = value; } }

    public string SetRemoteId(string? prefix=null,string? suffix=null)
    {
        string guid = Guid.NewGuid().ToString();
        if (string.IsNullOrWhiteSpace(prefix))
        {
            guid = $"{prefix}-{guid}";
        }
        if (string.IsNullOrWhiteSpace(suffix))
        {
            guid = $"{guid}-{suffix}";
        }
        return _remoteId = guid;
    }
}

public partial class TallyObject : BaseTallyObject, ITallyObject
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
