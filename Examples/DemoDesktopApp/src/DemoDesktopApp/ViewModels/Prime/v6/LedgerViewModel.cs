using System.Windows.Data;
using TallyConnector.Core.Models.Request;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;
public class LedgerViewModel : AbstractDataFetcherViewModel
{
    private readonly TallyPrimeService _tallyService;

    public LedgerViewModel(TallyPrimeService tallyService)
    {
        _tallyService = tallyService;
        ObjectType = "Ledgers";
    }

    public override async Task FetchData()
    {
        var options = new RequestOptions();

        var data = await _tallyService.GetLedgersAsync(options).ConfigureAwait(false);
        DataView = CollectionViewSource.GetDefaultView(data);
        DataView.Refresh();


    }
    public override async Task FetchPaginatedData()
    {
        var options = new PaginatedRequestOptions();
        var response = await _tallyService.GetLedgersAsync(options);
        DataView = CollectionViewSource.GetDefaultView(response.Data);
    }
}
