using DemoDesktopApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DemoDesktopApp.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    [ObservableProperty]
    private int _tallyPort;

    [ObservableProperty]
    private string _tallyBaseUrl;

    private readonly TallyConnector.Services.TallyPrime.V6.TallyPrimeService _tallyService;

    public SettingsViewModel(TallyConnector.Services.TallyPrime.V6.TallyPrimeService tallyPrimeService)
    {
        _tallyService = tallyPrimeService;
        var settings = AppSettings.Load();
        TallyPort = settings.TallyPort;
        TallyBaseUrl = settings.TallyBaseUrl;
    }

    [RelayCommand]
    private void Save()
    {
        var settings = new AppSettings
        {
            TallyPort = TallyPort,
            TallyBaseUrl = TallyBaseUrl
        };
        settings.Save();
        _tallyService.SetupTallyService(TallyBaseUrl, TallyPort);
    }
}
