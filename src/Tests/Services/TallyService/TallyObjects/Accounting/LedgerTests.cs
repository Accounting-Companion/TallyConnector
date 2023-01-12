using TallyConnector.Core.Models.Masters;

namespace Tests.Services.TallyService.TallyObjects.Accounting;
internal class LedgerTests : BaseTallyServiceTest
{
    [Test]
    public async Task CheckGetAllLedgers()
    {
        var objects = await _tallyService.GetLedgersAsync();
        Assert.That(objects, Is.Not.Null);
        Assert.That(objects, Has.Count.EqualTo(745));
    }
    [Test]
    public async Task CheckGetLedgersPaginated()
    {
        var objects = await _tallyService.GetLedgersAsync(new() { });
        Assert.That(objects, Is.Not.Null);
        Assert.That(objects, Has.Count.EqualTo(500));
    }
    [Test]
    public async Task CheckGetAllLedgerswithOptions()
    {
        var objects = await _tallyService.GetLedgersAsync((TCM.RequestOptions)(new()
        {
            FetchList = new List<string>()
                {
                    "MasterId", "*","CanDelete"
                }
        }));
        Assert.That(objects, Is.Not.Null);
        Assert.That(objects, Has.Count.EqualTo(745));
    }

    //[Test]
    //public async Task CheckLedger_Create_Read_Delete()
    //{
    //    var createresult = await _tallyService.PostLedgerAsync(new Ledger() { Name = "Test From Server", Group = "Sundry Debtors" });
    //    //var createresult2 = await _tallyService.PostLedgerAsync(new Ledger() { Name = "Test \"name\" in Quotes", Group = "Sundry Debtors" });
    //    var result = await _tallyService.GetObjectAsync<TCMA.Ledger>("Test From Server");

    //    Assert.That(result, Is.Not.Null);
    //    Assert.That(result.Name, Is.EqualTo("Test From Server"));
    //}
}
