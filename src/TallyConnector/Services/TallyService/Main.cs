using System.Collections.Concurrent;
using System.Data;

namespace TallyConnector.Services;
public partial class TallyService : ITallyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger? _logger;
    private readonly TLogger? Logger;
    private int _port;

    private string _baseURL;

    private BaseCompany? Company { get; set; }

    private string FullURL => _baseURL + ":" + _port;
    /// <summary>
    /// Intiate Tally Service with Default Parameters
    /// </summary>
    public TallyService()
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
    public TallyService(string baseURL,
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
    public TallyService(HttpClient httpClient,
                        ILogger<TallyService>? logger = null,
                        int timeoutMinutes = 3)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);
        _baseURL = "http://localhost";
        _port = 9000;
        _logger = logger;
        Logger = new(_logger);
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
    public async Task<LicenseInfo?> GetLicenseInfoAsync()
    {
        var LicenseInfo = await GetTDLReportAsync<LicenseInfo, LicenseInfo>();
        return LicenseInfo;
    }

    /// <inheritdoc/>
    public async Task<TallyResult> PostObjectToTallyAsync<ObjType>(ObjType Object,
                                                                   PostRequestOptions? postRequestOptions = null) where ObjType : TallyXmlJson, ITallyObject
    {
        Object.PrepareForExport();
        Envelope<ObjType> Objectenvelope = new(Object,
                                               new()
                                               {
                                                   SVCompany = postRequestOptions?.Company ?? Company?.Name
                                               });
        string ReqXml = Objectenvelope.GetXML(postRequestOptions?.XMLAttributeOverrides);
        TallyResult tallyResult = await SendRequestAsync(ReqXml);
        if (tallyResult.Status == RespStatus.Sucess)
        {
            return ParseResponse(tallyResult);
        }
        return tallyResult;
    }
    /// <inheritdoc/>
    public async Task<ObjType> GetObjectAsync<ObjType>(string lookupValue,
                                                       MasterRequestOptions? requestOptions = null) where ObjType : TallyBaseObject, INamedTallyObject
    {
        // If received FetchList in collectionOptions we will use that else use default fetchlist
        requestOptions ??= new();
        requestOptions.FetchList ??= new() { "MasterId", "CanDelete", "*" };
        string filterformulae;
        if (requestOptions.LookupField is MasterLookupField.MasterId or MasterLookupField.AlterId)
        {
            filterformulae = $"${requestOptions.LookupField} = {lookupValue}";
        }
        else
        {
            filterformulae = $"${requestOptions.LookupField} = \"{lookupValue}\"";
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
                                     lookupValue,
                                     Company?.Name!);

    }


    /// <inheritdoc/>
    public async Task<ObjType> GetObjectAsync<ObjType>(string lookupValue,
                                                       VoucherRequestOptions? requestOptions = null) where ObjType : Voucher
    {
        // If received FetchList in collectionOptions we will use that else use default fetchlist
        requestOptions ??= new();
        requestOptions.FetchList ??=
                new List<string>()
                {
                    "MasterId", "*",
                };
        string filterformulae;
        if (requestOptions.LookupField is VoucherLookupField.MasterId or VoucherLookupField.AlterId)
        {
            filterformulae = $"${requestOptions.LookupField} = {lookupValue}";
        }
        else
        {
            filterformulae = $"${requestOptions.LookupField} = \"{lookupValue}\"";
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
                                     lookupValue,
                                     Company?.Name!);

    }

    /// <inheritdoc/>
    public async Task<List<ObjType>?> GetObjectsAsync<ObjType>(PaginatedRequestOptions? objectOptions = null) where ObjType : TallyBaseObject
    {
        //Gets Root attribute of ReturnObject
        string? RootElemet = AttributeHelper.GetXmlRootElement(typeof(ObjType), _logger);

        Logger?.BuildingOptions(typeof(CollectionRequestOptions));

        CollectionRequestOptions collectionOptions = new()
        {
            CollectionType = RootElemet ?? typeof(ObjType).Name,
            FromDate = objectOptions?.FromDate,
            ToDate = objectOptions?.ToDate,
            FetchList = (objectOptions?.FetchList) != null ? new(objectOptions.FetchList) : null,
            Filters = (objectOptions?.Filters) != null ? new(objectOptions.Filters) : null,
            Compute = (objectOptions?.Compute) != null ? new(objectOptions.Compute) : null,
            ComputeVar = (objectOptions?.ComputeVar) != null ? new(objectOptions.ComputeVar) : null,
            Pagination = objectOptions?.Pagination,
            XMLAttributeOverrides = objectOptions?.XMLAttributeOverrides,
            IsInitialize = objectOptions?.IsInitialize ?? YesNo.No,
        };
        var mapping = Mappings.TallyObjectMappings
                .FirstOrDefault(map => map.TallyMasterType.Equals(collectionOptions.CollectionType, StringComparison.OrdinalIgnoreCase));
        collectionOptions.Compute ??= new();
        collectionOptions.Filters ??= new();
        if (mapping != null)
        {
            if (mapping.ComputeFields != null)
            {
                collectionOptions.Compute.AddRange(mapping.ComputeFields);
            }
            if (mapping.Filters != null)
            {
                collectionOptions.Filters.AddRange(mapping.Filters);
            }
            if (mapping.Objects != null)
            {
                collectionOptions.Objects = mapping.Objects;
            }
        }
        //Adding xmlelement name according to RootElement name of ReturnObject
        collectionOptions.XMLAttributeOverrides ??= new();
        XmlAttributes attrs = new();
        attrs.XmlElements.Add(new(collectionOptions.CollectionType));
        collectionOptions.XMLAttributeOverrides.Add(typeof(Colllection<ObjType>), "Objects", attrs);

        var objects = await GetCustomCollectionAsync<ObjType>(collectionOptions);
        objects?.ForEach(obj =>
        {
            obj.RemoveNullChilds();
            if (obj is IAliasTallyObject AliasObj)
            {
                AliasObj.Alias = AliasObj.LanguageNameList?.First()?.LanguageAlias;
            }

        });

        return objects;

    }

    /// <inheritdoc/>
    public async Task<List<ObjType>> GetAllObjectsAsync<ObjType>(RequestOptions? objectOptions = null,
                                                                 IProgress<ReportProgressHelper>? progress = null) where ObjType : TallyBaseObject
    {
        XmlRootAttribute? RootAttribute = (XmlRootAttribute?)Attribute.GetCustomAttribute(typeof(ObjType), typeof(XmlRootAttribute));

        CollectionRequestOptions collectionOptions = new()
        {
            CollectionType = RootAttribute?.ElementName ?? typeof(ObjType).Name,
            FromDate = objectOptions?.FromDate,
            ToDate = objectOptions?.ToDate,
            FetchList = (objectOptions?.FetchList) != null ? new(objectOptions.FetchList) : null,
            Compute = (objectOptions?.Compute) != null ? new(objectOptions.Compute) : null,
            ComputeVar = (objectOptions?.ComputeVar) != null ? new(objectOptions.ComputeVar) : null,
            XMLAttributeOverrides = objectOptions?.XMLAttributeOverrides,
            Filters = objectOptions?.Filters,
            IsInitialize = YesNo.Yes,
        };

        var mapping = Mappings.TallyObjectMappings
                .FirstOrDefault(map => map.TallyMasterType.Equals(collectionOptions.CollectionType, StringComparison.OrdinalIgnoreCase));

        collectionOptions.Filters ??= new();
        if (mapping != null)
        {
            if (mapping.Filters != null)
            {
                collectionOptions.Filters.AddRange(mapping.Filters);
            }
        }
        int? TotalCount = await GetObjectCountAync(new()
        {
            CollectionType = collectionOptions.CollectionType,
            FromDate = collectionOptions.FromDate,
            ToDate = collectionOptions.ToDate,
            Filters = collectionOptions.Filters,
        });
        Pagination pagination = new(TotalCount ?? 0, mapping?.DefaultPaginateCount ?? 1000);
        ConcurrentBag<ObjType> objects = new();
        List<Task> tasks = new();
        for (int i = 0; i < pagination.TotalPages; i++)
        {

            Pagination tpagination = new(TotalCount ?? 0, mapping?.DefaultPaginateCount ?? 1000, i + 1);
            _logger?.LogInformation("getting {type} from {start} to {end} (Page {cur} of {Total})",
                                    mapping!.MasterType,
                                    tpagination.Start,
                                    tpagination.End,
                                    tpagination.PageNum,
                                    tpagination.TotalPages);
            var options = new PaginatedRequestOptions()
            {
                FromDate = objectOptions?.FromDate,
                ToDate = objectOptions?.ToDate,
                FetchList = objectOptions?.FetchList,
                Filters = objectOptions?.Filters,
                Compute = objectOptions?.Compute,
                ComputeVar = objectOptions?.ComputeVar,
                Pagination = tpagination,
                XMLAttributeOverrides = objectOptions?.XMLAttributeOverrides,
                IsInitialize = objectOptions?.IsInitialize ?? YesNo.No,
            };
            var tempobjects = await GetObjectsAsync<ObjType>(options);
            if (tempobjects != null)
            {
                tempobjects.AsParallel().ForAll(t => objects.Add(t));
            }
            progress?.Report(new(tpagination.TotalCount, tpagination.End - tpagination.Start, tpagination.End));
        }
        //await Task.WhenAll(tasks.ToArray());
        return objects.ToList();
    }

    /// <inheritdoc/>
    public async Task<int?> GetObjectCountAync(CountRequestOptions options)
    {
        RequestEnvelope requestEnvelope = new(HType.Function, "$$NUMITEMS", new()
        {
            SVFromDate = options.FromDate,
            SVToDate = options.ToDate ?? DateTime.Now,
            SVCompany = options.Company ?? Company?.Name,
        });
        string CollectionName = $"CUSTOM{options.CollectionType.ToUpper()}COL";
        requestEnvelope.Body.Desc.FunctionParams = new() { Param = new() { CollectionName } };
        requestEnvelope.Body.Desc.TDL.TDLMessage.Collection.Add(new(CollectionName,
                                                                    options.CollectionType,
                                                                    filters: options.Filters?.Select(c => c.FilterName).ToList()!));


        options.Filters?.ForEach(filter => requestEnvelope.Body.Desc.TDL.TDLMessage.System?.Add(new(name: filter.FilterName!,
                                                 text: filter.FilterFormulae!)));

        string Reqxml = requestEnvelope.GetXML();
        var Response = await SendRequestAsync(Reqxml);
        if (Response.Status == RespStatus.Sucess && Response.Response != null)
        {
            Envelope<string>? Envelope = XMLToObject.GetObjfromXml<Envelope<string>>(Response.Response, null, _logger);
            return int.Parse(Envelope?.Body.Data.FuncResult.Result);
        }
        return 0;
    }

    /// <inheritdoc/>
    public async Task<List<ObjType>?> GetCustomCollectionAsync<ObjType>(CollectionRequestOptions collectionOptions) where ObjType : TallyBaseObject
    {
        string Reqxml = GenerateCollectionXML(collectionOptions);

        var Response = await SendRequestAsync(Reqxml);
        if (Response.Status == RespStatus.Sucess && Response.Response != null)
        {
            try
            {
                Envelope<ObjType>? Envelope = XMLToObject.GetObjfromXml<Envelope<ObjType>>(Response.Response,
                                                                                           collectionOptions.XMLAttributeOverrides, _logger);
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

    /// <inheritdoc/>
    public string GenerateCollectionXML(CollectionRequestOptions collectionOptions)
    {
        StaticVariables staticVariables = new()
        {
            SVCompany = collectionOptions.Company ?? Company?.Name,
            SVFromDate = collectionOptions.FromDate ?? Company?.BooksFrom!,
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
                                                   objects: collectionOptions.Objects,
                                                   isInitialize: collectionOptions.IsInitialize);



        if (collectionOptions.Pagination != null)
        {

            ColEnvelope.Body.Desc.TDL.TDLMessage.Collection.Add(new()
            {
                Name = ColEnvelope.Header.ID + "PAGINATED",
                Collections = ColEnvelope.Header!.ID,
                Compute = new() { "LineIndex : ##vLineIndex" },
                ComputeVar = new() { "vLineIndex: Number : IF $$IsEmpty:##vLineIndex THEN 1 ELSE ##vLineIndex + 1" },
                NativeFields = new() { "*" },
                Filters = new() { "Pagination" }
            });
            ColEnvelope.Body.Desc.TDL.TDLMessage.System?.Add(new("Pagination", collectionOptions.Pagination.GetFilterFormulae()));
            ColEnvelope.Header!.ID += "PAGINATED";
            //collectionOptions.Filters.Add(new("Pagination", collectionOptions.Pagination.GetFilterFormulae()));

        }
        string Reqxml = ColEnvelope.GetXML();
        return Reqxml;
    }

    /// <inheritdoc/>
    public async Task<ReturnType?> GetTDLReportAsync<ReportType, ReturnType>(DateFilterRequestOptions? requestOptions = null) where ReturnType : TallyBaseObject
    {
        StaticVariables sv = new()
        {
            SVCompany = requestOptions?.Company ?? Company?.Name,
            SVExportFormat = "XML",
            SVFromDate = requestOptions?.FromDate,
            SVToDate = requestOptions?.ToDate
        };
        TDLReport report = TDLReportHelper.CreateTDLReport(typeof(ReportType));

        RequestEnvelope requestEnvelope = new(report, sv);
        var Reqxml = requestEnvelope.GetXML();
        TallyResult Response = await SendRequestAsync(Reqxml);
        if (Response.Status == RespStatus.Sucess && Response.Response != null)
        {
            ReturnType? tallyReport = XMLToObject.GetObjfromXml<ReturnType>(Response.Response);
            return tallyReport;
        }

        return default;
    }

    /// <inheritdoc/>
    public async Task<ReturnType?> GetTDLReportAsync<ReturnType>(DateFilterRequestOptions? requestOptions = null) where ReturnType : TallyBaseObject
    {
        return await GetTDLReportAsync<ReturnType, ReturnType>(requestOptions);
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

}
