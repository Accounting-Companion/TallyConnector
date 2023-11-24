using TallyConnector.Core.Models;

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
        //_tallyService.Setup("localhost",900);
        var licenseInfo = await _tallyService.GetLicenseInfoAsync();
        Assert.That((bool)licenseInfo.IsEducationalMode, Is.EqualTo(true));
    }
    [Test]
    public async Task TestGetMasterStatistics()
    {
        _tallyService.Setup("localhost", 900);
        var masterstat = await _tallyService.GetMasterStatisticsAsync();
        Assert.That(masterstat, Has.Count.EqualTo(16));
    }

    [Test]
    public async Task TestGetVoucherStatistics()
    {
        List<VoucherTypeStat>? vchtats = await _tallyService.GetVoucherStatisticsAsync(new DateFilterRequestOptions());
        //Get Vouchertype count from Master Statistics
        //var vouchertypecount = masterstats.FirstOrDefault(C => C.Name.Replace(" ", "") == TCM.TallyObjectType.VoucherTypes.ToString()).Count;
        AutoVoucherStatisticsEnvelope autoVoucherStatisticsEnvelope = await _tallyService.GetVoucherStatisticsAsync(new TallyConnector.Core.Models.AutoColumnReportPeriodRequestOprions() { FromDate = new DateTime(2009, 04, 01),ToDate= new DateTime(2023, 03, 31),Periodicity= PeriodicityType.Year });
        //Assert.That(voucherstat, Has.Count.EqualTo(vouchertypecount));
    }

    
}
