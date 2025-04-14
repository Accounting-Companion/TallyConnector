using TallyConnector.Services;

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
        await TallyService.GetVoucherStatisticsAsync(new TallyConnector.Core.Models.Request.AutoColumnReportPeriodRequestOptions());
    }
}
