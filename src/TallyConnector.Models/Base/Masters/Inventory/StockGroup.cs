using TallyConnector.Models.Common;

namespace TallyConnector.Models.Base.Masters.Inventory;
[XmlRoot(ElementName = "STOCKGROUP")]
[XmlType(AnonymousType = true)]
public class StockGroup : BaseAliasedMasterObject
{
    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "ISADDABLE")]
    public bool? IsAddable { get; set; }

    [XmlElement(ElementName = "GSTAPPLICABLE")]
    public string? GSTApplicability { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    public string? BaseUnit { get; set; }

    [XmlElement(ElementName = "GSTDETAILS.LIST")]
    [TDLCollection(CollectionName = "GSTDETAILS", ExplodeCondition = "$$NUMITEMS:GSTDETAILS>0")]
    public List<GSTDetail>? GSTDetails { get; set; }
}


