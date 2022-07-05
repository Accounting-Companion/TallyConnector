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
        VoucherStatistics? statistics = await GetTDLReportAsync<VoucherTypeStat, VoucherStatistics>(requestOptions);
        return statistics?.VoucherStats;
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

    public async Task<List<CompanyOnDisk>?> GetCompaniesinDefaultPathAsync()
    {
        return await GetObjectsAsync<CompanyOnDisk>(new()
        {
            IsInitialize = YesNo.Yes,
            FetchList = new()
            {
                "NAME", "STARTINGFROM", "ENDINGAT","COMPANYNUMBER"
            }
        });
    }
    public async Task<BaseCompany?> GetActiveCompanyAsync()
    {
        PaginatedRequestOptions? paginatedRequestOptions = new()
        {
            FetchList = new() { "NAME", "GUID", "BOOKSFROM", "STARTINGFROM", "COMPANYNUMBER", "ENDINGAT" },
            Filters = new() { new("ActiveCompFilt", "$Name = ##SVCURRENTCOMPANY") },
            IsInitialize = YesNo.Yes,
        };
        var Companies = await GetObjectsAsync<BaseCompany>(paginatedRequestOptions);
        if (Companies != null && Companies.Count > 0)
        {
            return Companies[0];
        }
        else
        {
            return null;
        }
    }
}
