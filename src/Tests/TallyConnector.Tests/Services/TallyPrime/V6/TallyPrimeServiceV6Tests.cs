using System.Text.Json;
using TallyConnector.Models.Common;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Services.TallyPrime.V6;

namespace TallyConnector.Tests.Services.TallyPrime.V6;
public class TallyPrimeServiceV6Tests
{
    private readonly TallyPrimeService primeService;
    public TallyPrimeServiceV6Tests()
    {
        primeService = new TallyPrimeService();
    }
    [Test]
    public async Task TestGetLedgerAsync()
    {
        var ledgers = await primeService.GetObjectsAsync<Ledger>();
        
        
        //Assert.That(JsonSerializer.Serialize(new DateOnly(2022, 03, 31)), Is.EqualTo("2022-03-31"));
    }
}
