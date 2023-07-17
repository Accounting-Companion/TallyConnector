using TallyConnector.Core.Extensions;
using TallyConnector.Core.Models;
namespace Tests.Services.TallyService;
internal class GeneralReports : BaseTallyServiceTest
{
    [Test]
    public async Task TestGetCompaniesList()
    {
        var Companies = await _tallyService.GetCompaniesAsync();
        var cs = await _tallyService.GetTaxUnitsAsync();
        string v = Companies.ToJson();
        Assert.That(Companies, Has.Count.EqualTo(3));
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
        _tallyService.Setup("http://localhost",900);
        var Company = await _tallyService.GetActiveCompanyAsync();
        string v = await _tallyService.GetActiveSimpleCompanyNameAsync();
        Assert.That(Company, Is.Not.Null);
    }
    [Test]
    public async Task TestGetAlterIds()
    {
        //_tallyService.Setup("http://localhost",900);
        LastAlterIdsRoot lastMasterIdsRoot = await _tallyService.GetLastAlterIdsAsync();
        Assert.That(lastMasterIdsRoot, Is.Not.Null);
    }
}
