using System.Net;
using System.Xml.Serialization;
using TallyConnector.Core.Converters.XMLConverterHelpers;
using TallyConnector.Core.Exceptions;

namespace TallyConnector.Services;
public partial class TallyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger? _logger;
    private readonly TLogger? Logger;
    private int _port;

    private string _baseURL;

    private BaseCompany? _company { get; set; }

    private string FullURL => _baseURL + ":" + _port;
    public TallyService()
    {
        _httpClient = new();
        _baseURL = "http://localhost";
        _port = 9000;
    }
    public TallyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseURL = "http://localhost";
        _port = 9000;
    }

    /// <summary>
    /// Intiate TallyService with Url and port
    /// Setup Http Client logger and httpClient Timeout Seconds
    /// </summary>
    /// <param name="baseURL"></param>
    /// <param name="port"></param>
    /// <param name="httpClient"></param>
    /// <param name="Logger"></param>
    /// <param name="timeoutMinutes"></param>
    public TallyService(string baseURL,
                        int port,
                        HttpClient httpClient,
                        ILogger<TallyService>? logger = null,
                        int timeoutMinutes = 1)
    {
        _httpClient = httpClient;
        //Check if schema exists in URL, if not exists add http://
        if (!baseURL.Contains("http") && !baseURL.Contains("https"))
        {
            baseURL = $"http://{baseURL}";
        }
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);
        _baseURL = baseURL;
        _port = port;
        _logger = logger;
        Logger = new(_logger);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="Logger"></param>
    /// <param name="timeoutMinutes"></param>
    public TallyService(HttpClient httpClient,
                        ILogger<TallyService>? logger = null,
                        int timeoutMinutes = 1)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);
        _baseURL = "http://localhost";
        _port = 9000;
        _logger = logger;
        Logger = new(_logger);
    }


    /// <summary>
    /// Checks whether Tally is running at given url and port
    /// </summary>
    /// <returns>true or false</returns>
    public async Task<bool> CheckAsync()
    {
        TallyResult tallyResult = await SendRequestAsync();
        if (tallyResult.Status == RespStatus.Sucess)
        {
            return true;
        }
        return false;
    }


    public async Task GetLicenseInfoAsync()
    {
        TallyResult tallyResult = await SendRequestAsync();
    }


    public async Task<TallyResult> PostObjectToTallyAsync<ObjType>(ObjType Object,
                                                              PostRequestOptions? postRequestOptions = null) where ObjType : TallyXmlJson, ITallyObject
    {
        Object.PrepareForExport();
        Envelope<ObjType> Objectenvelope = new(Object,
                                               new()
                                               {
                                                   SVCompany = postRequestOptions?.Company ?? _company?.Name
                                               });
        string ReqXml = Objectenvelope.GetXML(postRequestOptions?.XMLAttributeOverrides);
        TallyResult tallyResult = await SendRequestAsync(ReqXml);
        if (tallyResult.Status == RespStatus.Sucess)
        {
            return ParseResponse(tallyResult);
        }
        return new();
    }

    public async Task<ObjType> GetObjectAsync<ObjType>(string LookupValue,
                                                       MasterRequestOptions? requestOptions = null) where ObjType : TallyXmlJson, ITallyObject
    {
        // If received FetchList in collectionOptions we will use that else use default fetchlist
        requestOptions ??= new();
        requestOptions.FetchList ??= new() { "MasterId", "CanDelete", "*" };
        string filterformulae;
        if (requestOptions.LookupField is MasterLookupField.MasterId or MasterLookupField.AlterId)
        {
            filterformulae = $"${requestOptions.LookupField} = {LookupValue}";
        }
        else
        {
            filterformulae = $"${requestOptions.LookupField} = \"{LookupValue}\"";
        }
        List<Filter> filters = new() { new Filter() { FilterName = "Objfilter", FilterFormulae = filterformulae } };

        CollectionRequestOptions collectionRequestOptions = new() { FetchList = requestOptions.FetchList, Filters = filters };

        List<ObjType>? objects = await GetObjectsAsync<ObjType>(collectionRequestOptions);
        if (objects != null && objects.Count > 0)
        {
            return objects[0];
        }
        throw new ObjectDoesNotExist(typeof(ObjType).Name,
                                     requestOptions.LookupField.ToString(),
                                     LookupValue,
                                     _company?.Name!);

    }



    public async Task<ObjType> GetObjectAsync<ObjType>(string LookupValue,
                                                       VoucherRequestOptions? requestOptions = null) where ObjType : Voucher
    {
        // If received FetchList in collectionOptions we will use that else use default fetchlist
        requestOptions ??= new();
        requestOptions.FetchList ??=
                new List<string>()
                {
                    "MasterId", "*", "AllledgerEntries", "ledgerEntries", "Allinventoryenntries",
                    "InventoryEntries", "InventoryEntriesIn", "InventoryEntriesOut"
                };
        string filterformulae;
        if (requestOptions.LookupField is VoucherLookupField.MasterId or VoucherLookupField.AlterId)
        {
            filterformulae = $"${requestOptions.LookupField} = {LookupValue}";
        }
        else
        {
            filterformulae = $"${requestOptions.LookupField} = \"{LookupValue}\"";
        }
        List<Filter> filters = new() { new Filter() { FilterName = "Objfilter", FilterFormulae = filterformulae } };

        CollectionRequestOptions collectionRequestOptions = new() { FetchList = requestOptions.FetchList, Filters = filters };

        List<ObjType>? objects = await GetObjectsAsync<ObjType>(collectionRequestOptions);
        if (objects != null && objects.Count > 0)
        {
            return objects[0];
        }
        throw new ObjectDoesNotExist(typeof(ObjType).Name,
                                     requestOptions.LookupField.ToString(),
                                     LookupValue,
                                     _company?.Name!);

    }


    public async Task<List<ObjType>?> GetObjectsAsync<ObjType>(PaginatedRequestOptions? objectOptions = null) where ObjType : TallyXmlJson
    {
        //Gets Root attribute of ReturnObject
        XmlRootAttribute? RootAttribute = (XmlRootAttribute?)Attribute.GetCustomAttribute(typeof(ObjType), typeof(XmlRootAttribute));

        if (RootAttribute != null)
        {
            CollectionRequestOptions collectionOptions = new()
            {
                CollectionType = RootAttribute.ElementName,
                FromDate = objectOptions?.FromDate,
                ToDate = objectOptions?.ToDate,
                FetchList = objectOptions?.FetchList,
                Filters = objectOptions?.Filters,
                Compute = objectOptions?.Compute,
                ComputeVar = objectOptions?.ComputeVar,
                Pagination = objectOptions?.Pagination,
                XMLAttributeOverrides = objectOptions?.XMLAttributeOverrides,
            };
            var mapping = TallyObjectMapping.TallyObjectMappings
                    .FirstOrDefault(map => map.TallyMasterType.Equals(collectionOptions.CollectionType, StringComparison.OrdinalIgnoreCase));
            collectionOptions.Compute ??= new();
            if (mapping != null && mapping.ComputeFields != null)
            {
                collectionOptions.Compute.AddRange(mapping.ComputeFields);
            }

            if (collectionOptions.Pagination != null)
            {
                collectionOptions.Compute.Add("LineIndex : ##vLineIndex");
                collectionOptions.ComputeVar ??= new();
                collectionOptions.ComputeVar.Add("vLineIndex: Number : IF $$IsEmpty:##vLineIndex THEN 1 ELSE ##vLineIndex + 1");
                collectionOptions.Filters ??= new();
                collectionOptions.Filters.Add(new("Pagination", collectionOptions.Pagination.GetFilterFormulae()));
            }
            //Adding xmlelement name according to RootElement name of ReturnObject
            collectionOptions.XMLAttributeOverrides ??= new();
            XmlAttributes attrs = new();
            attrs.XmlElements.Add(new(collectionOptions.CollectionType));
            collectionOptions.XMLAttributeOverrides.Add(typeof(Colllection<ObjType>), "Objects", attrs);

            List<ObjType>? objects = await GetCustomCollectionAsync<ObjType>(collectionOptions);
            if (objects != null)
            {
                return objects;
            }
            else
            {
                return null;
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public async Task<List<ObjType>?> GetCustomCollectionAsync<ObjType>(CollectionRequestOptions collectionOptions) where ObjType : TallyXmlJson
    {
        StaticVariables staticVariables = new()
        {
            SVCompany = collectionOptions.Company ?? _company?.Name,
            SVFromDate = collectionOptions.FromDate ?? _company?.BooksFrom!,
            SVToDate = collectionOptions.ToDate ?? DateTime.Now,
        };
        string CollectionName = $"CUSTOM{collectionOptions.CollectionType.ToUpper()}COL";
        RequestEnvelope ColEnvelope = new(HType.Collection, CollectionName, staticVariables); //Collection Envelope

        ColEnvelope.Body.Desc.TDL.TDLMessage = new(colName: CollectionName,
                                                   colType: collectionOptions.CollectionType,
                                                   childof: collectionOptions.ChildOf,
                                                   nativeFields: collectionOptions.FetchList,
                                                   filters: collectionOptions.Filters,
                                                   computevar: collectionOptions.ComputeVar,
                                                   compute: collectionOptions.Compute,
                                                   collectionOptions.IsInitialize);

        string Reqxml = ColEnvelope.GetXML();

        var Response = await SendRequestAsync(Reqxml);
        if (Response.Status == RespStatus.Sucess && Response.Response != null)
        {
            try
            {
                Envelope<ObjType>? Envelope = XMLToObject.GetObjfromXml<Envelope<ObjType>>(Response.Response,
                                                                                           collectionOptions.XMLAttributeOverrides);
                return Envelope?.Body.Data.Collection?.Objects;

            }
            catch (Exception)
            {

                throw;
            }
        }
        else
        {
            return null;
        }

    }


    /// <summary>
    /// A helper function to send request to Tally
    /// </summary>
    /// <param name="xml">xml that is required to send</param>
    /// <returns></returns>
    /// <returns></returns>
    public async Task<TallyResult> SendRequestAsync(string? xml = null)
    {
        TallyResult result = new();
        HttpRequestMessage requestMessage = new(HttpMethod.Post, FullURL);
        //Check whether xml is null or empty
        if (xml != null && xml != string.Empty)
        {
            Logger?.LogTallyRequest(xml);
            //Tally requires UTF-16/Unicode encoding
            requestMessage.Content = new StringContent(xml, Encoding.Unicode, "application/xml");
        }
        try
        {
            HttpResponseMessage tallyResponse = await _httpClient.SendAsync(requestMessage);
            var resXml = await tallyResponse.Content.ReadAsStringAsync();
            // If Status code is 200 
            if (tallyResponse.StatusCode == HttpStatusCode.OK)
            {

                //var resp = await tallyResponse.Content.ReadAsStreamAsync();
                //using StreamReader streamReader = new(resp, Encoding.Unicode);
                //var resXml = streamReader.ReadToEnd();

                Logger?.LogTallyResponse(resXml);
                result.Status = RespStatus.Sucess;
                result.Response = resXml;
            }
            else
            {
                result.Status = RespStatus.Failure;
                result.Response = resXml;
            }
        }
        catch (Exception exc)
        {
            result.Status = RespStatus.Failure;
            result.Response = exc.Message;
            //TLogger.TallyReqError(exc.Message);
            //throw new TallyConnectivityException("Tally is not running", FullURL);
        }

        return result;
    }


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
}
