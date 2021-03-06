using TallyConnector.Core.Models;

namespace Tests.Services.TallyService;
internal class GeneralReports : BaseTallyServiceTest
{
    [Test]
    public async Task TestGetCompaniesList()
    {
        var Companies = await _tallyService.GetCompaniesAsync<Company>();
        Assert.That(Companies, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task TestGetCompaniesinPath()
    {
        var Companies = await _tallyService.GetCompaniesinDefaultPathAsync();
        Assert.That(Companies, Is.Not.Null);
    }
    [Test]
    public async Task TestActiveCompany()
    {
        var Company = await _tallyService.GetActiveCompanyAsync();
        Assert.That(Company, Is.Not.Null);
    }
}
