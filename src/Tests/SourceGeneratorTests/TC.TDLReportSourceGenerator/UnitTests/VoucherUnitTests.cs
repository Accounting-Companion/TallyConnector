﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests;

[TestClass]
public class VoucherUnitTests
{
    [TestMethod]
    public async Task VoucherTest()
    {
        var src = @"
using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.TallyComplexObjects;
using TallyConnector.Services;
using System.Collections.Generic;

namespace Test;

[GenerateHelperMethod<RVoucher>]
public partial class TallyServiceCVoucher : BaseTallyService
{

}
[XmlRoot(ElementName = ""VOUCHER"")]
public class RVoucher : ITallyBaseObject
{
    [XmlElement(ElementName = ""VOUCHERNUMBER"")]
    public string? VoucherNumber { get; set; }

    [XmlElement(ElementName = ""VOUCHERTYPENAME"")]
    public string? VoucherType { get; set; }
    [XmlElement(ElementName = ""PERSISTEDVIEW"")]
    public string? PersistedView { get; set; }

    [TDLCollection(CollectionName = ""LedgerEntries"")]
    [XmlElement(ElementName = ""LEDGERENTRIES.LIST"")]
    public List<RLedgerEntry> LedgerEntries { get; set; }

    [TDLCollection(CollectionName = ""ALLINVENTORYENTRIES"")]
    [XmlElement(ElementName = ""ALLINVENTORYENTRIES.LIST"")]
    public List<RInventoryEntry> InventoryEntries { get; set; }
}


public class RLedgerEntry : RBaseLedgerEntry
{
    [XmlElement(ElementName = ""BILLALLOCATIONS.LIST"")]
    [TDLCollection(CollectionName = ""BILLALLOCATIONS"")]
    public List<RBillAllocation>? BillAllocations { get; set; }
}

public class RBaseLedgerEntry
{
    [XmlElement(ElementName = ""LEDGERNAME"")]
    public string? LedgerName { get; set; }

    [XmlElement(ElementName = ""AMOUNT"")]
    public Amount Amount { get; set; }
}

public class RBillAllocation
{
    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

    [XmlElement(ElementName = ""BILLTYPE"")]
    public string? BillType { get; set; }
    [XmlElement(ElementName = ""BILLDATE"")]
    public string? BillDate { get; set; }

    [XmlElement(ElementName = ""BILLCREDITPERIOD"")]
    public TallyConnector.Core.Models.TallyComplexObjects.DueDate? BillCreditPeriod { get; set; }
}

public partial class RInventoryEntry
{
    [XmlElement(ElementName = ""STOCKITEMNAME"")]
    public string? StockItemName { get; set; }

    [XmlElement(ElementName = ""AMOUNT"")]
    public Amount Amount { get; set; }

    [XmlElement(ElementName = ""RATE"")]
    public Rate Rate { get; set; }

    [XmlElement(ElementName = ""ACTUALQTY"")]
    public Quantity? ActualQuantity { get; set; }

    [XmlElement(ElementName = ""BILLEDQTY"")]
    public Quantity BilledQuantity { get; set; }

    [TDLCollection(CollectionName = ""ACCOUNTINGALLOCATIONS"")]
    [XmlArrayItem(ElementName = ""ACCOUNTINGALLOCATIONS.LIST"")]
    public List<RAccountingLedgerEntry> LedgerEntries { get; set; }
}

public class RAccountingLedgerEntry: RBaseLedgerEntry
{

}";
        var resp = @"";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src, ("TestNameSpace.Group.TallyService.TDLReport.g.cs", resp));
    }
}
