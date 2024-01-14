using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
using TallyConnector.Core.Models;
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
        var vchs = await tallyServiceCVoucher.GetCVouchers();
    }
}
[GenerateHelperMethod<CVoucher>]
public partial class TallyServiceCVoucher : BaseTallyService
{

}
[XmlRoot(ElementName = "VOUCHER")]
public class CVoucher : ITallyBaseObject
{
    [XmlElement(ElementName = "VOUCHERNUMBER")]
    public string? VoucherNumber { get; set; }

    [XmlElement(ElementName = "VOUCHERTYPENAME")]
    public string? VoucherType { get; set; }
    [XmlElement(ElementName = "PERSISTEDVIEW")]
    public string? PersistedView { get; set; }

    [TDLCollection(CollectionName = "LedgerEntries")]
    [XmlElement(ElementName = "LEDGERENTRIES.LIST")]
    public List<CLedgerEntry> LedgerEntries { get; set; }

    [TDLCollection(CollectionName = "ALLINVENTORYENTRIES")]
    [XmlElement(ElementName = "ALLINVENTORYENTRIES.LIST")]
    public List<CInventoryEntry> InventoryEntries { get; set; }
}
public class CLedgerEntry
{
    [XmlElement(ElementName = "LEDGERNAME")]
    public string? LedgerName { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public Amount Amount { get; set; }

    [XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
    [TDLCollection(CollectionName = "BILLALLOCATIONS")]
    public List<BillAllocation>? BillAllocations { get; set; }
}
public class BillAllocation
{
    [XmlElement(ElementName = "NAME")]
    public string? NAME { get; set; }

    [XmlElement(ElementName = "BILLTYPE")]
    public string? BillType { get; set; }
    [XmlElement(ElementName = "BILLDATE")]
    public string? BillDate { get; set; }

    [XmlElement(ElementName = "BILLCREDITPERIOD")]
    public TallyConnector.Core.Models.TallyComplexObjects.DueDate? BillCreditPeriod { get; set; }
}

public partial class CInventoryEntry
{
    [XmlElement(ElementName = "STOCKITEMNAME")]
    public string? StockItemName { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public Amount Amount { get; set; }

    [TDLCollection(CollectionName = "ACCOUNTINGALLOCATIONS")]
    [XmlArrayItem(ElementName = "ACCOUNTINGALLOCATIONS.LIST")]
    public List<CLedgerEntry> LedgerEntries { get; set; }
}