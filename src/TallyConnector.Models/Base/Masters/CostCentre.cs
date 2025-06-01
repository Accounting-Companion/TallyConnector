namespace TallyConnector.Models.Base.Masters;
[XmlRoot(ElementName = "COSTCENTRE")]
[XmlType(AnonymousType = true)]
public partial class CostCentre : BaseAliasedMasterObject
{
    [XmlElement(ElementName = "CATEGORY")]
    public string? Category { get; set; }

    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "EMAILID")]
    public string? EmailId { get; set; }

    [XmlElement(ElementName = "REVENUELEDFOROPBAL")]
    public bool? ShowOpeningBal { get; set; }

}
