using System.Text.RegularExpressions;
using TallyConnector.Core.Extensions;
using TallyConnector.Core.Models.Request;
using TallyConnector.Services.TallyPrime.V4;

namespace TallyConnector.Tests.Services.TallyPrime.V4;

public class TallyPrimeServiceTests
{
    TallyPrimeService _service;
    public TallyPrimeServiceTests()
    {
        _service = new TallyPrimeService();
    }

    [Test]
    public async Task TestGetCurrenciesAsync()
    {

        var data = await _service.GetCurrenciesAsync();

        Assert.That(data, Is.Not.Null, "");

    }
    [Test]
    public async Task TestGetGroupsAsync()
    {

        PaginatedRequestOptions paginatedRequestOptions = new()
        {
            RecordsPerPage = 5,
            PageNum=2
        };
        var grps = await _service.GetGroupsAsync(paginatedRequestOptions);

        Assert.That(grps, Is.Not.Null, "");

    }
    [Test]
    public async Task TestGetObjectsAsync()
    {

        //var grps = await _service.GetObjects();
        //foreach (var item in grps)
        //{
        //    if(item is TallyConnector.Core.Models.TallyPrime.V4.Masters.Group)
        //    {

        //    }
        //}
        //Assert.That(grps, Is.Not.Null, "");

    }
}
