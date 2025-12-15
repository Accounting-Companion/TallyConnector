using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace DemoDesktopApp.ViewModels.Prime;
public abstract partial class AbstractDataFetcherViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _objectType = string.Empty;

    [ObservableProperty]
    private ICollectionView _dataView;
    protected ObservableCollection<object> _items;

    public AbstractDataFetcherViewModel()
    {
       _items = [];
       DataView = CollectionViewSource.GetDefaultView(_items);
    }

    protected async Task LoadStreamAsync<T>(IAsyncEnumerable<T> stream)
    {
        Application.Current.Dispatcher.Invoke(_items.Clear);
        var buffer = new List<object>();
        const int batchSize = 20;

        await foreach (var item in stream)
        {
            buffer.Add(item!);
            if (buffer.Count >= batchSize)
            {
                var batch = buffer.ToList();
                buffer.Clear();
                // Batch update on UI thread
                Application.Current.Dispatcher.Invoke(() => 
                {
                    foreach (var i in batch) _items.Add(i);
                }, System.Windows.Threading.DispatcherPriority.Background);
                
                // Small delay to ensure UI remains responsive
                //await Task.Delay(1);
            }
        }

        // Add remaining items
        if (buffer.Count > 0)
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                foreach (var i in buffer) _items.Add(i);
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
    }

    public abstract Task FetchData();

    public abstract Task FetchPaginatedData();
}
