using System.Collections.ObjectModel;
using System.IO;
using TallyConnector.Core.Models;
using TallyConnector.Models.TallyPrime.V4.Masters;
using TallyConnector.Core.Models.Request;
using TallyConnector.Services;
using TallyConnector.Models.Common;

namespace DemoDesktopApp.ViewModels;
public partial class DashBoardViewModel : BaseViewModel
{
    private readonly BaseTallyService _baseTallyService;


    [ObservableProperty]
    ObservableCollection<BaseCompany> _companyList;

    [ObservableProperty]
    List<MasterStatistics> _masterStats;

    [ObservableProperty]
    List<VoucherStatistics> _voucherStats;
    [ObservableProperty]
    string _selectedCompanyName;
    public DashBoardViewModel(BaseTallyService baseTallyService)
    {
        _baseTallyService = baseTallyService;
    }
    [RelayCommand]
    public async Task OnLoaded()
    {
       // LastAlterIdsRoot lastAlterIdsRoot = await _baseTallyService.GetLastAlterIdsAsync();
        SelectedCompanyName = await _baseTallyService.GetActiveSimpleCompanyNameAsync();
       // CompanyList = new(await _baseTallyService.GetCompaniesAsync());
       // //var license = await _baseTallyService.GetLicenseInfoAsync();
       // PaginatedRequestOptions requestOptions = new()
       // {
       //     PageNum = 1,
       //     RecordsPerPage = 1000,
       //     Filters = [new("vch_Filter", "$VOUCHERTYPENAME = 'SALES' AND $$NUMITEMS:ALLINVENTORYENTRIES>0")]
       // };
       // TallyService tallyService = new();

       // TallyConnector.Core.Models.Common.Pagination.PaginatedResponse<Voucher> paginatedResponse = await tallyService.GetVouchersAsync();
       // List<Voucher>? data = paginatedResponse.Data;
       // var objs = await tallyService.GetVouchersAsync(requestOptions);
        
       // List<LedgerDTO> objects = [new LedgerDTO() { OldName = "Test Ledger from new connecotr", Group = "Sundry Debtors", Action = TallyConnector.Core.Models.Common.Action.Alter }];
       //// var postResults = await tallyService.PostObjectsAsync(objs.Data.Select(c=>(VoucherDTO)c));
       // var envp = new global::TallyConnector.Core.Models.PostRequestEnvelope<global::TallyConnector.Services.Models.TallyServicePostRequestEnvelopeMessage>(global::TallyConnector.Core.Models.Request.RequestType.Import, global::TallyConnector.Core.Models.Request.HType.Data, "AllMasters");
       // var msg = new global::TallyConnector.Services.Models.TallyServicePostRequestEnvelopeMessage();
       // msg.Vouchers = objs.Data.Select(c => (VoucherDTO)c).ToList();
       // envp.Body.RequestData.RequestMessage = msg;
       // TallyService.AddCustomResponseReportForPost(envp);
       // await File.WriteAllTextAsync("Sales.xml", envp.GetXML());

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
        //MasterStats = await _baseTallyService.GetMasterStatisticsAsync(reqOptions);
        //VoucherStats = await _baseTallyService.GetVoucherStatisticsAsync(reqOptions);
    }
}
