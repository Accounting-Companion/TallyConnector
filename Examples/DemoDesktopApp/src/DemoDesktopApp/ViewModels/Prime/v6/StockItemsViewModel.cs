
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Data;
using TallyConnector.Core.Models.Request;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;
public partial class StockItemsViewModel : AbstractDataFetcherViewModel
{
    private readonly TallyPrimeService _tallyService;

    public StockItemsViewModel(TallyPrimeService tallyService)
    {
        _tallyService = tallyService;
        ObjectType = "Stock Items";
    }

    public override async Task FetchData()
    {
        var options = new RequestOptions
        {
            FromDate = new DateTime(2009, 04, 01)
        };
        var data = await _tallyService.GetStockItemsAsync(options).ConfigureAwait(false);
        DataView = CollectionViewSource.GetDefaultView(data);
        DataView.Refresh();


    }
    public override async Task FetchPaginatedData()
    {
        var options = new PaginatedRequestOptions();
        var response = await _tallyService.GetStockItemsAsync(options);
        DataView = CollectionViewSource.GetDefaultView(response.Data);
    }
}
