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
        TallyCommonService  tallyCommonService = new TallyCommonService();
       // var objects =await tallyCommonService.GetObjectsAsync<Models.TallyPrime.V6.Masters.Ledger>(new());
       // await new TallyPrimeService().GetGroupsAsync();
        //await TallyService.(new TallyConnector.Core.Models.Request.AutoColumnReportPeriodRequestOptions());
    }
}
