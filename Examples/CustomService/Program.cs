// See https://aka.ms/new-console-template for more information
using CustomService.Models;
using CustomService.Services;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
namespace CustomService;
internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        await Test();
    }
    public static async Task Test()
    {
        try
        {

            TallyService tallyService = new();
            var bills = (await tallyService.GetBills()).Where(c=>!c.Balance.IsDebit);
            foreach (var bill in bills)
            {
                Voucher voucher = new()
                {
                    View = TallyConnector.Core.Models.VoucherViewType.AccountingVoucherView,
                    Date = new DateTime(2024, 04, 01),
                    VoucherType = "Payment",
                    LedgerEntries =
                    [
                        new LedgerEntry()
                        {
                            LedgerName = bill.LedgerName,
                            Amount = new(bill.Balance.Amount, true),
                            BillAllocations = [new BillAllocation() { Name = bill.Name,BillType= "Agst Ref", Amount = new(bill.Balance.Amount, true) }]
                        },
                        new LedgerEntry() { LedgerName = "SBI", Amount = new(bill.Balance.Amount, false) }
                        ]
                };
                var envelope = new TallyConnector.Core.Models.Envelope<Services.Models.VoucherDTO>(voucher, new()).GetXML();
                var resp = await tallyService.SendRequestAsync(envelope);
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}