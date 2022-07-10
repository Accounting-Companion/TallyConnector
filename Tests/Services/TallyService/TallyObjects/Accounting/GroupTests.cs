using Tests.Services.TallyService;

namespace Tests.Services.TallyService.TallyObjects.Accounting;
internal class GroupTests : BaseTallyServiceTest
{

    [Test]
    public async Task CheckGetAllGroups()
    {

        var Groups = await _tallyService.GetObjectsAsync<TCMA.Group>(new() { FetchList = new() { "*" } });
        Assert.NotNull(Groups);
        Assert.AreEqual(74, Groups.Count);
    }
    [Test]
    public async Task CheckGetAllGroups2()
    {

        var Groups = await _tallyService.GetAllObjectsAsync<TCMA.Group>(new() { FetchList = new() { "*" } });
        Assert.NotNull(Groups);
        Assert.AreEqual(74, Groups.Count);
    }
    [Test]
    public async Task CheckGetAllGroupsbyPaginate()
    {
        List<TCMA.Group> Groups = new();
        var Stat = await _tallyService.GetMasterStatisticsAsync();
        var TotalCount = Stat.FirstOrDefault(c => c.Name == TCM.TallyObjectType.Groups.ToString()).Count;
        TCM.Pagination pagination = new(73, 100);
        for (int i = 0; i < pagination.TotalPages; i++)
        {
            var TGroups = await _tallyService.GetObjectsAsync<TCMA.Group>(new() { Pagination = pagination });
            Groups.AddRange(TGroups);
            pagination.NextPage();
        }
        Assert.NotNull(Groups);
        Assert.AreEqual(74, Groups.Count);
    }

    [Test]
    public async Task CheckPostGroup_Create()
    {
        await _tallyService.PostGroupAsync(new TCMA.Group("Test NA") { AddlAllocType = TCM.AdAllocType.NotApplicable });
    }
}
