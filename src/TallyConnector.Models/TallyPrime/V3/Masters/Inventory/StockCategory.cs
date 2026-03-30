namespace TallyConnector.Models.TallyPrime.V3.Masters.Inventory;

[XmlRoot(ElementName = "STOCKCATEGORY")]
[XmlType(AnonymousType = true)]
[GenerateITallyRequestableObect]
[GenerateMeta]
public partial class StockCategory : Base.Masters.Inventory.StockCategory
{
}
