
using System.Windows.Data;
using TallyConnector.Core.Models.Request;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;
public partial class VoucherViewModel : AbstractDataFetcherViewModel
{
    private readonly TallyPrimeService _tallyService;

    public VoucherViewModel(TallyPrimeService tallyService)
    {
        _tallyService = tallyService;
        ObjectType = "Vouchers";
    }


    public override async Task FetchData()
    {
        var options = new RequestOptions();

        try
        {
            var data = await _tallyService.GetVouchersAsync(options).ConfigureAwait(false);
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
