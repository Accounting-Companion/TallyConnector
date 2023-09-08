using TallyConnector.Core.Extensions;
using TallyConnector.Core.Models;

namespace Tests.Services.TallyService.TallyObjects;
internal class VoucherTests : BaseTallyServiceTest
{
    [Test]
    public async Task CheckGetAllVouchers()
    {
        
        var ActngVchrs = new List<Voucher>();

        foreach (var mapping in Mappings.VoucherViewTypeMappings)
        {
            var vchs = await _tallyService.GetVouchersAsync<Voucher>(new RequestOptions()
            {
                FromDate = new(2009, 4, 1),
                FetchList = mapping.FetchList,
                Filters = new List<Filter>() { mapping.Filter }
               
            });
            ActngVchrs.AddRange(vchs);
        }
        var value = ActngVchrs.GroupBy(c => c.VchType).ToDictionary(c => c.Key, c => c.ToList());

        foreach (var v in value)
        {
            var Subdict = v.Value.GroupBy(c => c.View).ToDictionary(c => c.Key, c => c.ToList());
            foreach (var item in Subdict)
            {
                string json = item.Value.ToJson();
                var k = json.FromJson<Voucher>();
                await File.WriteAllTextAsync("json/" + v.Key + "_" + item.Key + ".json", json);
            }

        }

        Assert.That(ActngVchrs, Is.Not.Null);
        Assert.That(ActngVchrs, Has.Count.EqualTo(8636));
        //Assert.That(count, Is.EqualTo(8636));
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
                new(){LedgerName="ABCD India Pvt Ltd",Amount=118 },
                new(){LedgerName="Gst @ 18%",Amount=-18 },

            },
            InventoryAllocations = new()
            {
                new()
                {
                    StockItemName = "Mouse",
                    Rate = new(10,"Nos"),
                    ActualQuantity =new(10,"Nos"),
                    BilledQuantity =new(10,"Nos"),
                    Amount = -100,
                    BatchAllocations = new()
                    {
                        new(){OrderNo="frt",Amount=-100,BilledQuantity=new(10,"Nos"),ActualQuantity=new(10,"Nos")}
                    },
                    Ledgers = new(){ new() { LedgerName = "Purchase", Amount = -100 } }

                }
            }
        };
        string ks = voucher.GetJson(true);
        TallyResult tallyResult = await _tallyService.PostVoucherAsync(voucher);
    }


    [Test]
    public async Task CheckGetAccountingVoucher()
    {

        Voucher voucher = await _tallyService.GetVoucherAsync<Voucher>("52889497-5b6b-403d-8f83-224e3c7759b4-00001411",
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
        Voucher voucher = await _tallyService.GetVoucherAsync<Voucher>("5154",
                                                                       new()
                                                                       {
                                                                           LookupField = VoucherLookupField.MasterId,
                                                                           FetchList = Constants.Voucher.InvoiceViewFetchList.All
                                                                       });


        string json = voucher.GetJson();

    }


    [Test]
    public async Task GetVoucher()
    {
        var vouche = await _tallyService.GetVoucherAsync("910", new() { LookupField = VoucherLookupField.MasterId });
        vouche.ReferenceDate = vouche.Date;
        vouche.OtherFields = null;
        vouche.OtherAttributes = null;
        TallyResult tallyResult = await _tallyService.PostVoucherAsync(new Voucher()
        {
            MasterId = vouche.MasterId,
            Action = TallyConnector.Core.Models.Action.Delete
        });
    }

    [Test]
    public async Task CheckAlterVoucher()
    {

        Voucher vch = await _tallyService.GetVoucherAsync("5211", new() { FetchList = TCM.Constants.Voucher.AccountingViewFetchList.All, LookupField = VoucherLookupField.MasterId });

        vch.Ledgers.First().Amount = -25000;
        vch.Ledgers.Skip(1).First().Amount = 25000;
        XmlDocument doc = new XmlDocument();
        XmlAttribute TagNameAttribute = doc.CreateAttribute("TAGNAME");
        TagNameAttribute.Value = "MasterId";
        XmlAttribute TagValueAttribute = doc.CreateAttribute("TAGVALUE");
        TagValueAttribute.Value = vch.MasterId.ToString();

        vch.OtherAttributes = new XmlAttribute[] { TagNameAttribute, TagValueAttribute };
        TallyResult tallyResult = await _tallyService.PostVoucherAsync(vch);

    }
}

