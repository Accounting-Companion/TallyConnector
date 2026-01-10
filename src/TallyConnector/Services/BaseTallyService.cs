using System.Diagnostics;
using System.Globalization;
using TallyConnector.Core;
using TallyConnector.Core.Extensions;
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
        try
        {
            
            TallyResult tallyResult = await SendRequestAsync(requestType: "Test Request",
                                                             token: token);
            if (tallyResult.Status == RespStatus.Sucess)
            {
                return true;
            }
        }
        catch (TallyConnectivityException ex)
        {
            _logger?.LogError(ex, "Tally connectivity error");
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
            RequestEnvelope? Envelope = XMLToObject.GetObjfromXml<RequestEnvelope>(tallyResult.Response, null, _logger);
            string result = Envelope?.Body?.RequestData?.FuncResult ?? throw new Exception($"No Active Company in Tally,Either Select any non group company in tally or Use {nameof(SetCompany)} method to set company");
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
        var respXml = await SendRequestAsync(reqXml, reqType, token).ConfigureAwait(false);

        var XMLAttributeOverrides = new XMLOverrideswithTracking().AddCollectionArrayItemAttributeOverrides(objectName.ToUpper(), typeof(LicenseInfo));
        RequestEnvelope envelope = XMLToObject.GetObjfromXml<RequestEnvelope>(respXml.Response ?? throw new Exception("Error While Getting License"), XMLAttributeOverrides);
        var data = envelope.Body.RequestData.Data;
        if (data != null && data is [LicenseInfo licenseInfo])
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
            HttpResponseMessage tallyResponse = await _httpClient.SendAsync(requestMessage, token).ConfigureAwait(false);
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
    public async Task<Stream> SendRequestAsStreamAsync(Stream requestStream, string? requestType = null, CancellationToken token = default)
    {
        Activity.Current?.AddTag("Tally URL", FullURL);
        HttpRequestMessage requestMessage = new(HttpMethod.Post, FullURL);

        if (requestType != null)
        {
            LogRequestType(requestType);
        }

        if (requestStream != null && requestStream.Length > 0)
        {
#if DEBUG
            if (requestStream.CanSeek)
            {

                requestStream.Position = 0;
                using StreamReader reader = new(requestStream, Encoding.Unicode, true, 1024, true);
                var xml = await reader.ReadToEndAsync();
                _logger?.LogDebug("Tally Request: {xml}", xml);
                requestStream.Position = 0;

            }
#endif
            var streamContent = new StreamContent(requestStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml") { CharSet = "utf-16" };
            requestMessage.Content = streamContent;
        }

        try
        {
            Activity.Current?.AddEvent(new ActivityEvent("Sending Request"));
            HttpResponseMessage tallyResponse = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            Activity.Current?.AddEvent(new ActivityEvent("Received Response"));

            if (tallyResponse.StatusCode == HttpStatusCode.OK)
            {
#if DEBUG
                var stream = await tallyResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return new LoggingStream(stream, _logger);
#else
                return await tallyResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
            }
            else
            {
                var errorXml = await tallyResponse.Content.ReadAsStringAsync();
                throw new Exception($"Tally returned failure: {tallyResponse.StatusCode}. Response: {errorXml}");
            }
        }
        catch (HttpRequestException exc)
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error, $"Tally is not running on {FullURL}");
            throw new TallyConnectivityException("Tally is not running", FullURL, exc);
        }
        catch (TaskCanceledException)
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error, $"Task Cancelled");
            throw;
        }
        catch (OperationCanceledException)
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error, $"Operation Cancelled");
            throw;
        }
        catch (Exception exc)
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error, exc.Message);
            throw;
        }
    }

#if DEBUG
    private class LoggingStream : Stream
    {
        private readonly Stream _inner;
        private readonly ILogger? _logger;

        public LoggingStream(Stream inner, ILogger? logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public override bool CanRead => _inner.CanRead;

        public override bool CanSeek => _inner.CanSeek;

        public override bool CanWrite => _inner.CanWrite;

        public override long Length => _inner.Length;

        public override long Position { get => _inner.Position; set => _inner.Position = value; }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = _inner.Read(buffer, offset, count);
            Log(buffer, offset, read);
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var read = await _inner.ReadAsync(buffer, offset, count, cancellationToken);
            Log(buffer, offset, read);
            return read;
        }

        private void Log(byte[] buffer, int offset, int count)
        {
            if (count > 0 && _logger != null)
            {
                try
                {
                    var chunk = Encoding.Unicode.GetString(buffer, offset, count);
                    _logger.LogDebug("Response Chunk: {chunk}", chunk);
                }
                catch { }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
        }
    }
#endif

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
                var AsciiCode = Convert.ToUInt32(innerText, 16);
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



    public static DateTime GetToDate(DateTime now)
    {
        return new DateTime(now.Month > 3 ? now.Year + 1 : now.Year, 3, 31);
    }
    public static DateTime GetToDate()
    {
        return GetToDate(DateTime.Now);
    }
}

