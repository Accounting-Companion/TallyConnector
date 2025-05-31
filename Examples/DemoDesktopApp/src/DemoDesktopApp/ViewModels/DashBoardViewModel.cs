using System.Collections.ObjectModel;
using TallyConnector.Core.Models;
using TallyConnector.Models.Common;
using TallyConnector.Models.TallyPrime.V6;

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
