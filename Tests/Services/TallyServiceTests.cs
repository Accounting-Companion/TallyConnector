using TallyConnector.Services;

namespace Tests.Services;
internal class TallyServiceTests
{
    TallyService _tallyService = new();
    [Test]
    public async Task TestTallyCheck()
    {
        var isRunning = await _tallyService.CheckAsync();
        Assert.AreEqual(true, isRunning);
    }
}
