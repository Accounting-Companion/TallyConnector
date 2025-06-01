using System.ComponentModel;
using System.Windows.Data;
using TallyConnector.Models.Base;

namespace DemoDesktopApp.ViewModels.Prime;
public abstract partial class AbstractDataFetcherViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _objectType = string.Empty;

    [ObservableProperty]
    private ICollectionView _dataView;
    public AbstractDataFetcherViewModel()
    {
        DataView = CollectionViewSource.GetDefaultView(new List<BaseTallyObject>());
    }

    public abstract Task FetchData();

    public abstract Task FetchPaginatedData();
}
