using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
using TallyConnector.Models.Base;
using TallyConnector.Models.Common;

namespace SourceGenReproduceTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var c = Voucher.Meta.Fields;
    }
}
public class GSTDetail : TallyConnector.Models.Common.GSTDetail
{
    [XmlIgnore]
    public int Id { get; set; }
}
public partial class StockItem : TallyConnector.Models.TallyPrime.V7.Masters.Inventory.StockItem
{
    [XmlElement(ElementName = "GSTRATEDETAILS.LIST", Type = typeof(GSTRateDetail))]
    [TDLCollection(CollectionName = "GSTRATEDETAILS", ExplodeCondition = "$$NUMITEMS:GSTRATEDETAILS>0")]
    public new List<GSTDetail>? GSTDetails { get; set; }
}
public partial class Voucher : TallyConnector.Models.TallyPrime.V7.Voucher
{
    [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST", Type = typeof(AllLedgerEntry))]
    public new List<AllLedgerEntry> LedgerEntries { get; set; } = [];

    [XmlElement(ElementName = "ALLINVENTORYENTRIES.LIST", Type = typeof(AllInventoryEntries))]
    //[XmlElement(ElementName = "INVENTORYENTRIES.LIST", Type = typeof(InventoryEntries))]
    public new List<AllInventoryEntries>? InventoryAllocations { get; set; } = [];
}
public partial class BaseLedgerEntry : TallyConnector.Models.Base.BaseLedgerEntry
{
    [System.Xml.Serialization.XmlElement(ElementName = "GSTRATEDETAILS.LIST")]
    [TDLCollection(CollectionName = "GSTRATEDETAILS", ExplodeCondition = "$$NUMITEMS:GSTRATEDETAILS>0")]
    public new List<GSTRateDetail>? GSTRateDetails { get; set; }
}
public partial class AllLedgerEntry : BaseLedgerEntry
{
}
public partial class BaseInventoryEntry : TallyConnector.Models.Base.BaseInventoryEntry
{
    [System.Xml.Serialization.XmlElement(ElementName = "GSTRATEDETAILS.LIST")]
    [TDLCollection(CollectionName = "GSTRATEDETAILS", ExplodeCondition = "$$NUMITEMS:GSTRATEDETAILS>0")]
    public new List<GSTRateDetail>? GSTRateDetails { get; set; }
}
public partial class AllInventoryEntries : BaseInventoryEntry
{
}
[XmlType(AnonymousType = true)]
public partial class GSTRateDetail : TallyConnector.Models.Common.GSTRateDetail
{
    [XmlIgnore]
    public int Id { get; set; }
} 