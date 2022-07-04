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
        var objects = await _tallyService.GetObjectsAsync<TCMA.Ledger>(new() { FetchList = new() { "*" } });
        Assert.NotNull(objects);
        Assert.AreEqual(292, objects.Count);
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
