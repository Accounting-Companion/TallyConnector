using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services.TallyService.TallyObjects.Accounting;
internal class LedgerTests : BaseTallyServiceTest
{
    [Test]
    public async Task CheckGetAllLedgers()
    {
        var objects = await _tallyService.GetAllObjectsAsync<TCM.Voucher>(new()
        {
            FromDate = new(2010,4,1),
            FetchList = new List<string>()
                {
                    "MasterId", "*", "AllledgerEntries", "ledgerEntries", "Allinventoryenntries",
                    "InventoryEntries", "InventoryEntriesIn", "InventoryEntriesOut"
                }
        });
        Assert.NotNull(objects);
        Assert.AreEqual(1131, objects.Count);
    }
    [Test]
    public async Task CheckCreateLedger()
    {
        //var result = await _tallyService.PostObjectToTallyAsync(new TCMA.Ledger("Test Ledg NA", "Sundry Debtors") { GSTPartyType = TCM.GSTPartyType.DeemedExport });
    }
    [Test]
    public async Task CheckGetLedger()
    {
        var result = await _tallyService.GetObjectAsync<TCMA.Ledger>("Test Ledg NA");
    }
}
