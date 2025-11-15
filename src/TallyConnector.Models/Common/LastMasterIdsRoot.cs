namespace TallyConnector.Models.Common;
[XmlRoot(ElementName = "ENVELOPE")]
public class LastAlterIdsRoot : IBaseObject
{
    [XmlElement(ElementName = "MASTERSLASTID")]
    public uint MastersLastAlterId { get; set; }
    [XmlElement(ElementName = "VOUCHERSLASTID")]
    public uint VouchersLastAlterId { get; set; }
}
