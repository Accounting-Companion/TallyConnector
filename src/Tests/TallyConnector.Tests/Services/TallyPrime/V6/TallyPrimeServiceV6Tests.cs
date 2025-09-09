using TallyConnector.Core.Models.Request;
using TallyConnector.Models.TallyPrime.V6;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Services.TallyPrime.V6;

namespace TallyConnector.Tests.Services.TallyPrime.V6;
public class TallyPrimeServiceV6Tests
{
    private readonly TallyPrimeService primeService;
    public TallyPrimeServiceV6Tests()
    {
        primeService = new TallyPrimeService();
        //primeService.SetupTallyService("http://localhost", 9001);
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
    public async Task PostGroupAsync()
    {
        Group group = new() { Name = "Test Group", Parent = "Sundry Debtors" };
        Group group2 = new() { Name = "Test Group2", Parent = "Sundry Debtor" };
        //Ledger ledger = new() { Name = "Test ledger",Group = "Test Group" };
        //CostCategory cat = new() { Name = "Test CCAt" };
        var resp = await primeService.PostGroupsAsync([group, group2]);
    }
    [Test]
    public async Task PostVoucherAsync()
           {
        Voucher voucher = new();
        voucher.VoucherType = "Sales";
        voucher.Date = new DateTime(2025, 09, 01);
        voucher.Narration = "From Tally Connector library";
        voucher.View = Models.Base.VoucherViewType.InvoiceVoucherView;
        voucher.IsInvoice = true;
        Models.Base.AllInventoryAllocations inventoryAlloc1 = new();
        inventoryAlloc1.StockItemName = "Item1";
        inventoryAlloc1.Rate = new() { Rate = 20, Unit = "Nos" };
        inventoryAlloc1.ActualQuantity = new() { Quantity = 50, Unit = "Nos" };
        inventoryAlloc1.Amount = new Core.Models.TallyComplexObjects.TallyAmountField(1000, false);
        inventoryAlloc1.IsDeemedPositive = false;
        inventoryAlloc1.Ledgers = [new() { LedgerName = "Sales", Amount = inventoryAlloc1.Amount,IsDeemedPositive=false }];
        voucher.InventoryAllocations = [inventoryAlloc1];
        Models.Base.LedgerEntry partyledgerEntry = new();
        partyledgerEntry.LedgerName = "Cust1";
        partyledgerEntry.Amount = new(1000, true);
        partyledgerEntry.IsDeemedPositive = true;
        voucher.PartyName = partyledgerEntry.LedgerName;
        voucher.LedgerEntries = [partyledgerEntry];
        voucher.MasterId = 69;
        var resp = await primeService.PostVouchersAsync([voucher]);
    }
    public async  Task AlterVoucherAsync()
    {

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