using System.Windows.Data;
using TallyConnector.Core.Models.Request;
using TallyConnector.Models.TallyPrime.V6.Masters.Inventory;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;

public class StockItemViewModel : GenericDataFetcherViewModel<StockItem>
{
    public StockItemViewModel(TallyPrimeService tallyService) : base(tallyService)
    {
        ObjectType = "Stock Items";
    }
}
