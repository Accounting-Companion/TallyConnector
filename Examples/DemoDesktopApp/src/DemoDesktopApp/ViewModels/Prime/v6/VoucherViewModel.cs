using TallyConnector.Models.TallyPrime.V6;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;

public partial class VoucherViewModel : GenericDataFetcherViewModel<Voucher>
{
    public VoucherViewModel(TallyPrimeService tallyService) : base(tallyService)
    {
        ObjectType = "Vouchers";
    }
}
