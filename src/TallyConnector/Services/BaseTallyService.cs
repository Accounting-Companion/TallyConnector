using System.Diagnostics;
using System.Globalization;
using TallyConnector.Core.Extensions;
using TallyConnector.Core.Models.Common;

namespace TallyConnector.Services;
/// <summary>
/// Base Tally Service
/// </summary>
[GenerateHelperMethod<MasterStatistics>(MethodNameSuffixPlural = nameof(MasterStatistics), GenerationMode = GenerationMode.GetMultiple, Args = [typeof(BaseRequestOptions)])]
[GenerateHelperMethod<VoucherStatistics>(MethodNameSuffixPlural = nameof(VoucherStatistics), GenerationMode = GenerationMode.GetMultiple, Args = [typeof(DateFilterRequestOptions)])]
[GenerateHelperMethod<Company>(MethodNameSuffixPlural = "Companies", GenerationMode = GenerationMode.GetMultiple)]
[GenerateHelperMethod<CompanyOnDisk>(MethodNameSuffixPlural = "GetCompaniesinDefaultPath", GenerationMode = GenerationMode.GetMultiple)]

[SetActivitySource(ActivitySource = nameof(BaseTallyServiceActivitySource))]
public partial class BaseTallyService : IBaseTallyService
{
    private readonly HttpClient _httpClient;
    private int _port;

    private string _baseURL;
    private string FullURL => _baseURL + ":" + _port;

    protected readonly ILogger _logger;

    protected BaseCompany? Company { get; set; }

#if NET7_0_OR_GREATER
    private static readonly System.Text.RegularExpressions.Regex _xmlTextRegex = GetXmlTextRegex();
    private static readonly System.Text.RegularExpressions.Regex _xmlAttributeRegex = GetXmlAttributeRegex();
    private static readonly System.Text.RegularExpressions.Regex _xmlTextAsciiRegex = GetXmlTextAsciiRegex();
    private static readonly System.Text.RegularExpressions.Regex _xmlTextHexRegex = GetXmlTextHexRegex();
#elif NET48_OR_GREATER || NET6_0_OR_GREATER
    private static readonly System.Text.RegularExpressions.Regex _xmlTextRegex = new(@"(?<=>)(?!<)((.|\n)*?)(?=<\/[^>]+>|<[^>]+>)");
    private static readonly System.Text.RegularExpressions.Regex _xmlAttributeRegex = new(@"(?<=="")([^""]*)(?="")");
    private static readonly System.Text.RegularExpressions.Regex _xmlTextAsciiRegex = new(@"[\x00-\x1F]");
    private static readonly System.Text.RegularExpressions.Regex _xmlTextHexRegex = new(@"(?<=&#x)(.*?)(?=;)");
#endif



    /// <summary>
    /// Intiate Tally Service with Default Parameters
    /// </summary>
    public BaseTallyService()
    {
        _httpClient = new();
        _baseURL = "http://localhost";
        _port = 9000;
        _httpClient.Timeout = TimeSpan.FromMinutes(3);
        _logger = NullLogger.Instance;
    }

    /// <summary>
    /// Intiaite Tally Service with Custom base url, port and timeoutMinutes
    /// </summary>
    /// <param name="baseURL">URL on which Tally is running</param>
    /// <param name="port">Port on which tally is running</param>
    /// <param name="timeoutMinutes">Request timeout in Minutes</param>
    public BaseTallyService(string baseURL,
                        int port,
                        double timeoutMinutes = 3)
    {
        _httpClient = new();
        //Check if schema exists in URL, if not exists add http://
        if (!baseURL.Contains("http") && !baseURL.Contains("https"))
        {
            baseURL = $"http://{baseURL}";
        }
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);
        _baseURL = baseURL;
        _port = port;
        _logger = NullLogger.Instance;
    }

    /// <summary>
    /// Intiaite Tally Service with httpclient , logger and timeoutMinutes
    /// </summary>
    /// <param name="httpClient">http client</param>
    /// <param name="logger">logger</param>
    /// <param name="timeoutMinutes">Request timeout in Minutes</param>
    public BaseTallyService(HttpClient httpClient,
                            ILogger? logger = null,
                            double timeoutMinutes = 3)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);
        _baseURL = "http://localhost";
        _port = 9000;
        _logger = logger ?? NullLogger.Instance;
    }
    /// <inheritdoc/>
    public async Task<bool> CheckAsync(CancellationToken token = default)
    {
        var reqType = "Test Request";
        using var activity = BaseTallyServiceActivitySource.StartActivity(reqType);
        TallyResult tallyResult = await SendRequestAsync(requestType: "Test Request",
                                                         token: token);
        if (tallyResult.Status == RespStatus.Sucess)
        {
            return true;
        }
        return false;
    }
    public async Task<string> GetActiveSimpleCompanyNameAsync(CancellationToken token = default)
    {
        const string RequestType = "Getting Active Simple Company Name";
        using var activity = BaseTallyServiceActivitySource.StartActivity(RequestType);
        RequestEnvelope requestEnvelope = new(HType.Function, "$$CurrentSimpleCompany");

        string Reqxml = requestEnvelope.GetXML();
        TallyResult tallyResult = await SendRequestAsync(Reqxml, RequestType, token);
        if (tallyResult.Status == RespStatus.Sucess && tallyResult.Response != null)
        {
            Envelope<string>? Envelope = XMLToObject.GetObjfromXml<Envelope<string>>(tallyResult.Response, null, _logger);
            string result = Envelope?.Body?.Data?.FuncResult?.Result ?? throw new Exception($"No Active Company in Tally,Either Select any non group company in tally or Use {nameof(SetCompany)} method to set company");
            return result;
        }
        else
        {
            throw new Exception(tallyResult.Response);
        }
    }
    /// <inheritdoc/>
    public async Task<LicenseInfo> GetLicenseInfoAsync(CancellationToken token = default)
    {
        var reqType = "Getting LicenseInfo";
        using var activity = BaseTallyServiceActivitySource.StartActivity(reqType);
        const string prefix = "TC_LicenseInfo";
        const string collectionName = $"{prefix}Collection";
        const string objectName = $"{prefix}Object";
        var reqEnvelope = new RequestEnvelope(HType.Collection, collectionName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Collection = [new Collection(collectionName, objectName)];
        tdlMsg.Functions = GetDefaultTDLFunctions();
        tdlMsg.Object = [new TallyCustomObject(objectName,
            [
                "SERIALNUMBER:$$LicenseInfo:SerialNumber",
                "REMOTESERIALNUMBER:$$LicenseInfo:RemoteSerialNumber",
                "ACCOUNTID:$$LicenseInfo:AccountID",
                "ADMINMAILID:$$LicenseInfo:AdminEmailID",
                $"ISADMIN:$${Constants.GetBooleanFromLogicFieldFunctionName}:$$LicenseInfo:IsAdmin",
                $"ISEDUCATIONALMODE:$${Constants.GetBooleanFromLogicFieldFunctionName}:$$LicenseInfo:IsEducationalMode",
                $"ISSILVER:$${Constants.GetBooleanFromLogicFieldFunctionName}:$$LicenseInfo:IsAdmin",
                $"ISGOLD:$${Constants.GetBooleanFromLogicFieldFunctionName}:$$LicenseInfo:IsAdmin",

                "PLANNAME:If $$LicenseInfo:IsEducationalMode Then \"Educational Version\" ELSE  If $$LicenseInfo:IsSilver Then \"Silver\" ELSE  If $$LicenseInfo:IsGold Then \"Gold\" else \"\"",
                $"ISINDIAN:$${Constants.GetBooleanFromLogicFieldFunctionName}:$$LicenseInfo:IsIndian",
                $"ISREMOTEACCESSMODE:$${Constants.GetBooleanFromLogicFieldFunctionName}:$$LicenseInfo:IsRemoteAccessMode",
                $"ISLICCLIENTMODE:$${Constants.GetBooleanFromLogicFieldFunctionName}:$$LicenseInfo:IsLicClientMode",
                "APPLICATIONPATH:$$SysInfo:ApplicationPath",
                "DATAPATH:##SVCurrentPath",
                "USERLEVEL:$$cmpuserlevel",
                "USERNAME:$$cmpusername",
                $"TALLYVERSION  :{Constants.License}",
            ])];
        var reqXml = reqEnvelope.GetXML();
        var respXml = await SendRequestAsync(reqXml, reqType, token);
        var XMLAttributeOverrides = new XmlAttributeOverrides();
        var XMLAttributes = new XmlAttributes();
        XMLAttributes.XmlElements.Add(new(objectName.ToUpper()));
        XMLAttributeOverrides.Add(typeof(Colllection<LicenseInfo>), "Objects", XMLAttributes);
        Envelope<LicenseInfo> envelope = XMLToObject.GetObjfromXml<Envelope<LicenseInfo>>(respXml.Response ?? throw new Exception("Error While Getting License"), XMLAttributeOverrides);
        return envelope.Body.Data.Collection?.Objects?.FirstOrDefault() ?? new();
    }

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
        await PopulateDefaultOptions(requestEnvelope, token);
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
            throw new Exception("Error hile getting Last AlterIds");
        }

    }

    
    public async Task<AutoVoucherStatisticsEnvelope> GetVoucherStatisticsAsync(AutoColumnReportPeriodRequestOprions requestOptions, CancellationToken token = default)
    {
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
        List<string> rootFields = [nameFieldName, totalCountFieldName, periodCountRootFieldName];
        Line line = new(reportName, rootFields, "VchTypeStat") { Repeat = [periodCountRootFieldName] };
        tDLMessage.Line = [line];
        tDLMessage.Field =
        [
            new(nameFieldName,"Name","$Name"),
            new(totalCountFieldName,"TotalCount","$TotalCount"),
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
                    "TotalPeriodCount :  $$DirectTotalVch:$Name",
                    "CancelledCount : $$DirectCancVch:$Name",
                    "OptionalCount :  $$DirectOptionalVch:$Name",
                    "TotalCount : $$DirectAllVch:$Name",
                ]
            }
        ];
        tDLMessage.Functions = GetDefaultTDLFunctions();
        await PopulateDefaultOptions(requestEnvelope, token);
        string requestXml = requestEnvelope.GetXML();
        string RequestType = $"VchType Auto Column Stats({periodicity}) of company - {sv.SVCompany} ({sv.SVFromDate} to {sv.SVToDate})";
        TallyResult tallyResult = await SendRequestAsync(requestXml, RequestType, token);
        if (tallyResult.Status == RespStatus.Sucess)
        {
            var k = XMLToObject.GetObjfromXml<AutoVoucherStatisticsEnvelope>(tallyResult.Response!);
            return k;
        }
        else throw new Exception(tallyResult.Response);
    }
    private static string GetPeriodictyString(AutoColumnReportPeriodRequestOprions? requestOptions)
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
    /// <inheritdoc/>
    public void Setup(string url,
                      int port)
    {
        if (!url.Contains("http") && !url.Contains("https"))
        {
            url = $"http://{url}";
        }
        _baseURL = url;
        _port = port;
    }


    /// <inheritdoc/>
    public void SetCompany(Company company)
    {
        Company = company;
    }


    /// <inheritdoc/>
    public async Task<TallyResult> SendRequestAsync(string? xml = null, string? requestType = null, CancellationToken token = default)
    {
        TallyResult result = new();
        Activity.Current?.AddTag("Tally URL", FullURL);
        HttpRequestMessage requestMessage = new(HttpMethod.Post, FullURL);
        //Check whether xml is null or empty
        if (xml != null && requestType != null)
        {
            LogRequestXML(xml, requestType);
        }
        if (xml != null && xml != string.Empty)
        {
            //Tally requires UTF-16/Unicode encoding
            requestMessage.Content = new StringContent(CleanRequestXML(xml), Encoding.Unicode, "application/xml");
        }
        try
        {
            Activity.Current?.AddEvent(new ActivityEvent("Sending Request"));
            HttpResponseMessage tallyResponse = await _httpClient.SendAsync(requestMessage, token);
            Activity.Current?.AddEvent(new ActivityEvent("Received Response"));
#if NET48
            var resXml = CleanResponseXML(await tallyResponse.Content.ReadAsStringAsync());
#else
            var resXml = CleanResponseXML(await tallyResponse.Content.ReadAsStringAsync(token));
#endif
            // If Status code is 200 
            if (tallyResponse.StatusCode == HttpStatusCode.OK)
            {
                //var resp = await tallyResponse.Content.ReadAsStreamAsync();
                //using StreamReader streamReader = new(resp, Encoding.Unicode);
                //resXml = streamReader.ReadToEnd();

                //Logger?.LogTallyResponse(resXml, requestType);
                //CheckTallyError(resXml);
                result.Status = RespStatus.Sucess;
                result.Response = resXml;
            }
            else
            {
                result.Status = RespStatus.Failure;
                result.Response = resXml;
            }
        }
        catch (HttpRequestException exc)
        {
            result.Response = exc.Message;
            Activity.Current?.SetStatus(ActivityStatusCode.Error, $"Tally is not running on {FullURL}");

            throw new TallyConnectivityException("Tally is not running", FullURL, exc);
        }
        catch (TaskCanceledException ex)
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error, $"Task Cancelled");
            _logger?.LogError(ex.Message);
            throw;
        }
        catch (OperationCanceledException)
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error, $"Operation Cancelled");
            throw;
        }
        catch (Exception exc)
        {
            result.Status = RespStatus.Failure;
            result.Response = exc.Message;
            Activity.Current?.SetStatus(ActivityStatusCode.Error, exc.Message);
            //Logger?.TallyReqError(exc.Message);
            throw;
        }
        return result;
    }

    /// <inheritdoc/>
    private static string CleanResponseXML(string Xml)
    {
        var reqType = "Cleaning Response XML";
        using var activity = BaseTallyServiceActivitySource.StartActivity(reqType);
        Xml = _xmlTextRegex.Replace(Xml, CleanXml);
        Xml = _xmlAttributeRegex.Replace(Xml, CleanXml);
        Xml = Xml.Replace("&#4; ", "");
        return Xml;
    }
    /// <inheritdoc/>
    private static string CleanRequestXML(string Xml)
    {
        var reqType = "Cleaning Request XML";
        using var activity = BaseTallyServiceActivitySource.StartActivity(reqType);

        Xml = _xmlTextRegex.Replace(Xml, CleanValue);
        Xml = _xmlAttributeRegex.Replace(Xml, CleanValue);
        Xml = Xml.Replace("&#x", "&#");
        return Xml;
    }

    private static string CleanValue(System.Text.RegularExpressions.Match c)
    {

        string value = c.Value;
        if (string.IsNullOrWhiteSpace(c.Value))
        {
            return value;
        }
        value = value.Replace("\n", "&#10;");
        value = value.Replace("\r", "&#13;");
        if (_xmlTextHexRegex.IsMatch(value))
        {
            string v = _xmlTextHexRegex.Replace(value, innerValue =>
            {
                var innerText = innerValue.Value;
                if (string.IsNullOrEmpty(innerText))
                {
                    return innerText;
                }
                var AsciiCode = System.Convert.ToUInt32(innerText, 16);
                return AsciiCode.ToString();
            });
            return v;
        }
        return value;

    }
    private static string CleanXml(System.Text.RegularExpressions.Match c)
    {
        string value = c.Value;
        if (string.IsNullOrWhiteSpace(c.Value))
        {
            return value;
        }
        value = value.Replace("\n", "&#10;");
        value = value.Replace("\r", "&#13;");
        if (_xmlTextAsciiRegex.IsMatch(value))
        {
            string v = _xmlTextAsciiRegex.Replace(value, innerValue =>
            {
                var innerText = innerValue.Value;
                if (string.IsNullOrEmpty(innerText))
                {
                    return innerText;
                }
                var AsciiCode = ((int)innerText[0]);
                return $"&#{AsciiCode};";
            });
            return v;
        }
        return value;

    }

    /// <inheritdoc/>
    public TallyResult ParseResponse(TallyResult tallyResult)
    {
        if (!tallyResult.Response!.Contains("RESPONSE"))
        {
            ResponseEnvelope? Resp = XMLToObject.GetObjfromXml<ResponseEnvelope>(tallyResult.Response!); //Response from tally on sucess

            if (Resp?.Body?.Data?.LineError != null)
            {
                tallyResult.Status = RespStatus.Failure;
                tallyResult.Response = Resp.Body.Data.LineError;

            }
            if (Resp?.Body?.Data?.ImportResult != null)
            {

                tallyResult.Status = RespStatus.Sucess;
                if (Resp.Body.Data.ImportResult.Created != 0)
                {
                    tallyResult.Response = "Created Sucessfully";
                }
                else if (Resp.Body.Data.ImportResult.Altered != 0)
                {
                    tallyResult.Response = "Altered Sucessfully";
                }
                else if (Resp.Body.Data.ImportResult.Deleted != 0)
                {
                    tallyResult.Response = "Deleted Sucessfully";
                }
                else if (Resp.Body.Data.ImportResult.Cacelled != 0)
                {
                    tallyResult.Response = "Cancelled Sucessfully";
                }
                else if (Resp.Body.Data.ImportResult.Combined != 0)
                {
                    tallyResult.Response = "Combined Sucessfully";
                }

                if (Resp.Body.Data.ImportResult.LastVchId != null && Resp.Body.Data.ImportResult.LastVchId != 0)
                {
                    tallyResult.Response += ", LastVchId - " + Resp.Body.Data.ImportResult.LastVchId.ToString(); //Returns VoucherMaster ID
                }
            }
        }
        else
        {
            FailureResponse? resp = XMLToObject.GetObjfromXml<FailureResponse>(tallyResult.Response!); //Response from tally on Failure
            tallyResult.Status = RespStatus.Failure;
            tallyResult.Response = resp?.ToString();
        }
        return tallyResult;
    }

    /// <inheritdoc/>
    public string? CheckTallyError(string ResXml)
    {
        if (ResXml.Contains("LINEERROR"))
        {
            FailureResponse? resp = XMLToObject.GetObjfromXml<FailureResponse>(ResXml);
            return resp?.ToString();
        }
        return null;
    }
    /// <summary>
    /// Default functions used in Request Envelope XML
    /// </summary>
    /// <returns></returns>
    public static List<TDLFunction> GetDefaultTDLFunctions()
    {
        List<TDLFunction> functions = [];
        functions.Add(new TDLFunction(Constants.GetBooleanFromLogicFieldFunctionName)
        {
            Parameters = ["val : Logical : None"],
            Returns = "String",
            Actions = [
                "000 :   If  : $$ISEmpty:##val",
                "001 :Return : ##val",
                "002 : Else    :",
                "003 : If  :  ##val ",
                "004 :Return :\"true\"",
                "005 : Else    :",
                "006 :Return : \"false\"",
                "007 : End If",
                "008 : End If",
            ]
        });
        functions.Add(new TDLFunction(Constants.TransformDateFunctionName)
        {
            Parameters = ["ParamInputDate   : Date"],
            Variables = [
                "ParamSeparator        : String : \"-\"",
                "TempVarYear           : String",
                "TempVarMonth          : String",
                "TempVarDate           : String",
            ],
            Returns = "String",
            Actions = [
                "01  : If        : NOT $$IsEmpty:##ParamInputDate",
                "02  :   Set     : TempVarYear       : $$Zerofill:($$YearofDate:##ParamInputDate):4",
                "03  :   Set     : TempVarMonth      : $$Zerofill:($$MonthofDate:##ParamInputDate):2",
                "04  :   Set     : TempVarDate       : $$Zerofill:($$DayofDate:##ParamInputDate):2",
                "05  :   Return  : $$String:##TempVarYear + $$String:##ParamSeparator + $$String:##TempVarMonth + $$String:##ParamSeparator + $$String:##TempVarDate",
                "06  : End If",
                "07  : Return    : \"\""
            ],

        });
        return functions;
    }

    public static string? GetTallyString<T>(T src)
    {
        return src switch
        {
            bool b => b ? "Yes" : "No",
            DateTime date => date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),

            _ => src?.ToString(),
        };
    }

#if NET7_0_OR_GREATER
    [System.Text.RegularExpressions.GeneratedRegex(@"(?<=>)(?!<)((.|\n)*?)(?=<\/[^>]+>|<[^>]+>)")]
    private static partial System.Text.RegularExpressions.Regex GetXmlTextRegex();
    [System.Text.RegularExpressions.GeneratedRegex(@"(?<=="")([^""]*)(?="")")]
    private static partial System.Text.RegularExpressions.Regex GetXmlAttributeRegex();
    [System.Text.RegularExpressions.GeneratedRegex(@"[\x00-\x1F]")]
    private static partial System.Text.RegularExpressions.Regex GetXmlTextAsciiRegex();
    [System.Text.RegularExpressions.GeneratedRegex(@"(?<=&#x)(.*?)(?=;)")]
    private static partial System.Text.RegularExpressions.Regex GetXmlTextHexRegex();
#endif
    protected async Task PopulateDefaultOptions(RequestEnvelope requestEnvelope, CancellationToken token = default)
    {
        using var activity = BaseTallyServiceActivitySource.StartActivity();
        var staticVariables = requestEnvelope.Body.Desc.StaticVariables ??= new();
        staticVariables.SVFromDate ??= Company?.StartingFrom;
        staticVariables.SVToDate ??= GetToDate();
        staticVariables.SVCompany ??= Company?.Name;
        // If company is not set , then we will fetch active simple company
        // this is required because if active company is group company we get memory access violation error from Tally
        staticVariables.SVCompany ??= await GetActiveSimpleCompanyNameAsync(token);
        if (staticVariables.SVCompany != null)
        {
            activity?.SetTag("SVCompany", staticVariables.SVCompany);
        }
        if (staticVariables.SVFromDate != null)
        {
            activity?.SetTag("SVFromDate", staticVariables.SVFromDate);
        }
        if (staticVariables.SVToDate != null)
        {
            activity?.SetTag("SVToDate", staticVariables.SVToDate);
        }
    }
    public static DateTime GetToDate(DateTime now)
    {
        return new DateTime(now.Month > 3 ? now.Year + 1 : now.Year, 3, 31);
    }
    public static DateTime GetToDate()
    {
        return GetToDate(DateTime.Now);
    }
}

