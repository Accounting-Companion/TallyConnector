using System.Collections.ObjectModel;
using System.IO;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Masters;
using TallyConnector.Services;
using TallyConnector.Services.Models;
using TallyConnector.Services.TallyPrime;

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
        LastAlterIdsRoot lastAlterIdsRoot = await _baseTallyService.GetLastAlterIdsAsync();
        SelectedCompanyName = await _baseTallyService.GetActiveSimpleCompanyNameAsync();
        CompanyList = new(await _baseTallyService.GetCompaniesAsync());
        //var license = await _baseTallyService.GetLicenseInfoAsync();
        PaginatedRequestOptions requestOptions = new()
        {
            PageNum = 1,
            RecordsPerPage = 1000,
            Filters = [new("vch_Filter", "$VOUCHERTYPENAME = 'SALES' AND $$NUMITEMS:ALLINVENTORYENTRIES>0")]
        };
        TallyService tallyService = new();

        TallyConnector.Core.Models.Common.Pagination.PaginatedResponse<Voucher> paginatedResponse = await tallyService.GetVouchersAsync();
        List<Voucher>? data = paginatedResponse.Data;
        var objs = await tallyService.GetVouchersAsync(requestOptions);
        
        List<LedgerDTO> objects = new List<LedgerDTO>() { new LedgerDTO() { OldName = "Test Ledger from new connecotr", Group = "Sundry Debtors", Action = TallyConnector.Core.Models.Action.Alter } };
       // var postResults = await tallyService.PostObjectsAsync(objs.Data.Select(c=>(VoucherDTO)c));
        var envp = new global::TallyConnector.Core.Models.TallyEnvelope<global::TallyConnector.Services.Models.TallyServicePostRequestEnvelopeMessage>(global::TallyConnector.Core.Models.RequestType.Import, global::TallyConnector.Core.Models.HType.Data, "AllMasters");
        var msg = new global::TallyConnector.Services.Models.TallyServicePostRequestEnvelopeMessage();
        msg.Vouchers = objs.Data.Select(c => (VoucherDTO)c).ToList();
        envp.Body.RequestData.RequestMessage = msg;
        TallyService.AddCustomResponseReportForPost(envp);
        await File.WriteAllTextAsync("Sales.xml", envp.GetXML());

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
