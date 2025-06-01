using System.Collections.ObjectModel;
using TallyConnector.Core.Models.Request;
using TallyConnector.Models.Common;
using TallyConnector.Services.TallyPrime.V6;
using TallyConnector.Models.TallyPrime.V6;

namespace DemoDesktopApp.ViewModels;
public partial class V6View : BaseViewModel
{
    private readonly TallyPrimeService _tallyService;


    [ObservableProperty]
    ObservableCollection<Company> _companyList;

    [ObservableProperty]
    List<MasterStatistics> _masterStats;

    [ObservableProperty]
    List<VoucherStatistics> _voucherStats;
    [ObservableProperty]
    string _selectedCompanyName;
    public V6View(TallyPrimeService tallyService)
    {
        _tallyService = tallyService;
    }
    [RelayCommand]
    public async Task OnLoaded()
    {
        SelectedCompanyName = await _tallyService.GetActiveSimpleCompanyNameAsync();
        CompanyList = new(await _tallyService.GetCompaniesAsync());
        var license = await _tallyService.GetLicenseInfoAsync();
        // PaginatedRequestOptions requestOptions = new()
        // {
        //     PageNum = 1,
        //     RecordsPerPage = 1000,
        //     Filters = [new("vch_Filter", "$VOUCHERTYPENAME = 'SALES' AND $$NUMITEMS:ALLINVENTORYENTRIES>0")]
        // };
        // TallyService _tallyService = new();

        // TallyConnector.Core.Models.Common.Pagination.PaginatedResponse<Voucher> paginatedResponse = await _tallyService.GetVouchersAsync();
        // List<Voucher>? data = paginatedResponse.Data;
        // var objs = await _tallyService.GetVouchersAsync(requestOptions);

        // List<LedgerDTO> objects = [new LedgerDTO() { OldName = "Test Ledger from new connecotr", Group = "Sundry Debtors", Action = TallyConnector.Core.Models.Common.Action.Alter }];
        //// var postResults = await _tallyService.PostObjectsAsync(objs.Data.Select(c=>(VoucherDTO)c));
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
        MasterStats = await _tallyService.GetMasterStatisticsAsync(new BaseRequestOptions().SetCompany(SelectedCompanyName));
        VoucherStats = await _tallyService.GetVoucherStatisticsAsync(new DateFilterRequestOptions().SetCompany(SelectedCompanyName));
    }
}
