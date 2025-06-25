using System.ComponentModel;

namespace DemoDesktopApp.ViewModels.Prime;
public abstract partial class AbstractDataFetcherViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _objectType = string.Empty;

    [ObservableProperty]
    private ICollectionView _dataView;
    public AbstractDataFetcherViewModel()
    {
       // DataView = CollectionViewSource.GetDefaultView();
    }

    public abstract Task FetchData();

    public abstract Task FetchPaginatedData();
}
