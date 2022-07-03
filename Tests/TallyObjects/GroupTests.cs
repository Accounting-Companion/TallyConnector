namespace Tests.TallyObjects;
internal class GroupTests : BaseTallyTest
{

    [Test]
    public async Task CheckGetAllGroups()
    {
        var Groups = await Tally.GetObjectsAsync<TCMA.Group>();
        Assert.NotNull(Groups);
        Assert.AreEqual(73, Groups.Count);
    }
    [Test]
    public async Task CheckGetAllGroupsbyPaginate()
    {
        List<TCMA.Group> Groups = new();
        var Stat = await Tally.GetMasterStatisticsAsync();
        var TotalCount = Stat.FirstOrDefault(c => c.Name == TCM.TallyObjectType.Groups.ToString()).Count;
        TCM.Pagination pagination = new(TotalCount, 100);
        var TGroups = await Tally.GetObjectsAsync<TCMA.Group>();
        Groups.AddRange(TGroups);
        Assert.NotNull(Groups);
        Assert.AreEqual(73, Groups.Count);
    }
}
