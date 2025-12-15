using System.Text.Json;
using System.Windows.Data;
using TallyConnector.Core.Models.Request;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;

public class LedgerViewModel : GenericDataFetcherViewModel<Ledger>
{
    public LedgerViewModel(TallyPrimeService tallyService) : base(tallyService)
    {
        ObjectType = "Ledgers";
    }
}
