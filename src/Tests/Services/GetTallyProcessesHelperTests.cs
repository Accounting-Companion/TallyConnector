namespace Tests.Services;
internal class GetTallyProcessesHelperTests
{
    [Test]
    public async Task CheckGettallyprocesses()
    {
        List<TallyProcessInfo> tallyProcessInfos = GetTallyProcessesHelper.GetTallyProcesses();
        ConfigureServerPortHelper.ConfigureTallyServerPort(tallyProcessInfos[0], 9005);
    }
}
