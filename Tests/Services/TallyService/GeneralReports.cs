namespace Tests.Services.TallyService;
internal class GeneralReports : BaseTallyServiceTest
{
    [Test]
    public async Task TestGetCompaniesList()
    {
        var Companies = await _tallyService.GetCompaniesAsync();
        Assert.AreEqual(1, Companies.Count);
    }

    [Test]
    public async Task TestGetCompaniesinPath()
    {
        var Companies = await _tallyService.GetCompaniesinDefaultPathAsync();
        Assert.NotNull( Companies);
    }
    [Test]
    public async Task TestActiveCompany()
    {
        var Company = await _tallyService.GetActiveCompanyAsync();
        Assert.NotNull(Company);
    }
}
