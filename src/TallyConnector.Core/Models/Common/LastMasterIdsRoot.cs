namespace TallyConnector.Core.Models.Common;
[XmlRoot(ElementName = "ENVELOPE")]
public class LastAlterIdsRoot : IBaseObject
{
    [XmlElement(ElementName = "MASTERSLASTID")]
    public int MastersLastAlterId { get; set; }
    [XmlElement(ElementName = "VOUCHERSLASTID")]
    public int VouchersLastAlterId { get; set; }
}
