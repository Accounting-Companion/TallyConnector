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
        var objects = await _tallyService.GetObjectsAsync<TCMA.Ledger>(new()
        {
            FetchList = new List<string>()
                {
                    "MasterId", "*","CanDelete"
                }
        });
        Assert.That(objects, Is.Not.Null);
        Assert.That(objects, Has.Count.EqualTo(271));
    }
    [Test]
    public async Task CheckGetAllLedgers2()
    {
        var objects = await _tallyService.GetAllObjectsAsync<TCMA.Ledger>(new()
        {
            FetchList = new List<string>()
                {
                    "MasterId", "*","CanDelete"
                }
        });
        Assert.That(objects, Is.Not.Null);
        Assert.That(objects, Has.Count.EqualTo(271));
    }

    [Test]
    public async Task CheckLedger_Create_Read_Delete()
    {
        var result = await _tallyService.GetObjectAsync<TCMA.Ledger>("Test From Server");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("ABCD India Pvt Ltd"));
    }
}
