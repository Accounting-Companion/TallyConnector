using System.Collections.Concurrent;

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
    public TallyService()
    {
        _httpClient = new();
        _baseURL = "http://localhost";
        _port = 9000;
        _httpClient.Timeout = TimeSpan.FromMinutes(10);
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
                        int timeoutMinutes = 1)
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

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="httpClient"></param>
    ///// <param name="Logger"></param>
    ///// <param name="timeoutMinutes"></param>
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
        if (!url.Contains("http") && !url.Contains("https"))
        {
            url = $"http://{url}";
        }
        _baseURL = url;
        _port = port;
    }


    public void SetCompany(Company company)
    {
        Company = company;
    }

    public async Task<LicenseInfo?> GetLicenseInfoAsync()
    {
        var LicenseInfo = await GetTDLReportAsync<LicenseInfo, LicenseInfo>();
        return LicenseInfo;
    }


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

    public async Task<ObjType> GetObjectAsync<ObjType>(string lookupValue,
                                                       MasterRequestOptions? requestOptions = null) where ObjType : TallyBaseObject, ITallyObject
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



    public async Task<ObjType> GetObjectAsync<ObjType>(string lookupValue,
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


    public async Task<List<ObjType>?> GetObjectsAsync<ObjType>(PaginatedRequestOptions? objectOptions = null) where ObjType : TallyBaseObject
    {
        //Gets Root attribute of ReturnObject
        XmlRootAttribute? RootAttribute = (XmlRootAttribute?)Attribute.GetCustomAttribute(typeof(ObjType), typeof(XmlRootAttribute));

        CollectionRequestOptions collectionOptions = new()
        {
            CollectionType = RootAttribute?.ElementName ?? typeof(ObjType).Name,
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
        var mapping = TallyObjectMapping.TallyObjectMappings
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
        }

        if (collectionOptions.Pagination != null)
        {
            collectionOptions.Compute.Add("LineIndex : ##vLineIndex");
            collectionOptions.ComputeVar ??= new();
            collectionOptions.ComputeVar.Add("vLineIndex: Number : IF $$IsEmpty:##vLineIndex THEN 1 ELSE ##vLineIndex + 1");
            collectionOptions.Filters.Add(new("Pagination", collectionOptions.Pagination.GetFilterFormulae()));
        }
        //Adding xmlelement name according to RootElement name of ReturnObject
        collectionOptions.XMLAttributeOverrides ??= new();
        XmlAttributes attrs = new();
        attrs.XmlElements.Add(new(collectionOptions.CollectionType));
        collectionOptions.XMLAttributeOverrides.Add(typeof(Colllection<ObjType>), "Objects", attrs);

        var objects = await GetCustomCollectionAsync<ObjType>(collectionOptions);

        return objects;


    }

    public async Task<List<ObjType>> GetAllObjectsAsync<ObjType>(RequestOptions? objectOptions = null) where ObjType : TallyBaseObject
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
            IsInitialize = YesNo.Yes,
        };

        var mapping = TallyObjectMapping.TallyObjectMappings
                .FirstOrDefault(map => map.TallyMasterType.Equals(collectionOptions.CollectionType, StringComparison.OrdinalIgnoreCase));

        collectionOptions.Filters ??= new();
        if (mapping != null)
        {
            if (mapping.Filters != null)
            {
                collectionOptions.Filters.AddRange(mapping.Filters);
            }
        }
        int? TotalCount = await GetObjectCountAync(mapping!.MasterType, collectionOptions);
        Pagination pagination = new(TotalCount ?? 0, mapping?.DefaultPaginateCount ?? 1000);
        ConcurrentBag<ObjType> objects = new();
        List<Task> tasks = new();
        for (int i = 0; i < pagination.TotalPages; i++)
        {
            Pagination tpagination = new(TotalCount ?? 0, mapping?.DefaultPaginateCount ?? 1000, i + 1);
            var options = new PaginatedRequestOptions()
            {
                FromDate = objectOptions?.FromDate,
                ToDate = objectOptions?.ToDate,
                FetchList = objectOptions?.FetchList,
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
        }
        //await Task.WhenAll(tasks.ToArray());
        return objects.ToList();
    }

    public async Task<int?> GetObjectCountAync(TallyObjectType objectType, DateFilterRequestOptions options)
    {
        if (objectType is TallyObjectType.Vouchers)
        {
            var count = 0;
            var vchtypescount = await GetVoucherStatisticsAsync(options);
            vchtypescount?.ForEach(c => count += c.TotalCount);
            return count;
        }
        else
        {
            var stats = await GetMasterStatisticsAsync();
            var stat = stats?.FirstOrDefault(c => c.Name.Replace(" ", "") == objectType.ToString());
            int? Count = stat?.Count;
            //Adding below is reuired as employee group and employee are filtered
            if (stat != null && objectType is TallyObjectType.CostCentres)
            {
                Count += stats?.FirstOrDefault(c => c.Name.Replace(" ", "") == TallyObjectType.EmployeeGroups.ToString())?.Count;
                Count += stats?.FirstOrDefault(c => c.Name.Replace(" ", "") == TallyObjectType.Employees.ToString())?.Count;
            }
            else if (stat != null && objectType is TallyObjectType.EmployeeGroups)
            {
                Count += stats?.FirstOrDefault(c => c.Name.Replace(" ", "") == TallyObjectType.CostCentres.ToString())?.Count;
                Count += stats?.FirstOrDefault(c => c.Name.Replace(" ", "") == TallyObjectType.Employees.ToString())?.Count;
            }
            else if (stat != null && objectType is TallyObjectType.Employees)
            {
                Count += stats?.FirstOrDefault(c => c.Name.Replace(" ", "") == TallyObjectType.CostCentres.ToString())?.Count;
                Count += stats?.FirstOrDefault(c => c.Name.Replace(" ", "") == TallyObjectType.EmployeeGroups.ToString())?.Count;
            }
            return Count;
        }
    }
    public async Task<List<ObjType>?> GetCustomCollectionAsync<ObjType>(CollectionRequestOptions collectionOptions) where ObjType : TallyBaseObject
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

    public async Task<ReturnType?> GetTDLReportAsync<ReturnType>(DateFilterRequestOptions? requestOptions = null) where ReturnType : TallyBaseObject
    {
        return await GetTDLReportAsync<ReturnType, ReturnType>(requestOptions);
    }

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
            //TLogger.TallyReqError(exc.Message);
            //throw new TallyConnectivityException("Tally is not running", FullURL);
        }
        return result;
    }

    public static string ReplaceXMLText(string Xml)
    {
        Xml = Xml.Replace("&#4; ", "");
        Xml = Xml.Replace("0x20B9", "");
        return Xml;
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
