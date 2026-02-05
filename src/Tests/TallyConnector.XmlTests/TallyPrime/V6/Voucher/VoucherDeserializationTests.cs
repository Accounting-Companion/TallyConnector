using NUnit.Framework;
using TallyConnector.Models.Base;
using V6Voucher = TallyConnector.Models.TallyPrime.V6.Voucher;

namespace TallyConnector.XmlTests.TallyPrime.V6.Voucher;

[TestFixture]
public class VoucherDeserializationTests : XmlTestBase
{
    protected override string ResourceSubPath => "TallyPrime/V6/Voucher";

    #region Contra Voucher Tests (AccountingVoucherView - uses ALLLEDGERENTRIES.LIST)

    [Test]
    public void Test_ContraVoucher_BasicProperties()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_contra.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(voucher, Is.Not.Null);
            Assert.That(voucher.VoucherType, Is.EqualTo("Contra"));
            Assert.That(voucher.VoucherNumber, Is.EqualTo("1"));
            Assert.That(voucher.Date, Is.EqualTo(new DateTime(2010, 1, 12)));
            Assert.That(voucher.View, Is.EqualTo(VoucherViewType.AccountingVoucherView));
            Assert.That(voucher.Narration, Is.EqualTo("Ch. No. :1000196 Being Amount transferred."));
            Assert.That(voucher.IsCancelled, Is.False);
            Assert.That(voucher.IsInvoice, Is.False);
        };
    }

    [Test]
    public void Test_ContraVoucher_Polymorphism_AllLedgerEntries()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_contra.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            // Polymorphism: ALLLEDGERENTRIES.LIST maps to AllLedgerEntry type
            Assert.That(voucher.LedgerEntries, Has.Count.EqualTo(2));
            Assert.That(voucher.LedgerEntries[0], Is.TypeOf<AllLedgerEntry>());
            Assert.That(voucher.LedgerEntries[1], Is.TypeOf<AllLedgerEntry>());
        };
    }

    [Test]
    public void Test_ContraVoucher_LedgerEntry_NestedAmount()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_contra.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            // First ledger entry (Credit)
            var entry1 = voucher.LedgerEntries[0];
            Assert.That(entry1.LedgerName, Is.EqualTo("HDFC Bank"));
            Assert.That(entry1.IsDeemedPositive, Is.False);
            Assert.That(entry1.Amount.Amount, Is.EqualTo(1000000m));
            Assert.That(entry1.Amount.IsDebit, Is.False);

            // Second ledger entry (Debit)
            var entry2 = voucher.LedgerEntries[1];
            Assert.That(entry2.LedgerName, Is.EqualTo("Canara Bank"));
            Assert.That(entry2.IsDeemedPositive, Is.True);
            Assert.That(entry2.Amount.Amount, Is.EqualTo(1000000m));
            Assert.That(entry2.Amount.IsDebit, Is.True);
        };
    }

    #endregion

    #region Sales Voucher Tests (InvoiceVoucherView - uses LEDGERENTRIES.LIST & ALLINVENTORYENTRIES.LIST)

    [Test]
    public void Test_SalesVoucher_BasicProperties()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(voucher, Is.Not.Null);
            Assert.That(voucher.VoucherType, Is.EqualTo("Sales"));
            Assert.That(voucher.VoucherNumber, Is.EqualTo("1"));
            Assert.That(voucher.View, Is.EqualTo(VoucherViewType.InvoiceVoucherView));
            Assert.That(voucher.IsInvoice, Is.True);
            Assert.That(voucher.PartyName, Is.EqualTo("Party B"));
            Assert.That(voucher.ConsigneeName, Is.EqualTo("Party B"));
        };
    }

    [Test]
    public void Test_SalesVoucher_Polymorphism_LedgerEntries()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            // Polymorphism: LEDGERENTRIES.LIST maps to LedgerEntry type (subclass of AllLedgerEntry)
            Assert.That(voucher.LedgerEntries, Has.Count.EqualTo(1));
            Assert.That(voucher.LedgerEntries[0], Is.TypeOf<LedgerEntry>());
        };
    }

    [Test]
    public void Test_SalesVoucher_LedgerEntry_NestedBillAllocations()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            var ledgerEntry = voucher.LedgerEntries[0];
            Assert.That(ledgerEntry.LedgerName, Is.EqualTo("Party B"));
            Assert.That(ledgerEntry.IsPartyLedger, Is.True);

            // Nested: BillAllocations
            Assert.That(ledgerEntry.BillAllocations, Has.Count.EqualTo(1));
            Assert.That(ledgerEntry.BillAllocations![0].BillType, Is.EqualTo(BillRefType.NewRef));
            Assert.That(ledgerEntry.BillAllocations[0].Name, Is.EqualTo("INV-001"));
            Assert.That(ledgerEntry.BillAllocations[0].Amount?.Amount, Is.EqualTo(250000m));
        };
    }

    [Test]
    public void Test_SalesVoucher_Polymorphism_InventoryAllocations()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            // Polymorphism: ALLINVENTORYENTRIES.LIST maps to AllInventoryEntries type
            Assert.That(voucher.InventoryAllocations, Has.Count.EqualTo(1));
            Assert.That(voucher.InventoryAllocations[0], Is.TypeOf<AllInventoryEntries>());
        };
    }

    [Test]
    public void Test_SalesVoucher_InventoryEntry_BasicProperties()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            var invEntry = voucher.InventoryAllocations[0];
            Assert.That(invEntry.StockItemName, Is.EqualTo("Assembled PIV"));
            Assert.That(invEntry.IsDeemedPositive, Is.False);
            Assert.That(invEntry.Discount, Is.EqualTo(0m));
            Assert.That(invEntry.IsScrap, Is.False);
        };
    }

    [Test]
    public void Test_SalesVoucher_InventoryEntry_NestedRate()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            var invEntry = voucher.InventoryAllocations[0];
            
            // Nested: Rate
            Assert.That(invEntry.Rate?.Rate, Is.EqualTo(25000m));
            Assert.That(invEntry.Rate?.Unit, Is.EqualTo("Nos"));
        };
    }

    [Test]
    public void Test_SalesVoucher_InventoryEntry_NestedQuantities()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            var invEntry = voucher.InventoryAllocations[0];
            
            // Nested: ActualQuantity
            Assert.That(invEntry.ActualQuantity?.Quantity, Is.EqualTo(10m));
            Assert.That(invEntry.ActualQuantity?.Unit, Is.EqualTo("Nos"));

            // Nested: BilledQuantity
            Assert.That(invEntry.BilledQuantity?.Quantity, Is.EqualTo(10m));
        };
    }

    [Test]
    public void Test_SalesVoucher_InventoryEntry_NestedBatchAllocations()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            var invEntry = voucher.InventoryAllocations[0];

            // Nested: BatchAllocations
            Assert.That(invEntry.BatchAllocations, Has.Count.EqualTo(1));
            var batch = invEntry.BatchAllocations![0];
            Assert.That(batch.GodownName, Is.EqualTo("Assembly Floor"));
            Assert.That(batch.BatchName, Is.EqualTo("Primary Batch"));
            Assert.That(batch.TrackingNo, Is.EqualTo("Not Applicable"));

            // Deeply nested: BatchAllocation -> Amount
            Assert.That(batch.Amount?.Amount, Is.EqualTo(250000m));
            Assert.That(batch.Amount?.IsDebit, Is.False);

            // Deeply nested: BatchAllocation -> Quantity
            Assert.That(batch.ActualQuantity?.Quantity, Is.EqualTo(10m));
        };
    }

    [Test]
    public void Test_SalesVoucher_InventoryEntry_NestedAccountingAllocations()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            var invEntry = voucher.InventoryAllocations[0];

            // AllInventoryEntries has Ledgers (ACCOUNTINGALLOCATIONS.LIST)
            Assert.That(invEntry.Ledgers, Has.Count.EqualTo(1));
            var accountLedger = invEntry.Ledgers![0];
            Assert.That(accountLedger.LedgerName, Is.EqualTo("Sales"));
            Assert.That(accountLedger.Amount.Amount, Is.EqualTo(250000m));
            Assert.That(accountLedger.Amount.IsDebit, Is.False);
        };
    }

    [Test]
    public void Test_SalesVoucher_InventoryEntry_UserDescriptions()
    {
        var xml = File.ReadAllText(GetResourcePath("voucher_sales.xml"));
        var voucher = XmlTestHelper.ParseXml<V6Voucher>(xml);

        using (Assert.EnterMultipleScope())
        {
            var invEntry = voucher.InventoryAllocations[0];

            // Nested List: UserDescriptions
            Assert.That(invEntry.UserDescriptions, Has.Count.EqualTo(2));
            Assert.That(invEntry.UserDescriptions[0], Is.EqualTo("Desc1"));
            Assert.That(invEntry.UserDescriptions[1], Is.EqualTo("Desc2"));
        };
    }

    #endregion
}
