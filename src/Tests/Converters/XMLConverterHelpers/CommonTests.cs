using System.Text.RegularExpressions;

namespace Tests.Converters.XMLConverterHelpers;
internal class CommonTests
{

    [Test]
    public async Task CheckStockItemforCustomCustomTallyObjects()
    {
        TallyService tally = new();
        TCMI.StockItem stockItem = new()
        {
            Name = "Test StockItem",
            BaseUnit = "Nos",

        };
        stockItem.OpeningBal = new(stockItem, 50);
        stockItem.OpeningValue = new(-5000);
        TCM.TallyResult result = await tally.PostStockItemAsync<TCMI.StockItem>(stockItem);

        Assert.That(result.Status, Is.EqualTo(TCM.RespStatus.Sucess));
        TCMI.StockItem tStockItem = await tally.GetStockItemAsync<TCMI.StockItem>("Test StockItem");
        Assert.Multiple(() =>
        {
            Assert.That(tStockItem.Name, Is.EqualTo("Test StockItem"));
            Assert.That(tStockItem.BaseUnit, Is.EqualTo("Nos"));
            Assert.That(tStockItem.OpeningBal.Number, Is.EqualTo(50));
            Assert.That(tStockItem.OpeningBal.PrimaryUnits.Number, Is.EqualTo(50));
            Assert.That(tStockItem.OpeningBal.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(tStockItem.OpeningValue.Amount, Is.EqualTo(5000));
            Assert.That(tStockItem.OpeningRate.Unit, Is.EqualTo("Nos"));
            Assert.That(tStockItem.OpeningRate.RatePerUnit, Is.EqualTo(100.00));
        });
        TCM.TallyResult Delresult = await tally.PostStockItemAsync<TCMI.StockItem>(new() { OldName = "Test StockItem", Action = TCM.Action.Delete });

        Assert.That(Delresult.Status, Is.EqualTo(TCM.RespStatus.Sucess));
    }

    [Test]
    public async Task CheckStockItemforCustomCustomTallyObjectswithRate()
    {
        TallyService tally = new();
        TCMI.StockItem stockItem = new()
        {
            Name = "Test StockItem",
            BaseUnit = "Nos",

        };
        stockItem.OpeningBal = new(stockItem, 50);
        stockItem.OpeningRate = new(100, "Nos");
        stockItem.OpeningValue = new(-5000);
        TCM.TallyResult result = await tally.PostStockItemAsync<TCMI.StockItem>(stockItem);

        Assert.That(result.Status, Is.EqualTo(TCM.RespStatus.Sucess));
        TCMI.StockItem tStockItem = await tally.GetStockItemAsync<TCMI.StockItem>("Test StockItem");
        Assert.Multiple(() =>
        {
            Assert.That(tStockItem.Name, Is.EqualTo("Test StockItem"));
            Assert.That(tStockItem.BaseUnit, Is.EqualTo("Nos"));
            Assert.That(tStockItem.OpeningBal.Number, Is.EqualTo(50));
            Assert.That(tStockItem.OpeningBal.PrimaryUnits.Number, Is.EqualTo(50));
            Assert.That(tStockItem.OpeningBal.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(tStockItem.OpeningValue.Amount, Is.EqualTo(5000));
            Assert.That(tStockItem.OpeningRate.Unit, Is.EqualTo("Nos"));
            Assert.That(tStockItem.OpeningRate.RatePerUnit, Is.EqualTo(100.00));
        });
        TCM.TallyResult Delresult = await tally.PostStockItemAsync<TCMI.StockItem>(new() { OldName = "Test StockItem", Action = TCM.Action.Delete });

        Assert.That(Delresult.Status, Is.EqualTo(TCM.RespStatus.Sucess));
    }

    [Test]
    public async Task CheckStockItemforCustomCustomTallyObjectswithAdditionalUnit()
    {
        TallyService tally = new();
        TCMI.StockItem stockItem = new()
        {
            Name = "Test StockItem",
            BaseUnit = "Nos",
            AdditionalUnits = "Boxes",
            Conversion = 1,
            Denominator = 10,

        };
        stockItem.OpeningBal = new(stockItem, 50);
        stockItem.OpeningValue = new(-5000);
        TCM.TallyResult result = await tally.PostStockItemAsync<TCMI.StockItem>(stockItem);

        Assert.That(result.Status, Is.EqualTo(TCM.RespStatus.Sucess));
        TCMI.StockItem tStockItem = await tally.GetStockItemAsync<TCMI.StockItem>("Test StockItem");
        Assert.Multiple(() =>
        {
            Assert.That(tStockItem.Name, Is.EqualTo("Test StockItem"));
            Assert.That(tStockItem.BaseUnit, Is.EqualTo("Nos"));
            Assert.That(tStockItem.AdditionalUnits, Is.EqualTo("Boxes"));
            Assert.That(tStockItem.Conversion, Is.EqualTo(1));
            Assert.That(tStockItem.Denominator, Is.EqualTo(10));
            Assert.That(tStockItem.OpeningBal.Number, Is.EqualTo(50));
            Assert.That(tStockItem.OpeningBal.PrimaryUnits.Number, Is.EqualTo(50));
            Assert.That(tStockItem.OpeningBal.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(tStockItem.OpeningBal.SecondaryUnits.Number, Is.EqualTo(5));
            Assert.That(tStockItem.OpeningBal.SecondaryUnits.Unit, Is.EqualTo("Boxes"));
            Assert.That(tStockItem.OpeningValue.Amount, Is.EqualTo(5000));
            Assert.That(tStockItem.OpeningRate.Unit, Is.EqualTo("Nos"));
            Assert.That(tStockItem.OpeningRate.RatePerUnit, Is.EqualTo(100.00));
        });
        TCM.TallyResult Delresult = await tally.PostStockItemAsync<TCMI.StockItem>(new() { OldName = "Test StockItem", Action = TCM.Action.Delete });

        Assert.That(Delresult.Status, Is.EqualTo(TCM.RespStatus.Sucess));
    }

    [Test]
    public async Task CheckCreateVoucherforCustomCustomTallyObjects()
    {
        TallyService tally = new();
        TCM.Voucher voucher = new()
        {
            Date = new DateTime(2022, 05, 31),
            View = TCM.VoucherViewType.AccountingVoucherView,
            VoucherType = "Sales",
            Ledgers = new(),
            VoucherNumber = "sdfgh"
        };
        voucher.EffectiveDate = voucher.Date;
        voucher.Ledgers.Add(new()
        {
            LedgerName = "abc India Pvt. Ltd.",
            Amount = -10_000,
            BillAllocations = new()
            {
                new() { BillType = TCM.BillRefType.NewRef, Name = "jhgf", Amount = -10_000,BillCreditPeriod = new(new DateTime(2022, 07, 31)) }
            }
        });
        TCM.VoucherLedger SalesLedger = new() { LedgerName = "Sales", Amount = 10_000 };
        voucher.Ledgers.Add(SalesLedger);
        SalesLedger.InventoryAllocations = new();
        TCM.InventoryAllocations inventoryAllocations = new()
        {
            StockItemName = "Assembled",
            ActualQuantity = new(25, "Nos"),
            BilledQuantity = new(20, "Nos"),
            Rate = new("Nos", 10, 50, "$"),
            Amount = new(200, 50, "$"),
            BatchAllocations = new()
        {
            new() { GodownName = "Main Location", BatchName = "Primary Batch", BilledQuantity = new(15, "Nos"), Amount = new(200,50,"$") }
        }
        };

        SalesLedger.InventoryAllocations.Add(inventoryAllocations);
        inventoryAllocations.CostCategoryAllocations = new()
        {
            new()
            {
                CostCategoryName = "Primary Cost Category",
                CostCenterAllocations = new()
                {
                    new()
                    {
                        Name = "Accounts",
                        Amount = 10_000
                    }
                }
            }
        };

        TCM.TallyResult result = await tally.PostVoucherAsync<TCM.Voucher>(voucher);

        Assert.That(result.Status, Is.EqualTo(TCM.RespStatus.Sucess));
        var y = Regex.Matches(result.Response, @"[0-9.]+")[0].Value;
        TCM.Voucher Tvoucher = await tally.GetVoucherAsync<TCM.Voucher>(y, new() { LookupField = TCM.VoucherLookupField.MasterId, FetchList = TCM.Constants.Voucher.AccountingViewFetchList.All });

        Assert.That(Tvoucher, Is.Not.Null);



        var InvAlloc = Tvoucher.Ledgers[1].InventoryAllocations[0];
        Assert.Multiple(() =>
        {
            Assert.That(InvAlloc.Rate.RatePerUnit, Is.EqualTo(500));
            Assert.That(InvAlloc.Rate.Unit, Is.EqualTo("Nos"));
            Assert.That(InvAlloc.Amount.Amount, Is.EqualTo(10_000));
            Assert.That(InvAlloc.ActualQuantity.Number, Is.EqualTo(25));
            Assert.That(InvAlloc.ActualQuantity.PrimaryUnits.Number, Is.EqualTo(25));
            Assert.That(InvAlloc.ActualQuantity.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(InvAlloc.ActualQuantity.SecondaryUnits.Number, Is.EqualTo(2.50));
            Assert.That(InvAlloc.ActualQuantity.SecondaryUnits.Unit, Is.EqualTo("Boxes"));
            Assert.That(InvAlloc.BilledQuantity.Number, Is.EqualTo(20));
            Assert.That(InvAlloc.BilledQuantity.SecondaryUnits.Number, Is.EqualTo(2.00));
        });
    }
}
