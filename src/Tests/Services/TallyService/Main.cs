namespace Tests.Services.TallyService;
internal class Main : BaseTallyServiceTest
{

    [Test]
    public async Task TestTallyCheck()
    {
        var isRunning = await _tallyService.CheckAsync();
        Assert.That(isRunning, Is.EqualTo(true));
    }

    [Test]
    public async Task TestGetLicenseInfo()
    {
        var licenseInfo = await _tallyService.GetLicenseInfoAsync();
        Assert.That((bool)licenseInfo.IsEducationalMode, Is.EqualTo(true));
    }
    [Test]
    public async Task TestGetMasterStatistics()
    {
        var masterstat = await _tallyService.GetMasterStatisticsAsync();
        Assert.That(masterstat, Has.Count.EqualTo(16));
    }

    [Test]
    public async Task TestGetVoucherStatistics()
    {
        var masterstats = await _tallyService.GetMasterStatisticsAsync();
        //Get Vouchertype count from Master Statistics
        var vouchertypecount = masterstats.FirstOrDefault(C => C.Name.Replace(" ", "") == TCM.TallyObjectType.VoucherTypes.ToString()).Count;
        var voucherstat = await _tallyService.GetVoucherStatisticsAsync(new() { FromDate = new DateTime(2010, 04, 01) });
        Assert.That(voucherstat, Has.Count.EqualTo(vouchertypecount));
    }


}
