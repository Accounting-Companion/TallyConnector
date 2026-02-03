using TallyConnector.Core.Models.Request;
using TallyConnector.Models.TallyPrime.V6;
using TallyConnector.Models.TallyPrime.V6.DTO;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Models.TallyPrime.V6.Masters.Inventory;
using TallyConnector.Models.TallyPrime.V6.Masters.Inventory.DTO;
using TallyConnector.Models.TallyPrime.V6.Masters.Meta;
using TallyConnector.Services.TallyPrime.V6;

namespace TallyConnector.Tests.Services.TallyPrime.V6;

public class TallyPrimeServiceV6Tests
{
    private readonly TallyPrimeService primeService;
    public TallyPrimeServiceV6Tests()
    {
        primeService = new TallyPrimeService();
        primeService.SetupTallyService("http://localhost", 9000);
    }
    [Test]
    public async Task TestGetLedgerAsync()
    {
        var ledgers = await primeService.GetLedgersAsync(new PaginatedRequestOptions());
        //var c = new Vucheta();
        //var scsa = c.Ledgers.Amount;
        //var scds = c.Ledgers.As<AllLedgerEntryMeta>().Amount;
    }
    [Test]
    public async Task TestGetUnitsAsync()
    {
        var ledgers = await primeService.GetUnitsAsync(new PaginatedRequestOptions());
        //var c = new Vucheta();
        //var scsa = c.Ledgers.Amount;
        //var scds = c.Ledgers.As<AllLedgerEntryMeta>().Amount;
    }
    [Test]
    public async Task PostGroupAsync()
    {
        Group group = new() { Name = "Test Group", Parent = "Sundry Debtors" };
        Group group2 = new() { Name = "Test Group2", Parent = "Sundry Debtor" };
        //Ledger ledger = new() { Name = "Test ledger",Group = "Test Group" };
        //CostCategory cat = new() { Name = "Test CCAt" };
        var resp = await primeService.PostGroupsAsync([group, group2]);
    }
    [Test]
    public async Task PostUnitsAsync()
    {
        Unit unit = new() { Name = "dzn", FormalName = "Dozens" };
        var resp = await primeService.PostUnitsAsync([unit]);

    }
    [Test]
    public async Task PostVoucherAsync()
    {
        Voucher voucher = new()
        {
            VoucherType = "Sales",
            Date = new DateTime(2025, 09, 02),
            Narration = "From Tally Connector library_",
            View = Models.Base.VoucherViewType.InvoiceVoucherView,
            IsInvoice = true
        };
        Models.Base.AllInventoryEntries inventoryAlloc1 = new();
        inventoryAlloc1.StockItemName = "Item1";
        inventoryAlloc1.Rate = new() { Rate = 20, Unit = "Nos" };
        inventoryAlloc1.ActualQuantity = new() { Quantity = 50, Unit = "Nos" };
        inventoryAlloc1.Amount = new Core.Models.TallyComplexObjects.TallyAmountField(1000, false);
        inventoryAlloc1.IsDeemedPositive = false;
        inventoryAlloc1.Ledgers = [new() { LedgerName = "Sales", Amount = inventoryAlloc1.Amount, IsDeemedPositive = false }];

        voucher.InventoryAllocations = [inventoryAlloc1];
        Models.Base.LedgerEntry partyledgerEntry = new();
        partyledgerEntry.LedgerName = "Cust1";
        partyledgerEntry.Amount = new(1000, true);
        partyledgerEntry.IsDeemedPositive = true;
        voucher.PartyName = partyledgerEntry.LedgerName;
        voucher.LedgerEntries = [partyledgerEntry];
        voucher.MasterId = 86;
        var resp = await primeService.PostDTOObjectsAsyncNew<VoucherDTO>([voucher]);
    }

    [Test]
    public async Task GetVoucherAsync()
    {
        PaginatedRequestOptions requestOptions = new() { Filters = [new("tc_GUIDFilter", "$GUID='52889497-5b6b-403d-8f83-224e3c7759b4-00001422'")] };
        requestOptions.From(new(2022, 04, 01));
        requestOptions.To(new(2022, 04, 01));
        await  primeService.GetVouchersAsync(requestOptions);
    }

    [Test]
    public async Task TestPostStockItemsAsync()
    {
        var item1 = new StockItem();
        item1.Name = "TC_Item1";
        //item1.BaseUnit = "Nos";
        item1.StockGroup = "Components";
        item1.OpeningBalance = new(7_000, "Nos");
        item1.OpeningRate = new(10, "Nos");
        item1.OpeningValue = new(60_000, true);
        //item1.GSTDetails = [new() { SourceOfGSTDetails = "Specify Details Here", ApplicableFrom = new DateTime(2026, 04, 01), Taxability = Models.Common.GSTTaxabilityType.Taxable,
        //    StateWiseDetails=[new() { StateName = "Any", GSTRateDetails = [new() { GSTRate = 18, DutyHead="IGST"}] }] }];
        //var itemdto = (StockItemDTO)item1.ToDTO();
        ////itemdto.Action = Core.Models.Action.Alter;
        //itemdto.LanguageNameList.Last().Names.Add("faswf");
        var stockItems = await primeService.PostStockItemsAsync([item1]);
    }
    [Test]
    public async Task TestPostLedgers()
    {
        var ledger1 = new Ledger();
        ledger1.Name = "TC_Test Ledger";
        ledger1.Group = "Sundry Debtors";
        ledger1.ContactDetails = [new() { Name = "Contact 1", PhoneNumber = "12314" }];
        var resp = await primeService.PostLedgersAsync([ledger1]);
    }
}
//public class Vucheta
//{
//    public CustomMeta Ledgers => new("LedgerEntries_fe", pathPrefix: "6476");
//}
//public class CustomMeta : AllLedgerEntryMeta, IMultiXMLMeta
//{
//    Dictionary<string, AllLedgerEntryMeta> _metas => new()
//        {
//            { "name", this},
//        };
//    public CustomMeta(string name, string? xmlTag = null, string pathPrefix = "") : base(name, xmlTag, pathPrefix)
//    {

//    }
//    public new List<string> FetchText => [.. base.FetchText,];
//    public T As<T>() where T : MetaObject
//    {
//        var name = typeof(T).FullName;
//        _metas.TryGetValue(name!, out var value);
//        if (value is T castedMeta)
//        {
//            return castedMeta;
//        }
//        throw new Exception($"{name} is not valid type");
//    }
//}
//public interface IMultiXMLMeta
//{
//    public T As<T>() where T : MetaObject;
//}