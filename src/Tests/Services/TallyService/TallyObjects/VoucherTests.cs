using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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
        int count = 0;
        IProgress<ReportProgressHelper> progress = new Progress<ReportProgressHelper>(c => count += c.ProcessedCount);
        RequestOptions requestOptions = new()
        {
            FromDate = new(2022, 4, 1),
            FetchList = Constants.Voucher.AccountingViewFetchList.All,
            Filters = new List<Filter>() { Constants.Voucher.Filters.ViewTypeFilters.AccountingVoucherFilter }
        };
        var ActngVchrs = await _tallyService.GetAllObjectsAsync<TCM.Voucher>(requestOptions, progress);

        requestOptions.Filters[0] = Constants.Voucher.Filters.ViewTypeFilters.InvoiceVoucherFilter;

        requestOptions.FetchList = Constants.Voucher.InvoiceViewFetchList.All;
        ActngVchrs.AddRange(await _tallyService.GetAllObjectsAsync<TCM.Voucher>(requestOptions, progress));

        requestOptions.Filters[0] = Constants.Voucher.Filters.ViewTypeFilters.InventoryVoucherFilter;
        // Voucher voucher = await _tallyService.GetVoucherAsync<Voucher>("52889497-5b6b-403d-8f83-224e3c7759b4-00001285", new() { LookupField = VoucherLookupField.GUID });
        //StockItem stockItem = await _tallyService.GetStockItemAsync<StockItem>("Floppy Drive");

        ActngVchrs.AddRange(await _tallyService.GetAllObjectsAsync<TCM.Voucher>(requestOptions, progress));

        requestOptions.Filters[0] = Constants.Voucher.Filters.ViewTypeFilters.PayslipVoucherFilter;
        List<Voucher> collection = await _tallyService.GetAllObjectsAsync<TCM.Voucher>(requestOptions, progress);
        ActngVchrs.AddRange(collection);

        requestOptions.Filters[0] = Constants.Voucher.Filters.ViewTypeFilters.MfgJournalVoucherFilter;
        ActngVchrs.AddRange(await _tallyService.GetAllObjectsAsync<TCM.Voucher>(requestOptions, progress));

        //var value = ActngVchrs.GroupBy(c => c.VchType).ToDictionary(c => c.Key, c => c.ToList());

        //foreach (var v in value)
        //{
        //    string json = JsonSerializer.Serialize(v.Value, new JsonSerializerOptions()
        //    {
        //        WriteIndented = true,
        //        //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        //        Converters = { new JsonStringEnumConverter(), new TallyDateJsonConverter() }
        //    });
        //    await File.WriteAllTextAsync("json/" + v.Key + ".json", json);
        //}

        Assert.That(ActngVchrs, Is.Not.Null);
        Assert.That(ActngVchrs.Count, Is.EqualTo(1107));
        Assert.That(count, Is.EqualTo(1107));
    }



    public class testobj
    {
        public string Vchtype { get; set; }
        public int Count { get; set; }
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

        Voucher voucher = await _tallyService.GetVoucherAsync<Voucher>("52889497-5b6b-403d-8f83-224e3c7759b4-000013e3",
                                                                       new()
                                                                       {
                                                                           LookupField = VoucherLookupField.GUID,
                                                                           FetchList = Constants.Voucher.AccountingViewFetchList.All
                                                                       });

    }
    [Test]
    public async Task CheckGetInvoiceVoucher()
    {

       // TallyResult tallyResult = await _tallyService.PostVoucherAsync<Voucher>(new() { MasterId= 4393,Action=TCM.Action.Delete });
        Voucher voucher = await _tallyService.GetVoucherAsync<Voucher>("4393",
                                                                       new()
                                                                       {
                                                                           LookupField = VoucherLookupField.MasterId,
                                                                           FetchList = Constants.Voucher.InvoiceViewFetchList.All
                                                                       });


        string json = voucher.GetJson();

    }


}

