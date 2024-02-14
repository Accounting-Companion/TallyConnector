namespace DemoDesktopApp.State;
public partial class NavigationState : ObservableObject
{
    [ObservableProperty]
    private ViewType _viewType;

    [ObservableProperty]
    private bool _isMenuVisible;

    [ObservableProperty]
    private BaseViewModel _viewModel = null!;

    private readonly ViewModelFactory _viewModelFactory;

    public NavigationState(ViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;

    }
    public void NavigateWithParams(ViewType viewType, Dictionary<string, object>? extraParams = null)
    {
        ViewType = viewType;

        ViewModel?.Dispose();
        ViewModel = _viewModelFactory.CreateViewModel(ViewType, extraParams);
    }
    [RelayCommand]
    public void Navigate(ViewType viewType)
    {
        NavigateWithParams(viewType, null);
    }
}


