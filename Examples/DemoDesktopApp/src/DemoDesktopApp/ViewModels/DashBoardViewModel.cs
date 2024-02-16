using System.Collections.ObjectModel;
using TallyConnector.Core.Models;
using TallyConnector.Services;

namespace DemoDesktopApp.ViewModels;
public partial class DashBoardViewModel : BaseViewModel
{
    private readonly BaseTallyService _baseTallyService;


    [ObservableProperty]
    ObservableCollection<BaseCompany> _companyList;

    [ObservableProperty]
    List<TallyConnector.Core.Models.Common.MasterStatistics> _masterStats;

    [ObservableProperty]
    List<TallyConnector.Core.Models.Common.VoucherStatistics> _voucherStats;
    [ObservableProperty]
    string _selectedCompanyName;
    public DashBoardViewModel(BaseTallyService baseTallyService)
    {
        _baseTallyService = baseTallyService;
    }
    [RelayCommand]
    public async Task OnLoaded()
    {
        SelectedCompanyName = await _baseTallyService.GetActiveSimpleCompanyNameAsync();
        CompanyList = new(await _baseTallyService.GetCompaniesAsync());
        var objs = await new TallyService().GetVouchersAsync();
    }

    partial void OnSelectedCompanyNameChanged(string value)
    {
        LoadStatistics();
    }

    private async void LoadStatistics()
    {
        DateFilterRequestOptions reqOptions = new()
        {
            Company = SelectedCompanyName
        };
        MasterStats = await _baseTallyService.GetMasterStatisticsAsync(reqOptions);
        VoucherStats = await _baseTallyService.GetVoucherStatisticsAsync(reqOptions);
    }
}
