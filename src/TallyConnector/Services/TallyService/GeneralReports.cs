namespace TallyConnector.Services;
public partial class TallyService
{
    public async Task<List<MasterTypeStat>?> GetMasterStatisticsAsync(BaseRequestOptions? requestOptions = null, CancellationToken token = default)
    {
        string requestType = "Master Statistics";
        if (requestOptions != null)
        {
            if (requestOptions.Company != null)
            {
                requestType = $"Master Statistics ({requestOptions.Company})";
            }
        }
        MasterStatistics? masterStatistics = await GetTDLReportAsync<MasterTypeStat, MasterStatistics>(requestOptions: new()
        {
            Company = requestOptions?.Company,
            XMLAttributeOverrides = requestOptions?.XMLAttributeOverrides
        }, requestType: requestType, token: token);
        return masterStatistics?.MasterStats;
    }

    public async Task<List<VoucherTypeStat>?> GetVoucherStatisticsAsync(DateFilterRequestOptions? requestOptions = null, CancellationToken token = default)
    {
        string requestType = "Voucher Statistics";
        if (requestOptions != null)
        {
            if (requestOptions.FromDate != null && requestOptions.ToDate != null && requestOptions.Company != null)
            {
                requestType = $"Voucher Statistics ({requestOptions.Company} - {requestOptions.FromDate:dd-MM-yyyy} to {requestOptions.ToDate:dd-MM-yyyy})";
            }
            //requestType = $"Voucher Statistics";
        }
        VoucherStatistics? statistics = await GetTDLReportAsync<VoucherTypeStat, VoucherStatistics>(requestOptions: requestOptions,
                                                                                                    requestType: requestType,
                                                                                                    token: token);
        return statistics?.VoucherStats;
    }
    public async Task<AutoVoucherStatisticsEnvelope> GetVoucherStatisticsAsync(AutoColumnReportPeriodRequestOprions? requestOptions = null, CancellationToken token = default)
    {
        StaticVariables sv = new()
        {
            SVCompany = requestOptions?.Company ?? Company?.Name ?? await GetActiveSimpleCompanyNameAsync(token),
            SVExportFormat = "XML",
            SVFromDate = requestOptions?.FromDate ?? Company?.StartingFrom,
            SVToDate = requestOptions?.ToDate ?? GetToDate()
        };
        const string reportName = "TC_AutoColumnStats";
        RequestEnvelope requestEnvelope = new(HType.Data, reportName, sv);
        TDLMessage tDLMessage = requestEnvelope.Body.Desc.TDL.TDLMessage;
        string periodicity = GetPeriodicty(requestOptions);
        Report report = new(reportName)
        {
            Repeat = new() { " SVFromDate, SVToDate" },
            Variable = new() { "DoSetAutoColumn,SVPeriodicity" },
            Set = new()
            {
                "DoSetAutoColumn:No",
                $"SVPeriodicity:\"{periodicity}\"",
                "DSPRepeatCollection:\"Period Collection\""
            }
        };
        string RequestType = $"VchType Auto Column Stats({periodicity}) of company - {sv.SVCompany} ({sv.SVFromDate} to {sv.SVToDate})";

        tDLMessage.Report = new() { report };
        Form form = new(reportName)
        {
            Option = new() { "TC_SETAUTOOPTION:$$SetAutoColumns:SVFromDATE,SVTODATE" }
        };
        tDLMessage.Form = new() { form, new("TC_SETAUTOOPTION") { IsOption = YesNo.Yes, PartName = string.Empty } };
        const string CollectionName = "TC_VchTypeCollection";
        Part part = new(reportName, CollectionName);
        tDLMessage.Part = new() { part };
        const string nameFieldName = "TC_VchTypeName";
        const string totalCountFieldName = "TC_VchTypeTotalCount";
        const string periodCountRootFieldName = "TC_VchTypePeriodStat";
        const string fromDateFieldName = "TC_VchTypeFromDate";
        const string toDateFieldName = "TC_VchTypeToDate";
        const string cancelledCountFieldName = "TC_VchTypeCancCount";
        const string optionalCountFieldName = "TC_VchTypeOptCount";
        const string totalPeriodCountFieldName = "TC_VchTypeTotalPeriodCount";
        List<string> rootFields = new() { nameFieldName, totalCountFieldName, periodCountRootFieldName };
        Line line = new(reportName, rootFields, "VchTypeStat") { Repeat = new() { periodCountRootFieldName } };
        tDLMessage.Line = new() { line };
        tDLMessage.Field = new()
        {
            new(nameFieldName,"Name","$Name"),
            new(totalCountFieldName,"TotalCount","$TotalCount"),
            new(periodCountRootFieldName,"PeriodStat",null){Fields=new(){ fromDateFieldName,toDateFieldName,cancelledCountFieldName,optionalCountFieldName,totalPeriodCountFieldName } },
            new(fromDateFieldName,"FromDate","##SVFromDate"){Use="Short Date Field"},
            new(toDateFieldName,"ToDate","##SVToDate"){Use="Short Date Field"},
            new(totalPeriodCountFieldName,"TotalCount","$TotalPeriodCount"),
            new(optionalCountFieldName,"OtionalCount","$OptionalCount"),
            new(cancelledCountFieldName,"CancelledCount","$CancelledCount")
        };
        tDLMessage.Collection = new()
        {
            new(colName:CollectionName,colType: "VoucherTypes")
            {
                Compute = new()
                {
                    "TotalPeriodCount :  $$DirectTotalVch:$Name",
                    "CancelledCount : $$DirectCancVch:$Name",
                    "OptionalCount :  $$DirectOptionalVch:$Name",
                    "TotalCount : $$DirectAllVch:$Name",
                }
            }
        };
        string requestXml = requestEnvelope.GetXML();

        TallyResult tallyResult = await SendRequestAsync(requestXml, RequestType, token);
        if (tallyResult.Status == RespStatus.Sucess)
        {
            var k = XMLToObject.GetObjfromXml<AutoVoucherStatisticsEnvelope>(tallyResult.Response!);
            return k;
        }
        else throw new Exception(tallyResult.Response);
    }

    private static string GetPeriodicty(AutoColumnReportPeriodRequestOprions? requestOptions)
    {
        return requestOptions?.Periodicity switch
        {
            PeriodicityType.Month => Constants.Periodicty.Month,
            PeriodicityType.Day => Constants.Periodicty.Day,
            PeriodicityType.Week => Constants.Periodicty.Week,
            PeriodicityType.Fortnight => Constants.Periodicty.Fortnight,
            PeriodicityType.ThreeMonth => Constants.Periodicty.ThreeMonth,
            PeriodicityType.SixMonth => Constants.Periodicty.SixMonth,
            PeriodicityType.Year => Constants.Periodicty.Year,
            null => Constants.Periodicty.Month,
            _ => throw new NotImplementedException()
        };
    }

    public async Task<List<CompanyType>?> GetCompaniesAsync<CompanyType>(CancellationToken token = default) where CompanyType : BaseCompany
    {
        return await GetAllObjectsAsync<CompanyType>(new()
        {
            IsInitialize = YesNo.Yes,
            FetchList = new()
            {
                "Name", "StartingFrom", "GUID", "MobileNo, RemoteFullListName", "*"
            },
            Filters = new() { new("SimpleCompany", "", false) }
        }, token: token);
    }
    public async Task<List<Company>?> GetCompaniesAsync(CancellationToken token = default)
    {
        return await GetCompaniesAsync<Company>(token);
    }
    /// <inheritdoc/>
    public async Task<LastAlterIdsRoot?> GetLastAlterIdsAsync(CancellationToken token = default)
    {
        _logger?.LogInformation("Getting Last AlterIds from Tally");
        string reportName = "AlterIdsReport";
        RequestEnvelope requestEnvelope = new(HType.Data, reportName, new()
        {
            SVFromDate = Company?.StartingFrom,
            SVToDate = GetToDate(),
            SVCompany = Company?.Name,
        });
        TDLMessage tdlMessage = new()
        {
            Report = new() { new(reportName) },
            Form = new() { new(reportName) },
            Part = new() { new() { Name = reportName, Lines = new() { reportName } } },
            Line = new() { new(reportName, fields: new() { "TC_MastersLastId", "TC_VouchersLastId" }) },
            Field = new()
            {
                new("TC_MastersLastId", "MastersLastId", "if $$IsEmptyCollection:TC_MastersCollection THEN 0 else $$CollectionField:$ALTERID:last:TC_MastersCollection"),
                new("TC_VouchersLastId", "VouchersLastId", "if $$IsEmptyCollection:TC_VouchersCollection THEN 0 else $$CollectionField:$ALTERID:last:TC_VouchersCollection")
            },
            Collection = new()
            {
                new(colName:"TC_MastersCollection",colType:"Masters",nativeFields:new(){"ALTERID"}){Sort=new(){"@@Default: -$Alterid" } },
                new(colName:"TC_VouchersCollection",colType:"Vouchers",nativeFields:new(){"ALTERID"}){Sort=new(){"@@Default: -$Alterid" } }
            }
        };
        tdlMessage.Part![0].SetAttributes();

        requestEnvelope.Body.Desc.TDL.TDLMessage = tdlMessage;
        string xml = requestEnvelope.GetXML();

        TallyResult result = await SendRequestAsync(xml: xml, requestType: "last AlterIds Report", token: token);
        if (result.Status == RespStatus.Sucess)
        {
            LastAlterIdsRoot? lastMasterIdsRoot = XMLToObject.GetObjfromXml<LastAlterIdsRoot>(result.Response!);
            _logger?.LogInformation("Received Last AlterIds from Tally");
            return lastMasterIdsRoot;
        }
        else
        {
            return null;
        }
    }

    private static DateTime GetToDate(DateTime now)
    {
        return new DateTime(now.Month > 3 ? now.Year + 1 : now.Year, 3, 31);
    }
    private static DateTime GetToDate()
    {
        return GetToDate(DateTime.Now);
    }
    public async Task<List<CompanyOnDisk>?> GetCompaniesinDefaultPathAsync(CancellationToken token = default)
    {
        return await GetAllObjectsAsync<CompanyOnDisk>(new()
        {
            IsInitialize = YesNo.Yes,
            FetchList = new()
            {
                "NAME", "STARTINGFROM", "ENDINGAT","COMPANYNUMBER"
            }
        }, token: token);
    }
    public async Task<BaseCompany?> GetActiveCompanyAsync(CancellationToken token = default)
    {
        XmlAttributeOverrides xmlAttributeOverrides = new();
        PaginatedRequestOptions? paginatedRequestOptions = new()
        {
            Company = await GetActiveSimpleCompanyNameAsync(token),
            FetchList = new() { "NAME", "GUID", "BOOKSFROM", "STARTINGFROM", "COMPANYNUMBER", "ENDINGAT" },
            Filters = new() { new("ActiveCompFilt", "$Name = ##SVCURRENTCOMPANY") },
            IsInitialize = YesNo.Yes,
            XMLAttributeOverrides = xmlAttributeOverrides
        };
        var PaginatedResp = await GetObjectsAsync<BaseCompany>(paginatedRequestOptions, token);
        if (PaginatedResp != null && PaginatedResp.Data?.Count > 0)
        {
            return PaginatedResp.Data[0];
        }
        else
        {
            return null;
        }
    }
    public async Task<string?> GetActiveSimpleCompanyNameAsync(CancellationToken token = default)
    {

        RequestEnvelope requestEnvelope = new(HType.Function, "$$CurrentSimpleCompany");

        string Reqxml = requestEnvelope.GetXML();
        TallyResult tallyResult = await SendRequestAsync(Reqxml, "Active Simple Company Name");
        if (tallyResult.Status == RespStatus.Sucess && tallyResult.Response != null)
        {
            Envelope<string>? Envelope = XMLToObject.GetObjfromXml<Envelope<string>>(tallyResult.Response, null, _logger);
            return Envelope?.Body?.Data?.FuncResult?.Result;
        }
        return null;
    }



}
