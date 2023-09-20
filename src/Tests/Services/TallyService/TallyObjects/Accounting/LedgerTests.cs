using TallyConnector.Core.Models.Masters;

namespace Tests.Services.TallyService.TallyObjects.Accounting;
internal class LedgerTests : BaseTallyServiceTest
{
    [Test]
    public async Task CheckGetAllLedgers()
    {
        var objects = await _tallyService.GetLedgersAsync();
        //string v = objects.ToJson(new());
        //string tv = JsonSerializer.Serialize(objects);
        Assert.That(objects, Is.Not.Null);
        Assert.That(objects, Has.Count.EqualTo(745));
    }
    [Test]
    //[TestCase(100,8)]
    [TestCase(500, 2)]
    public async Task CheckGetLedgersPaginated(int recordsPerPage, int ResultPages)
    {
        TCM.Pagination.PaginatedResponse<TCMA.Ledger> paginatedResponse = await _tallyService.GetLedgersAsync(new() { RecordsPerPage = recordsPerPage });
        var objects = paginatedResponse.Data;
        Assert.Multiple(() =>
        {
            Assert.That(paginatedResponse.PageNum, Is.EqualTo(1));
            Assert.That(paginatedResponse.TotalCount, Is.EqualTo(745));
            Assert.That(paginatedResponse.TotalPages, Is.EqualTo(ResultPages));
            Assert.That(objects, Is.Not.Null);
        });
        Assert.That(objects, Has.Count.EqualTo(recordsPerPage));
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

    [Test]
    public async Task CheckLedger_Create_Read_Delete()
    {
        var createresult = await _tallyService.PostLedgerAsync(new Ledger() { Name = "Test WrongData ", Group = "Duties & Taxes",TaxType=TallyConnector.Core.Models.TaxType.GST });
        //var createresult2 = await _tallyService.PostLedgerAsync(new Ledger() { Name = "Test \"name\" in Quotes", Group = "Sundry Debtors" });
        var result = await _tallyService.GetObjectAsync<TCMA.Ledger>("Test From Server");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test From Server"));
    }

    [Test]
    public async Task CheckGetLedgerwithSpecialSymbols()
    {
        //var result = await _tallyService.PostLedgerAsync(new Ledger("Test & Ledger \".Sd\"fg", "Sundry Debtors"));
        var result = await _tallyService.GetLedgerAsync("Test & Ledgegr\r\n");

        //var name = result.Alias;
        //Assert.That(result, Is.Not.Null);
        //Assert.That(result.Name, Is.EqualTo("Test From Server"));
    }
}
