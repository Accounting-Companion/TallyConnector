namespace TallyConnector.Models.Base.Masters;
[XmlRoot(ElementName = "COSTCATEGORY")]
[XmlType(AnonymousType = true)]
public partial class CostCategory : BaseAliasedMasterObject
{
    [XmlElement(ElementName = "ALLOCATEREVENUE")]
    public bool? AllocateRevenue { get; set; }

    [XmlElement(ElementName = "ALLOCATENONREVENUE")]
    public bool? AllocateNonRevenue { get; set; }
}
