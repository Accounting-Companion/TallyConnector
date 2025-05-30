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
        //primeService.SetupTallyService("http://localhost", 9001);
    }
    [Test]
    public async Task TestGetLedgerAsync()
    {
        var ledgers = await primeService.GetObjectsAsync<Ledger>();
        
    }
}
