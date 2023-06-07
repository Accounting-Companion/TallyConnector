using System.Collections.Concurrent;
using System.Data;
using TallyConnector.Core.Models.Masters;
using TallyConnector.Core.Models.Masters.CostCenter;
using TallyConnector.Core.Models.Masters.Inventory;
using TallyConnector.Core.Models.Masters.Payroll;

namespace TallyConnector.Services;
/// <summary>
/// contains API to interact with Tally
/// </summary>
[GenerateHelperMethods<Currency>(PluralName = "Currencies")]
[GenerateHelperMethods<Group>()]
[GenerateHelperMethods<Ledger>()]
[GenerateHelperMethods<CostCategory>(PluralName = "CostCategories")]
[GenerateHelperMethods<CostCentre>(MethodName = "CostCenter")]
[GenerateHelperMethods<VoucherType>(TypeName = "VchType")]
[GenerateHelperMethods<Voucher>()]

[GenerateHelperMethods<Unit>()]
[GenerateHelperMethods<Godown>()]
[GenerateHelperMethods<StockGroup>()]
[GenerateHelperMethods<StockCategory>(PluralName = "StockCategories")]
[GenerateHelperMethods<StockItem>()]

[GenerateHelperMethods<AttendanceType>(TypeName = "AtndType")]
[GenerateHelperMethods<EmployeeGroup>()]
[GenerateHelperMethods<Employee>()]
public partial class TallyService : ITallyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger? _logger;
    private readonly TLogger? Logger;
    private int _port;

    private string _baseURL;

    protected BaseCompany? Company { get; set; }

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
    }

    /// <summary>
    /// Intiaite Tally Service with httpclient , logger and timeoutMinutes
    /// </summary>
    /// <param name="httpClient">http client</param>
    /// <param name="logger">logger</param>
    /// <param name="timeoutMinutes">Request timeout in Minutes</param>
    public TallyService(HttpClient httpClient,
                        ILogger<TallyService>? logger = null,
                        double timeoutMinutes = 3)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(timeoutMinutes);
        _baseURL = "http://localhost";
        _port = 9000;
        _logger = logger;
        Logger = new(_logger);
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
    public async Task<LicenseInfo?> GetLicenseInfoAsync(CancellationToken token = default)
    {
        var LicenseInfo = await GetTDLReportAsync<LicenseInfo, LicenseInfo>(token: token, requestType: "License Info");
        return LicenseInfo;
    }

    /// <inheritdoc/>
    public async Task<TallyResult> PostObjectToTallyAsync<ObjType>(ObjType Object,
                                                                   PostRequestOptions? postRequestOptions = null,
                                                                   string? objectType = null,
                                                                   CancellationToken token = default) where ObjType : TallyXmlJson, ITallyObject
    {
        if (Object is Voucher vch)
        {
            postRequestOptions ??= new();
            postRequestOptions.XMLAttributeOverrides ??= new();

            if (vch.View != VoucherViewType.AccountingVoucherView)
            {
                vch.IsInvoice = true;
                var attributes = postRequestOptions.XMLAttributeOverrides[typeof(ObjType), "Ledgers"];
                XmlAttributes xmlattribute = new();
                if (attributes != null)
                {
                    xmlattribute = attributes;
                }
                else
                {
                    postRequestOptions.XMLAttributeOverrides.Add(typeof(ObjType), "Ledgers", xmlattribute);
                }
                xmlattribute.XmlElements.Add(new("LEDGERENTRIES.LIST"));

            }
        }
        Object.PrepareForExport();
        Envelope<ObjType> Objectenvelope = new(Object,
                                               new()
                                               {
                                                   SVCompany = postRequestOptions?.Company ?? Company?.Name
                                               });
        string ReqXml = Objectenvelope.GetXML(postRequestOptions?.XMLAttributeOverrides);

        TallyResult tallyResult = await SendRequestAsync(xml: ReqXml,
                                                         requestType: string.IsNullOrEmpty(objectType) ? null : $"Sending {objectType} to Tally",
                                                         token: token);
        if (tallyResult.Status == RespStatus.Sucess)
        {
            return ParseResponse(tallyResult);
        }
        return tallyResult;
    }
    /// <inheritdoc/>
    public async Task<ObjType> GetObjectAsync<ObjType>(string lookupValue,
                                                       MasterRequestOptions? requestOptions = null,
                                                       CancellationToken token = default) where ObjType : TallyBaseObject, INamedTallyObject
    {
        // If received FetchList in collectionOptions we will use that else use default fetchlist
        requestOptions ??= new();
        requestOptions.FetchList ??= Constants.DefaultFetchList;
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

        PaginatedRequestOptions collectionRequestOptions = new() { FetchList = requestOptions.FetchList, Filters = filters, XMLAttributeOverrides = requestOptions.XMLAttributeOverrides };

        List<ObjType>? objects = (await GetObjectsAsync<ObjType>(collectionRequestOptions, token))?.Data;
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
                                                       VoucherRequestOptions? requestOptions = null, CancellationToken token = default) where ObjType : Voucher
    {
        // If received FetchList in collectionOptions we will use that else use default fetchlist
        requestOptions ??= new();
        requestOptions.FetchList ??= Constants.DefaultFetchList;
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

        PaginatedRequestOptions paginatedRequestOptions = new()
        {
            FetchList = requestOptions.FetchList,
            Filters = filters,
            Objects = requestOptions.Objects,
            FromDate = requestOptions.FromDate,
            ToDate = requestOptions.ToDate,
            Company = requestOptions.Company,
            Compute = requestOptions.Compute,
            ComputeVar = requestOptions.ComputeVar,
        };

        List<ObjType>? objects = (await GetObjectsAsync<ObjType>(paginatedRequestOptions, token))?.Data;
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
    public async Task<PaginatedResponse<ObjType>?> GetObjectsAsync<ObjType>(PaginatedRequestOptions? objectOptions = null,
                                                                            CancellationToken token = default) where ObjType : TallyBaseObject
    {
        //Gets Root attribute of ReturnObject
        Type objType = typeof(ObjType);
        string? RootElemet = AttributeHelper.GetXmlRootElement(objType, _logger);
        Logger?.BuildingOptions(typeof(CollectionRequestOptions));

        string name = objType.Name;
        CollectionRequestOptions collectionOptions = new()
        {
            CollectionType = RootElemet ?? name,
            Company = objectOptions?.Company,
            FromDate = objectOptions?.FromDate,
            ToDate = objectOptions?.ToDate,
            FetchList = (objectOptions?.FetchList) != null ? new(objectOptions.FetchList) : new() { "MasterId", "CanDelete", "*" },
            Filters = (objectOptions?.Filters) != null ? new(objectOptions.Filters) : null,
            Compute = (objectOptions?.Compute) != null ? new(objectOptions.Compute) : null,
            ComputeVar = (objectOptions?.ComputeVar) != null ? new(objectOptions.ComputeVar) : null,
            Objects = objectOptions?.Objects,
            PageNum = objectOptions?.PageNum ?? 1,
            Pagination = true,
            RecordsPerPage = objectOptions?.RecordsPerPage,
            XMLAttributeOverrides = objectOptions?.XMLAttributeOverrides,
            IsInitialize = objectOptions?.IsInitialize ?? YesNo.No,
        };
        TallyObjectType? tallyObjType = AttributeHelper.GetTallyObjectTypeAttribute(objType);
        var mapping = Mappings.TallyObjectMappings
                .FirstOrDefault(map => map.MasterType == tallyObjType);
        collectionOptions.Compute ??= new();
        collectionOptions.Filters ??= new();
        collectionOptions.Objects ??= new();
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
            mapping.Objects?.ForEach(obj =>
                {
                    if (!collectionOptions.Objects.Contains(obj))
                    {
                        collectionOptions.Objects.Add(obj);
                    }
                });
        }
        //Adding xmlelement name according to RootElement name of ReturnObject
        collectionOptions.XMLAttributeOverrides ??= new();
        XmlAttributes attrs = new();
        attrs.XmlElements.Add(new(collectionOptions.CollectionType));
        try
        {
            collectionOptions.XMLAttributeOverrides.Add(typeof(Colllection<ObjType>), "Objects", attrs);
        }
        catch (Exception ex)
        {
        }
        //collectionOptions.Pagination = await GetFirstPagePagination(collectionOptions, objectOptions?.RecordsPerPage ?? mapping?.DefaultPaginateCount);
        //if (objectOptions != null && objectOptions.PageNum != 1)
        //{
        //    collectionOptions.Pagination.GoToPage(objectOptions.PageNum);
        //}
        collectionOptions.RecordsPerPage ??= mapping?.DefaultPaginateCount;
        var paginatedData = await GetCustomCollectionAsync<ObjType>(collectionOptions, $"Getting {mapping?.MasterType} from Tally", token);
        paginatedData?.Data?.ForEach(obj =>
        {
            obj.RemoveNullChilds();
            if (obj is IAliasTallyObject AliasObj)
            {
                AliasObj.Alias = AliasObj.LanguageNameList != null && AliasObj.LanguageNameList.Count > 1 ? AliasObj.LanguageNameList?.First()?.LanguageAlias : string.Empty;
            }

        });
        if (paginatedData != null && paginatedData.Data != null)
        {
            return paginatedData;
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<List<ObjType>> GetAllObjectsAsync<ObjType>(RequestOptions? objectOptions = null,
                                                                 IProgress<ReportProgressHelper>? progress = null,
                                                                 CancellationToken token = default) where ObjType : TallyBaseObject
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
            Objects = objectOptions?.Objects,
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
        //Pagination pagination = await GetFirstPagePagination(collectionOptions, mapping?.DefaultPaginateCount ?? 1000);
        ConcurrentBag<ObjType> objects = new();
        List<Task> tasks = new();
        int TotalPages = 0;
        int CurrentPage = 1;

        do
        {
            var options = new PaginatedRequestOptions()
            {
                FromDate = objectOptions?.FromDate,
                ToDate = objectOptions?.ToDate,
                FetchList = objectOptions?.FetchList,
                Filters = objectOptions?.Filters,
                Compute = objectOptions?.Compute,
                ComputeVar = objectOptions?.ComputeVar,
                Objects = objectOptions?.Objects,
                PageNum = CurrentPage,
                RecordsPerPage = mapping?.DefaultPaginateCount,
                XMLAttributeOverrides = objectOptions?.XMLAttributeOverrides,
                IsInitialize = objectOptions?.IsInitialize ?? YesNo.No,
            };
            var paginatedResp = await GetObjectsAsync<ObjType>(options, token);
            if (mapping != null)
            {
                _logger?.LogInformation("Received {type} from {start} to {end} (Page {cur} of {Total})",
                                    mapping?.MasterType,
                                    (CurrentPage - 1) * options.RecordsPerPage,
                                    CurrentPage * options.RecordsPerPage,
                                    CurrentPage,
                                    paginatedResp?.TotalPages);
            }

            TotalPages = paginatedResp?.TotalPages ?? TotalPages;
            CurrentPage++;
            paginatedResp?.Data?.AsParallel().ForAll(t => objects.Add(t));
            progress?.Report(new(paginatedResp!.TotalCount, options.RecordsPerPage ?? 0, objects.Count));
        }
        while (CurrentPage <= TotalPages);
        //await Task.WhenAll(tasks.ToArray());
        return objects.ToList();
    }

    /// <inheritdoc/>
    public async Task<int?> GetObjectCountAync(CountRequestOptions options, string? requestType = null, CancellationToken token = default)
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
                                                                    options.ChildOf,
                                                                    filters: options.Filters?.Where(f => !f.ExcludeinCollection).Select(c => c.FilterName).ToList()!));


        options.Filters?.ForEach(filter => requestEnvelope.Body.Desc.TDL.TDLMessage.System?.Add(new(name: filter.FilterName!,
                                                 text: filter.FilterFormulae!)));

        string Reqxml = requestEnvelope.GetXML();
        var Response = await SendRequestAsync(xml: Reqxml,
                                              requestType: requestType ?? $"Count of {options.CollectionType}",
                                              token: token);
        if (Response.Status == RespStatus.Sucess && Response.Response != null)
        {
            Envelope<string>? Envelope = XMLToObject.GetObjfromXml<Envelope<string>>(Response.Response, null, _logger);
            return int.Parse(Envelope?.Body?.Data?.FuncResult?.Result ?? "0");
        }
        return 0;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponse<ObjType>?> GetCustomCollectionAsync<ObjType>(CollectionRequestOptions collectionOptions,
                                                                                     string? requestType = null,
                                                                                     CancellationToken token = default) where ObjType : TallyBaseObject
    {
        collectionOptions.RecordsPerPage ??= 1000;
        string Reqxml = await GenerateCollectionXML(collectionOptions);

        var Response = await SendRequestAsync(xml: Reqxml,
                                              requestType: requestType ?? $"Collection of {collectionOptions.CollectionType}",
                                              token: token);
        if (Response.Status == RespStatus.Sucess && Response.Response != null)
        {
            try
            {
                Envelope<ObjType>? Envelope = XMLToObject.GetObjfromXml<Envelope<ObjType>>(Response.Response,
                                                                                           collectionOptions.XMLAttributeOverrides, _logger);
                // return Envelope?.Body.Data.Collection?.Objects;
                int? v = await GetObjectCountAync(new CountRequestOptions()
                {
                    CollectionType = collectionOptions.CollectionType,
                    Filters = collectionOptions.Filters,
                    FromDate = collectionOptions.FromDate,
                    ToDate = collectionOptions.ToDate,
                    Company = collectionOptions.Company,

                }, token: token);
                return new PaginatedResponse<ObjType>(v ?? 0,
                                                      collectionOptions.RecordsPerPage ?? 1000,
                                                      Envelope?.Body.Data.Collection?.Objects,
                                                      collectionOptions.PageNum);
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
    public async Task<string> GenerateCollectionXML(CollectionRequestOptions collectionOptions, bool indented = false)
    {
        StaticVariables staticVariables = new()
        {
            SVCompany = collectionOptions.Company ?? Company?.Name ?? await GetActiveSimpleCompanyNameAsync(),
            SVFromDate = collectionOptions.FromDate ?? Company?.BooksFrom,
            SVToDate = collectionOptions.ToDate ?? (collectionOptions.FromDate == null ? null : DateTime.Now),
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



        if (collectionOptions.Pagination)
        {
            // ColEnvelope.Body.Desc.TDL.TDLMessage.Collection.First().Sort = new() { "@@Default : $MASTERID" };
            ColEnvelope.Body.Desc.TDL.TDLMessage.Object ??= new();
            //ColEnvelope.Body.Desc.TDL.TDLMessage.Object!.Add(new("Pagination", new() { $"TotalCount:$$NUMITEMS:{ColEnvelope.Header?.ID}" }));

            ColEnvelope.Body.Desc.TDL.TDLMessage.Collection.Add(new()
            {
                Name = ColEnvelope.Header?.ID + "PAGINATED",
                Collections = ColEnvelope.Header?.ID,
                Compute = new() { "LineIndex : ##vLineIndex" },
                ComputeVar = new() { "vLineIndex: Number : IF $$IsEmpty:##vLineIndex THEN 1 ELSE ##vLineIndex + 1" },
                NativeFields = new() { "*" },
                Filters = new() { "PaginationFilter" }
            });
            int? Start = collectionOptions.RecordsPerPage * (collectionOptions.PageNum - 1);
            ColEnvelope.Body.Desc.TDL.TDLMessage.System?.Add(new("PaginationFilter",
                                                                 $"##vLineIndex <= {Start + collectionOptions.RecordsPerPage} AND ##vLineIndex > {Start}"));

            //ColEnvelope.Body.Desc.TDL.TDLMessage.Collection.Add(new()
            //{
            //    Name = ColEnvelope.Header?.ID + "WITHPAGINATED",
            //    Collections = ColEnvelope.Header?.ID + "PAGINATED",
            //    NativeFields = new() { "*" },
            //    
            //});
            ColEnvelope.Header!.ID += "PAGINATED";

        }
        string Reqxml = ColEnvelope.GetXML(indent: indented);
        return Reqxml;
    }

    /// <inheritdoc/>
    public async Task<ReturnType?> GetTDLReportAsync<ReportType, ReturnType>(DateFilterRequestOptions? requestOptions = null,
                                                                             string? requestType = null,
                                                                             CancellationToken token = default)
    {
        StaticVariables sv = new()
        {
            SVCompany = requestOptions?.Company ?? Company?.Name ?? await GetActiveSimpleCompanyNameAsync(token),
            SVExportFormat = "XML",
            SVFromDate = requestOptions?.FromDate,
            SVToDate = requestOptions?.ToDate
        };
        TDLReport report = TDLReportHelper.CreateTDLReport(typeof(ReportType));

        RequestEnvelope requestEnvelope = new(report, sv);
        var Reqxml = requestEnvelope.GetXML();
        TallyResult Response = await SendRequestAsync(xml: Reqxml,
                                                      requestType: requestType ?? $"Getting Custom TDL Report - {report.FieldName}",
                                                      token: token);
        if (Response.Status == RespStatus.Sucess && Response.Response != null)
        {
            ReturnType tallyReport = XMLToObject.GetObjfromXml<ReturnType>(Response.Response);
            return tallyReport;
        }

        return default;
    }

    /// <inheritdoc/>
    public async Task<ReturnType?> GetTDLReportAsync<ReturnType>(DateFilterRequestOptions? requestOptions = null,
                                                                 CancellationToken token = default) where ReturnType : TallyBaseObject
    {
        return await GetTDLReportAsync<ReturnType, ReturnType>(requestOptions: requestOptions, requestType: null, token: token);
    }

    /// <inheritdoc/>
    public async Task<TallyResult> SendRequestAsync(string? xml = null, string? requestType = null, CancellationToken token = default)
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
            HttpResponseMessage tallyResponse = await _httpClient.SendAsync(requestMessage, token);
#if NET48
            var resXml = ReplaceXMLText(await tallyResponse.Content.ReadAsStringAsync());
#else
            var resXml = ReplaceXMLText(await tallyResponse.Content.ReadAsStringAsync(token));
#endif

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
