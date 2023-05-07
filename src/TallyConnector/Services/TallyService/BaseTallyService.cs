using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models.Service;

namespace TallyConnector.Services;

public interface IBaseTallyService
{
    Task<bool> CheckAsync(CancellationToken token = default);
    Task<TallyResult> SendRequestAsync(string? xml = null, string? requestType = null, CancellationToken token = default);
    void Setup(string url, int port);
}

/// <summary>
/// Base Tally Service that implements logic to send and receive requests from Tally
/// </summary>
public class BaseTallyService : IBaseTallyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger? _logger;
    private int _port;
    private readonly TLogger? Logger;
    private string _baseURL;

    protected string? Company { get; set; }
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
        //Check if schema exists in URL, if not exists add http://
        if (!baseURL.Contains("http") && !baseURL.Contains("https"))
        {
            baseURL = $"http://{baseURL}";
        }
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);
        _baseURL = baseURL;
        _port = port;
    }
    /// <summary>
    /// Intiaite Tally Service with httpclient , logger and timeoutMinutes
    /// </summary>
    /// <param name="httpClient">http client</param>
    /// <param name="logger">logger</param>
    /// <param name="timeoutMinutes">Request timeout in Minutes</param>
    public BaseTallyService(HttpClient httpClient,
                        ILogger<BaseTallyService>? logger = null,
                        int timeoutMinutes = 3)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);
        _baseURL = "http://localhost";
        _port = 9000;
        _logger = logger;
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
    public async Task<TallyResult> SendRequestAsync(string? xml = null,
                                                    string? requestType = null,
                                                    CancellationToken token = default)
    {

        TallyResult result = new();
        HttpRequestMessage requestMessage = new(HttpMethod.Post, FullURL);
        //Check whether xml is null or empty
        Logger?.LogTallyRequest(xml, requestType);
        if (xml != null && xml != string.Empty)
        {
            //Tally requires UTF-16/Unicode encoding
            requestMessage.Content = new StringContent(xml, Encoding.Unicode, "application/xml");
        }
        try
        {
            token.ThrowIfCancellationRequested();
            HttpResponseMessage tallyResponse = await _httpClient.SendAsync(requestMessage, token);

#if NET48
            string originalXML = await tallyResponse.Content.ReadAsStringAsync();
#else
            string originalXML = await tallyResponse.Content.ReadAsStringAsync(token);
#endif
            var resXml = ReplaceXMLText(originalXML);
            // If Status code is 200 
            if (tallyResponse.StatusCode == HttpStatusCode.OK)
            {
                //var resp = await tallyResponse.Content.ReadAsStreamAsync();
                //using StreamReader streamReader = new(resp, Encoding.Unicode);
                //resXml = streamReader.ReadToEnd();

                Logger?.LogTallyResponse(resXml, requestType);
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
            Logger?.TallyReqError(exc.Message);
            throw;
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
