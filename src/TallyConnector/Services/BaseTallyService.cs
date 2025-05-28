using System.Diagnostics;
using System.Globalization;
using TallyConnector.Core;
using TallyConnector.Core.Models.Interfaces;
using TallyConnector.Core.Models.Response;

namespace TallyConnector.Services;
/// <summary>
/// Base Tally Service
/// </summary>
public partial class BaseTallyService : IBaseTallyService
{
    private readonly HttpClient _httpClient;
    private int _port;

    private string _baseURL;
    private string FullURL => _baseURL + ":" + _port;

    protected readonly ILogger _logger;

    private LicenseInfo? _licenseInfo { get; set; }
    public LicenseInfo LicenseInfo
    {
        get
        {
            if (_licenseInfo == null)
            {
                try
                {
                    _licenseInfo = GetLicenseInfoAsync().Result;
                }
                finally
                {

                }
            }
            return _licenseInfo;
        }
    }
    protected ICompany? Company { get; set; }

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
    /// Intiaite Tally Service with httpclient , _logger and timeoutMinutes
    /// </summary>
    /// <param name="httpClient">http client</param>
    /// <param name="logger">_logger</param>
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
            RequestEnvelope<string>? Envelope = XMLToObject.GetObjfromXml<RequestEnvelope<string>>(tallyResult.Response, null, _logger);
            string result = Envelope?.Body?.Data?.FuncResult ?? throw new Exception($"No Active Company in Tally,Either Select any non group company in tally or Use {nameof(SetCompany)} method to set company");
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
                $"TALLYSHORTVERSION  :@@VersionReleaseString",
                $"IsTallyPrime:$$TC_GetBooleanFromLogicField:$$IsProdTallyPrime",
                $"IsTallyPrimeEditLog:$$TC_GetBooleanFromLogicField:$$IsProdTallyPrimeEL",
                $"IsTallyPrimeServer:$$TC_GetBooleanFromLogicField:$$IsProdTallyServer",
            ])];
        var reqXml = reqEnvelope.GetXML();
        var respXml = await SendRequestAsync(reqXml, reqType, token);
        var XMLAttributeOverrides = new XmlAttributeOverrides();
        var XMLAttributes = new XmlAttributes();
        XMLAttributes.XmlElements.Add(new(objectName.ToUpper()));
        XMLAttributeOverrides.Add(typeof(Colllection<LicenseInfo>), "Objects", XMLAttributes);
        RequestEnvelope<LicenseInfo> envelope = XMLToObject.GetObjfromXml<RequestEnvelope<LicenseInfo>>(respXml.Response ?? throw new Exception("Error While Getting License"), XMLAttributeOverrides);
        LicenseInfo? licenseInfo = envelope.Body.Data.Collection?.Objects?.FirstOrDefault();
        if (licenseInfo != null)
        {
            _licenseInfo = licenseInfo;
        }
        else
        {
            throw new Exception("No License info received from Tally");
        }
        return _licenseInfo;
    }



    /// <inheritdoc/>
    public void Setup(string url,
                      int port)
    {
        if (!url.Contains("http") && !url.Contains("https"))
        {
            url = $"http://{url}";
        }
        // if url or port is changed we remove old LicenseInfo
        if (url != _baseURL || port != _port) { _licenseInfo = null; }
        _baseURL = url;
        _port = port;

    }


    /// <inheritdoc/>
    public void SetCompany(ICompany company)
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
        if (xml != null)
        {
            LogRequestXML(xml);
        }
        if (requestType != null)
        {
            LogRequestType(requestType);
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
    /// Default functions used in Request RequestEnvelope XML
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
    public async Task PopulateDefaultOptions(RequestEnvelope requestEnvelope, CancellationToken token = default)
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


    public static void AddCustomResponseReportForPost<T>(PostRequestEnvelope<T> requestEnvelope) where T : class
    {
        var tDLMessage = requestEnvelope.Body.Desc.TDL.TDLMessage;

        const string TDLVarName = "CCTotalMisMatch";
        const string TDLObjectTypeVarName = "VTMark";
        const string RemoteIdVarName = "VchType";
        const string guidVarName = "VchDate";
        const string MasterIdVarName = "VchMID";
        const string ActionVarName = "CCCatName";
        const string NameVarName = "CCLedName";
        const string errorvarName = "VchNumber";


        const string customReportName = "TC_CustomReportAndEvents";
        const string collectionName = "TC_CustResultsColl";
        const string importStartFunctionName = "TC_OnImportStart";
        const string importObjectFunctionName = "TC_BeforeImportObject";
        const string afterImportObjectFunctionName = "TC_AfterImportObject";
        const string importEndFunctionName = "TC_OnImportEnd";

        const string objectTypeFieldName = "TC_ObjectTypeField";
        const string nameFieldName = "TC_NameField";
        const string masterIdFieldName = "TC_MasterIdField";
        const string guidFieldName = "TC_guidField";
        const string remoteIdFieldName = "TC_RemoteIdField";
        const string actionTypeFieldName = "TC_ActionTypeField";
        const string respMessageFieldName = "TC_RespMsgField";


        const string onKeyword = "On";
        tDLMessage.ImportFile = [new("ALL MASTERS", [customReportName+":Yes"]){ IsModify=YesNo.Yes},
            new(customReportName)
            {
                IsOption = YesNo.Yes,
                ResponseReport = customReportName,
                Delete =[onKeyword],
                Add =
                [
                    $"{onKeyword} : Start Import : Yes : Call : {importStartFunctionName}",
                    $"{onKeyword} : Import Object : Yes : Call : {importObjectFunctionName}",
                    $"{onKeyword} : Import Object : Yes :  Import Object ",
                    $"{onKeyword} : After Import Object  : Yes : Call : {afterImportObjectFunctionName}",
                    $"{onKeyword} : End Import : Yes : Call : {importEndFunctionName}",
                ]
            }];
        tDLMessage.Report = [new(customReportName)];
        tDLMessage.Form = [new(customReportName) { ReportTag = "RESULTS" }];
        tDLMessage.Part = [new(customReportName, collectionName)];
        tDLMessage.Line = [new(customReportName, [objectTypeFieldName, nameFieldName, masterIdFieldName, guidFieldName, remoteIdFieldName, actionTypeFieldName, respMessageFieldName], "RESULT")];
        tDLMessage.Field =
        [
            new(objectTypeFieldName, "ObjectType", $"${TDLObjectTypeVarName}"),
            new(nameFieldName, "Name", $"${NameVarName}"),
            new(masterIdFieldName, "MasterId", $"${MasterIdVarName}"),
            new(guidFieldName, "GUID", $"${guidVarName}"),
            new(remoteIdFieldName, "REMOTEID", $"${RemoteIdVarName}"),
            new(actionTypeFieldName, "ACTION", $"${ActionVarName}"),
            new(respMessageFieldName, "Error", $"${errorvarName}"),
        ];
        int ifcounter = 1;
        int actionCOunter = 2;
        string[] objectTypes = ["Group", "Ledger", "CostCategory"];
        var mastersObjectActions = objectTypes.SelectMany(c =>
        {
            List<string> actions = [$"TC_IF{ifcounter:00} : IF : ##TC_ObjecType=\"{c}\""];
            ifcounter++;
            actions.Add($"TC_C{actionCOunter:00} : Set Object   : ({c},$Name).");
            actionCOunter++;
            actions.Add($"TC_IF{ifcounter:00} : ENDIF");
            ifcounter++;
            return actions;
        });
        tDLMessage.Functions =
        [
            new(importStartFunctionName){ Actions=[$"01A    : LISTDELETE : {TDLVarName}"]},
            new(importObjectFunctionName){
                Variables =["TC_ObjecType : String:$$type"],
                Actions=
                [
                    $"TC00   : LISTADD   :{TDLVarName} :$REMOTEALTGUID:$REMOTEALTGUID:{RemoteIdVarName}",
                    $"TC01   : LISTADD   :{TDLVarName} :$REMOTEALTGUID:##TC_ObjecType:{TDLObjectTypeVarName}",
                ]},
            new(afterImportObjectFunctionName){
                Variables =["TC_ObjecType : String:$$type", "TC_Action : String:$$ImportAction"],
                Actions=
                [
                    $"TC_C00 : LISTADD : {TDLVarName}:$REMOTEALTGUID:##TC_Action:{ActionVarName}",
                    $"TC_C01 : LISTADD : {TDLVarName}:$REMOTEALTGUID:$$LastImportError:{errorvarName}",

                    ..mastersObjectActions,

                    $"TC_IF{ifcounter:00} : IF : ##TC_ObjecType=\"Voucher\"",
                    $"TC_C{actionCOunter:00} : Set Object   : (Voucher,$$LastCreatedVchId).",
                    $"TC_IF{ifcounter+1:00} : ENDIF",

                    $"TC_IF{ifcounter+2:00} : IF :  not $$IsEmpty:$MasterId",
                    $"TC_C50 : LISTADD : {TDLVarName}:$REMOTEALTGUID:$MasterId:{MasterIdVarName}",
                    $"TC_C51 : LISTADD : {TDLVarName}:$REMOTEALTGUID:$Name:{NameVarName}",
                    $"TC_C52 : LISTADD : {TDLVarName}:$REMOTEALTGUID:$GUID:{guidVarName}",
                     $"TC_IF{ifcounter+3:00} : ENDIF",

                ],
            },
            new(importEndFunctionName),
        ];
        tDLMessage.Collection = [new() { Name = collectionName, DataSource = $"Variable:{TDLVarName}" }];
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

