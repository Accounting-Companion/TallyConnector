using TallyConnector.Core.Models;
using TallyConnector.Models.Base;
using TallyConnector.Models.Base.Masters;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Services;
using TallyConnector.Services.TallyPrime.V6;

namespace TallyConnector.Tests.Services;

public class BaseTallyServiceTests

{
    BaseTallyService TallyService;
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
        var objects =await tallyCommonService.GetObjectsAsync<Ledger>(new());
       // await new TallyPrimeService().GetGroupsAsync();
        //await TallyService.(new TallyConnector.Core.Models.Request.AutoColumnReportPeriodRequestOptions());
    }
}
