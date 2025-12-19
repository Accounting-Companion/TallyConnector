namespace TallyConnector.Models.Common;
[XmlRoot(ElementName = "ENVELOPE")]
public class LastAlterIdsRoot : IBaseObject
{
    [XmlElement(ElementName = "MASTERSLASTID")]
    public ulong MastersLastAlterId { get; set; }
    [XmlElement(ElementName = "VOUCHERSLASTID")]
    public ulong VouchersLastAlterId { get; set; }
}
