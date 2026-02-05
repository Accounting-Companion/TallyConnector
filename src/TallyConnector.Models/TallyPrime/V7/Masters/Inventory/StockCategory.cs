namespace TallyConnector.Models.TallyPrime.V7.Masters.Inventory;

[XmlRoot(ElementName = "STOCKCATEGORY")]
[XmlType(AnonymousType = true)]
[GenerateITallyRequestableObect]
[GenerateMeta]
public partial class StockCategory : V6.Masters.Inventory.StockCategory
{
}
