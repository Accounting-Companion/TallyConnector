
using System.Windows.Data;
using TallyConnector.Core.Models.Request;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;
public partial class GSTRegistrationsViewModel : AbstractDataFetcherViewModel
{
    private readonly TallyPrimeService _tallyService;

    public GSTRegistrationsViewModel(TallyPrimeService tallyService)
    {
        _tallyService = tallyService;
        ObjectType = "GSTRegistrations";
    }


    public override async Task FetchData()
    {
        var options = new RequestOptions();

        try
        {
            var data = await _tallyService.GetObjectsAsync<GSTRegistration>(options).ConfigureAwait(false);
            DataView = CollectionViewSource.GetDefaultView(data);
            DataView.Refresh();
        }
        catch (Exception ex)
        {

            throw;
        }
    }
    public override async Task FetchPaginatedData()
    {
        var options = new PaginatedRequestOptions();
        var response = await _tallyService.GetVouchersAsync(options);
        DataView = CollectionViewSource.GetDefaultView(response.Data);
    }
}
