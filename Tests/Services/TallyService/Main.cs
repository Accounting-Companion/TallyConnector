namespace Tests.Services.TallyService;
internal class Main : BaseTallyServiceTest
{

    [Test]
    public async Task TestTallyCheck()
    {
        var isRunning = await _tallyService.CheckAsync();
        Assert.AreEqual(true, isRunning);
    }

    [Test]
    public async Task TestGetLicenseInfo()
    {
        var licenseInfo = await _tallyService.GetLicenseInfoAsync();
        Assert.AreEqual(true, (bool)licenseInfo.IsEducationalMode);
    }
    [Test]
    public async Task TestGetMasterStatistics()
    {
        var masterstat = await _tallyService.GetMasterStatisticsAsync();
        Assert.AreEqual(16, masterstat.Count);
    }

    [Test]
    public async Task TestGetVoucherStatistics()
    {
        var masterstats = await _tallyService.GetMasterStatisticsAsync();
        //Get Vouchertype count from Master Statistics
        var vouchertypecount = masterstats.FirstOrDefault(C => C.Name.Replace(" ", "") == TCM.TallyObjectType.VoucherTypes.ToString()).Count;
        var voucherstat = await _tallyService.GetVoucherStatisticsAsync(new() { FromDate=new DateTime(2010,04,01)});
        Assert.AreEqual(vouchertypecount, voucherstat.Count);
    }

    
}
