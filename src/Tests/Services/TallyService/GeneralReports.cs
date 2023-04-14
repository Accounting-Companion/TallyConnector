using TallyConnector.Core.Extensions;
using TallyConnector.Core.Models;
namespace Tests.Services.TallyService;
internal class GeneralReports : BaseTallyServiceTest
{
    [Test]
    public async Task TestGetCompaniesList()
    {
        var Companies = await _tallyService.GetCompaniesAsync();
        string v = Companies.ToJson();
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
    [Test]
    public async Task TestGetAlterIds()
    {
        _tallyService.Setup("http://localhost",900);
        LastAlterIdsRoot lastMasterIdsRoot = await _tallyService.GetLastAlterIdsAsync();
        Assert.That(lastMasterIdsRoot, Is.Not.Null);
    }
}
