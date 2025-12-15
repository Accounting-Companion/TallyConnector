using System;
using System.Threading.Tasks;
using System.Windows;
using TallyConnector.Core.Models.Interfaces;
using TallyConnector.Core.Models.Request;
using TallyConnector.Services.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels.Prime;

public class GenericDataFetcherViewModel<T> : AbstractDataFetcherViewModel
    where T : ITallyRequestableObject, IBaseObject, new()
{
    protected readonly TallyPrimeService _tallyService;

    public GenericDataFetcherViewModel(TallyPrimeService tallyService)
    {
        _tallyService = tallyService;
        ObjectType = typeof(T).Name;
    }

    public override async Task FetchData()
    {
        var options = new PaginatedRequestOptions { RecordsPerPage = 10 };
        // Use the base class helper - Single Responsibility Principle
        try
        {
           //await _tallyService.GetObjectsAsync<T>(options);
            await LoadStreamAsync(_tallyService.GetObjectsAsyncNew<T>(options));
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public override async Task FetchPaginatedData()
    {
        // Placeholder for future pagination implementation if needed
        // For now, FetchData handles the initial load (streaming)
        // If strict pagination is required, one can implement logical pagination on top of the stream or use older methods.
        // Assuming user wants streaming for "FetchData" button primarily.
        
        var options = new PaginatedRequestOptions();
        var response = await _tallyService.GetObjectsAsync<T>(options);
        
        // Since base _items is ObservableCollection<object>, we can clear and add
        // or just replace DataView source.
        // But AbstractDataFetcherViewModel seems to rely on _items now.
        
        Application.Current.Dispatcher.Invoke(() => 
        {
            _items.Clear();
            foreach(var item in response.Data)
            {
                _items.Add(item);
            }
        });
    }
}
