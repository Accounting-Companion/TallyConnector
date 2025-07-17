using TallyConnector.Core.Models.Request;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Models.TallyPrime.V6.Masters.Meta;
using TallyConnector.Services;

namespace TallyConnector.Tests.Services;

public class BaseTallyServiceTests

{
    private readonly BaseTallyService TallyService;
    public BaseTallyServiceTests()
    {
        TallyService = new();
    }

    [Test]
    public async Task TestGetActiveSimpleCompanyNameAsync()
    {
        string companyName = await TallyService.GetActiveSimpleCompanyNameAsync();
        Assert.That(companyName, Is.Not.Null);

    }
    [Test]
    public async Task TestAutoColStats()
    {
        TallyCommonService tallyCommonService = new TallyCommonService();
        // var objects =await tallyCommonService.GetObjectsAsync<Models.TallyPrime.V6.Masters.Ledger>(new());
        // await new TallyPrimeService().GetGroupsAsync();
        //await TallyService.(new TallyConnector.Core.Models.Request.AutoColumnReportPeriodRequestOptions());
    }

    public async Task TestOptionsBuilder()
    {
        LedgerRequestOptionsBuilder builder = new();
        var c = 0 < 5;
       builder.Where(c => c.Name == "" && c.OpeningBalance > 20);
    }
}
public class LedgerRequestOptionsBuilder : RequestOptionsBuilder<Ledger, LedgerMeta>
{

}