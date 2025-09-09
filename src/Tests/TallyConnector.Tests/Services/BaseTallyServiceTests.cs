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
    public async Task TestGetLicenseInfoAsync()
    {
        var licenseInfo = await TallyService.GetLicenseInfoAsync();
        Assert.That(licenseInfo, Is.Not.Null);

    }
    [Test]
    public async Task TestAutoColStats()
    {
        //TallyAbstractClient tallyCommonService = new TallyAbstractClient();
        // var objects =await tallyCommonService.GetObjectsAsync<Models.TallyPrime.V6.Masters.Ledger>(new());
        // await new TallyPrimeService().GetGroupsAsync();
        //await TallyService.(new TallyConnector.Core.Models.Request.AutoColumnReportPeriodRequestOptions());
    }

    public async Task TestOptionsBuilder()
    {
       // new RequestOptions<LedgerMeta>().Where(C=>C.MasterId.);
    }
}
