using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TallyConnector.Core.Models;

namespace Tests.Services.TallyService.TallyObjects;
internal class VoucherTests : BaseTallyServiceTest
{
    [Test]
    public async Task CheckGetAllVouchers()
    {
        var objects = await _tallyService.GetAllObjectsAsync<TCM.Voucher>(new()
        {
            FromDate = new(2010, 4, 1),
            FetchList = new List<string>()
                {
                    "MasterId", "*", "AllledgerEntries", "ledgerEntries", "Allinventoryenntries",
                    "InventoryEntries", "InventoryEntriesIn", "InventoryEntriesOut"
                }
        });
        // Voucher voucher = await _tallyService.GetVoucherAsync<Voucher>("52889497-5b6b-403d-8f83-224e3c7759b4-00001285", new() { LookupField = VoucherLookupField.GUID });
        //StockItem stockItem = await _tallyService.GetStockItemAsync<StockItem>("Floppy Drive");
        Assert.That(objects, Is.Not.Null);
        Assert.That(objects, Has.Count.EqualTo(1206));
    }

    [Test]
    public async Task CreatePurchaseVoucher()
    {
        //string json = voucher1.GetJson();
        Voucher voucher = new()
        {
            VoucherType = "Purchase Order",
            View = VoucherViewType.InvoiceVoucherView,
            Date = new DateTime(2022, 04, 01),
            Reference = "frt",
            Ledgers = new()
            {
                new(){LedgerName="Test Party",Amount=400 },

            },
            InventoryAllocations = new()
            {
                new()
                {
                    StockItemName = "Mouse",
                    Rate = new(35,"Nos"),
                    ActualQuantity =new(10,"Nos"),
                    BilledQuantity =new(10,"Nos"),
                    Amount = -350,
                    BatchAllocations = new()
                    {
                        new(){OrderNo="frt",Amount=-350,BilledQuantity=new(10,"Nos")}
                    },
                    Ledgers = new(){ new() { LedgerName = "Purchase", Amount = -350 } }

                }
            }
        };
        TallyResult tallyResult = await _tallyService.PostVoucherAsync(voucher);
    }


    [Test]
    public async Task CheckGetAccountingVoucher()
    {

        Voucher voucher = await _tallyService.GetVoucherAsync<Voucher>("52889497-5b6b-403d-8f83-224e3c7759b4-000013d5",
                                                                       new()
                                                                       {
                                                                           LookupField = VoucherLookupField.GUID,
                                                                           FetchList = Constants.Voucher.AccountingViewFetchList.All
                                                                       });

    }
    [Test]
    public async Task CheckGetInvoiceVoucher()
    {

        Voucher voucher = await _tallyService.GetVoucherAsync<Voucher>("52889497-5b6b-403d-8f83-224e3c7759b4-000013db",
                                                                       new()
                                                                       {
                                                                           LookupField = VoucherLookupField.GUID,
                                                                           FetchList = Constants.Voucher.InvoiceViewFetchList.All
                                                                       });

        string json = voucher.GetJson();

    }


}

