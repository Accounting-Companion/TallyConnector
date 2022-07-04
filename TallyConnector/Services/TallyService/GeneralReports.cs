namespace TallyConnector.Services;
public partial class TallyService
{
    public async Task<List<MasterTypeStat>?> GetMasterStatisticsAsync(BaseRequestOptions? requestOptions = null)
    {
        MasterStatistics? masterStatistics = await GetTDLReportAsync<MasterTypeStat, MasterStatistics>(new()
        {
            Company = requestOptions?.Company,
            XMLAttributeOverrides = requestOptions?.XMLAttributeOverrides
        });
        return masterStatistics?.MasterStats;
    }

    public async Task<List<VoucherTypeStat>?> GetVoucherStatisticsAsync(DateFilterRequestOptions? requestOptions = null)
    {
        VoucherStatistics? masterStatistics = await GetTDLReportAsync<VoucherTypeStat, VoucherStatistics>(requestOptions);
        return masterStatistics?.VoucherStats;
    }

    public async Task<List<Company>?> GetCompaniesAsync()
    {
        return await GetObjectsAsync<Company>(new()
        {
            IsInitialize = YesNo.Yes,
            FetchList = new()
            {
                "Name", "StartingFrom", "GUID", "MobileNo, RemoteFullListName", "*"
            }
        });
    }
}
