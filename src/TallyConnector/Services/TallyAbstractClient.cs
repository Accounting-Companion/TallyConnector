using TallyConnector.Core.Extensions;
using TallyConnector.Core.Models.Interfaces;
using TallyConnector.Core.Models.Response;
using static TallyConnector.Core.Constants;
using System.IO;
using System.Text;
using System.Linq;
using XmlSourceGenerator.Abstractions;
using System.Runtime.CompilerServices;
using System.Numerics;

//[assembly: InternalsVisibleTo("TestProject")]

namespace TallyConnector.Services;

public abstract class TallyAbstractClient : ITallyAbstractClient
{
    protected readonly ILogger _logger;
    protected readonly IBaseTallyService _baseHandler;

    public TallyAbstractClient()
    {
        _baseHandler = new BaseTallyService();
        _logger = NullLogger.Instance;
    }
    public TallyAbstractClient(IBaseTallyService baseTallyService)
    {
        _baseHandler = baseTallyService;
        _logger = NullLogger.Instance;
    }

    public TallyAbstractClient(ILogger logger, IBaseTallyService baseTallyService)
    {
        _logger = logger;
        _baseHandler = baseTallyService;
    }

    public void SetupTallyService(string url, int port)
    {
        _baseHandler.Setup(url, port);
    }

    public Task<bool> CheckAsync() => _baseHandler.CheckAsync();

    public Task<string> GetActiveSimpleCompanyNameAsync() => _baseHandler.GetActiveSimpleCompanyNameAsync();
    public void SetCompany(ICompany company) => _baseHandler.SetCompany(company);
    public async Task<LastAlterIdsRoot> GetLastAlterIdsAsync(BaseRequestOptions? baseRequestOptions = null, CancellationToken token = default)
    {
        var reqType = "Getting Last AlterIds from Tally";

        _logger.LogInformation(reqType);
        using var activity = BaseTallyServiceActivitySource.StartActivity(reqType);
        string reportName = "AlterIdsReport";
        RequestEnvelope requestEnvelope = new(HType.Data, reportName);
        TDLMessage tdlMessage = new()
        {
            Report = [new(reportName)],
            Form = [new(reportName)],
            Part = [new(reportName, "TC_CompanyCollection")],
            Line = [new(reportName, fields: ["TC_MastersLastId", "TC_VouchersLastId"])],
            Field =
            [
                new("TC_MastersLastId", "MastersLastId", "if $$IsEmpty:$ALTMSTID THEN 0 else $ALTMSTID"),
                new("TC_VouchersLastId", "VouchersLastId", "if $$IsEmpty:$ALTVCHID THEN 0 else $ALTVCHID")
            ],
            Collection =
            [
                new(colName:"TC_CompanyCollection",colType:"Company",nativeFields:["ALTMSTID,ALTVCHID"]){Filters=["TC_CurCompFilter"] },
            ],
            System =
            [
                new("TC_CurCompFilter","$Name=##SVCURRENTCOMPANY"),
            ]

        };
        tdlMessage.Part![0].SetAttributes();

        requestEnvelope.Body.Desc.TDL.TDLMessage = tdlMessage;
        requestEnvelope.PopulateOptions(baseRequestOptions);
        await _baseHandler.PopulateDefaultOptions(requestEnvelope, token);
        string xml = requestEnvelope.GetXML();

        TallyResult result = await _baseHandler.SendRequestAsync(xml: xml, requestType: "last AlterIds Report", token: token);
        if (result.Status == RespStatus.Sucess)
        {
            LastAlterIdsRoot? lastMasterIdsRoot = XMLToObject.GetObjfromXml<LastAlterIdsRoot>(result.Response!);
            _logger?.LogInformation("Received Last AlterIds from Tally");
            return lastMasterIdsRoot;
        }
        else
        {
            throw new Exception("Error hile getting Last AlterIds");
        }

    }


    public async Task<List<AutoColVoucherTypeStat>> GetPeriodicVoucherStatisticsAsync(AutoColumnReportPeriodRequestOptions requestOptions, CancellationToken token = default)
    {
        using var activity = BaseTallyServiceActivitySource.StartActivity();
        StaticVariables sv = new()
        {
            SVCompany = requestOptions?.Company,
            SVExportFormat = "XML",
            SVFromDate = requestOptions?.FromDate,
            SVToDate = requestOptions?.ToDate
        };
        const string reportName = "TC_AutoColumnStats";
        RequestEnvelope requestEnvelope = new(HType.Data, reportName, sv);
        TDLMessage tDLMessage = requestEnvelope.Body.Desc.TDL.TDLMessage;
        string periodicity = GetPeriodictyString(requestOptions);

        Report report = new(reportName)
        {
            Repeat = [" SVFromDate, SVToDate"],
            Variable = ["DoSetAutoColumn,SVPeriodicity"],
            Set =
            [
                "DoSetAutoColumn:No",
                $"SVPeriodicity:\"{periodicity}\"",
                "DSPRepeatCollection:\"Period Collection\""
            ]
        };
        tDLMessage.Report = [report];
        Form form = new(reportName)
        {
            Option = ["TC_SETAUTOOPTION:$$SetAutoColumns:SVFromDATE,SVTODATE"]
        };
        tDLMessage.Form = [form, new("TC_SETAUTOOPTION") { IsOption = YesNo.Yes, PartName = string.Empty }];
        const string CollectionName = "TC_VchTypeCollection";
        Part part = new(reportName, CollectionName);
        tDLMessage.Part = [part];
        const string nameFieldName = "TC_VchTypeName";
        const string totalCountFieldName = "TC_VchTypeTotalCount";
        const string periodCountRootFieldName = "TC_VchTypePeriodStat";
        const string fromDateFieldName = "TC_VchTypeFromDate";
        const string toDateFieldName = "TC_VchTypeToDate";
        const string cancelledCountFieldName = "TC_VchTypeCancCount";
        const string optionalCountFieldName = "TC_VchTypeOptCount";
        const string totalPeriodCountFieldName = "TC_VchTypeTotalPeriodCount";
        List<string> rootFields = [nameFieldName, totalCountFieldName];
        const string line2Name = $"{reportName}Repeat";
        Line line = new(reportName, rootFields, "VchTypeStat") { Option = [$"{line2Name}:$MigVal>0"] };
        Line line2 = new(line2Name, [periodCountRootFieldName]) { Name = line2Name, Repeat = [periodCountRootFieldName], IsOption = YesNo.Yes };
        tDLMessage.Line = [line, line2];
        tDLMessage.Field =
        [
            new(nameFieldName,"Name","$Name"),
            new(totalCountFieldName,"TotalCount","($$DirectTotalVch:$Name)+($$DirectOptionalVch:$Name)+($$DirectCancVch:$Name)"),
            new(periodCountRootFieldName,"PeriodStat",null){Fields=[fromDateFieldName,toDateFieldName,cancelledCountFieldName,optionalCountFieldName,totalPeriodCountFieldName] },
            new(fromDateFieldName,"FromDate","$$TC_TransformDateToXSD:##SVFromDate"){Invisible="$$ISEmpty:$$value"},
            new(toDateFieldName,"ToDate","$$TC_TransformDateToXSD:##SVToDate"){Use="$$ISEmpty:$$value"},
            new(totalPeriodCountFieldName,"TotalCount","$TotalPeriodCount"),
            new(optionalCountFieldName,"OtionalCount","$OptionalCount"),
            new(cancelledCountFieldName,"CancelledCount","$CancelledCount")
        ];
        tDLMessage.Collection =
        [
            new(colName:CollectionName,colType: "VoucherTypes")
            {
                Compute =
                [
                    "CancelledCount : $$DirectCancVch:$Name",
                    "OptionalCount :  $$DirectOptionalVch:$Name",
                    "TotalPeriodCount : ($$DirectTotalVch:$Name) + $OptionalCount + $CancelledCount",
                ]
            }
        ];
        tDLMessage.Functions = BaseTallyService.GetDefaultTDLFunctions();
        await _baseHandler.PopulateDefaultOptions(requestEnvelope, token);
        string requestXml = requestEnvelope.GetXML();
        string RequestType = $"VchType Auto Column Stats({periodicity}) of company - {sv.SVCompany} ({sv.SVFromDate} to {sv.SVToDate})";
        if (activity != null)
        {
            activity.DisplayName = RequestType;
        }
        TallyResult tallyResult = await _baseHandler.SendRequestAsync(requestXml, RequestType, token);
        if (tallyResult.Status == RespStatus.Sucess)
        {
            var k = XMLToObject.GetObjfromXml<AutoVoucherStatisticsEnvelope>(tallyResult.Response!);
            return k.VoucherTypeStats ?? [];
        }
        else throw new Exception(tallyResult.Response);
    }
    private static string GetPeriodictyString(AutoColumnReportPeriodRequestOptions? requestOptions)
    {
        return requestOptions?.Periodicity switch
        {
            PeriodicityType.Month => Periodicty.Month,
            PeriodicityType.Day => Periodicty.Day,
            PeriodicityType.Week => Periodicty.Week,
            PeriodicityType.Fortnight => Periodicty.Fortnight,
            PeriodicityType.ThreeMonth => Periodicty.ThreeMonth,
            PeriodicityType.SixMonth => Periodicty.SixMonth,
            PeriodicityType.Year => Periodicty.Year,
            null => Periodicty.Month,
            _ => throw new NotImplementedException()
        };
    }

    public Task<LicenseInfo> GetLicenseInfoAsync() => _baseHandler.GetLicenseInfoAsync();


    /// <inheritdoc/>
    public async Task<List<T>> GetObjectsAsync<T>(BaseRequestOptions? options = null, CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject
    {
        var reqEnvelope = T.GetRequestEnvelope();
        reqEnvelope.PopulateOptions(options);
        await _baseHandler.PopulateDefaultOptions(reqEnvelope, token);
        var reqXml = reqEnvelope.GetXML();
        var resp = await _baseHandler.SendRequestAsync(reqXml, "", token);
        var respEnv = XMLToObject.GetObjfromXml<ReportResponseEnvelope<T>>(resp.Response!, T.GetXMLAttributeOverides());
        return respEnv.Objects;
    }
    public async IAsyncEnumerable<T> GetObjectsAsyncNew<T>(BaseRequestOptions? options = null, [EnumeratorCancellation] CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject, new()
    {
        var reqEnvelope = T.GetRequestEnvelope();
        reqEnvelope.PopulateOptions(options);
        await _baseHandler.PopulateDefaultOptions(reqEnvelope, token);
        using var requestStream = new MemoryStream();
        await GenericXmlStreamer.WriteDataToStreamAsync(requestStream, reqEnvelope, new XmlSerializationOptions { Encoding = Encoding.Unicode });
        requestStream.Position = 0;
        using var responseStream = await _baseHandler.SendRequestAsStreamAsync(requestStream, "", token);

        using var streamReader = new StreamReader(responseStream, Encoding.Unicode);

        using var cleaner = new TallyResponseCleaner(streamReader);


        IEnumerator<T> enumerator = GenericXmlStreamer.ReadNestedListDataFromTextReader<T>(
            cleaner,
            ["ENVELOPE"]
        ).GetEnumerator();

        while (true)
        {
            T? item = default;
            try
            {
                if (!enumerator.MoveNext()) break;
                item = enumerator.Current;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while parsing XML response from Tally");
                throw;
            }
            yield return item;
        }
    }

    /// <inheritdoc/>
    public async Task<ulong> GetCountAsync<T>(BaseRequestOptions? options = null, CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject
    {
        const string objectCountName = "TC_ObjectsCount_Req";
        var reqEnvelope = T.GetCountRequestEnvelope();
        var tDLMessage = reqEnvelope.Body.Desc.TDL.TDLMessage;
        var report = tDLMessage.Report!.First();
        Part part = new(report.Name, null, objectCountName);
        tDLMessage.Part = [part];
        tDLMessage.Line = [new(objectCountName, [objectCountName])];
        Collection collection = tDLMessage.Collection.First();
        tDLMessage.Field = [new Field(objectCountName, "TC_TotalCount", $"$$NUMITEMS:{collection.Name}")];
        reqEnvelope.PopulateOptions(options);
        await _baseHandler.PopulateDefaultOptions(reqEnvelope, token);
        await using var requestStream = new MemoryStream();
        await GenericXmlStreamer.WriteDataToStreamAsync(requestStream, reqEnvelope, new XmlSerializationOptions { Encoding = Encoding.Unicode });
        var respStream = await _baseHandler.SendRequestAsStreamAsync(requestStream, "Count Request", token);
        var respEnv = GenericXmlStreamer.ReadDataFromStream<CountResponseEnvelope>(respStream);
        return respEnv?.TotalCount ?? 0;
    }
    /// <inheritdoc/>
    public async Task<PaginatedResponse<T>> GetObjectsAsync<T>(PaginatedRequestOptions options, CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject
    {
        var reqEnvelope = T.GetRequestEnvelope();
        reqEnvelope.PopulateOptions(options);
        await _baseHandler.PopulateDefaultOptions(reqEnvelope, token);
        var reqXml = reqEnvelope.GetXML();
        var resp = await _baseHandler.SendRequestAsync(reqXml, "Getting Objects", token);
        var respEnv = XMLToObject.GetObjfromXml<ReportResponseEnvelope<T>>(resp.Response!, T.GetXMLAttributeOverides());
        return new(respEnv.TotalCount ?? 0, options?.RecordsPerPage ?? 1000, respEnv.Objects, options?.PageNum ?? 1);
    }


    private static IEnumerable<TallyObjectDTO> GetDtos<T>(IEnumerable<T> objects) where T : BaseTallyObject, IBaseObject
    {
        foreach (var obj in objects)
        {
            yield return obj.ToDTO();
        }
    }

    public Task<List<PostResponse>> PostObjectsAsyncNew<T>(IEnumerable<T> objects,
                                          PostRequestOptions? options = null,
                                          CancellationToken token = default) where T : BaseTallyObject, IBaseObject => PostDTOObjectsAsyncNew(GetDtos(objects), options, token);



    public Task<List<PostResponse>> PostObjectsAsync<T>(IEnumerable<T> objects,
                                          PostRequestOptions? options = null,
                                          CancellationToken token = default) where T : BaseTallyObject, IBaseObject => PostDTOObjectsAsync(GetDtos(objects), options, token);
    public async Task<List<PostResponse>> PostDTOObjectsAsync<T>(IEnumerable<T> objects,
                                          PostRequestOptions? options = null,
                                          CancellationToken token = default) where T : TallyObjectDTO, IBaseObject
    {

        var postEnvelope = new RequestEnvelope();
        postEnvelope.AddCustomResponseReportForPost();
        postEnvelope.PopulateOptions(options);
        var sv = postEnvelope.Body.Desc.StaticVariables ?? new();
        bool stopatFirstError = options?.StopatFirstError ?? false;
        switch (_baseHandler.LicenseInfo.TallyShortVersion.MajorVersion)
        {
            case > 3 and <= 6:
                sv.ExtraVars.Add(new System.Xml.Linq.XElement("SVIMPBEHAVIOUREXCP", stopatFirstError ? "Stop Import at First Exception" : "Ignore Exceptions and Import"));
                break;
            default:
                break;
        }
        postEnvelope.Body.RequestData.Data ??= [];
        foreach (var obj in objects)
        {
            postEnvelope.Body.RequestData.Data.Add(obj);
        }
        var reqXml = postEnvelope.GetXML(GetPostXMLOverrides());
        var resp = await _baseHandler.SendRequestAsync(reqXml, "Posting Objects", token);
        var respEnvelope = XMLToObject.GetObjfromXml<PostResponseEnvelope>(resp.Response!);
        return respEnvelope.Objects;
    }
    public async Task<List<PostResponse>> PostDTOObjectsAsyncNew<T>(IEnumerable<T> objects,
                                      PostRequestOptions? options = null,
                                      CancellationToken token = default) where T : TallyObjectDTO, IBaseObject
    {
        var postEnvelope = new RequestEnvelope();
        postEnvelope.AddCustomResponseReportForPost();
        postEnvelope.PopulateOptions(options);

        var sv = postEnvelope.Body.Desc.StaticVariables ?? new();
        bool stopatFirstError = options?.StopatFirstError ?? false;
        switch (_baseHandler.LicenseInfo.TallyShortVersion.MajorVersion)
        {
            case > 3 and <= 6:
                sv.ExtraVars.Add(new System.Xml.Linq.XElement("SVIMPBEHAVIOUREXCP", stopatFirstError ? "Stop Import at First Exception" : "Ignore Exceptions and Import"));
                break;
            default:
                break;
        }

        postEnvelope.Body.RequestData.Data ??= [];
        foreach (var obj in objects)
        {
            postEnvelope.Body.RequestData.Data.Add(obj);
        }

        using var requestStream = new MemoryStream();
        await GenericXmlStreamer.WriteDataToStreamAsync(requestStream, postEnvelope, new XmlSerializationOptions { Encoding = Encoding.Unicode, IgnoreNullValues = true });
        requestStream.Position = 0;

        using var resp = await _baseHandler.SendRequestAsStreamAsync(requestStream, "Posting Objects", token);

        var respEnvelope = GenericXmlStreamer.ReadDataFromStream<PostResponseEnvelope>(resp);
        return respEnvelope?.Objects ?? [];
    }

    public virtual XMLOverrideswithTracking? GetPostXMLOverrides()
    {

        return null;
    }

}

