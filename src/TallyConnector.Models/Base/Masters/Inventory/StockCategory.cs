namespace TallyConnector.Models.Base.Masters.Inventory;

[XmlRoot(ElementName = "STOCKCATEGORY")]
[XmlType(AnonymousType = true)]
public class StockCategory : BaseAliasedMasterObject
{
    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }
}
