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
        TCM.PResult result = await tally.PostStockItemAsync<TCMI.StockItem>(stockItem);

        Assert.AreEqual(result.Status, TCM.RespStatus.Sucess);
        TCMI.StockItem tStockItem = await tally.GetStockItemAsync<TCMI.StockItem>("Test StockItem");

        Assert.AreEqual(tStockItem.Name, "Test StockItem");
        Assert.AreEqual(tStockItem.BaseUnit, "Nos");
        Assert.AreEqual(tStockItem.OpeningBal.Number, 50);
        Assert.AreEqual(tStockItem.OpeningBal.PrimaryUnits.Number, 50);
        Assert.AreEqual(tStockItem.OpeningBal.PrimaryUnits.Unit, "Nos");
        Assert.AreEqual(tStockItem.OpeningValue.Amount, 5000);
        Assert.AreEqual(tStockItem.OpeningRate.Unit, "Nos");
        Assert.AreEqual(tStockItem.OpeningRate.RatePerUnit, 100.00);

        TCM.PResult Delresult = await tally.PostStockItemAsync<TCMI.StockItem>(new() { OldName = "Test StockItem", Action = TCM.Action.Delete });

        Assert.AreEqual(Delresult.Status, TCM.RespStatus.Sucess);

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
        TCM.PResult result = await tally.PostStockItemAsync<TCMI.StockItem>(stockItem);

        Assert.AreEqual(result.Status, TCM.RespStatus.Sucess);
        TCMI.StockItem tStockItem = await tally.GetStockItemAsync<TCMI.StockItem>("Test StockItem");

        Assert.AreEqual(tStockItem.Name, "Test StockItem");
        Assert.AreEqual(tStockItem.BaseUnit, "Nos");
        Assert.AreEqual(tStockItem.OpeningBal.Number, 50);
        Assert.AreEqual(tStockItem.OpeningBal.PrimaryUnits.Number, 50);
        Assert.AreEqual(tStockItem.OpeningBal.PrimaryUnits.Unit, "Nos");
        Assert.AreEqual(tStockItem.OpeningValue.Amount, 5000);
        Assert.AreEqual(tStockItem.OpeningRate.Unit, "Nos");
        Assert.AreEqual(tStockItem.OpeningRate.RatePerUnit, 100.00);

        TCM.PResult Delresult = await tally.PostStockItemAsync<TCMI.StockItem>(new() { OldName = "Test StockItem", Action = TCM.Action.Delete });

        Assert.AreEqual(Delresult.Status, TCM.RespStatus.Sucess);

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
        TCM.PResult result = await tally.PostStockItemAsync<TCMI.StockItem>(stockItem);

        Assert.AreEqual(result.Status, TCM.RespStatus.Sucess);
        TCMI.StockItem tStockItem = await tally.GetStockItemAsync<TCMI.StockItem>("Test StockItem");

        Assert.AreEqual(tStockItem.Name, "Test StockItem");
        Assert.AreEqual(tStockItem.BaseUnit, "Nos");
        Assert.AreEqual(tStockItem.AdditionalUnits, "Boxes");
        Assert.AreEqual(tStockItem.Conversion, 1);
        Assert.AreEqual(tStockItem.Denominator, 10);
        Assert.AreEqual(tStockItem.OpeningBal.Number, 50);
        Assert.AreEqual(tStockItem.OpeningBal.PrimaryUnits.Number, 50);
        Assert.AreEqual(tStockItem.OpeningBal.PrimaryUnits.Unit, "Nos");
        Assert.AreEqual(tStockItem.OpeningBal.SecondaryUnits.Number, 5);
        Assert.AreEqual(tStockItem.OpeningBal.SecondaryUnits.Unit, "Boxes");
        Assert.AreEqual(tStockItem.OpeningValue.Amount, 5000);
        Assert.AreEqual(tStockItem.OpeningRate.Unit, "Nos");
        Assert.AreEqual(tStockItem.OpeningRate.RatePerUnit, 100.00);

        TCM.PResult Delresult = await tally.PostStockItemAsync<TCMI.StockItem>(new() { OldName = "Test StockItem", Action = TCM.Action.Delete });

        Assert.AreEqual(Delresult.Status, TCM.RespStatus.Sucess);

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
        voucher.Ledgers.Add(new()
        {
            LedgerName = "abc India Pvt. Ltd.",
            Amount = -10_000,
            BillAllocations = new()
            {
                new() { BillType = TCM.BillRefType.NewRef, Name = "jhgf", Amount = -10_000 }
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

        TCM.TallyResult result = await tally.PostVoucher<TCM.Voucher>(voucher);

        Assert.AreEqual(result.Status, TCM.RespStatus.Sucess);
        TCM.Voucher Tvoucher = await tally.GetVoucherAsync<TCM.Voucher>("");

        Assert.IsNotNull(Tvoucher);



        var InvAlloc = Tvoucher.Ledgers[1].InventoryAllocations[0];

        Assert.AreEqual(InvAlloc.Rate.RatePerUnit, 500);
        Assert.AreEqual(InvAlloc.Rate.Unit, "Nos");
        Assert.AreEqual(InvAlloc.Amount.Amount, 10_000);
        Assert.AreEqual(InvAlloc.ActualQuantity.Number, 25);
        Assert.AreEqual(InvAlloc.ActualQuantity.PrimaryUnits.Number, 25);
        Assert.AreEqual(InvAlloc.ActualQuantity.PrimaryUnits.Unit, "Nos");
        Assert.AreEqual(InvAlloc.ActualQuantity.SecondaryUnits.Number, 2.50);
        Assert.AreEqual(InvAlloc.ActualQuantity.SecondaryUnits.Unit, "Boxes");
        Assert.AreEqual(InvAlloc.BilledQuantity.Number, 20);
        Assert.AreEqual(InvAlloc.BilledQuantity.SecondaryUnits.Number, 2.00);
    }
}
