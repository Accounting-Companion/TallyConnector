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

    public async Task<List<CompanyType>?> GetCompaniesAsync<CompanyType>() where CompanyType : BaseCompany
    {
        return await GetAllObjectsAsync<CompanyType>(new()
        {
            IsInitialize = YesNo.Yes,
            FetchList = new()
            {
                "Name", "StartingFrom", "GUID", "MobileNo, RemoteFullListName", "*"
            }
        });
    }
    public async Task<List<Company>?> GetCompaniesAsync()
    {
        return await GetCompaniesAsync<Company>();
    }
    /// <inheritdoc/>
    public async Task<LastAlterIdsRoot?> GetLastAlterIdsAsync()
    {
        _logger?.LogInformation("Getting Last MasterIds from Tally");
        string reportName = "AlterIdsReport";
        RequestEnvelope requestEnvelope = new(HType.Data, reportName, new()
        {
            SVFromDate = Company?.StartingFrom,
            SVToDate = Company?.StartingFrom is null ? null : DateTime.Now,
            SVCompany = Company?.Name,
        });
        TDLMessage tdlMessage = new()
        {
            Report = new() { new(reportName) },
            Form = new() { new(reportName) },
            Part = new() { new() { Name = reportName, Lines = new() { reportName } } },
            Line = new() { new(reportName, fields: new() { "MastersLastId", "VouchersLastId" }) },
            Field = new()
            {
                new("MastersLastId", "MastersLastId", "$$CollectionField:$ALTERID:last:MastersCollection"),
                new("VouchersLastId", "VouchersLastId", "$$CollectionField:$ALTERID:last:VouchersCollection")
            },
            Collection = new()
            {
                new(colName:"MastersCollection",colType:"Masters",nativeFields:new(){"ALTERID"}){Sort=new(){"@@Default: -$Alterid" } },
                new(colName:"VouchersCollection",colType:"Vouchers",nativeFields:new(){"ALTERID"}){Sort=new(){"@@Default: -$Alterid" } }
            }
        };
        tdlMessage.Part![0].SetAttributes();

        requestEnvelope.Body.Desc.TDL.TDLMessage = tdlMessage;
        string xml = requestEnvelope.GetXML();

        TallyResult result = await SendRequestAsync(xml);
        if (result.Status == RespStatus.Sucess)
        {
            LastAlterIdsRoot? lastMasterIdsRoot = XMLToObject.GetObjfromXml<LastAlterIdsRoot>(result.Response!);
            _logger?.LogInformation("Received Last MasterIds from Tally");
            return lastMasterIdsRoot;
        }
        else
        {
            return null;
        }
    }
    public async Task<List<CompanyOnDisk>?> GetCompaniesinDefaultPathAsync()
    {
        return await GetAllObjectsAsync<CompanyOnDisk>(new()
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
        var PaginatedResp = await GetObjectsAsync<BaseCompany>(paginatedRequestOptions);
        if (PaginatedResp != null && PaginatedResp.Data.Count > 0)
        {
            return PaginatedResp.Data[0];
        }
        else
        {
            return null;
        }
    }

}
