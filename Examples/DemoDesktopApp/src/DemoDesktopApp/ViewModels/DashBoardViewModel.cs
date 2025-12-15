using TallyConnector.Core.Models;

namespace DemoDesktopApp.ViewModels;
public partial class DashBoardViewModel : BaseViewModel
{
    private readonly IBaseTallyService _tallyService;

    [ObservableProperty]
    string? _selectedCompanyName;

    [ObservableProperty]
    LicenseInfo? _licenseInfo;
    public DashBoardViewModel(IBaseTallyService tallyService)
    {
        _tallyService = tallyService;
    }
    [RelayCommand]
    public async Task OnLoaded()
    {
        try
         {
            SelectedCompanyName = await _tallyService.GetActiveSimpleCompanyNameAsync();
            LicenseInfo = _tallyService.LicenseInfo;
        }
        catch (Exception ex)
        {

        }
        
    }


}
