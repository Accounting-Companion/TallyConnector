using System.Windows.Data;
using TallyConnector.Core.Models.Request;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;
public class StockCategoryVieModel : AbstractDataFetcherViewModel
{
    private readonly TallyPrimeService _tallyService;

    public StockCategoryVieModel(TallyPrimeService tallyService)
    {
        _tallyService = tallyService;
        ObjectType = "StockCategories";
    }

    public override async Task FetchData()
    {
        var options = new RequestOptions();

        var data = await _tallyService.GetStockCategoriesAsync(options).ConfigureAwait(false);
        DataView = CollectionViewSource.GetDefaultView(data);
        DataView.Refresh();


    }
    public override async Task FetchPaginatedData()
    {
        var options = new PaginatedRequestOptions();
        var response = await _tallyService.GetStockCategoriesAsync(options);
        DataView = CollectionViewSource.GetDefaultView(response.Data);
    }
}
