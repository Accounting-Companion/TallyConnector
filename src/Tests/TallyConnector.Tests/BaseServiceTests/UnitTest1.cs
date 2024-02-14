using TallyConnector.Services;

namespace TallyConnector.Tests.BaseServiceTests;

public class BaseService
{
    BaseTallyService _baseTallyService;
    public BaseService()
    {
        _baseTallyService = new BaseTallyService();
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestGetLicenseInfo()
    {
        var prime3Ledgers = await new Services.TallyPrime.TallyPrime3Service().GetLedgersAsync(new Core.Models.RequestOptions() { Filters = [new("TC_TaxFilter", "$TAXTYPE='GST'")] });
        var v = await _baseTallyService.GetLicenseInfoAsync();
        Assert.That(v, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(v.IsEducationalMode, Is.True);
            Assert.That(v.IsIndian, Is.True);
        });
    }
    [Test]
    public async Task TestCheckMethod()
    {
        var v = await _baseTallyService.CheckAsync();
        Assert.That(v, Is.True);

    }
    [Test]
    public async Task TestMasterStatisticsMethod()
    {
        var v = await _baseTallyService.GetMasterStatisticsAsync();
        Assert.That(v, Is.Not.Null);

    }
    [Test]
    public async Task TestVoucherStatisticsMethod()
    {
        var v = await _baseTallyService.GetVoucherStatisticsAsync();
        Assert.That(v, Is.Not.Null);

    }
}