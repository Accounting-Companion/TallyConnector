namespace TallyConnector.Tests.Services.TallyPrime.V4;

//public class TallyPrimeServiceTests
//{
//    TallyPrimeService _service;
//    public TallyPrimeServiceTests()
//    {
//        _service = new TallyPrimeService();
//    }

//    [Test]
//    public async Task TestGetCurrenciesAsync()
//    {

//        var data = await _service.GetCurrenciesAsync();

//        Assert.That(data, Is.Not.Null, "");

//    }
//    [Test]
//    public async Task TestGetGroupsAsync()
//    {

//        PaginatedRequestOptions paginatedRequestOptions = new()
//        {
//            RecordsPerPage = 5,
//            PageNum=2
//        };
//        var grps = await _service.GetGroupsAsync(paginatedRequestOptions);

//        Assert.That(grps, Is.Not.Null, "");

//    }
//    [Test]
//    public async Task TestGetLedgersAsync()
//    {


//        RequestOptions requestOptions = new();
//        //{ Filters = [new("ledgFilter", "$Name ='K.P.Bhutia'")] };
        
//        var ledgers = await _service.GetLedgersAsync(requestOptions);
      
//        //Assert.That(grps, Is.Not.Null, "");

//    }
//}
