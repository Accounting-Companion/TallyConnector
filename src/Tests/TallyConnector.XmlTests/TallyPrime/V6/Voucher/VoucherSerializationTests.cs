using TallyConnector.Models.Base;
using V6Voucher = TallyConnector.Models.TallyPrime.V6.Voucher;

namespace TallyConnector.XmlTests.TallyPrime.V6.Voucher;

[TestFixture]
public class VoucherSerializationTests : XmlTestBase
{
    private XMLOverrideswithTracking overides;

    public VoucherSerializationTests()
    {
        overides = new TallyPrimeService().GetPostXMLOverrides() ?? new();
    }

    protected override string ResourceSubPath => "TallyPrime/V6/Voucher";

    #region Helper Methods

    /// <summary>
    /// Creates a full Contra voucher matching voucher_contra.xml structure.
    /// </summary>
    private static V6Voucher CreateContraVoucher()
    {
        return new V6Voucher
        {
            Date = new DateTime(2010, 1, 12),
            VoucherType = "Contra",
            VoucherNumber = "1",
            View = VoucherViewType.AccountingVoucherView,
            Narration = "Ch. No. :1000196 Being Amount transferred.",
            IsInvoice = false,
            IsCancelled = false,
            RemoteId = "52889497-5b6b-403d-8f83-224e3c7759b4",
            LedgerEntries =
            [
                new AllLedgerEntry
                {
                    LedgerName = "HDFC Bank",
                    IsDeemedPositive = false,
                    IsPartyLedger = true,
                    Amount = new TallyAmountField(1000000m, false),
                },
                new AllLedgerEntry
                {
                    LedgerName = "Canara Bank",
                    IsDeemedPositive = true,
                    IsPartyLedger = true,
                    Amount = new TallyAmountField(1000000m, true),
                }
            ]
        };
    }

    /// <summary>
    /// Creates a full Sales voucher matching voucher_sales.xml structure.
    /// </summary>
    private static V6Voucher CreateSalesVoucher()
    {
        return new V6Voucher
        {
            Date = new DateTime(2009, 5, 31),
            VoucherType = "Sales",
            VoucherNumber = "1",
            View = VoucherViewType.InvoiceVoucherView,
            IsInvoice = true,
            PartyName = "Party B",
            ConsigneeName = "Party B",
            IsCancelled = false,
            RemoteId = "52889497-5b6b-403d-8f83-224e3c7759b4",
            LedgerEntries =
            [
                new LedgerEntry
                {
                    LedgerName = "Party B",
                    IsPartyLedger = true,
                    IsDeemedPositive = true,
                    Amount = new TallyAmountField(250000m, true),
                    BillAllocations =
                    [
                        new BillAllocations
                        {
                            BillType = BillRefType.NewRef,
                            Name = "INV-001",
                            Amount = new TallyAmountField(250000m, true),
                        }
                    ]
                }
            ],
            InventoryAllocations =
            [
                new AllInventoryEntries
                {
                    StockItemName = "Assembled PIV",
                    IsDeemedPositive = false,
                    Discount = 0m,
                    IsScrap = false,
                    UserDescriptions = ["Desc1", "Desc2"],
                    Rate = new TallyRateField(25000m, "Nos"),
                    ActualQuantity = new TallyQuantityField(10m, "Nos"),
                    BilledQuantity = new TallyQuantityField(10m, "Nos"),
                    Amount = new TallyAmountField(250000m, false),
                    BatchAllocations =
                    [
                        new BatchAllocation
                        {
                            TrackingNo = "Not Applicable",
                            OrderNo = "Not Applicable",
                            GodownName = "Assembly Floor",
                            BatchName = "Primary Batch",
                            Amount = new TallyAmountField(250000m, false),
                            ActualQuantity = new TallyQuantityField(10m, "Nos"),
                            BilledQuantity = new TallyQuantityField(10m, "Nos"),
                        }
                    ],
                    Ledgers =
                    [
                        new BaseLedgerEntry
                        {
                            LedgerName = "Sales",
                            IsDeemedPositive = false,
                            Amount = new TallyAmountField(250000m, false),
                        }
                    ]
                }
            ]
        };
    }

    #endregion

    #region XmlSerializer - Contra Voucher Tests

    [Test]
    public void Test_ContraVoucher_BasicProperties()
    {
        // Arrange
        var voucher = CreateContraVoucher();

        // Act
        var obj = voucher.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(doc.Root?.Name.LocalName, Is.EqualTo("VOUCHER"));
            Assert.That(doc.Root?.Element("VOUCHERTYPENAME")?.Value, Is.EqualTo("Contra"));
            Assert.That(doc.Root?.Element("VOUCHERNUMBER")?.Value, Is.EqualTo("1"));
            Assert.That(doc.Root?.Element("PERSISTEDVIEW")?.Value, Is.EqualTo("Accounting Voucher View"));
            Assert.That(doc.Root?.Element("NARRATION")?.Value, Is.EqualTo("Ch. No. :1000196 Being Amount transferred."));
            Assert.That(doc.Root?.Element("ISINVOICE")?.Value, Is.EqualTo("No"));
            Assert.That(doc.Root?.Element("ISCANCELLED")?.Value, Is.EqualTo("No"));
        }
        ;
    }

    [Test]
    public void Test_ContraVoucher_AllLedgerEntries()
    {
        // Arrange
        var voucher = CreateContraVoucher();

        // Act
        var obj = voucher.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        var allLedgerEntries = doc.Root?.Elements("ALLLEDGERENTRIES.LIST").ToList();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(allLedgerEntries, Has.Count.EqualTo(2));

            // First entry (Credit)
            Assert.That(allLedgerEntries![0].Element("LEDGERNAME")?.Value, Is.EqualTo("HDFC Bank"));
            Assert.That(allLedgerEntries[0].Element("ISDEEMEDPOSITIVE")?.Value, Is.EqualTo("No"));

            // Second entry (Debit)
            Assert.That(allLedgerEntries[1].Element("LEDGERNAME")?.Value, Is.EqualTo("Canara Bank"));
            Assert.That(allLedgerEntries[1].Element("ISDEEMEDPOSITIVE")?.Value, Is.EqualTo("Yes"));
        }
        ;
    }

    #endregion

    #region XmlSerializer - Sales Voucher Tests

    [Test]
    public void Test_SalesVoucher_BasicProperties()
    {
        // Arrange
        var voucher = CreateSalesVoucher();

        // Act
        var obj = voucher.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(doc.Root?.Name.LocalName, Is.EqualTo("VOUCHER"));
            Assert.That(doc.Root?.Element("VOUCHERTYPENAME")?.Value, Is.EqualTo("Sales"));
            Assert.That(doc.Root?.Element("VOUCHERNUMBER")?.Value, Is.EqualTo("1"));
            Assert.That(doc.Root?.Element("PERSISTEDVIEW")?.Value, Is.EqualTo("Invoice Voucher View"));
            Assert.That(doc.Root?.Element("ISINVOICE")?.Value, Is.EqualTo("Yes"));
            Assert.That(doc.Root?.Element("PARTYNAME")?.Value, Is.EqualTo("Party B"));
            Assert.That(doc.Root?.Element("BASICBUYERNAME")?.Value, Is.EqualTo("Party B"));
        }
        ;
    }

    [Test]
    public void Test_SalesVoucher_LedgerEntries_WithBillAllocations()
    {
        // Arrange
        var voucher = CreateSalesVoucher();

        // Act
        var obj = voucher.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        var ledgerEntry = doc.Root?.Element("LEDGERENTRIES.LIST");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ledgerEntry, Is.Not.Null);
            Assert.That(ledgerEntry?.Element("LEDGERNAME")?.Value, Is.EqualTo("Party B"));
            Assert.That(ledgerEntry?.Element("ISPARTYLEDGER")?.Value, Is.EqualTo("Yes"));

            // Nested: BillAllocations
            var billAlloc = ledgerEntry?.Element("BILLALLOCATIONS.LIST");
            Assert.That(billAlloc, Is.Not.Null);
            Assert.That(billAlloc?.Element("BILLTYPE")?.Value, Is.EqualTo("New Ref"));
            Assert.That(billAlloc?.Element("NAME")?.Value, Is.EqualTo("INV-001"));
        }
        ;
    }

    [Test]
    public void Test_SalesVoucher_InventoryEntries()
    {
        // Arrange
        var voucher = CreateSalesVoucher();

        // Act
        var obj = voucher.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        var invEntry = doc.Root?.Element("ALLINVENTORYENTRIES.LIST");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(invEntry, Is.Not.Null);
            Assert.That(invEntry?.Element("STOCKITEMNAME")?.Value, Is.EqualTo("Assembled PIV"));
            Assert.That(invEntry?.Element("ISDEEMEDPOSITIVE")?.Value, Is.EqualTo("No"));
            Assert.That(invEntry?.Element("DISCOUNT")?.Value, Is.EqualTo("0"));
            Assert.That(invEntry?.Element("ISSCRAP")?.Value, Is.EqualTo("No"));

            // Nested: Rate
            var rate = invEntry?.Element("RATE");
            Assert.That(rate, Is.Not.Null);

            // Nested: Quantities
            var actualQty = invEntry?.Element("ACTUALQTY");
            Assert.That(actualQty, Is.Not.Null);
            var billedQty = invEntry?.Element("BILLEDQTY");
            Assert.That(billedQty, Is.Not.Null);

            // Nested: UserDescriptions
            var userDescList = invEntry?.Element("BASICUSERDESCRIPTION.LIST");
            Assert.That(userDescList, Is.Not.Null);
            var userDescs = userDescList?.Elements("BASICUSERDESCRIPTION").ToList();
            Assert.That(userDescs, Has.Count.EqualTo(2));

            // Nested: AccountingAllocations
            var acctAlloc = invEntry?.Element("ACCOUNTINGALLOCATIONS.LIST");
            Assert.That(acctAlloc, Is.Not.Null);
            Assert.That(acctAlloc?.Element("LEDGERNAME")?.Value, Is.EqualTo("Sales"));
        }
        ;
    }

    [Test]
    public void Test_SalesVoucher_InventoryEntry_BatchAllocations()
    {
        // Arrange
        var voucher = CreateSalesVoucher();

        // Act
        var obj = voucher.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        var batchAlloc = doc.Root?.Element("ALLINVENTORYENTRIES.LIST")?.Element("BATCHALLOCATIONS.LIST");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(batchAlloc, Is.Not.Null);
            Assert.That(batchAlloc?.Element("TRACKINGNUMBER")?.Value, Is.EqualTo("Not Applicable"));
            Assert.That(batchAlloc?.Element("ORDERNO")?.Value, Is.EqualTo("Not Applicable"));
            Assert.That(batchAlloc?.Element("GODOWNNAME")?.Value, Is.EqualTo("Assembly Floor"));
            Assert.That(batchAlloc?.Element("BATCHNAME")?.Value, Is.EqualTo("Primary Batch"));
        }
        ;
    }

    #endregion

    #region XmlSerializer - Party & Consignee Details

    [Test]
    public void Test_Voucher_PartyDetails()
    {
        // Arrange
        var voucher = new V6Voucher
        {
            Date = new DateTime(2024, 1, 1),
            VoucherType = "Sales",
            View = VoucherViewType.InvoiceVoucherView,
            RemoteId = "test-party-001",
            PartyName = "ABC Traders",
            PartyMailingName = "ABC Traders Pvt Ltd",
            State = "Karnataka",
            Country = "India",
            PartyGSTIN = "29ABCDE1234F1Z5",
            PlaceOfSupply = "Karnataka",
            PINCode = "560001",
        };

        // Act
        var obj = voucher.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(doc.Root?.Element("PARTYNAME")?.Value, Is.EqualTo("ABC Traders"));
            Assert.That(doc.Root?.Element("PARTYMAILINGNAME")?.Value, Is.EqualTo("ABC Traders Pvt Ltd"));
            Assert.That(doc.Root?.Element("STATENAME")?.Value, Is.EqualTo("Karnataka"));
            Assert.That(doc.Root?.Element("COUNTRYOFRESIDENCE")?.Value, Is.EqualTo("India"));
            Assert.That(doc.Root?.Element("PARTYGSTIN")?.Value, Is.EqualTo("29ABCDE1234F1Z5"));
            Assert.That(doc.Root?.Element("PLACEOFSUPPLY")?.Value, Is.EqualTo("Karnataka"));
            Assert.That(doc.Root?.Element("PARTYPINCODE")?.Value, Is.EqualTo("560001"));
        }
        ;
    }

    [Test]
    public void Test_Voucher_ConsigneeDetails()
    {
        // Arrange
        var voucher = new V6Voucher
        {
            Date = new DateTime(2024, 1, 1),
            VoucherType = "Sales",
            View = VoucherViewType.InvoiceVoucherView,
            RemoteId = "test-consignee-001",
            ConsigneeName = "XYZ Industries",
            ConsigneeMailingName = "XYZ Industries Ltd",
            ConsigneeState = "Tamil Nadu",
            ConsigneeCountry = "India",
            ConsigneeGSTIN = "33ABCDE5678F1Z9",
            ConsigneePinCode = "600001",
        };

        // Act
        var obj = voucher.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(doc.Root?.Element("BASICBUYERNAME")?.Value, Is.EqualTo("XYZ Industries"));
            Assert.That(doc.Root?.Element("CONSIGNEEMAILINGNAME")?.Value, Is.EqualTo("XYZ Industries Ltd"));
            Assert.That(doc.Root?.Element("CONSIGNEESTATENAME")?.Value, Is.EqualTo("Tamil Nadu"));
            Assert.That(doc.Root?.Element("CONSIGNEECOUNTRYNAME")?.Value, Is.EqualTo("India"));
            Assert.That(doc.Root?.Element("CONSIGNEEGSTIN")?.Value, Is.EqualTo("33ABCDE5678F1Z9"));
            Assert.That(doc.Root?.Element("CONSIGNEEPINCODE")?.Value, Is.EqualTo("600001"));
        }
        ;
    }

    #endregion

    #region XmlSerializer - EWay Bill Details

    [Test]
    public void Test_Voucher_EwayBillDetails()
    {
        // Arrange
        var voucher = new V6Voucher
        {
            Date = new DateTime(2024, 1, 1),
            VoucherType = "Sales",
            View = VoucherViewType.InvoiceVoucherView,
            RemoteId = "test-eway-001",
            OverrideEWayBillApplicability = false,
            EwayBillDetails =
            [
                new EwayBillDetails
                {
                    BillNumber = "EWB-001",
                    BillDate = new DateTime(2024, 1, 1),
                    DocumentType = "Tax Invoice",
                    SubType = "Supply",
                }
            ]
        };

        // Act
        var obj = voucher.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);
        var doc = System.Xml.Linq.XDocument.Parse(xml);

        // Assert
        var ewayDetails = doc.Root?.Element("EWAYBILLDETAILS.LIST");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ewayDetails, Is.Not.Null);
            Assert.That(ewayDetails?.Element("BILLNUMBER")?.Value, Is.EqualTo("EWB-001"));
            Assert.That(ewayDetails?.Element("DOCUMENTTYPE")?.Value, Is.EqualTo("Tax Invoice"));
            Assert.That(ewayDetails?.Element("SUBTYPE")?.Value, Is.EqualTo("Supply"));
        }
        ;
    }

    #endregion

    #region Parity: XmlSerializer vs Stream Output Comparison

    [Test]
    public async Task Test_Parity_ContraVoucher_SerializerVsStream()
    {
        // Arrange
        var voucher = CreateContraVoucher();

        // Act - XmlSerializer
        var obj = voucher.ToDTO();
        var xmlSerializerOutput = XmlTestHelper.GenerateXml(obj, overides);

        // Act - Stream (GenericXmlStreamer)
        var streamOutput = await XmlTestHelper.GenerateXmlFromStreamAsync(obj);

        // Assert - Both produce the same XML string
        Assert.That(streamOutput, Is.EqualTo(xmlSerializerOutput));
    }

    [Test]
    public async Task Test_Parity_SalesVoucher_SerializerVsStream()
    {
        // Arrange
        var voucher = CreateSalesVoucher();

        // Act - XmlSerializer
        var obj = voucher.ToDTO();
        var xmlSerializerOutput = XmlTestHelper.GenerateXml(obj, overides);

        // Act - Stream (GenericXmlStreamer)
        var streamOutput = await XmlTestHelper.GenerateXmlFromStreamAsync(obj);

        // Assert - Both produce the same XML string
        Assert.That(streamOutput, Is.EqualTo(xmlSerializerOutput));
    }

    #endregion

    #region Round-Trip: Stream Serialize → Stream Deserialize

    [Test]
    public async Task Test_RoundTrip_Stream_ContraVoucher()
    {
        // Arrange
        var original = CreateContraVoucher();

        // Act - Serialize
        var xml = await XmlTestHelper.GenerateXmlFromStreamAsync(original.ToDTO());

        // Act - Deserialize
        var deserialized = XmlTestHelper.ParseXmlFromStream<V6Voucher>(xml);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.VoucherType, Is.EqualTo("Contra"));
            Assert.That(deserialized.VoucherNumber, Is.EqualTo("1"));
            Assert.That(deserialized.Date, Is.EqualTo(new DateTime(2010, 1, 12)));
            Assert.That(deserialized.View, Is.EqualTo(VoucherViewType.AccountingVoucherView));
            Assert.That(deserialized.LedgerEntries, Has.Count.EqualTo(2));
            Assert.That(deserialized.LedgerEntries[0].LedgerName, Is.EqualTo("HDFC Bank"));
            Assert.That(deserialized.LedgerEntries[1].LedgerName, Is.EqualTo("Canara Bank"));
        }
        ;
    }

    [Test]
    public async Task Test_RoundTrip_Stream_SalesVoucher()
    {
        // Arrange
        var original = CreateSalesVoucher();

        // Act - Serialize
        var xml = await XmlTestHelper.GenerateXmlFromStreamAsync(original.ToDTO());

        // Act - Deserialize
        var deserialized = XmlTestHelper.ParseXmlFromStream<V6Voucher>(xml);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.VoucherType, Is.EqualTo("Sales"));
            Assert.That(deserialized.VoucherNumber, Is.EqualTo("1"));
            Assert.That(deserialized.PartyName, Is.EqualTo("Party B"));
            Assert.That(deserialized.ConsigneeName, Is.EqualTo("Party B"));
            Assert.That(deserialized.IsInvoice, Is.True);

            // LedgerEntries
            Assert.That(deserialized.LedgerEntries, Has.Count.EqualTo(1));
            Assert.That(deserialized.LedgerEntries[0].LedgerName, Is.EqualTo("Party B"));
            Assert.That(deserialized.LedgerEntries[0].BillAllocations, Has.Count.EqualTo(1));
            Assert.That(deserialized.LedgerEntries[0].BillAllocations![0].Name, Is.EqualTo("INV-001"));

            // InventoryAllocations
            Assert.That(deserialized.InventoryAllocations, Has.Count.EqualTo(1));
            var invEntry = deserialized.InventoryAllocations[0];
            Assert.That(invEntry.StockItemName, Is.EqualTo("Assembled PIV"));
            Assert.That(invEntry.BatchAllocations, Has.Count.EqualTo(1));
            Assert.That(invEntry.BatchAllocations![0].GodownName, Is.EqualTo("Assembly Floor"));
            Assert.That(invEntry.Ledgers, Has.Count.EqualTo(1));
            Assert.That(invEntry.Ledgers![0].LedgerName, Is.EqualTo("Sales"));
        }
        ;
    }

    #endregion

    #region Round-Trip: XmlSerializer Serialize → XmlSerializer Deserialize

    [Test]
    public void Test_RoundTrip_XmlSerializer_ContraVoucher()
    {
        // Arrange
        var original = CreateContraVoucher();

        // Act - Serialize via XmlSerializer
        var obj = original.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);

        // Act - Deserialize via XmlSerializer
        var deserialized = XmlTestHelper.ParseXml<V6Voucher>(xml);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized.VoucherType, Is.EqualTo("Contra"));
            Assert.That(deserialized.VoucherNumber, Is.EqualTo("1"));
            Assert.That(deserialized.LedgerEntries, Has.Count.EqualTo(2));
            Assert.That(deserialized.LedgerEntries[0].LedgerName, Is.EqualTo("HDFC Bank"));
            Assert.That(deserialized.LedgerEntries[1].LedgerName, Is.EqualTo("Canara Bank"));
        }
        ;
    }

    [Test]
    public void Test_RoundTrip_XmlSerializer_SalesVoucher()
    {
        // Arrange
        var original = CreateSalesVoucher();

        // Act - Serialize via XmlSerializer
        var obj = original.ToDTO();
        var xml = XmlTestHelper.GenerateXml(obj, overides);

        // Act - Deserialize via XmlSerializer
        var deserialized = XmlTestHelper.ParseXml<V6Voucher>(xml);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized.VoucherType, Is.EqualTo("Sales"));
            Assert.That(deserialized.PartyName, Is.EqualTo("Party B"));
            Assert.That(deserialized.LedgerEntries, Has.Count.EqualTo(1));
            Assert.That(deserialized.LedgerEntries[0].LedgerName, Is.EqualTo("Party B"));

            Assert.That(deserialized.InventoryAllocations, Has.Count.EqualTo(1));
            Assert.That(deserialized.InventoryAllocations[0].StockItemName, Is.EqualTo("Assembled PIV"));
            Assert.That(deserialized.InventoryAllocations[0].Ledgers, Has.Count.EqualTo(1));
            Assert.That(deserialized.InventoryAllocations[0].Ledgers![0].LedgerName, Is.EqualTo("Sales"));
        }
        ;
    }

    #endregion
}
