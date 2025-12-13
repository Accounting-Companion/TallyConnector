using System.Windows.Data;
using TallyConnector.Core.Models.Request;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;

public class GodownVieModel : AbstractDataFetcherViewModel
{
    private readonly TallyPrimeService _tallyService;

    public GodownVieModel(TallyPrimeService tallyService)
    {
        _tallyService = tallyService;
        ObjectType = "Godown";
    }

    public override async Task FetchData()
    {
        var options = new RequestOptions();

        var data = await _tallyService.GetGodownsAsync(options).ConfigureAwait(false);
        DataView = CollectionViewSource.GetDefaultView(data);
        DataView.Refresh();


    }
    public override async Task FetchPaginatedData()
    {
        var options = new PaginatedRequestOptions();
        var response = await _tallyService.GetGodownsAsync(options);
        DataView = CollectionViewSource.GetDefaultView(response.Data);
    }
}
