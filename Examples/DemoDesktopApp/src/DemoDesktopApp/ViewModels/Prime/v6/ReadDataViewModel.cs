
namespace DemoDesktopApp.ViewModels.Prime.v6;
public partial class ReadDataViewModel : BaseViewModel
{

    private Dictionary<string, AbstractDataFetcherViewModel> viewModels;

    [ObservableProperty]
    private HashSet<string> _objectTypes;


    [ObservableProperty]
    private bool _isPaginated;

    [ObservableProperty]
    private string _objectType;

    [ObservableProperty]
    private AbstractDataFetcherViewModel _currentViewModel;

    public ReadDataViewModel(IEnumerable<AbstractDataFetcherViewModel> abstractDataFetcherViewModels)
    {
        viewModels = abstractDataFetcherViewModels.ToDictionary(c => c.ObjectType, c => c);
        ObjectTypes = [.. viewModels.Keys];
        ObjectType = ObjectTypes.FirstOrDefault();

    }
    partial void OnCurrentViewModelChanged(AbstractDataFetcherViewModel value)
    {
        FetchData();

    }

    private async Task FetchData()
    {
        
        await CurrentViewModel.FetchData().ConfigureAwait(false);
    }

    partial void OnObjectTypeChanged(string value)
    {
        CurrentViewModel = viewModels[value];
    }
}
