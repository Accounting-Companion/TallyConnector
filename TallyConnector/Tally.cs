using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;
using System.Xml.Xsl;
using TallyConnector.Core.Exceptions;

namespace TallyConnector;

public class Tally : IDisposable
{
    private readonly HttpClient client = new();

    private ILogger Logger { get; }
    private CLogger CLogger { get; }

    private int Port;
    private string BaseURL;

    public string? Status { get; private set; }
    public string? ReqStatus { get; private set; }

    public string? Company { get; private set; }
    public string? FromDate { get; private set; }
    public string? ToDate { get; private set; }

    public List<MastersBasicInfo<BasicTallyObject>>? Masters { get; private set; }

    private Dictionary<Type, PropertyInfo[]> PropertyInfoList { get; set; } = new();

    private bool disposedValue;

    //Gets Full Url from Baseurl and Port
    private string FullURL => BaseURL + ":" + Port;


    public List<Company>? CompaniesList { get; private set; }

    public LicenseInfo? LicenseInfo { get; private set; }

    /// <summary>
    /// Intiate Tally with <strong>baseURL</strong> and <strong>port</strong>
    /// </summary>
    /// <param name="baseURL">Url on which Tally is Running</param>
    /// <param name="port">Port on which Tally is Running</param>
    public Tally(string baseURL,
                 int port,
                 ILogger<Tally>? Logger = null,
                 int Timeoutseconds = 30)
    {
        this.Logger = Logger ?? NullLogger<Tally>.Instance;
        CLogger = new CLogger(Logger);
        Port = port;
        BaseURL = baseURL;
        client.Timeout = TimeSpan.FromSeconds(Timeoutseconds);
    }


    /// <summary>
    /// If nothing Specified during Intialisation default Url will be <strong>http://localhost</strong> running on port <strong>9000</strong>
    /// </summary>
    public Tally(ILogger<Tally>? Logger = null,
                 int Timeoutseconds = 30)
    {
        this.Logger = Logger ?? NullLogger<Tally>.Instance;
        CLogger = new CLogger(Logger);
        client.Timeout = TimeSpan.FromSeconds(Timeoutseconds);
        BaseURL = "http://localhost";
        Port = 9000;

    }


    /// <summary>
    /// Setup instance default Static-varibales instead of specifying in each method
    /// </summary>
    /// <param name="baseURL">Url on which Tally is Running</param>
    /// <param name="port">Port on which Tally is Running</param>
    /// <param name="company">Specify Company name - If multiple companies are opened in tally set this param to get from spcific ompany</param>
    /// <param name="fromDate">Default from date from to use for fetching info</param>
    /// <param name="toDate">Default from date from to use for fetching info<</param>
    public void Setup(string baseURL,
                      int port,
                      string? company = null,
                      string? fromDate = null,
                      string? toDate = null)
    {
        BaseURL = baseURL;
        Port = port;
        Company = company;
        FromDate = fromDate;
        ToDate = toDate;

        CLogger?.SetupLog(baseURL, port, company!, fromDate!, toDate!);
    }


    /// <summary>
    /// Setup instance default Static-varibales instead of specifying in each method
    /// </summary>
    /// <param name="company">Specify Company name - If multiple companies are opened in tally set this param to get from spcific ompany</param>
    /// <param name="fromDate">Default from date from to use for fetching info</param>
    /// <param name="toDate">Default from date from to use for fetching info<</param>
    public void ChangeCompany(string company,
                              string? fromDate = null,
                              string? toDate = null)
    {
        Company = company;
        FromDate = fromDate;
        ToDate = toDate;
        CLogger?.SetupLog(company, fromDate!, toDate!);

    }

    /// <summary>
    /// Checks whether Tally is running in given URL and port
    /// </summary>
    /// <returns>Return true if running,else false</returns>
    public async Task<bool> Check()
    {
        try
        {
            CLogger.TallyCheck(FullURL);
            HttpResponseMessage response = await client.GetAsync(FullURL);
            response.EnsureSuccessStatusCode();
            string res = await response.Content.ReadAsStringAsync();

            Status = "Running";
            CLogger.TallyRunning(FullURL);
            return true;
        }
        catch (HttpRequestException ex)
        {
            HttpRequestException e = ex;
            CLogger.TallyNotRunning(FullURL);
            Status = $"Tally is not opened \n or Tally is not running in given port - {Port} )\n or Given URL - {BaseURL} \n" +
                e.Message;
            //throw new TallyConnectivityException("Tally is not running", FullURL);
        }
        catch (Exception e)
        {
            CLogger.TallyError(FullURL, e.Message);
        }
        return false;
    }

    public async Task<LicenseInfo> GetLicenseInfo()
    {
        string ReqType = "Tally Info";
        CLogger.TallyReqStart(ReqType);
        List<string> LicenseInfoFormulas = new()
        {
            "IsEducationalMode: $$LicenseInfo:IsEducationalMode",
            "IsSilver: $$LicenseInfo:IsSilver",
            "IsGold: $$LicenseInfo:IsGold",
            "PlanName:" +
            " If $$LicenseInfo:IsEducationalMode Then \"Educational Version\"" +
            " ELSE " +
            " If $$LicenseInfo:IsSilver Then \"Silver\"" +
            " ELSE " +
            " If $$LicenseInfo:IsGold Then \"Gold\"" +
            " else \"\"",
            "SerialNumber: $$LicenseInfo:SerialNumber",
            "AccountId:$$LicenseInfo:AccountID",
            "IsIndian: $$LicenseInfo:IsIndian",
            "RemoteSerialNumber: $$LicenseInfo:RemoteSerialNumber",
            "IsRemoteAccessMode: $$LicenseInfo:IsRemoteAccessMode",
            "IsLicClientMode: $$LicenseInfo:IsLicClientMode",
            "AdminMailId:$$LicenseInfo:AdminEmailID",
            "IsAdmin:$$LicenseInfo:IsAdmin",
            "ApplicationPath:$$SysInfo:ApplicationPath",
            "DataPath:##SVCurrentPath",
            "UserName:$$cmpusername",
            "UserLevel:$$cmpuserlevel"
        };

        List<TallyCustomObject> tallyCustomObjects = new()
        {
            new TallyCustomObject("LicenseInfo", LicenseInfoFormulas)
        };
        string CollectionName = "LicenseInfo";
        RequestEnvelope ColEnvelope = new(HType.Collection, CollectionName); //Collection Envelope

        //Collection collection = new() { Objects = tallyCustomObjects };


        ColEnvelope.Body.Desc.TDL.TDLMessage = new(tallyCustomObjects: tallyCustomObjects,
                                                   objCollectionName: CollectionName,
                                                   ObjNames: "LicenseInfo");
        string Reqxml = ColEnvelope.GetXML();
        String RespXml = await SendRequest(Reqxml);
        LicenseInfo? licenseInfo = GetObjfromXml<LicInfoEnvelope>(RespXml)?.Body.Data.Collection.LicenseInfo;
        LicenseInfo = licenseInfo!;
        return LicenseInfo;
        //string xml = GetCustomCollectionXML("TallyInfo", tallyCustomObjects);
    }

    /// <summary>
    /// Gets List of Companies opened in tally and saves in Model.Company List
    /// </summary>
    /// <returns>return list of Model.Company List</returns>
    public async Task<List<Company>?> GetCompaniesList()
    {
        string ReqType = "List of companies opened in Tally";
        await Check(); //Checks Whether Tally is running
        if (Status == "Running")
        {
            try
            {
                CLogger.TallyReqStart(ReqType);
                List<string> NativeFields = new() { "Name", "StartingFrom", "GUID", "MobileNo, RemoteFullListName", "*" };
                CompaniesList = await GetNativeCollectionXML<Company>(NativeFields: NativeFields, isInitialize: YesNo.Yes);
                // CompaniesList = GetObjfromXml<ComListEnvelope>(xml).Body.Data.Collection.CompaniesList;
                CLogger.TallyReqCompleted(ReqType);
            }
            catch (Exception e)
            {
                CLogger.RequestError(ReqType, e.Message);
                //return CompList;
            }

        }
        return CompaniesList;
    }

    /// <summary>
    /// Gets List of Companies in tally default Tally path
    /// </summary>
    /// <returns>return list of Model.Company List</returns>
    public async Task<List<CompanyOnDisk>?> GetCompaniesListinPath()
    {
        string ReqType = "List of companies in Default Tally path";
        List<CompanyOnDisk>? Companies = new();
        await Check(); //Checks Whether Tally is running
        if (Status == "Running")
        {
            CLogger.TallyReqStart(ReqType);
            List<string> NativeFields = new() { "*" };
            List<Filter> filters = new()
            {
                new() { FilterName = "NonGroupFilter", FilterFormulae = $"$ISAGGREGATE = \"No\"" }
            };

            try
            {
                Companies = await GetNativeCollectionXML<CompanyOnDisk>(NativeFields: NativeFields,
                                                                     filters: filters);
                CLogger.TallyReqCompleted(ReqType);
            }
            catch (Exception e)
            {
                CLogger.RequestError(ReqType, e.Message);
            }

        }
        return Companies;
    }


    /// <summary>
    /// Fetch All Masters like Groups,Ledgers,..etc From Tally
    /// </summary>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <returns></returns>
    public async Task FetchAllTallyData()
    {
        string ReqType = "Masters from Tally";
        CLogger.TallyReqStart(ReqType);

        Masters = new();
        List<Task> tasks = new();
        foreach (var mapping in TallyObjectMapping.MastersMappings)
        {
            tasks.Add(GetBasicMasterInfo(mapping));

        }
        await Task.WhenAll(tasks);
        CLogger.TallyReqCompleted(ReqType);
    }

    private async Task GetBasicMasterInfo(TallyObjectMapping mapping)
    {
        List<BasicTallyObject>? basicTallyObjects = await GetBasicObjectData(ObjectType: mapping.TallyMasterType, filters: mapping.Filters);
        Masters?.Add(new MastersBasicInfo<BasicTallyObject>(mapping.MasterType, basicTallyObjects!));
    }

    public List<BasicTallyObject>? GetMasters(TallyObjectType masterType)
    {
        MastersBasicInfo<BasicTallyObject>? mastersBasicInfo = Masters?.FirstOrDefault(info => info.MasterType == masterType);
        return mastersBasicInfo?.Masters;
    }


    public async Task<string?> GetActiveTallyCompany()
    {
        RequestEnvelope requestEnvelope = new(HType.Function, "$$string");
        requestEnvelope.Body.Desc.FunctionParams = new(new() { "##SVCURRENTCOMPANY" });
        string Reqxml = requestEnvelope.GetXML();
        string Resxml = await SendRequest(Reqxml);
        Envelope<FunctionResult>? result = GetObjfromXml<Envelope<FunctionResult>>(Resxml);
        return result?.Body.Data.FuncResult?.Result;
    }

    /// <summary>
    /// Get Statistics of Company,
    /// from date and to date has no impact on master statistics
    /// </summary>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <returns></returns>
    public async Task<Statistics?> GetStatistics(string? company = null,
                                    string? fromDate = null,
                                    string? toDate = null)
    {
        company ??= Company;
        fromDate ??= FromDate;
        toDate ??= ToDate;
        StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate, SVExportFormat = "XML" };
        RequestEnvelope CusColEnvelope = new(HType.Data, "CustomStatistics", sv);

        CusColEnvelope.Body.Desc.TDL.TDLMessage = new()
        {
            Report = new() { new("CustomStatistics") },
            Form = new()
            {
                new("CustomStatistics")
                {
                    PartName = "CustVchStatistics,CustMstrStatistics",
                    ReportTag = "STATISTICS",
                }

            },
            Part = new()
            {
                new("CustVchStatistics", "STATVchType", "VchStatistics"),
                new("CustMstrStatistics", "STATObjects", "MstrStatistics")
            },
            Line = new()
            {
                new()
                {
                    Name = "VchStatistics",
                    Fields = new() { "Name,Count,CancelledCount" },
                    XMLTag = "VoucherType"
                },
                new()
                {
                    Name = "MstrStatistics",
                    Fields = new() { "Name,Count" },
                    XMLTag = "MasterType"
                }
            },
            Field = new()
            {
                new("Name", "Name"),
                new("CancelledCount", "CancVal") { XMLTag = "CancelledCount" },
                new("Count", "StatVal") { XMLTag = "Count" },
            }
        };

        string Reqxml = CusColEnvelope.GetXML();
        string Resxml = await SendRequest(Reqxml);

        Statistics? statistics = GetObjfromXml<Statistics>(Resxml);
        statistics?.CalculateTotals();
        return statistics;

    }

    /// <summary>
    /// Get Basic Object data for Given Type - TallyId,GUID,AlterId
    /// fromDate and toDate has no effect unless ObjectType is Voucher
    /// </summary>
    /// <param name="ObjectType">Type of Object, Ex: Group,Ledger</param>
    ///  <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public async Task<List<BasicTallyObject>?> GetBasicObjectData(string ObjectType,
                                                                 string? company = null,
                                                                 string? fromDate = null,
                                                                 string? toDate = null,
                                                                 List<Filter>? filters = null)
    {
        string Resxml;
        company ??= Company;
        fromDate ??= FromDate;
        toDate ??= ToDate;
        StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate, SVExportFormat = "XML" };

        ReportField rootreportField = new(ObjectType, $"Cust{ObjectType}Collection".ToUpper(), ObjectType);

        GetTDLReport(typeof(BasicTallyObject), rootreportField);

        RequestEnvelope CusColEnvelope = new(HType.Data, $"LISTOF{ObjectType}".ToUpper(), sv);

        CusColEnvelope.Body.Desc.TDL.TDLMessage = new(rootreportField, filters);

        filters?.ForEach(filter => CusColEnvelope?.Body?.Desc?.TDL?.TDLMessage?.System?.Add(new(name: filter.FilterName!,
                                                                                                text: filter.FilterFormulae!)));

        string Reqxml = CusColEnvelope.GetXML();
        Resxml = await SendRequest(Reqxml);

        XmlAttributeOverrides xmlAttributeOverrides = new();
        //Adding xmlelement name according to RootElement name of ReturnObject
        xmlAttributeOverrides ??= new();
        XmlAttributes attrs = new();

        attrs.XmlElements.Add(new(ObjectType.ToUpper()));

        xmlAttributeOverrides.Add(typeof(CustomReportEnvelope<BasicTallyObject>), "Objects", attrs);

        var BasicObjects = GetObjfromXml<CustomReportEnvelope<BasicTallyObject>>(Resxml, xmlAttributeOverrides);
        return BasicObjects?.Objects;
    }


    public async Task<List<ReturnObjectType>?> GetObjectsfromTally<ReturnObjectType>(string? company = null,
                                                                                     string? fromDate = null,
                                                                                     string? toDate = null,
                                                                                     string? ColType = null,
                                                                                     string? childof = null,
                                                                                     List<string>? fetchList = null,
                                                                                     List<Filter>? filters = null,
                                                                                     YesNo isInitialize = YesNo.No,
                                                                                     XmlAttributeOverrides? xmlAttributeOverrides = null) where ReturnObjectType : BasicTallyObject
    {
        //If parameter is null Get value from instance
        company ??= Company;
        fetchList ??= new() { "GUID", "Masterid" };

        StaticVariables sv = new() { SVCompany = company, SVExportFormat = "XML", SVFromDate = fromDate, SVToDate = toDate };


        List<ReturnObjectType>? basicObjects = await GetNativeCollectionXML<ReturnObjectType>(Sv: sv,
                                                                                             ColType: ColType,
                                                                                             childof: childof,
                                                                                             NativeFields: fetchList,
                                                                                             filters: filters,
                                                                                             isInitialize: isInitialize,
                                                                                             TallyType: ColType?.ToUpper(),
                                                                                             xmlAttributeOverrides: xmlAttributeOverrides);
        basicObjects?.ForEach(Object =>
        {
            try
            {
                PropertyInfo? Aliasinfo = typeof(ReturnObjectType).GetProperty("Alias");
                if (Aliasinfo != null)
                {
                    List<LanguageNameList>? languageNameLists = (List<LanguageNameList>?)typeof(ReturnObjectType).GetProperty("LanguageNameList")?.GetValue(Object);
                    if (languageNameLists is not null && languageNameLists.Count > 0)
                    {
                        Aliasinfo.SetValue(Object, languageNameLists[0].LanguageAlias);
                    }
                }
                //Name
                PropertyInfo? NamePropertyinfo = typeof(ReturnObjectType).GetProperty("Name");
                var name = NamePropertyinfo?.GetValue(Object);
                if (name is null && NamePropertyinfo != null)
                {
                    NamePropertyinfo.SetValue(Object, typeof(ReturnObjectType).GetProperty("OldName")?.GetValue(Object));
                }
            }
            catch (Exception exc)
            {

                throw;
            }

        });
        return basicObjects;
    }

    /// <summary>
    /// Gets Existing Voucher from Tally based on LookupValue
    /// </summary>
    /// <typeparam name="ReturnType">Type must extend from Voucher </typeparam>
    /// <param name="LookupValue">Lookupvalue based on LookupField</param>
    /// <param name="LookupField">Lookupfield to use</param>
    /// <param name="isDynamicBal">If true openining balance and closing balance change depending on from date and to date</param>
    ///  <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally</param>
    /// <returns></returns>
    /// <exception cref="ObjectDoesNotExist"></exception>
    public async Task<ReturnType> GetObjectfromTally<ReturnType>(string LookupValue,
                                                                 VoucherLookupField LookupField = VoucherLookupField.MasterId,
                                                                 bool Isinventory = false,
                                                                 string? company = null,
                                                                 List<string>? fetchList = null,
                                                                 XmlAttributeOverrides? xmlAttributeOverrides = null) where ReturnType : Voucher
    {
        //If parameter is null Get value from instance
        company ??= Company;
        fetchList ??= new() { "MasterId", "*", "AllledgerEntries", "ledgerEntries", "Allinventoryenntries", "InventoryEntries", "InventoryEntriesIn", "InventoryEntriesOut" };

        StaticVariables sv = new() { SVCompany = company };
        sv.ViewName = Isinventory ? VoucherViewType.None : VoucherViewType.AccountingVoucherView;
        string filterformulae;
        if (LookupField is VoucherLookupField.MasterId or VoucherLookupField.AlterId)
        {
            filterformulae = $"${LookupField} = {LookupValue}";
        }
        else
        {
            filterformulae = $"${LookupField} = \"{LookupValue}\"";
        }
        List<Filter> filters = new() { new Filter() { FilterName = "masterfilter", FilterFormulae = filterformulae } };

        List<ReturnType>? objects = await GetNativeCollectionXML<ReturnType>(sv,
                                                                            NativeFields: fetchList,
                                                                            filters: filters,
                                                                            xmlAttributeOverrides: xmlAttributeOverrides);
        if (objects?.Count > 0)
        {
            var TallyObject = objects[0];

            return TallyObject;

        }
        else
        {
            throw new ObjectDoesNotExist(typeof(ReturnType).Name,
                                         LookupField.ToString(),
                                         LookupValue,
                                         company!);
        }


    }

    /// <summary>
    /// Gets Existing Masters from Tally based on LookupValue
    /// </summary>
    /// <typeparam name="ReturnType">Type must implement TallyBaseObject, ITallyObject </typeparam>
    /// <param name="LookupValue">Lookupvalue based on LookupField</param>
    /// <param name="LookupField">Lookupfield to use</param>
    /// <param name="isDynamicBal">If true openining balance and closing balance change depending on from date and to date</param>
    ///  <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally</param>
    /// <returns></returns>
    /// <exception cref="ObjectDoesNotExist"></exception>
    public async Task<ReturnType> GetObjectfromTally<ReturnType>(string LookupValue,
                                                                 MasterLookupField LookupField = MasterLookupField.Name,
                                                                 string? company = null,
                                                                 string? fromDate = null,
                                                                 string? toDate = null,
                                                                 List<string>? fetchList = null,
                                                                 XmlAttributeOverrides? xmlAttributeOverrides = null) where ReturnType : TallyXmlJson, ITallyObject
    {
        //If parameter is null Get value from instance
        company ??= Company;
        fromDate ??= FromDate;
        toDate ??= ToDate;
        fetchList ??= new() { "MasterId", "*" };

        StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
        string filterformulae;
        if (LookupField is MasterLookupField.MasterId or MasterLookupField.AlterId)
        {
            filterformulae = $"${LookupField} = {LookupValue}";
        }
        else
        {
            if (LookupField is MasterLookupField.Name && typeof(ReturnType).Name == typeof(Currency).Name)
            {
                filterformulae = $"$ORIGINALNAME = \"{LookupValue}\"";
            }
            else
            {
                filterformulae = $"${LookupField} = \"{LookupValue}\"";
            }
        }
        List<Filter> filters = new() { new Filter() { FilterName = "masterfilter", FilterFormulae = filterformulae } };

        List<ReturnType>? objects = await GetNativeCollectionXML<ReturnType>(Sv: sv,
                                                                            NativeFields: fetchList,
                                                                            filters: filters,
                                                                            xmlAttributeOverrides: xmlAttributeOverrides);
        if (objects?.Count > 0)
        {
            var TallyMaster = objects[0];
            //Alias
            PropertyInfo? Aliasinfo = typeof(ReturnType).GetProperty("Alias");
            if (Aliasinfo != null)
            {
                List<LanguageNameList>? languageNameLists = (List<LanguageNameList>?)typeof(ReturnType).GetProperty("LanguageNameList")?.GetValue(TallyMaster);
                Aliasinfo.SetValue(TallyMaster, languageNameLists?[0].LanguageAlias);
            }
            //Name
            PropertyInfo? NamePropertyinfo = typeof(ReturnType).GetProperty("Name");
            var name = NamePropertyinfo?.GetValue(TallyMaster);
            if (name is null && NamePropertyinfo != null)
            {
                NamePropertyinfo.SetValue(TallyMaster, typeof(ReturnType).GetProperty("OldName")?.GetValue(TallyMaster));
            }
            return TallyMaster;

        }
        else
        {
            throw new ObjectDoesNotExist(typeof(ReturnType).Name,
                                         LookupField.ToString(),
                                         LookupValue,
                                         company!);
        }


    }

    /// <summary>
    /// Able to create/Uptate/Delete tally objects
    /// </summary>
    /// <typeparam name="ObjectType">Type of Model You are sending</typeparam>
    /// <param name="Object">Instance of object</param>
    /// <param name="company">Company name to be sent</param>
    /// <param name="xmlAttributeOverrides"></param>
    /// <returns></returns>
    public async Task<PResult> PostObjectToTally<ObjectType>(ObjectType Object,
                                                             string? company = null,
                                                             XmlAttributeOverrides? xmlAttributeOverrides = null) where ObjectType : TallyXmlJson, ITallyObject
    {
        //If parameter is null Get value from instance
        company ??= Company;
        Object.PrepareForExport();
        Envelope<ObjectType> Objectenvelope = new(Object, new() { SVCompany = company });
        string ReqXml = Objectenvelope.GetXML(xmlAttributeOverrides);
        string RespXml = await SendRequest(ReqXml);
        PResult result = ParseResponse(RespXml);
        return result;
    }

    public async Task<List<ReturnObject>?> GetNativeCollectionXML<ReturnObject>(StaticVariables? Sv = null,
                                                                                string? ColType = null,
                                                                                string? childof = null,
                                                                                List<string>? NativeFields = null,
                                                                                List<Filter>? filters = null,
                                                                                List<string>? computevar = null,
                                                                                List<string>? compute = null,
                                                                                YesNo isInitialize = YesNo.No,
                                                                                string? TallyType = null,
                                                                                XmlAttributeOverrides? xmlAttributeOverrides = null) where ReturnObject : TallyXmlJson
    {
        string Resxml;

        //Gets Root attribute of ReturnObject
        XmlRootAttribute RootAttribute = (XmlRootAttribute)Attribute.GetCustomAttribute(typeof(ReturnObject), typeof(XmlRootAttribute))!;
        //ElementName of ReturnObject will match with TallyType
        TallyType ??= RootAttribute.ElementName;
        //ColType = CollectionMapping[typeof(ReturnObject).Name];
        string ColName = $"CUSTOM{TallyType}";

        RequestEnvelope ColEnvelope = new(HType.Collection, ColName, Sv); //Collection Envelope

        compute ??= new() { };
        var mapping = TallyObjectMapping.TallyObjectMappings.FirstOrDefault(map => map.TallyMasterType.Equals(TallyType, StringComparison.OrdinalIgnoreCase));
        if (mapping != null && mapping.ComputeFields != null)
        {
            compute.AddRange(mapping.ComputeFields);
        }
        ColEnvelope.Body.Desc.TDL.TDLMessage = new(colName: ColName,
                                                   colType: ColType ?? TallyType,
                                                   childof: childof,
                                                   nativeFields: NativeFields,
                                                   filters: filters,
                                                   computevar: computevar,
                                                   compute: compute,
                                                   isInitialize);


        string Reqxml = ColEnvelope.GetXML(); //Gets XML from Object

        Resxml = await SendRequest(Reqxml);

        //Adding xmlelement name according to RootElement name of ReturnObject
        xmlAttributeOverrides ??= new();
        XmlAttributes attrs = new();
        attrs.XmlElements.Add(new(TallyType));
        xmlAttributeOverrides.Add(typeof(Colllection<ReturnObject>), "Objects", attrs);

        Envelope<ReturnObject>? Envelope = GetObjfromXml<Envelope<ReturnObject>>(Resxml, xmlAttributeOverrides);
        return Envelope?.Body.Data.Collection?.Objects;
    }

    /// <summary>
    /// Gets Existing Group from Tally based on group name
    /// </summary>
    /// <typeparam name="GroupType">Must Extend from Group</typeparam>
    /// <param name="LookupValue">Specify the name of group/unique value of group to be fetched from Tally</param>
    /// <param name="LookupField">Specify the lookup field based on which to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// if field is in tally but it is not shown in Groupinstance then you need to extend Group model and specify that field</param>
    /// <returns>Returns instance of GroupType with data from tally</returns>
    public async Task<GroupType> GetGroup<GroupType>(string LookupValue,
                                                     MasterLookupField LookupField = MasterLookupField.Name,
                                                     string? company = null,
                                                     string? fromDate = null,
                                                     string? toDate = null,
                                                     List<string>? fetchList = null) where GroupType : Group
    {
        try
        {
            GroupType group = await GetObjectfromTally<GroupType>(LookupValue: LookupValue,
                                                      LookupField: LookupField,
                                                      company: company,
                                                      fromDate: fromDate,
                                                      toDate: toDate,
                                                      fetchList: fetchList);
            return group;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }


    /// <summary>
    /// Create/Alter/Delete Group in Tally by group name - Set Group.Action if you want to Alter/Delete existing Group
    /// </summary>
    /// <param name="group">Specify the instance of group to be Created/Altered/Deleted</param>
    /// <param name="company">Specify Company if not specified in Setup or different from setup</param>
    /// <returns>
    /// Returns Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    /// Presult.result will be empty if sucess
    ///  </returns>
    public async Task<PResult> PostGroup<GroupType>(GroupType group,
                                                    string? company = null,
                                                    XmlAttributeOverrides? xmlAttributeOverrides = null) where GroupType : Group
    {


        PResult result = await PostObjectToTally(Object: group,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }





    /// <summary>
    /// Gets Existing Ledger from Tally based on Ledger name
    /// </summary>
    /// <typeparam name="LedgerType">Must Extend from Ledger</typeparam>
    /// <param name="ledgerName">Specify the name of Ledger to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of LedgerType instance with data from tally</returns>
    public async Task<LedgerType> GetLedgerDynamic<LedgerType>(string LookupValue,
                                                               MasterLookupField LookupField = MasterLookupField.Name,
                                                               string? company = null,
                                                               string? fromDate = null,
                                                               string? toDate = null,
                                                               List<string>? fetchList = null) where LedgerType : Ledger
    {
        try
        {
            LedgerType Ledger = await GetObjectfromTally<LedgerType>(LookupValue: LookupValue,
                                                                     LookupField: LookupField,
                                                                     company: company,
                                                                     fromDate: fromDate,
                                                                     toDate: toDate,
                                                                     fetchList: fetchList);
            return Ledger;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }
    }

    /// <summary>
    /// Gets Existing Ledger from Tally based on Ledger name Opening balance is Static as per Master
    /// </summary>
    /// <typeparam name="LedgerType">Must Extend from Ledger</typeparam>
    /// <param name="ledgerName">Specify the name of Ledger to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of LedgerType instance with data from tally</returns>
    public async Task<LedgerType> GetLedger<LedgerType>(string LookupValue,
                                                    MasterLookupField LookupField = MasterLookupField.Name,
                                                    string? company = null,
                                                    List<string>? fetchList = null) where LedgerType : Ledger
    {
        try
        {
            LedgerType Ledger = await GetObjectfromTally<LedgerType>(LookupValue: LookupValue,
                                                                     LookupField: LookupField,
                                                                     company: company,
                                                                     fetchList: fetchList);
            return Ledger;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }
    }


    /// <summary>
    /// Create/Alter/Delete Ledger in Tally by Ledger name - Set Ledger.Action if you want to Alter/Delete existing Ledger
    /// </summary>
    /// <param name="ledger">Specify the instance of Ledger to be Created/Altered/Deleted</param>
    /// <param name="company">Specify Company if not specified in Setup or different from setup</param>
    /// <returns>
    /// Returns Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    /// Presult.result will be empty if sucess
    ///  </returns>
    public async Task<PResult> PostLedger<LedgerType>(LedgerType ledger,
                                          string? company = null,
                                          XmlAttributeOverrides? xmlAttributeOverrides = null) where LedgerType : Ledger
    {

        PResult result = await PostObjectToTally(Object: ledger,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }

    /// <summary>
    /// Gets CostCategory from Tally based on CostCategory name
    /// </summary>
    /// <param name="CostCategoryName">Specify the name of CostCategory to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.CostCategory instance with data from tally</returns>
    public async Task<CostCategory> GetCostCategory<CostCategoryType>(string LookupValue,
                                                    MasterLookupField LookupField = MasterLookupField.Name,
                                                    string? company = null,
                                                    string? fromDate = null,
                                                    string? toDate = null,
                                                    List<string>? fetchList = null) where CostCategoryType : CostCategory
    {
        try
        {
            CostCategoryType CostCategory = await GetObjectfromTally<CostCategoryType>(LookupValue: LookupValue,
                                                                                       LookupField: LookupField,
                                                                                       company: company,
                                                                                       fromDate: fromDate,
                                                                                       toDate: toDate,
                                                                                       fetchList: fetchList);
            return CostCategory;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }

    /// <summary>
    /// Create/Alter/Delete Costcategory in Tally
    /// To Alter/Delete existing CostCategory set CostCategory.Action to Alter/Delete
    /// </summary>
    /// <param name="CostCategory">Send Models.Costcategory</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostCostCategory<CostCategoryType>(CostCategory CostCategory,
                                                string? company = null,
                                                XmlAttributeOverrides? xmlAttributeOverrides = null) where CostCategoryType : CostCategory
    {
        PResult result = await PostObjectToTally(Object: CostCategory,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }




    /// <summary>
    /// Gets CostCenter from Tally based on CostCenter name
    /// </summary>
    /// <param name="CostCenterName">Specify the name of CostCenter to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.CostCenter instance with data from tally</returns>
    public async Task<CostCenter> GetCostCenter<CostCenterType>(string LookupValue,
                                                MasterLookupField LookupField = MasterLookupField.Name,
                                                string? company = null,
                                                string? fromDate = null,
                                                string? toDate = null,
                                                List<string>? fetchList = null) where CostCenterType : CostCenter
    {
        try
        {
            CostCenterType CostCenter = await GetObjectfromTally<CostCenterType>(LookupValue: LookupValue,
                                                                                 LookupField: LookupField,
                                                                                 company: company,
                                                                                 fromDate: fromDate,
                                                                                 toDate: toDate,
                                                                                 fetchList: fetchList);
            return CostCenter;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }


    /// <summary>
    /// Create/Alter/Delete CostCenter in Tally
    /// To Alter/Delete existing CostCenter set CostCenter.Action to Alter/Delete
    /// </summary>
    /// <param name="costCenter">Send Models.costCenter</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostCostCenter<CostCenterType>(CostCenter costCenter,
                                              string? company = null,
                                              XmlAttributeOverrides? xmlAttributeOverrides = null) where CostCenterType : CostCenter
    {


        PResult result = await PostObjectToTally(Object: costCenter,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }



    /// <summary>
    /// Gets StockGroup from Tally based on StockGroup name
    /// </summary>
    /// <param name="StockGroupName">Specify the name of StockGroup to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.StockGroup instance with data from tally</returns>
    public async Task<StockGroup> GetStockGroup<StockGroupType>(string LookupValue,
                                                MasterLookupField LookupField = MasterLookupField.Name,
                                                string? company = null,
                                                string? fromDate = null,
                                                string? toDate = null,
                                                List<string>? fetchList = null) where StockGroupType : StockGroup
    {
        try
        {
            StockGroupType StockGroup = await GetObjectfromTally<StockGroupType>(LookupValue: LookupValue,
                                                                                 LookupField: LookupField,
                                                                                 company: company,
                                                                                 fromDate: fromDate,
                                                                                 toDate: toDate,
                                                                                 fetchList: fetchList);
            return StockGroup;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }


    /// <summary>
    /// Create/Alter/Delete StockGroup in Tally
    /// To Alter/Delete existing StockGroup set StockGroup.Action to Alter/Delete
    /// </summary>
    /// <param name="stockGroup">Send Models.StockGroup</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostStockGroup<StockGroupType>(StockGroup stockGroup,
                                              string? company = null,
                                              XmlAttributeOverrides? xmlAttributeOverrides = null) where StockGroupType : StockGroup
    {

        PResult result = await PostObjectToTally(Object: stockGroup,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }



    /// <summary>
    /// Gets StockCategory from Tally based on StockCategory name
    /// </summary>
    /// <param name="StockCategoryName">Specify the name of StockCategory to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.StockCategory with data from tally</returns>
    public async Task<StockCategory> GetStockCategory<StockCategoryType>(string LookupValue,
                                                      MasterLookupField LookupField = MasterLookupField.Name,
                                                      string? company = null,
                                                      string? fromDate = null,
                                                      string? toDate = null,
                                                      List<string>? fetchList = null) where StockCategoryType : StockCategory
    {
        try
        {
            StockCategoryType StockCategory = await GetObjectfromTally<StockCategoryType>(LookupValue: LookupValue,
                                                                                          LookupField: LookupField,
                                                                                          company: company,
                                                                                          fromDate: fromDate,
                                                                                          toDate: toDate,
                                                                                          fetchList: fetchList);
            return StockCategory;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }


    /// <summary>
    /// Create/Alter/Delete StockCategory in Tally
    /// To Alter/Delete existing StockCategory set StockCategory.Action to Alter/Delete
    /// </summary>
    /// <param name="stockCategory">Send Models.StockGroup</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostStockCategory<StockCategoryType>(StockCategory stockCategory,
                                                                    string? company = null,
                                                                    XmlAttributeOverrides? xmlAttributeOverrides = null) where StockCategoryType : StockCategory
    {
        PResult result = await PostObjectToTally(Object: stockCategory,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }


    /// <summary>
    /// Gets StockItem from Tally based on StockItem name
    /// </summary>
    /// <param name="StockItemName">Specify the name of StockItem to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.StockItem  with data from tally</returns>
    public async Task<StockItem> GetStockItem<StockItemType>(string LookupValue,
                                                             MasterLookupField LookupField = MasterLookupField.Name,
                                                             string? company = null,
                                                             string? fromDate = null,
                                                             string? toDate = null,
                                                             List<string>? fetchList = null) where StockItemType : StockItem
    {
        try
        {
            StockItemType StockItem = await GetObjectfromTally<StockItemType>(LookupValue: LookupValue,
                                                                              LookupField: LookupField,
                                                                              company: company,
                                                                              fromDate: fromDate,
                                                                              toDate: toDate,
                                                                              fetchList: fetchList);
            return StockItem;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }

    /// <summary>
    /// Create/Alter/Delete StockItem in Tally
    /// To Alter/Delete existing StockItem set StockItem.Action to Alter/Delete
    /// </summary>
    /// <param name="stockItem">Send Models.StockItem</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostStockItem<StockItemType>(StockItem stockItem,
                                             string? company = null,
                                             XmlAttributeOverrides? xmlAttributeOverrides = null) where StockItemType : StockItem
    {

        PResult result = await PostObjectToTally(Object: stockItem,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }



    /// <summary>
    /// Gets Unit from Tally based on Unit name
    /// </summary>
    /// <param name="UnitName">Specify the name of Unit to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.Unit  with data from tally</returns>
    public async Task<Unit> GetUnit<UnitType>(string LookupValue,
                                              MasterLookupField LookupField = MasterLookupField.Name,
                                              string? company = null,
                                              string? fromDate = null,
                                              string? toDate = null,
                                              List<string>? fetchList = null) where UnitType : Unit
    {
        try
        {
            UnitType Unit = await GetObjectfromTally<UnitType>(LookupValue: LookupValue,
                                                               LookupField: LookupField,
                                                               company: company,
                                                               fromDate: fromDate,
                                                               toDate: toDate,
                                                               fetchList: fetchList);
            return Unit;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }


    /// <summary>
    /// Create/Alter/Delete Unit in Tally,
    /// To Alter/Delete existing Unit set Unit.Action to Alter/Delete
    /// </summary>
    /// <param name="unit">Send Models.Unit</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostUnit<UnitType>(Unit unit,
                                        string? company = null,
                                        XmlAttributeOverrides? xmlAttributeOverrides = null) where UnitType : Unit
    {


        PResult result = await PostObjectToTally(Object: unit,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }


    /// <summary>
    /// Gets Godown from Tally based on <strong>Godown name</strong>
    /// </summary>
    /// <param name="GodownName">Specify the name of Godown to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.Godown  with data from tally</returns>
    public async Task<Godown> GetGodown<GodownType>(string LookupValue,
                                        MasterLookupField LookupField = MasterLookupField.Name,
                                        string? company = null,
                                        string? fromDate = null,
                                        string? toDate = null,
                                        List<string>? fetchList = null) where GodownType : Godown
    {
        try
        {
            GodownType Godown = await GetObjectfromTally<GodownType>(LookupValue: LookupValue,
                                                                     LookupField: LookupField,
                                                                     company: company,
                                                                     fromDate: fromDate,
                                                                     toDate: toDate,
                                                                     fetchList: fetchList);
            return Godown;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }

    /// <summary>
    /// Create/Alter/Delete Godown in Tally,
    /// To Alter/Delete existing Godown set Godown.Action to Alter/Delete
    /// </summary>
    /// <param name="godown">Send Models.Godown</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostGodown<GodownType>(Godown godown,
                                          string? company = null,
                                          XmlAttributeOverrides? xmlAttributeOverrides = null) where GodownType : Godown
    {
        PResult result = await PostObjectToTally(Object: godown,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }



    /// <summary>
    /// Gets VoucherType from Tally based on <strong>VoucherType name</strong>
    /// </summary>
    /// <param name="VoucherTypeName">Specify the name of VoucherType to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.VoucherType  with data from tally</returns>
    public async Task<VoucherType> GetVoucherType<VchrType>(string LookupValue,
                                                  MasterLookupField LookupField = MasterLookupField.Name,
                                                  string? company = null,
                                                  string? fromDate = null,
                                                  string? toDate = null,
                                                  List<string>? fetchList = null) where VchrType : VoucherType
    {
        try
        {
            VchrType VoucherType = await GetObjectfromTally<VchrType>(LookupValue: LookupValue,
                                                                      LookupField: LookupField,
                                                                      company: company,
                                                                      fromDate: fromDate,
                                                                      toDate: toDate,
                                                                      fetchList: fetchList);
            return VoucherType;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }


    /// <summary>
    /// Create/Alter/Delete VoucherType in Tally,
    /// To Alter/Delete existing VoucherType set VoucherType.Action to Alter/Delete
    /// </summary>
    /// <param name="voucherType">Send Models.VoucherType</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostVoucherType<VoucherTypType>(VoucherType voucherType,
                                               string? company = null,
                                               XmlAttributeOverrides? xmlAttributeOverrides = null) where VoucherTypType : VoucherType
    {
        PResult result = await PostObjectToTally(Object: voucherType,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }


    /// <summary>
    /// Gets Currency from Tally based on <strong>Currency name</strong>
    /// </summary>
    /// <param name="CurrencyName">Specify the name of Currency to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.Currency  with data from tally</returns>
    public async Task<Currency> GetCurrency<CurrencyType>(string LookupValue,
                                            MasterLookupField LookupField = MasterLookupField.Name,
                                            string? company = null,
                                            string? fromDate = null,
                                            string? toDate = null,
                                            List<string>? fetchList = null) where CurrencyType : Currency
    {
        try
        {
            CurrencyType Currency = await GetObjectfromTally<CurrencyType>(LookupValue: LookupValue,
                                                                           LookupField: LookupField,
                                                                           company: company,
                                                                           fromDate: fromDate,
                                                                           toDate: toDate,
                                                                           fetchList: fetchList);
            return Currency;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }
    }

    /// <summary>
    /// Create/Alter/Delete VoucherType in Tally,
    /// To Alter/Delete existing VoucherType set VoucherType.Action to Alter/Delete
    /// </summary>
    /// <param name="currency">Send Models.VoucherType</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostCurrency<CurrencyType>(Currency currency,
                                        string? company = null, XmlAttributeOverrides? xmlAttributeOverrides = null) where CurrencyType : Currency
    {
        PResult result = await PostObjectToTally(Object: currency,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }


    /// <summary>
    /// Gets AttendanceType from Tally based on <strong>AttendanceType name</strong>
    /// </summary>
    /// <param name="AttendanceType">Specify the name of AttendanceType to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.AttendanceType  with data from tally</returns>
    public async Task<AttendanceType> GetAttendanceType<AttendnceType>(string LookupValue,
                                                        MasterLookupField LookupField = MasterLookupField.Name,
                                                        string? company = null,
                                                        string? fromDate = null,
                                                        string? toDate = null,
                                                        List<string>? fetchList = null) where AttendnceType : AttendanceType
    {
        try
        {
            AttendnceType AttendanceType = await GetObjectfromTally<AttendnceType>(LookupValue: LookupValue,
                                                                                   LookupField: LookupField,
                                                                                   company: company,
                                                                                   fromDate: fromDate,
                                                                                   toDate: toDate,
                                                                                   fetchList: fetchList);
            return AttendanceType;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }


    /// <summary>
    /// Create/Alter/Delete AttendanceType in Tally,
    /// To Alter/Delete existing AttendanceType set AttendanceType.Action to Alter/Delete
    /// </summary>
    /// <param name="AttendanceType">Send Models.AttendanceType</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be empty if sucess 
    /// </returns>
    public async Task<PResult> PostAttendanceType<AttendanceTypType>(AttendanceType AttendanceType,
                                                                     string? company = null,
                                                                     XmlAttributeOverrides? xmlAttributeOverrides = null) where AttendanceTypType : AttendanceType
    {
        PResult result = await PostObjectToTally(Object: AttendanceType,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }




    /// <summary>
    /// Gets CostCenter from Tally based on CostCenter name
    /// </summary>
    /// <param name="EmployeeGroupName">Specify the name of EmployeeGroupName to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.CostCenter instance with data from tally</returns>
    public async Task<EmployeeGroup> GetEmployeeGroup<EmployeeGroupType>(string LookupValue,
                                                      MasterLookupField LookupField = MasterLookupField.Name,
                                                      string? company = null,
                                                      string? fromDate = null,
                                                      string? toDate = null,
                                                      List<string>? fetchList = null) where EmployeeGroupType : EmployeeGroup
    {
        try
        {
            EmployeeGroupType EmployeeGroup = await GetObjectfromTally<EmployeeGroupType>(LookupValue: LookupValue,
                                                                                          LookupField: LookupField,
                                                                                          company: company,
                                                                                          fromDate: fromDate,
                                                                                          toDate: toDate,
                                                                                          fetchList: fetchList);
            return EmployeeGroup;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }

    ///// <summary>
    ///// Create/Alter/Delete EmployeeGroup in Tally
    ///// To Alter/Delete existing EmployeeGroup set EmployeeGroup.Action to Alter/Delete
    ///// </summary>
    ///// <param name="EmployeeGroup">Send Models.EmployeeGroup</param>
    ///// <param name="company">if not specified company is taken from instance</param>
    ///// <returns> Models.PResult if Presult.Status can be sucess or failure,
    ///// Presult.result will have failure message incase of failure,
    /////  Presult.result will be empty if sucess 
    ///// </returns>
    public async Task<PResult> PostEmployeeGroup<EmployeeGroupType>(EmployeeGroup EmployeeGroup,
                                                                    string? company = null,
                                                                    XmlAttributeOverrides? xmlAttributeOverrides = null) where EmployeeGroupType : EmployeeGroup
    {
        PResult result = await PostObjectToTally(Object: EmployeeGroup,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }


    /// <summary>
    /// Gets CostCenter from Tally based on CostCenter name
    /// </summary>
    /// <param name="EmployeeGroupName">Specify the name of EmployeeGroupName to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.CostCenter instance with data from tally</returns>
    public async Task<Employee> GetEmployee<EmployeeType>(string LookupValue,
                                                          MasterLookupField LookupField = MasterLookupField.Name,
                                                          string? company = null,
                                                          string? fromDate = null,
                                                          string? toDate = null,
                                                          List<string>? fetchList = null) where EmployeeType : Employee
    {
        try
        {
            EmployeeType Employee = await GetObjectfromTally<EmployeeType>(LookupValue: LookupValue,
                                                                           LookupField: LookupField,
                                                                           company: company,
                                                                           fromDate: fromDate,
                                                                           toDate: toDate,
                                                                           fetchList: fetchList);
            return Employee;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }

    ///// <summary>
    ///// Create/Alter/Delete Employee in Tally
    ///// To Alter/Delete existing Employee set Employee.Action to Alter/Delete
    ///// </summary>
    ///// <param name="Employee">Send Models.Employee</param>
    ///// <param name="company">if not specified company is taken from instance</param>
    ///// <returns> Models.PResult if Presult.Status can be sucess or failure,
    ///// Presult.result will have failure message incase of failure,
    /////  Presult.result will be empty if sucess 
    ///// </returns>
    public async Task<PResult> PostEmployee<EmployeeType>(Employee Employee,
                                                          string? company = null,
                                                          XmlAttributeOverrides? xmlAttributeOverrides = null) where EmployeeType : Employee
    {
        PResult result = await PostObjectToTally(Object: Employee,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }



    /// <summary>
    /// Gets Voucher from Tally based on criteria
    /// </summary>
    /// <param name="EmployeeGroupName">Specify the name of EmployeeGroupName to be fetched from Tally</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    /// <param name="toDate">Specify toDate if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.CostCenter instance with data from tally</returns>
    public async Task<Voucher> GetVoucher<VchType>(string LookupValue,
                                          VoucherLookupField LookupField = VoucherLookupField.VoucherNumber,
                                          string? company = null,
                                          List<string>? fetchList = null) where VchType : Voucher
    {
        try
        {
            VchType Voucher = await GetObjectfromTally<VchType>(LookupValue: LookupValue,
                                                                LookupField: LookupField,
                                                                company: company,
                                                                fetchList: fetchList);
            return Voucher;
        }
        catch (ObjectDoesNotExist)
        {
            throw;
        }

    }


    /// <summary>
    /// Gets Voucher based on <strong>Voucher number and Date</strong>
    /// </summary>
    /// <param name="VoucherNumber">Specify MasterID based on which vouchers to be fetched</param>
    /// <param name="Date">Specify voucher Date</param>
    /// <param name="company">Specify Company if not specified in Setup</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <returns>Returns instance of Models.Voucher with data from tally</returns>
    public async Task<Voucher> GetVoucherByVoucherNumber(string VoucherNumber,
                                                         string Date,
                                                         string? company = null,
                                                         List<string>? fetchList = null)
    {
        //If parameter is null Get value from instance
        company ??= Company;

        VoucherEnvelope? VchEnvelope = (await GetObjFromTally<VoucherEnvelope>(ObjName: $"Date: \'{Date}\' : VoucherNumber: \'{VoucherNumber}\'",
                                                                         ObjType: "Voucher",
                                                                         company: company,
                                                                         fetchList: fetchList,
                                                                         viewname: VoucherViewType.AccountingVoucherView));

        if (VchEnvelope?.Body.Data.Message.Voucher != null)
        {
            Voucher voucher = VchEnvelope.Body.Data.Message.Voucher;
            return voucher;
        }
        else
        {
            throw new ObjectDoesNotExist("Voucher");
        }
    }


    /// <summary>
    /// Create/Alter/Delete voucher in Tally,
    /// To Alter/Delete existing voucher set voucher.Action to Alter/Delete
    /// </summary>
    /// <param name="voucher">Send Models.VoucherType</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <returns> Models.PResult if Presult.Status can be sucess or failure,
    /// Presult.result will have failure message incase of failure,
    ///  Presult.result will be Voucher masterID if sucess 
    /// </returns>
    public async Task<PResult> PostVoucher<TVoucher>(Voucher voucher,
                                           string? company = null,
                                           XmlAttributeOverrides? xmlAttributeOverrides = null) where TVoucher : Voucher
    {


        xmlAttributeOverrides ??= new XmlAttributeOverrides();

        if (voucher.View != VoucherViewType.AccountingVoucherView)
        {
            XmlAttributes xmlattribute = new();
            xmlattribute.XmlElements.Add(new XmlElementAttribute() { ElementName = "LEDGERENTRIES.LIST" });
            xmlAttributeOverrides.Add(typeof(Voucher), "Ledgers", xmlattribute);
        }
        PResult result = await PostObjectToTally(Object: voucher,
                                                 company: company,
                                                 xmlAttributeOverrides: xmlAttributeOverrides);

        return result;
    }




    public void GetBasicVoucherData()
    {
        //Get
    }

    //#region Reports
    ///// <summary>
    ///// Gets List of Vouchers  based on <strong>Voucher Type</strong>
    ///// </summary>
    ///// <param name="VoucherType">Specify the name of VoucherType based on which vouchers to be fetched</param>
    ///// <param name="company">Specify Company if not specified in Setup</param>
    ///// <param name="fromDate">Specify fromDate if not specified in Setup</param>
    ///// <param name="toDate">Specify toDate if not specified in Setup</param>
    ///// <returns>Returns instance of Models.VouchersList with data from tally</returns>
    //public async Task<VouchersList> GetVouchersListByVoucherType(string VoucherType,
    //                                                             string company = null,
    //                                                             string fromDate = null,
    //                                                             string toDate = null)
    //{
    //    company ??= Company;

    //    Dictionary<string, string> fields = new() { { "$MASTERID", "MASTERID" }, { "$VoucherNumber", "VoucherNumber" }, { "$Date", "Date" } };
    //    StaticVariables staticVariables = new() { SVCompany = company, SVExportFormat = "XML", SVFromDate = fromDate, SVToDate = toDate };
    //    List<string> VoucherFilters = new() { "VoucherType" };
    //    List<string> VoucherSystemFilters = new() { $"$VoucherTypeName = \"{VoucherType}\"" };
    //    //string VouchersXml = await GetCustomReportXML(rName: "List Of Vouchers", Fields: fields, colType: "Voucher", Sv: staticVariables,
    //    //    Filters: VoucherFilters, SystemFilters: VoucherSystemFilters);

    //    VouchersList vl = GetNativeCollectionXML<VouchersList>();
    //    return vl;
    //}



    //#endregion
    /// <summary>
    /// Retrives Data from Tally in Objects 
    /// </summary>
    /// <typeparam name="T"> Object Type to be returned</typeparam>
    /// <param name="ObjName">Name of the object to be returned from tally</param>
    /// <param name="ObjType">Type of object to be returned from tally</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <param name="fromDate">if not specified fromDate is taken from instance</param>
    /// <param name="toDate">if not specified toDate is taken from instance</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <param name="viewname">if getting voucher object specify view name else leave</param>
    /// <returns>Return object type provided in typeparam with data from tally</returns>
    public async Task<T?> GetObjFromTally<T>(string ObjName,
                                            string ObjType,
                                            string? company = null,
                                            string? fromDate = null,
                                            string? toDate = null,
                                            List<string>? fetchList = null,
                                            VoucherViewType viewname = VoucherViewType.AccountingVoucherView)
    {
        //If parameter is null Get value from instance
        company ??= Company;
        fromDate ??= FromDate;
        toDate ??= ToDate;
        T? Obj = default;
        string ResXml = string.Empty;
        try
        {
            string ReqXml = GetObjXML(objType: ObjType,
                                      ObjName: ObjName,
                                      company: company,
                                      fromDate: fromDate,
                                      toDate: toDate,
                                      fetchList: fetchList,
                                      viewname: viewname);
            ResXml = await SendRequest(ReqXml);
            Obj = GetObjfromXml<T>(ResXml);
        }
        catch (Exception e)
        {
            Logger.LogError("Error ocuured while converting object from xml - {ResXml}", ResXml);
            Logger.LogError("Errrr - {Msg}", e.Message);
        }
        return Obj;
    }


    /// <summary>
    /// Creates xml to fetch objects from Tally
    /// </summary>
    /// <param name="objType">Type of object to be returned from tally</param>
    /// <param name="ObjName">Name of the object to be returned from tally</param>
    /// <param name="company">if not specified company is taken from instance</param>
    /// <param name="fromDate">if not specified fromDate is taken from instance</param>
    /// <param name="toDate">if not specified toDate is taken from instance</param>
    /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
    /// </param>
    /// <param name="viewname">if getting voucher object specify view name else leave</param>
    /// <returns>returns xml as string</returns>
    private string GetObjXML(string objType,
                             string ObjName,
                             string? company = null,
                             string? fromDate = null,
                             string? toDate = null,
                             List<string>? fetchList = null,
                             VoucherViewType viewname = VoucherViewType.AccountingVoucherView)
    {
        //If parameter is null Get value from instance
        company ??= Company;
        fromDate ??= FromDate;
        toDate ??= ToDate;

        ObjEnvelope Obj = new();
        Obj.Header = new(objType, ObjName);
        StaticVariables staticVariables = new()
        {
            SVCompany = company,
            SVFromDate = fromDate,
            SVToDate = toDate,
            SVExportFormat = "XML",
            ViewName = viewname

        };
        Obj.Body.Desc.StaticVariables = staticVariables;

        Obj.Body.Desc.FetchList = fetchList != null ? new(fetchList) : new();
        string ObjXML = Obj.GetXML();
        return ObjXML;
    }



    public PropertyInfo[] GetPropertyInfo(Type type)
    {
        PropertyInfo[] PropertyInfo;
        _ = PropertyInfoList.TryGetValue(type, out PropertyInfo!);
        if (PropertyInfo == null)
        {
            PropertyInfo = type.GetProperties();
            PropertyInfoList[type] = PropertyInfo;
        };
        return PropertyInfo;
    }

    public void GetObjectfromTallyBasedonClass(Type type)
    {

        string RootTag = GetRootTag(type);

        ReportField rootreportField = new(RootTag);

        GetTDLReport(type, rootreportField);

        RequestEnvelope CusColEnvelope = new(HType.Data, $"LISTOF{RootTag}");
        CusColEnvelope.Body.Desc.TDL.TDLMessage = new(rootreportField);
        string xml = CusColEnvelope.GetXML();
        //string Rxml = await SendRequest(xml);

    }

    public void GetTDLReport(Type type, ReportField rootreportField)
    {
        List<Type> IgnoreTypes = new() { typeof(string), typeof(int), typeof(int?) };
        PropertyInfo[] propertyInfoList = GetPropertyInfo(type);
        foreach (PropertyInfo propertyinfo in propertyInfoList)
        {
            Type Ctype = propertyinfo.PropertyType;

            if (Ctype.IsGenericType && (Ctype.GetGenericTypeDefinition() == typeof(List<>)))
            {
                Type ChildType = Ctype.GetGenericArguments()[0];
                if (!ChildType.IsPrimitive)
                {
                    string ColName = GetTDLCollectionName(propertyinfo)!;
                    string xmlElem = GetXmlElement(propertyinfo)!;
                    ReportField ChildreportField = new(xmlElem, ColName);
                    if (!IgnoreTypes.Contains(Ctype))
                    {
                        ChildreportField.FieldName = $"{rootreportField.FieldName?.Substring(0, 5)}_{ChildreportField.FieldName}";
                        GetTDLReport(ChildType, ChildreportField);
                    }
                    rootreportField.SubFields?.Add(ChildreportField);
                }

            }
            else if (!Ctype.IsPrimitive && !Ctype.IsEnum && !IgnoreTypes.Contains(Ctype))
            {
                GetChildReport(propertyinfo, rootreportField);
            }
            else
            {
                GetReportFields(rootreportField, propertyinfo);
            }

        }
    }



    private void GetChildReport(PropertyInfo propertyinfo, ReportField rootreportField)
    {
        string xmlElem = GetXmlElement(propertyinfo)!;
        if (xmlElem != null)
        {
            string ColName = GetTDLCollectionName(propertyinfo) ?? rootreportField.FieldName!;
            ReportField ChildreportField = new(xmlElem, ColName);
            ChildreportField.FieldName = $"{rootreportField.FieldName?.Substring(0, 5)}_{ChildreportField.FieldName}";
            GetTDLReport(propertyinfo.PropertyType, ChildreportField);
            rootreportField.SubFields?.Add(ChildreportField);
        }

    }

    //private void GetChildReport(Type ChildType, string xmlTag, ReportField rootreportField)
    //{
    //    PropertyInfo[] childpropertyinfo = GetPropertyInfo(ChildType);
    //    string ColName = GetTDLCollectionName(ChildType);
    //    ReportField subrootReportField = new(xmlTag, ColName);
    //    foreach (PropertyInfo chilPropertyinfo in childpropertyinfo)
    //    {
    //        Type Ctype = chilPropertyinfo.PropertyType;
    //        if (Ctype.IsGenericType && (Ctype.GetGenericTypeDefinition() == typeof(List<>)))
    //        {
    //            Type DChildType = Ctype.GetGenericArguments()[0];

    //            if (!DChildType.IsPrimitive)
    //            {
    //                string XmlTag = GetXmlElement(chilPropertyinfo);
    //                if (DChildType == typeof(string))
    //                {
    //                    string ChColName = GetTDLCollectionName(ChildType);
    //                    ReportField ChsubrootReportField = new(XmlTag, ChColName);
    //                    subrootReportField.SubFields.Add(ChsubrootReportField);
    //                }
    //                else
    //                {
    //                    GetChildReport(ChildType: ChildType, XmlTag, rootreportField: rootreportField);
    //                }
    //            }
    //        }
    //        else if (!Ctype.IsPrimitive && !Ctype.IsEnum && Ctype != typeof(string))
    //        {
    //            string rootTag = GetXmlElement(chilPropertyinfo);
    //            if (rootTag != null)
    //            {
    //                string XmlTag = GetXmlElement(chilPropertyinfo);
    //                GetChildReport(ChildType: Ctype, XmlTag, rootreportField: subrootReportField);
    //            }
    //        }
    //        else
    //        {
    //            GetReportFields(subrootReportField, chilPropertyinfo);
    //        }

    //    }
    //    rootreportField.SubFields.Add(subrootReportField);
    //}

    private static string GetRootTag(Type type)
    {
        XmlRootAttribute? Rootattribute = (XmlRootAttribute?)Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute));
        string RootTag = Rootattribute?.ElementName ?? string.Empty;
        return RootTag;
    }
    private static string? GetTDLCollectionName(Type type)
    {
        TDLCollectionAttribute? TDLColattribute = (TDLCollectionAttribute?)Attribute.GetCustomAttribute(type, typeof(TDLCollectionAttribute));
        string? CollectionName = TDLColattribute?.CollectionName;
        return CollectionName;
    }

    private string? GetTDLCollectionName(PropertyInfo propertyinfo)
    {
        TDLCollectionAttribute? TDLColattribute = (TDLCollectionAttribute?)propertyinfo.GetCustomAttributes().FirstOrDefault(Attribute => Attribute.GetType() == typeof(TDLCollectionAttribute));
        string? CollectionName = TDLColattribute?.CollectionName;
        return CollectionName;
    }
    private static void GetReportFields(ReportField rootreportField, PropertyInfo propertyinfo)
    {
        string xmlTag = GetXmlElement(propertyinfo)!;
        if (xmlTag != null)
        {
            rootreportField.SubFields?.Add(new ReportField(xmlTag));
        }
        string? xmlAttr = GetXmlAttribute(propertyinfo);
        if (xmlAttr != null)
        {
            rootreportField.Atrributes.Add(xmlAttr);
        }

    }

    private static string? GetXmlAttribute(PropertyInfo propertyinfo)
    {
        XmlAttributeAttribute? Cattribute = (XmlAttributeAttribute?)Attribute.GetCustomAttribute(propertyinfo, typeof(XmlAttributeAttribute));//propertyinfo.CustomAttributes.FirstOrDefault(Attributedata => Attributedata.AttributeType == typeof(XmlAttributeAttribute));

        string? xmlAttr = Cattribute?.AttributeName;
        return xmlAttr;
    }

    private static string? GetXmlElement(PropertyInfo propertyinfo)
    {
        XmlElementAttribute[] CElement = (XmlElementAttribute[])Attribute.GetCustomAttributes(propertyinfo, typeof(XmlElementAttribute));//propertyinfo.CustomAttributes.FirstOrDefault(Attributedata => Attributedata.AttributeType == typeof(XmlAttributeAttribute));
        if (CElement.Length > 0)
        {
            string xmlTag = CElement[0].ElementName;
            return xmlTag;
        }
        return null;
    }

    public async Task<string?> GetReportXML(string reportname, StaticVariables? Sv = null)
    {
        string? Resxml = null;
        await Check();
        if (Status == "Running")
        {
            string RName = reportname;
            RequestEnvelope ColEnvelope = new(HType.Data, RName, Sv); //Collection Envelope

            string Reqxml = ColEnvelope.GetXML(); //Gets XML from Object
            Resxml = await SendRequest(Reqxml);
        }
        return Resxml;
    }


    /// <summary>
    /// Posts XML to tally
    /// </summary>
    /// <param name="SXml">XML to be posted to tally</param>
    /// <returns>Response received from Tally</returns>
    /// <exception cref="TallyConnectivityException">If tally is not opened or not configured correctly</exception>
    public async Task<string> SendRequest(string SXml)
    {
        string Resxml;

        try
        {
            CLogger.TallyRequest(SXml);
            SXml = SXml.Replace("\t", "&#09;");
            StringContent TXML = new(SXml, Encoding.UTF8, "application/xml");
            HttpResponseMessage Res = await client.PostAsync(FullURL, TXML);
            Res.EnsureSuccessStatusCode();
            var resp = await Res.Content.ReadAsStreamAsync();
            using StreamReader streamReader = new StreamReader(resp, Encoding.UTF8);
            Resxml = streamReader.ReadToEnd();
            //var byteArray = await Res.Content.ReadAsByteArrayAsync();
            //Resxml = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length); ;
            Resxml = ReplaceXMLText(Resxml);
            CLogger.TallyResponse(Resxml);
            return Resxml;
        }
        catch (Exception e)
        {
            ReqStatus = e.Message;
            CLogger.TallyReqError(e.Message);
            throw new TallyConnectivityException("Tally is not running", FullURL);
        }

    }

    //Helper method to escape text for xml
    public static string ReplaceXML(string strText)
    {
        string result = string.Empty;
        if (strText != null)
        {
            result = strText.Replace("\r", "&#13;");
            result = result.Replace("\n", "&#10;");

        }
        return result;
    }

    //Helper method to convert escaped characters to text
    public static string ReplaceXMLText(string strXmlText)
    {
        string result = string.Empty;
        if (strXmlText != null)
        {
            result = strXmlText.Replace("&#x4;", "");
            result = result.Replace("&#4;", "");
            //result = strXmlText.Replace("&amp;", "&");
            //result = result.Replace( "&apos;", "'");
            //result = result.Replace("&quot;","\"\"");
            //result = result.Replace("&gt;", ">");
        }
        return result;
    }

    //Converts to given object from Xml
    public T? GetObjfromXml<T>(string Xml, XmlAttributeOverrides? attrOverrides = null)
    {
        try
        {
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            Xml = System.Text.RegularExpressions.Regex.Replace(Xml, re, "");
            XmlSerializer XMLSer = attrOverrides == null ? new(typeof(T)) : new(typeof(T), attrOverrides);

            NameTable nt = new();
            XmlNamespaceManager nsmgr = new(nt);
            nsmgr.AddNamespace("UDF", "TallyUDF");
            XmlParserContext context = new(null, nsmgr, null, XmlSpace.None);

            XmlReaderSettings xset = new()
            {
                CheckCharacters = false,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            XmlReader rd = XmlReader.Create(new StringReader(Xml), xset, context);
            //StringReader XmlStream = new StringReader(Xml);
            if (typeof(T).Name.Contains("VoucherEnvelope"))
            {
                XmlReader xslreader = XmlReader.Create(new StringReader("<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:template match=\"@*|node()\">    <xsl:copy>        <xsl:apply-templates select=\"@*|node()\" />    </xsl:copy></xsl:template><xsl:template match=\"/ENVELOPE/BODY/DATA/TALLYMESSAGE/VOUCHER/LEDGERENTRIES.LIST\">		<ALLLEDGERENTRIES.LIST><xsl:apply-templates select=\"@*|node()\" /></ALLLEDGERENTRIES.LIST></xsl:template>   <xsl:template match=\"/ENVELOPE/BODY/DATA/TALLYMESSAGE/VOUCHER/INVENTORYENTRIES.LIST\">		   <ALLINVENTORYENTRIES.LIST><xsl:apply-templates select=\"@*|node()\" /></ALLINVENTORYENTRIES.LIST>	   </xsl:template></xsl:stylesheet>"));
                XslCompiledTransform xslTransform = new();
                xslTransform.Load(xslreader);
                StringWriter textWriter = new();
                XmlWriter xmlwriter = XmlWriter.Create(textWriter, new XmlWriterSettings() { OmitXmlDeclaration = true, Encoding = Encoding.Unicode });
                xslTransform.Transform(rd, null, xmlwriter);
                rd = XmlReader.Create(new StringReader(textWriter.ToString()), xset, context);
            }
            T? obj = (T?)XMLSer.Deserialize(rd);

            return obj;
        }
        catch (Exception e)
        {
            Logger.LogError("Error  - {Msg}", e.Message);
            Logger.LogError("Error occured during de-serialization of - {Xml}", Xml);
            return default;
        }

    }

    /// <summary>
    /// Helper Mehhod to convert base class to derieved class
    /// </summary>
    /// <param name="Derieved">Derieved Class instance</param>
    /// <param name="Base">Baseclass Instance</param>
    public static void Basetoderieved(object Derieved, object Base)
    {
        Type sourceType = Base.GetType();
        Type destinationType = Derieved.GetType();

        foreach (PropertyInfo sourceProperty in sourceType.GetProperties())
        {
            PropertyInfo? destinationProperty = destinationType.GetProperty(sourceProperty.Name);
            if (destinationProperty != null)
            {
                destinationProperty.SetValue(Derieved, sourceProperty.GetValue(Base, null), null);
            }
        }

    }

    public PResult ParseResponse(string RespXml)
    {

        PResult result = new();

        if (!RespXml.Contains("RESPONSE")) //checks Unknown error
        {
            ResponseEnvelope? Resp = GetObjfromXml<ResponseEnvelope>(RespXml); //Response from tally on sucess
            if (Resp?.Body?.Data?.LineError != null)
            {
                result.Status = RespStatus.Failure;
                result.Result = Resp.Body.Data.LineError;

            }
            if (Resp?.Body?.Data?.ImportResult != null)
            {
                if (Resp.Body.Data.ImportResult.LastVchId != null && Resp.Body.Data.ImportResult.LastVchId != 0)
                {
                    result.VoucherMasterId = Resp.Body.Data.ImportResult.LastVchId.ToString(); //Returns VoucherMaster ID
                }
                result.Status = RespStatus.Sucess;
                if (Resp.Body.Data.ImportResult.Created != 0)
                {
                    result.Result = "Created Sucessfully";
                }
                else if (Resp.Body.Data.ImportResult.Altered != 0)
                {
                    result.Result = "Altered Sucessfully";
                }
                else if (Resp.Body.Data.ImportResult.Deleted != 0)
                {
                    result.Result = "Deleted Sucessfully";
                }
                else if (Resp.Body.Data.ImportResult.Cacelled != 0)
                {
                    result.Result = "Cancelled Sucessfully";
                }
                else if (Resp.Body.Data.ImportResult.Combined != 0)
                {
                    result.Result = "Combined Sucessfully";
                }
            }


        }
        else
        {
            FailureResponse? resp = GetObjfromXml<FailureResponse>(RespXml); //Response from tally on Failure
            result.Status = RespStatus.Failure;
            result.Result = resp?.ToString();
        }
        return result;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }
            //client.Dispose();

            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Tally()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

