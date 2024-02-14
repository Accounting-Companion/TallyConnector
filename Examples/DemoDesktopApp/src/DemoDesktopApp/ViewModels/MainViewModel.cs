namespace DemoDesktopApp.ViewModels;
public partial class MainViewModel : BaseViewModel
{
    public MainViewModel(NavigationState navigationState)
    {
        NavigationState = navigationState;
    }

    public NavigationState NavigationState { get; }

    [RelayCommand]
    public async Task OnLoaded()
    {

    }
}
