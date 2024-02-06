using Microsoft.Extensions.Logging;
using System.Globalization;

namespace TallyConnector.Services;
/// <summary>
/// Base Tally Service
/// </summary>
public partial class BaseTallyService : Services.IBaseTallyService
{
    private readonly HttpClient _httpClient;
    private int _port;

    private string _baseURL;
    private string FullURL => _baseURL + ":" + _port;

    protected readonly ILogger _logger;

    protected BaseCompany? Company { get; set; }




#if NET7_0_OR_GREATER
    private static System.Text.RegularExpressions.Regex XmlTextRegex = GetXmlTextRegex();
    private static System.Text.RegularExpressions.Regex XmlAttributeRegex = GetXmlAttributeRegex();
    private static System.Text.RegularExpressions.Regex XmlTextAsciiRegex = GetXmlTextAsciiRegex();
    private static System.Text.RegularExpressions.Regex XmlTextHexRegex = GetXmlTextHexRegex();
#elif NET48_OR_GREATER || NET6_0_OR_GREATER
    private static System.Text.RegularExpressions.Regex XmlTextRegex = new(@"(?<=>)(?!<)((.|\n)*?)(?=<\/[^>]+>|<[^>]+>)");
    private static System.Text.RegularExpressions.Regex XmlAttributeRegex = new(@"(?<=="")([^""]*)(?="")");
    private static System.Text.RegularExpressions.Regex XmlTextAsciiRegex = new(@"[\x00-\x1F]");
    private static System.Text.RegularExpressions.Regex XmlTextHexRegex = new(@"(?<=&#x)(.*?)(?=;)");
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
        TallyResult tallyResult = await SendRequestAsync(requestType: "Test Request",
                                                         token: token);
        if (tallyResult.Status == RespStatus.Sucess)
        {
            return true;
        }
        return false;
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
            HttpResponseMessage tallyResponse = await _httpClient.SendAsync(requestMessage, token);
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
            throw new TallyConnectivityException("Tally is not running", FullURL, exc);
        }
        catch (TaskCanceledException ex)
        {
            _logger?.LogError(ex.Message);
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exc)
        {
            result.Status = RespStatus.Failure;
            result.Response = exc.Message;
            //Logger?.TallyReqError(exc.Message);
            throw;
        }
        return result;
    }

    /// <inheritdoc/>
    private static string CleanResponseXML(string Xml)
    {
        Xml = XmlTextRegex.Replace(Xml, CleanXml);
        Xml = XmlAttributeRegex.Replace(Xml, CleanXml);
        Xml = Xml.Replace("&#4; ", "");
        return Xml;
    }
    /// <inheritdoc/>
    private static string CleanRequestXML(string Xml)
    {
        Xml = XmlTextRegex.Replace(Xml, CleanValue);
        Xml = XmlAttributeRegex.Replace(Xml, CleanValue);
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
        if (XmlTextHexRegex.IsMatch(value))
        {
            string v = XmlTextHexRegex.Replace(value, innerValue =>
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
        if (XmlTextAsciiRegex.IsMatch(value))
        {
            string v = XmlTextAsciiRegex.Replace(value, innerValue =>
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

}

