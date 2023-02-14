using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models.Interfaces;
using TallyConnector.Core.Models.Interfaces.Masters.Group;

namespace TallyConnector.Services;
/// <summary>
/// 
/// </summary>
public abstract class BaseTallyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger? _logger;
    private readonly TLogger? Logger;
    private int _port;

    private string _baseURL;

    protected abstract BaseCompany? Company { get; set; }

    private string FullURL => _baseURL + ":" + _port;
    /// <summary>
    /// Intiate Tally Service with Default Parameters
    /// </summary>
    public BaseTallyService()
    {
        _httpClient = new();
        _baseURL = "http://localhost";
        _port = 9000;
        _httpClient.Timeout = TimeSpan.FromMinutes(3);
    }
    /// <summary>
    /// Intiaite Tally Service with Custom base url, port and timeoutMinutes
    /// </summary>
    /// <param name="baseURL">URL on which Tally is running</param>
    /// <param name="port">Port on which tally is running</param>
    /// <param name="timeoutMinutes">Request timeout in Minutes</param>
    public BaseTallyService(string baseURL,
                        int port,
                        int timeoutMinutes = 3)
    {
        _httpClient = new();
        Setup(baseURL, port);
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);

    }

    /// <summary>
    /// Intiaite Tally Service with httpclient , logger and timeoutMinutes
    /// </summary>
    /// <param name="httpClient">http client</param>
    /// <param name="logger">logger</param>
    /// <param name="timeoutMinutes">Request timeout in Minutes</param>
    public BaseTallyService(HttpClient httpClient,
                        ILogger<TallyService>? logger = null,
                        int timeoutMinutes = 3)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);
        Setup("http://localhost", 9000);
        _logger = logger;
        Logger = new(_logger);
    }



    public List<T> GetData<T>() where T : TallyObject
    {
        // T.FetchList();
        return new();
    } 


    /// <inheritdoc/>
    public async Task<bool> CheckAsync()
    {
        TallyResult tallyResult = await SendRequestAsync();
        if (tallyResult.Status == RespStatus.Sucess)
        {
            return true;
        }
        return false;
    }

    public void Setup(string url,
                      int port)
    {
        //Check if schema exists in URL, if not exists add http://
        if (!url.Contains("http") && !url.Contains("https"))
        {
            url = $"http://{url}";
        }
        _baseURL = url;
        _port = port;
    }
    /// <inheritdoc/>
    public async Task<TallyResult> SendRequestAsync(string? xml = null)
    {
        TallyResult result = new();
        HttpRequestMessage requestMessage = new(HttpMethod.Post, FullURL);
        //Check whether xml is null or empty
        Logger?.LogTallyRequest(xml);
        if (xml != null && xml != string.Empty)
        {
            //Tally requires UTF-16/Unicode encoding
            requestMessage.Content = new StringContent(xml, Encoding.Unicode, "application/xml");
        }
        try
        {
            HttpResponseMessage tallyResponse = await _httpClient.SendAsync(requestMessage);
            var resXml = ReplaceXMLText(await tallyResponse.Content.ReadAsStringAsync());
            // If Status code is 200 
            if (tallyResponse.StatusCode == HttpStatusCode.OK)
            {
                //var resp = await tallyResponse.Content.ReadAsStreamAsync();
                //using StreamReader streamReader = new(resp, Encoding.Unicode);
                //resXml = streamReader.ReadToEnd();

                Logger?.LogTallyResponse(resXml);
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
            throw new TallyConnectivityException("Tally is not running", FullURL);
        }
        catch (Exception exc)
        {
            result.Status = RespStatus.Failure;
            result.Response = exc.Message;
            Logger?.TallyReqError(exc.Message);
            //throw new TallyConnectivityException("Tally is not running", FullURL);
        }
        return result;
    }
    /// <inheritdoc/>
    public static string ReplaceXMLText(string Xml)
    {
        Xml = Xml.Replace("&#4; ", "");
        Xml = Xml.Replace("0x20B9", "");
        return Xml;
    }

}
