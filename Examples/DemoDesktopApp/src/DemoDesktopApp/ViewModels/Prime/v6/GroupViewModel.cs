using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime.v6;
public class GroupViewModel : GenericDataFetcherViewModel<Group>
{
    public GroupViewModel(TallyPrimeService tallyService) : base(tallyService)
    {
        ObjectType = "Groups";
    }
}
