namespace TallyConnector.Core.Models;
[XmlRoot(ElementName = "ENVELOPE")]
public class LastAlterIdsRoot
{
    [XmlElement(ElementName = "MASTERSLASTID")]
    public int MastersLastAlterId { get; set; }
    [XmlElement(ElementName = "VOUCHERSLASTID")]
    public int VouchersLastAlterId { get; set; }
}
