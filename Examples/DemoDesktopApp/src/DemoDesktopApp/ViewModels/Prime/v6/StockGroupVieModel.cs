using System.Windows.Data;
using TallyConnector.Core.Models.Request;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;
public class StockGroupVieModel : AbstractDataFetcherViewModel
{
    private readonly TallyPrimeService _tallyService;

    public StockGroupVieModel(TallyPrimeService tallyService)
    {
        _tallyService = tallyService;
        ObjectType = "StockGroups";
    }

    public override async Task FetchData()
    {
        var options = new RequestOptions();

        var data = await _tallyService.GetStockGroupsAsync(options).ConfigureAwait(false);
        DataView = CollectionViewSource.GetDefaultView(data);
        DataView.Refresh();


    }
    public override async Task FetchPaginatedData()
    {
        var options = new PaginatedRequestOptions();
        var response = await _tallyService.GetStockGroupsAsync(options);
        DataView = CollectionViewSource.GetDefaultView(response.Data);
    }
}
