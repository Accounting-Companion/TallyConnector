using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Interfaces;
using TallyConnector.Core.Models.TallyComplexObjects;
using TallyConnector.Services;

namespace IntegrationTests;
[TestClass]
public class VoucherIntegrationTests
{
    [TestMethod]
    public async Task TestVoucher()
    {
        TallyServiceCVoucher tallyServiceCVoucher = new();
        var vchs = await tallyServiceCVoucher.GetRVouchers();
        var list = vchs
            .Select<RVoucher, RVoucherDTO>(c => c)
            .ToList();
    }
}
[GenerateHelperMethod<RVoucher>]
public partial class TallyServiceCVoucher : BaseTallyService
{
}
[XmlRoot(ElementName = "VOUCHER")]
public class RVoucher : ITallyBaseObject
{
    [XmlElement(ElementName = "VOUCHERNUMBER")]
    public string? VoucherNumber { get; set; }

    [XmlElement(ElementName = "VOUCHERTYPENAME")]
    public string? VoucherType { get; set; }
    [XmlElement(ElementName = "PERSISTEDVIEW")]
    public string? PersistedView { get; set; }


    [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST", Type = typeof(RAcLedgerEntry))]
    [XmlElement(ElementName = "LEDGERENTRIES.LIST", Type = typeof(RLedgerEntry))]
    public List<RLedgerEntry> LedgerEntries { get; set; }

    [TDLCollection(CollectionName = "ALLINVENTORYENTRIES")]
    [XmlElement(ElementName = "ALLINVENTORYENTRIES.LIST")]
    public List<RInventoryEntry> InventoryEntries { get; set; }
}

[TDLCollection(CollectionName = "LedgerEntries")]
public class RLedgerEntry : RBaseLedgerEntry
{
    [XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
    [TDLCollection(CollectionName = "BILLALLOCATIONS")]
    public List<RBillAllocation>? BillAllocations { get; set; }
}
public class RAcLedgerEntry : RLedgerEntry
{
}

public class RBaseLedgerEntry : IBaseLedgerEntry
{
    [XmlElement(ElementName = "LEDGERNAME")]
    public string LedgerName { get; set; } = null!;


    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmountField Amount { get; set; } = null!;
}

public class RBillAllocation
{
    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "BILLTYPE")]
    public string? BillType { get; set; }
    [TDLField(Invisible = "$$Value=''")]
    [XmlElement(ElementName = "BILLDATE")]
    public DateTime? BillDate { get; set; }

    [XmlElement(ElementName = "BILLCREDITPERIOD")]
    public TallyConnector.Core.Models.TallyComplexObjects.DueDate? BillCreditPeriod { get; set; }
}

public partial class RInventoryEntry
{
    [XmlElement(ElementName = "STOCKITEMNAME")]
    public string? StockItemName { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmountField Amount { get; set; }

    [XmlElement(ElementName = "RATE")]
    public TallyRateField Rate { get; set; }

    [XmlElement(ElementName = "ACTUALQTY")]
    public Quantity? ActualQuantity { get; set; }

    [XmlElement(ElementName = "BILLEDQTY")]
    public Quantity BilledQuantity { get; set; }

    [TDLCollection(CollectionName = "ACCOUNTINGALLOCATIONS")]
    [XmlElement(ElementName = "ACCOUNTINGALLOCATIONS.LIST")]
    public List<RAccountingLedgerEntry> LedgerEntries { get; set; }
}

public class RAccountingLedgerEntry : RBaseLedgerEntry
{

}