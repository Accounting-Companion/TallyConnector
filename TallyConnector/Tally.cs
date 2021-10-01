﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using TallyConnector.Models;
namespace TallyConnector
{
    public class Tally : IDisposable
    {
        private readonly HttpClient client = new();

        private ILogger Logger { get; }
        private CLogger CLogger { get; }

        private int Port;
        private string BaseURL;

        public string Status { get; private set; }
        public string ReqStatus { get; private set; }

        public string Company { get; private set; }
        public string FromDate { get; private set; }
        public string ToDate { get; private set; }

        private bool disposedValue;

        //Gets Full Url from Baseurl and Port
        private string FullURL => BaseURL + ":" + Port;

        public List<Group> Groups { get; private set; }
        public List<Ledger> Ledgers { get; private set; }
        public List<CostCategory> CostCategories { get; private set; }
        public List<CostCenter> CostCenters { get; private set; }
        public List<StockGroup> StockGroups { get; private set; }
        public List<StockCategory> StockCategories { get; private set; }

        public List<StockItem> StockItems { get; private set; }
        public List<Godown> Godowns { get; private set; }
        public List<VoucherType> VoucherTypes { get; private set; }
        public List<Unit> Units { get; private set; }
        public List<Currency> Currencies { get; private set; }

        public List<AttendanceType> AttendanceTypes { get; private set; }
        public List<EmployeeGroup> EmployeeGroups { get; private set; }
        public List<Employee> Employees { get; private set; }

        public List<Company> CompaniesList { get; private set; }


        /// <summary>
        /// Intiate Tally with <strong>baseURL</strong> and <strong>port</strong>
        /// </summary>
        /// <param name="baseURL">Url on which Tally is Running</param>
        /// <param name="port">Port on which Tally is Running</param>
        public Tally(string baseURL,
                     int port,
                     ILogger<Tally> Logger = null,
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
        public Tally(ILogger<Tally> logger = null,
                     int Timeoutseconds = 30)
        {
            Logger = logger;
            CLogger = new CLogger(Logger);
            client.Timeout = TimeSpan.FromSeconds(Timeoutseconds);
            Setup("http://localhost", 9000);
            
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
                          string company = null,
                          string fromDate = null,
                          string toDate = null)
        {
            BaseURL = baseURL;
            Port = port;
            Company = company;
            FromDate = fromDate;
            ToDate = toDate;

            CLogger?.SetupLog(baseURL,port,company,fromDate,  toDate);
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
                Status = $"Tally is not opened \n or Tally is not running in given port - { Port} )\n or Given URL - {BaseURL} \n" +
                    e.Message;
            }
            catch (Exception e)
            {
                CLogger.TallyError(FullURL,e.Message);
            }
            return false;
        }



        /// <summary>
        /// Gets List of Companies opened in tally and saves in Model.Company List
        /// </summary>
        /// <returns>return list of Model.Company List</returns>
        public async Task<List<Company>> GetCompaniesList()
        {
            string ReqType = "List of companies opened in Tally";
            await Check(); //Checks Whether Tally is running
            if (Status == "Running")
            {
                try
                {
                    CLogger.TallyReqStart(ReqType);
                    List<string> NativeFields = new() { "Name", "StartingFrom", "GUID", "*" };
                    string xml = await GetNativeCollectionXML(rName: "ListofCompanies",
                                                          colType: "Company", NativeFields: NativeFields, isInitialize: true);
                    CompaniesList = Tally.GetObjfromXml<ComListEnvelope>(xml).Body.Data.Collection.CompaniesList;
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
        public async Task<List<CompanyOnDisk>> GetCompaniesListinPath()
        {
            string ReqType = "List of companies in Default Tally path";
            List<CompanyOnDisk> Companies = new List<CompanyOnDisk>();
            await Check(); //Checks Whether Tally is running
            if (Status == "Running")
            {
                CLogger.TallyReqStart(ReqType);
                List<string> NativeFields = new() { "*" };
                List<string> Filters = new() { "NonGroupFilter" };
                List<string> SystemFilter = new() { $"$ISAGGREGATE = \"No\"" };
                string xml = await GetNativeCollectionXML(rName: "ListofCompaniesOpened",
                                                      colType: "Company On Disk", NativeFields: NativeFields,Filters:Filters,SystemFilters:SystemFilter);
                try
                {
                    Companies = Tally.GetObjfromXml<ComListinpathEnvelope>(xml).Body.Data.Collection.CompaniesList;
                    CLogger.TallyReqCompleted(ReqType);
                }
                catch(Exception e)
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
        public async Task FetchAllTallyData(string company = null)
        {
            string ReqType = "Masters from Tally";
            CLogger.TallyReqStart(ReqType);
            //Gets Groups from Tally
            Groups = await GetGroupsList();

            //Gets Ledgers from Tally
            Ledgers = await GetLedgersList();

            //Gets Cost Categories from Tally
            CostCategories = await GetCostCategoriesList();

            //Gets Cost Centers from Tally
            CostCenters = await GetCostCentersList();

            //Gets Stock Groups from Tally
            StockGroups =  await GetStockGroupsList();

            //Gets Stock Categories from Tally
            StockCategories = await GetStockCategories();

            //Gets Stock Items from Tally
            StockItems = await GetStockItemsList();

            //Gets Godowns from Tally
            Godowns = await GetGodownsList();

            //Gets Voucher Types from Tally
            VoucherTypes = await GetVoucherTypesList();

            //Gets Units from Tally
            Units = await GetUnitsList();

            //Gets Currencies from Tally
            Currencies = await GetCurrenciesList();

            //Gets AttendanceType from Tally
            AttendanceTypes = await GetAttendanceTypesList();

            //Gets EmployeeGroups from Tally
            EmployeeGroups = await GetEmployeeGroups();

            //Gets Employeees from Tally
            Employees = await GetEmployeesList();

            CLogger.TallyReqCompleted(ReqType);
        }

        /// <summary>
        /// By default Gets List of Group objects from Tally with basic data like MasterId,Name and GUID
        /// </summary>
        /// <param name="staticVariables">Static variables to be used in xml request</param>
        /// <param name="Nativelist">List of fields to be fetched from tally</param>
        /// <param name="Filters">If you want to get groups based on any filter use filter</param>
        /// <param name="SystemFilters">Based on which filter needs to be applied.If you specify filter you need to specify this </param>
        /// <returns>List of groups with basic data</returns>
        public async Task<List<Group>> GetGroupsList(StaticVariables staticVariables = null,
                                                     List<string> Nativelist = null,
                                                     List<string> Filters = null,
                                                     List<string> SystemFilters = null)
        {
            string ReqType = "List of companies in Default Tally path";
            CLogger.TallyReqStart(ReqType);
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            string GrpXml = await GetNativeCollectionXML(rName: "NativeGrpColl",
                                                         colType: "Group",
                                                         Sv: staticVariables,
                                                         NativeFields: Nativelist,
                                                         Filters: Filters,
                                                         SystemFilters: SystemFilters);
            GroupColl  GroupsColl = GetObjfromXml<GroupEnvelope>(GrpXml).Body.Data.Collection;
            List<Group> TGroups = GroupsColl.Groups ?? new();
            CLogger.TallyReqCompleted(ReqType);
            return TGroups;
        }

        public async Task<List<Ledger>> GetLedgersList(StaticVariables staticVariables = null,
                                                       List<string> Nativelist = null,
                                                       List<string> Filters = null,
                                                       List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };

            string LedXml = await GetNativeCollectionXML(rName: "NativeLedgColl",
                                                         colType: "Ledger",
                                                         Sv: staticVariables,
                                                         NativeFields: Nativelist,
                                                         Filters: Filters,
                                                         SystemFilters: SystemFilters);
            List<Ledger>  TLedgers = GetObjfromXml<LedgerEnvelope>(LedXml).Body.Data.Collection.Ledgers;
            return TLedgers;
        }

        public async Task<List<CostCategory>> GetCostCategoriesList(StaticVariables staticVariables = null,
                                                                    List<string> Nativelist = null,
                                                                    List<string> Filters = null,
                                                                    List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };

            string CostCategoryXml = await GetNativeCollectionXML(rName: "NativeCostCatColl",
                                                                              colType: "Costcategory",
                                                                              Sv: staticVariables,
                                                                              NativeFields: Nativelist);
            List<CostCategory> TCostCategories = GetObjfromXml<CostCatEnvelope>(CostCategoryXml).Body.Data.Collection.CostCategories;
            return TCostCategories;
        }

        public async Task<List<CostCenter>> GetCostCentersList(StaticVariables staticVariables = null,
                                                               List<string> Nativelist = null,
                                                               List<string> Filters = null,
                                                               List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };

            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            Filters ??= new() { "IsEmployeeGroup", "Payroll" };
            SystemFilters ??= new() { "Not $ISEMPLOYEEGROUP", "Not $FORPAYROLL" };

            string CostCenetrXml = await GetNativeCollectionXML(rName: "NativeCostCentColl",
                                                                            colType: "CostCenter",
                                                                            Sv: staticVariables,
                                                                            NativeFields: Nativelist,
                                                                            Filters: Filters,
                                                                            SystemFilters: SystemFilters);
            List<CostCenter> TCostCenters = GetObjfromXml<CostCentEnvelope>(CostCenetrXml).Body.Data.Collection.CostCenters;
            return TCostCenters;
        }


        private async Task<List<StockGroup>> GetStockGroupsList(StaticVariables staticVariables = null,
                                                                List<string> Nativelist = null,
                                                                List<string> Filters = null,
                                                                List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            string StockGroupXml = await GetNativeCollectionXML(rName: "NativeStckGrpColl",
                                                                colType: "StockGroup",
                                                                Sv: staticVariables,
                                                                NativeFields: Nativelist);
            List<StockGroup> TStockGroups = GetObjfromXml<StockGrpEnvelope>(StockGroupXml).Body.Data.Collection.StockGroups;
            return TStockGroups;

        }

        private async Task<List<StockCategory>> GetStockCategories(StaticVariables staticVariables = null,
                                                                   List<string> Nativelist = null,
                                                                   List<string> Filters = null,
                                                                   List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            string StockCategoryXml = await GetNativeCollectionXML(rName: "NativeStckCatColl",
                                                                   colType: "StockCategory",
                                                                   Sv: staticVariables,
                                                                   NativeFields: Nativelist);
            List<StockCategory> TStockCategories = GetObjfromXml<StockCatEnvelope>(StockCategoryXml).Body.Data.Collection.StockCategories;
            return TStockCategories;
        }

        private async Task<List<StockItem>> GetStockItemsList(StaticVariables staticVariables = null,
                                                              List<string> Nativelist = null,
                                                              List<string> Filters = null,
                                                              List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            string StockItemsXml = await GetNativeCollectionXML(rName: "NativeStckItmColl",
                                                                   colType: "StockItem",
                                                                   Sv: staticVariables,
                                                                   NativeFields: Nativelist);
            List<StockItem> TStockItems = GetObjfromXml<StockItemEnvelope>(StockItemsXml).Body.Data.Collection.StockItems;
            return TStockItems;
        }

        private async Task<List<Godown>> GetGodownsList(StaticVariables staticVariables = null,
                                                        List<string> Nativelist = null,
                                                        List<string> Filters = null,
                                                        List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            string GodownsXml = await GetNativeCollectionXML(rName: "NativeGdwnColl",
                                                                   colType: "Godown",
                                                                   Sv: staticVariables,
                                                                   NativeFields: Nativelist);
            List<Godown> TGodowns = GetObjfromXml<GodownEnvelope>(GodownsXml).Body.Data.Collection.Godowns;
            return TGodowns;
        }

        private async Task<List<VoucherType>> GetVoucherTypesList(StaticVariables staticVariables = null,
                                                                  List<string> Nativelist = null,
                                                                  List<string> Filters = null,
                                                                  List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            string VoucherTypesXml = await GetNativeCollectionXML(rName: "NativeVchTypeColl",
                                                                   colType: "VoucherType",
                                                                   Sv: staticVariables,
                                                                   NativeFields: Nativelist);
            List<VoucherType> TVoucherTypes = GetObjfromXml<VoucherTypeEnvelope>(VoucherTypesXml).Body.Data.Collection.VoucherTypes;
            return TVoucherTypes;
        }

        private async Task<List<Unit>> GetUnitsList(StaticVariables staticVariables = null,
                                                    List<string> Nativelist = null,
                                                    List<string> Filters = null,
                                                    List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            string UnitsXml = await GetNativeCollectionXML(rName: "NativeUnitColl",
                                                                   colType: "Unit",
                                                                   Sv: staticVariables,
                                                                   NativeFields: Nativelist);
            List<Unit> TUnits = GetObjfromXml<UnitEnvelope>(UnitsXml).Body.Data.Collection.Units;
            return TUnits;
        }

        private async Task<List<Currency>> GetCurrenciesList(StaticVariables staticVariables = null,
                                                             List<string> Nativelist = null,
                                                             List<string> Filters = null,
                                                             List<string> SystemFilters = null)
        {

            //Dictionary<string, string> Currenciesfields = new() { { "$EXPANDEDSYMBOL", "NAME" } };
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };

            string CurrenciesXml = await GetNativeCollectionXML(rName: "NativeCurrColl",
                                                                               colType: "Currency",
                                                                               Sv: staticVariables,
                                                                               NativeFields: Nativelist);
            List<Currency> TCurrencies = GetObjfromXml<CurrencyEnvelope>(CurrenciesXml).Body.Data.Collection.Currencies;
            return TCurrencies;
        }

        private async Task<List<AttendanceType>> GetAttendanceTypesList(StaticVariables staticVariables = null,
                                                                        List<string> Nativelist = null,
                                                                        List<string> Filters = null,
                                                                        List<string> SystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };

            string AttendanceTypesXml = await GetNativeCollectionXML(rName: "NativeAtndTypeColl",
                                                                               colType: "AttendanceType",
                                                                               Sv: staticVariables,
                                                                               NativeFields: Nativelist);
            List<AttendanceType> TAttendanceTypes = GetObjfromXml<AttendanceEnvelope>(AttendanceTypesXml).Body.Data.Collection.AttendanceTypes;
            return TAttendanceTypes;
        }

        private async Task<List<EmployeeGroup>> GetEmployeeGroups(StaticVariables staticVariables = null,
                                                                  List<string> Nativelist = null,
                                                                  List<string> EmployeeGroupFilters = null,
                                                                  List<string> EmployeeGroupSystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            EmployeeGroupFilters ??= new() { "IsEmployeeGroup" };
            EmployeeGroupSystemFilters ??= new() { "$ISEMPLOYEEGROUP" };
            string EmployeeGroupsXml = await GetNativeCollectionXML(rName: "NativeEmployeeGrpColl",
                                                                                colType: "CostCenter", Sv: staticVariables,
                                                                                NativeFields: Nativelist,
                                                                                Filters: EmployeeGroupFilters,
                                                                                SystemFilters: EmployeeGroupSystemFilters);
            List<EmployeeGroup> TEmployeeGroups = GetObjfromXml<EmployeeGroupEnvelope>(EmployeeGroupsXml).Body.Data.Collection.EmployeeGroups;
            return TEmployeeGroups;
        }

        private async Task<List<Employee>> GetEmployeesList(StaticVariables staticVariables = null,
                                                            List<string> Nativelist = null,
                                                            List<string> EmployeeFilters = null,
                                                            List<string> EmployeeSystemFilters = null)
        {
            staticVariables ??= new()
            {
                SVCompany = Company,
                SVExportFormat = "XML",
            };
            Nativelist ??= new() { "Name", "GUID", "Masterid" };
            EmployeeFilters ??= new() { "IsEmployeeGroup", "Payroll" };
            EmployeeSystemFilters ??= new() { "Not $ISEMPLOYEEGROUP", "$FORPAYROLL" };

            string EmployeeesXml = await GetNativeCollectionXML(rName: "NativeEmployeeColl",
                                                                                colType: "CostCenter", Sv: staticVariables,
                                                                                NativeFields: Nativelist,
                                                                                Filters: EmployeeFilters,
                                                                                SystemFilters: EmployeeSystemFilters);
            List<Employee> TEmployees = GetObjfromXml<EmployeeEnvelope>(EmployeeesXml).Body.Data.Collection.Employees;
            return TEmployees;
        }

        /// <summary>
        /// Gets Existing Group from Tally based on group name
        /// </summary>
        /// <param name="LookupValue">Specify the name of group/unique value of group to be fetched from Tally</param>
        /// <param name="LookupField">Specify the lookup field based on which to be fetched from Tally</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
        /// <param name="toDate">Specify toDate if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// if field is in tally but it is not shown in Groupinstance then you need to extend Group model and specify that field</param>
        /// <returns>Returns instance of Models.Group instance with data from tally</returns>
        public async Task<Group> GetGroup(string LookupValue, string LookupField = "Name",
                                          string company = null,
                                          string fromDate = null,
                                          string toDate = null,
                                          List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "MasterId", "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "Groupfilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };
            
            string xml = await GetNativeCollectionXML(rName: "CusGroupObj",
                                                      colType: "Group",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);

            Group Group = GetObjfromXml<GroupEnvelope>(xml).Body.Data.Collection.Groups[0];

            return Group;
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
        public async Task<PResult> PostGroup(Group group,
                                             string company = null)
        {

            //If parameter is null Get value from instance
            company ??= Company;

            GroupEnvelope groupEnvelope = new();
            groupEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            groupEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            groupEnvelope.Body.Data.Message.Group = group;
            if (group.Parent != null && group.Parent.Contains("Primary"))
            {
                group.Parent = null;
            }
            string GroupXML = groupEnvelope.GetXML();

            string RespXml = await SendRequest(GroupXML);

            PResult result = ParseResponse(RespXml);

            return result;
        }



        /// <summary>
        /// Gets Existing Ledger from Tally based on Ledger name
        /// </summary>
        /// <param name="ledgerName">Specify the name of Ledger to be fetched from Tally</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
        /// <param name="toDate">Specify toDate if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// </param>
        /// <returns>Returns instance of Models.Ledger instance with data from tally</returns>
        public async Task<Ledger> GetLedgerDynamic(string LookupValue,
                                                   string LookupField = "Name",
                                                   string company = null,
                                                   string fromDate = null,
                                                   string toDate = null,
                                                   List<string> Nativelist = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            Nativelist ??= new() { "Address","MasterId", "InterestCollection", "REMOTEGUID", "*" };
            StaticVariables sv = new() { SVCompany = company,SVFromDate=fromDate,SVToDate=toDate };
            List<string> Filters = new() { "LedgerFilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusLedgObj",
                                                      colType: "Ledger",
                                                      Sv: sv,
                                                      NativeFields: Nativelist,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);

            Ledger ledger = GetObjfromXml<LedgerEnvelope>(xml).Body.Data.Collection.Ledgers[0];
            return ledger;
        }

        /// <summary>
        /// Gets Existing Ledger from Tally based on Ledger name Opening balance is Static as per Master
        /// </summary>
        /// <param name="ledgerName">Specify the name of Ledger to be fetched from Tally</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// </param>
        /// <returns>Returns instance of Models.Ledger instance with data from tally</returns>
        public async Task<Ledger> GetLedger(string ledgerName,
                                            string company = null,
                                            List<string> Nativelist = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            Nativelist ??= new() { "Address", "InterestCollection", "*" };
            StaticVariables sv = new() { SVCompany = company };
            List<string> Filters = new() { "Cusfilter" };
            List<string> SystemFilter = new() { $"$Name = \"{ledgerName}\"" };

            string xml = await GetNativeCollectionXML(rName: "Ledgers",
                                                      colType: "Masters",
                                                      Sv: sv,
                                                      NativeFields: Nativelist,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);

            Ledger ledger = GetObjfromXml<LedgerEnvelope>(xml).Body.Data.Collection.Ledgers[0];
            return ledger;
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
        public async Task<PResult> PostLedger(Ledger ledger,
                                              string company = null)
        {

            //If parameter is null Get value from instance
            company ??= Company;

            LedgerEnvelope ledgerEnvelope = new();
            ledgerEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            ledgerEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            ledgerEnvelope.Body.Data.Message.Ledger = ledger;
            if (ledger.Group.Contains("Primary"))
            {
                ledger.Group = null;
            }
            string LedgXML = ledgerEnvelope.GetXML();

            string RespXml = await SendRequest(LedgXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<CostCategory> GetCostCategory(string LookupValue,
                                                        string LookupField = "Name",
                                                        string company = null,
                                                        string fromDate = null,
                                                        string toDate = null,
                                                        List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "MasterId","*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "Cusfilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusLedgObj",
                                                      colType: "CostCategory",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);

            CostCategory costCategory = GetObjfromXml<CostCatEnvelope>(xml).Body.Data.Collection.CostCategories[0];
            return costCategory;
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
        public async Task<PResult> PostCostCategory(CostCategory CostCategory,
                                                    string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            CostCatEnvelope costCat = new();
            costCat.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            costCat.Body.Desc.StaticVariables = new() { SVCompany = company };

            costCat.Body.Data.Message.CostCategory = CostCategory;

            string CostCatXML = costCat.GetXML();

            string RespXml = await SendRequest(CostCatXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<CostCenter> GetCostCenter(string LookupValue,
                                                    string LookupField = "Name",
                                                    string company = null,
                                                    string fromDate = null,
                                                    string toDate = null,
                                                    List<string> fetchList = null,
                                                    string format = "XML")
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "MasterId", "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "Cusfilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusCostCentObj",
                                                      colType: "CostCenter",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            CostCenter costCenter = GetObjfromXml<CostCentEnvelope>(xml).Body.Data.Collection.CostCenters[0];
            return costCenter;
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
        public async Task<PResult> PostCostCenter(CostCenter costCenter,
                                                  string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            CostCentEnvelope costCentEnvelope = new();
            costCentEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            costCentEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            costCentEnvelope.Body.Data.Message.CostCenter = costCenter;
            if (costCenter.Parent != null && costCenter.Parent.Contains("Primary"))
            {
                costCenter.Parent = null;
            }
            string CostCenterXML = costCentEnvelope.GetXML();

            string RespXml = await SendRequest(CostCenterXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<StockGroup> GetStockGroup(string LookupValue,
                                                    string LookupField = "Name",
                                                    string company = null,
                                                    string fromDate = null,
                                                    string toDate = null,
                                                    List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "MasterId", "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "Cusfilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusStckGrpObj",
                                                      colType: "StockGroup",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            StockGroup StockGroup = GetObjfromXml<StockGrpEnvelope>(xml).Body.Data.Collection.StockGroups[0];
            return StockGroup;
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
        public async Task<PResult> PostStockGroup(StockGroup stockGroup,
                                                  string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            StockGrpEnvelope StockGrpEnvelope = new();
            StockGrpEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            StockGrpEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            StockGrpEnvelope.Body.Data.Message.StockGroup = stockGroup;
            if (stockGroup.Parent !=null && stockGroup.Parent.Contains("Primary"))
            {
                stockGroup.Parent = null;
            }
            string StockGrpXML = StockGrpEnvelope.GetXML();

            string RespXml = await SendRequest(StockGrpXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<StockCategory> GetStockCategory(string LookupValue,
                                                          string LookupField = "Name",
                                                          string company = null,
                                                          string fromDate = null,
                                                          string toDate = null,
                                                          List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "MasterId", "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "Cusfilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusStockCatObj",
                                                      colType: "StockCategory",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            StockCategory StockCategory = GetObjfromXml<StockCatEnvelope>(xml).Body.Data.Collection.StockCategories[0];
            return StockCategory;
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
        public async Task<PResult> PostStockCategory(StockCategory stockCategory,
                                                     string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            StockCatEnvelope StockCatEnvelope = new();
            StockCatEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            StockCatEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            StockCatEnvelope.Body.Data.Message.StockCategory = stockCategory;
            if (stockCategory.Parent !=null && stockCategory.Parent.Contains("Primary"))
            {
                stockCategory.Parent = null;
            }
            string StockCatXML = StockCatEnvelope.GetXML();

            string RespXml = await SendRequest(StockCatXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<StockItem> GetStockItem(string LookupValue,
                                                  string LookupField = "Name",
                                                  string company = null,
                                                  string fromDate = null,
                                                  string toDate = null,
                                                  List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "MasterId", "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "Cusfilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusStckItmObj",
                                                      colType: "StockItem",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            StockItem StockItem = GetObjfromXml<StockItemEnvelope>(xml).Body.Data.Collection.StockItems[0];
            return StockItem;
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
        public async Task<PResult> PostStockItem(StockItem stockItem,
                                                 string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            StockItemEnvelope StockItmEnvelope = new();
            StockItmEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            StockItmEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            StockItmEnvelope.Body.Data.Message.StockItem = stockItem;
            if (stockItem.StockGroup != null && stockItem.StockGroup.Contains("Primary"))
            {
                stockItem.StockGroup = null;
            }
            string StockItmXML = StockItmEnvelope.GetXML();

            string RespXml = await SendRequest(StockItmXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<Unit> GetUnit(string LookupValue,
                                        string LookupField = "Name",
                                        string company = null,
                                        string fromDate = null,
                                        string toDate = null,
                                        List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "MasterId", "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "Cusfilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusUnitObj",
                                                      colType: "Unit",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            Unit Unit = GetObjfromXml<UnitEnvelope>(xml).Body.Data.Collection.Units[0];
            return Unit;
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
        public async Task<PResult> PostUnit(Unit unit,
                                            string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            UnitEnvelope UnitEnvelope = new();
            UnitEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            UnitEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            UnitEnvelope.Body.Data.Message.Unit = unit;

            string UnitXML = UnitEnvelope.GetXML();

            string RespXml = await SendRequest(UnitXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<Godown> GetGodown(string LookupValue,
                                            string LookupField = "Name",
                                            string company = null,
                                            string fromDate = null,
                                            string toDate = null,
                                            List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "MasterId", "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "Customfilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusGdwnObj",
                                                      colType: "Godown",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            Godown godown = GetObjfromXml<GodownEnvelope>(xml).Body.Data.Collection.Godowns[0];
            return godown;
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
        public async Task<PResult> PostGodown(Godown godown,
                                              string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            GodownEnvelope GdwnEnvelope = new();
            GdwnEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            GdwnEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            GdwnEnvelope.Body.Data.Message.Godown = godown;
            if (godown.Parent != null && godown.Parent.Contains("Primary"))
            {
                godown.Parent = null;
            }
            string GdwnXML = GdwnEnvelope.GetXML();

            string RespXml = await SendRequest(GdwnXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<VoucherType> GetVoucherType(string LookupValue,
                                                      string LookupField = "Name",
                                                      string company = null,
                                                      string fromDate = null,
                                                      string toDate = null,
                                                      List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "MasterId", "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "Cusfilter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusVchTypeObj",
                                                      colType: "VoucherType",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            VoucherType VoucherType = GetObjfromXml<VoucherTypeEnvelope>(xml).Body.Data.Collection.VoucherTypes[0];
            return VoucherType;
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
        public async Task<PResult> PostVoucherType(VoucherType voucherType,
                                                   string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            VoucherTypeEnvelope VchTypeEnvelope = new();
            VchTypeEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            VchTypeEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            VchTypeEnvelope.Body.Data.Message.VoucherType = voucherType;

            string GdwnXML = VchTypeEnvelope.GetXML();

            string RespXml = await SendRequest(GdwnXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<Currency> GetCurrency(string LookupValue,
                                                string LookupField = "Name",
                                                string company = null,
                                                string fromDate = null,
                                                string toDate = null,
                                                List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "filter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusCurrencyObj",
                                                      colType: "Currency",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            Currency Currency = GetObjfromXml<CurrencyEnvelope>(xml).Body.Data.Collection.Currencies[0];
            return Currency;
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
        public async Task<PResult> PostCurrency(Currency currency,
                                                string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            CurrencyEnvelope currencyEnvelope = new();
            currencyEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            currencyEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            currencyEnvelope.Body.Data.Message.Currency = currency;

            string GdwnXML = currencyEnvelope.GetXML();

            string RespXml = await SendRequest(GdwnXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<AttendanceType> GetAttendanceType(string LookupValue,
                                                            string LookupField = "Name",
                                                            string company = null,
                                                            string fromDate = null,
                                                            string toDate = null,
                                                            List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "filter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusAttndTypeObj",
                                                      colType: "AttendanceType",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            AttendanceType AttendanceType = GetObjfromXml<AttendanceEnvelope>(xml).Body.Data.Collection.AttendanceTypes[0];
            return AttendanceType;
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
        public async Task<PResult> PostAttendanceType(AttendanceType AttendanceType,
                                                      string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            AttendanceEnvelope AttndTypeEnvelope = new();
            AttndTypeEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            AttndTypeEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            AttndTypeEnvelope.Body.Data.Message.AttendanceType = AttendanceType;

            string AttndTypeXML = AttndTypeEnvelope.GetXML();

            string RespXml = await SendRequest(AttndTypeXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<EmployeeGroup> GetEmployeeGroup(string LookupValue,
                                                          string LookupField = "Name",
                                                          string company = null,
                                                          string fromDate = null,
                                                          string toDate = null,
                                                          List<string> fetchList = null,
                                                          string format = "XML")
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            fetchList ??= new() { "*" };
            StaticVariables sv = new() { SVCompany = company, SVFromDate = fromDate, SVToDate = toDate };
            List<string> Filters = new() { "filter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusEmployeeGrpObj",
                                                      colType: "Costcenter",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            EmployeeGroup EmployeeGroup = GetObjfromXml<EmployeeGroupEnvelope>(xml).Body.Data.Collection.EmployeeGroups[0];
            return EmployeeGroup;
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
        public async Task<PResult> PostEmployeeGroup(EmployeeGroup EmployeeGroup,
                                                     string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            EmployeeGroupEnvelope EmployeeGroupEnvelope = new();
            EmployeeGroupEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            EmployeeGroupEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            EmployeeGroupEnvelope.Body.Data.Message.EmployeeGroup = EmployeeGroup;

            string CostCenterXML = EmployeeGroupEnvelope.GetXML();

            string RespXml = await SendRequest(CostCenterXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<Employee> GetEmployee(string LookupValue,
                                                string LookupField = "Name",
                                                string company = null,
                                                List<string> fetchList = null,
                                                string format = "XML")
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fetchList ??= new() { "*" };
            StaticVariables sv = new() { SVCompany = company };
            List<string> Filters = new() { "filter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusEmployeeObj",
                                                      colType: "Costcenter",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            Employee Employee= GetObjfromXml<EmployeeEnvelope>(xml).Body.Data.Collection.Employees[0];
            return Employee;
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
        public async Task<PResult> PostEmployee(Employee Employee,
                                                string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            EmployeeEnvelope EmployeeEnvelope = new();
            EmployeeEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            EmployeeEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };

            EmployeeEnvelope.Body.Data.Message.Employee = Employee;

            string CostCenterXML = EmployeeEnvelope.GetXML();

            string RespXml = await SendRequest(CostCenterXML);

            PResult result = ParseResponse(RespXml);

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
        public async Task<Voucher> GetVoucher(string LookupValue,
                                              string LookupField = "VoucherNumber",
                                              string company = null,
                                              List<string> fetchList = null,
                                              string format = "XML")
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fetchList ??= new() { "MasterId", "*" };
            StaticVariables sv = new() { SVCompany = company};
            List<string> Filters = new() { "filter" };
            List<string> SystemFilter = new() { $"${LookupField} = \"{LookupValue}\"" };

            string xml = await GetNativeCollectionXML(rName: "CusVoucherObj",
                                                      colType: "Voucher",
                                                      Sv: sv,
                                                      NativeFields: fetchList,
                                                      Filters: Filters,
                                                      SystemFilters: SystemFilter);
            Voucher Vch = GetObjfromXml<VoucherEnvelope>(xml).Body.Data.Collection.Vouchers[0];
            return Vch;
        }


        /// <summary>
        /// Gets Voucher based on <strong>Voucher MasterID</strong>
        /// </summary>
        /// <param name="VoucherMasterID">Specify MasterID based on which vouchers to be fetched</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// </param>
        /// <returns>Returns instance of Models.Voucher with data from tally</returns>
        public async Task<Voucher> GetVoucherByMasterID(string VoucherMasterID,
                                                        bool isinventory = false,
                                                        string company = null,
                                                        List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            
            Models.Voucher voucher = (await GetObjFromTally<VoucherEnvelope>(ObjName: $"ID: {VoucherMasterID}",
                                                                             ObjType: "Voucher",
                                                                             company: company,
                                                                             fetchList: fetchList,
                                                                             viewname: isinventory ? "" : "Accounting Voucher View")).Body.Data.Message.Voucher;

            return voucher;
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
                                                             string company = null,
                                                             List<string> fetchList = null,
                                                             string format = "XML")
        {
            //If parameter is null Get value from instance
            company ??= Company;

            Models.Voucher voucher = (await GetObjFromTally<VoucherEnvelope>(ObjName: $"Date: \'{Date}\' : VoucherNumber: \'{VoucherNumber}\'",
                                                                             ObjType: "Voucher",
                                                                             company: company,
                                                                             fetchList: fetchList,
                                                                             viewname: "Accounting Voucher View")).Body.Data.Message.Voucher;

            return voucher;
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
        public async Task<PResult> PostVoucher(Voucher voucher,
                                               string company = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            VoucherEnvelope voucherEnvelope = new();
            voucherEnvelope.Header = new(Request: "Import", Type: "Data", ID: "All Masters");
            voucherEnvelope.Body.Desc.StaticVariables = new() { SVCompany = company };
            voucher.OrderLedgers(); //Ensures ledgers are ordered in correct way
            voucherEnvelope.Body.Data.Message.Voucher = voucher;

            string GdwnXML = voucherEnvelope.GetXML();

            string RespXml = await SendRequest(GdwnXML);

            PResult result = ParseResponse(RespXml);

            return result;
        }



        #region Reports
        /// <summary>
        /// Gets List of Vouchers  based on <strong>Voucher Type</strong>
        /// </summary>
        /// <param name="VoucherType">Specify the name of VoucherType based on which vouchers to be fetched</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
        /// <param name="toDate">Specify toDate if not specified in Setup</param>
        /// <returns>Returns instance of Models.VouchersList with data from tally</returns>
        public async Task<VouchersList> GetVouchersListByVoucherType(string VoucherType,
                                                                     string company = null,
                                                                     string fromDate = null,
                                                                     string toDate = null)
        {
            company ??= Company;

            Dictionary<string, string> fields = new() { { "$MASTERID", "MASTERID" }, { "$VoucherNumber", "VoucherNumber" }, { "$Date", "Date" } };
            StaticVariables staticVariables = new() { SVCompany = company, SVExportFormat = "XML", SVFromDate = fromDate, SVToDate = toDate };
            List<string> VoucherFilters = new() { "VoucherType" };
            List<string> VoucherSystemFilters = new() { $"$VoucherTypeName = \"{VoucherType}\"" };
            string VouchersXml = await GetCustomCollectionXML(rName: "List Of Vouchers", Fields: fields, colType: "Voucher", Sv: staticVariables,
                Filters: VoucherFilters, SystemFilters: VoucherSystemFilters);
            VouchersList vl = GetObjfromXml<VouchersList>(Xml: VouchersXml);
            return vl;
        }



        /// <summary>
        /// Get Vouchers of ledger
        /// </summary>
        /// <param name="ledgerName">Specify the name of Ledger from which vouchers to be fetched from Tally</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
        /// <param name="toDate">Specify toDate if not specified in Setup</param>
        /// </param>
        /// <returns>Returns instance of Models.Ledger instance with data from tally</returns>
        public async Task<List<Voucher>> GetLedgerVouchers(string ledgerName,
                                                           string company = null,
                                                           string fromDate = null,
                                                           string toDate = null,
                                                           List<string> Nativelist = null,
                                                           List<string> Filters = null,
                                                           List<string> SystemFilter = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            Nativelist = Nativelist == null?new() { "*" }: Nativelist;
            StaticVariables sv = new() { SVCompany = company,SVFromDate=fromDate,SVToDate=toDate };

            string xml = await GetNativeCollectionXML(rName: "Vouchers", colType: "Vouchers : Ledger", Sv: sv,childof:ledgerName,
                                                      NativeFields: Nativelist,Filters:Filters,SystemFilters:SystemFilter);

            List<Voucher> Vouchers = GetObjfromXml<VoucherEnvelope>(xml).Body.Data.Collection.Vouchers;
            return Vouchers;
        }


        /// <summary>
        /// Get Group Vouchers
        /// </summary>
        /// <param name="ledgerName">Specify the name of Group from which vouchers to be fetched from Tally</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
        /// <param name="toDate">Specify toDate if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// <returns>Returns List of Models.Voucher  with data from tally</returns>
        public async Task<List<Voucher>> GetGroupVouchers(string GroupName,
                                                          string company = null,
                                                          string fromDate = null,
                                                          string toDate = null,
                                                          List<string> Nativelist = null,
                                                          List<string> Filters = null,
                                                          List<string> SystemFilter = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            Nativelist = Nativelist == null?new() { "*" }: Nativelist;
            StaticVariables sv = new() { SVCompany = company,SVFromDate=fromDate,SVToDate=toDate };

            string xml = await GetNativeCollectionXML(rName: "Vouchers", colType: "Vouchers : Group", Sv: sv,childof: GroupName,
                                                      NativeFields: Nativelist, Filters: Filters, SystemFilters: SystemFilter);

            List<Voucher> Vouchers = GetObjfromXml<VoucherEnvelope>(xml).Body.Data.Collection.Vouchers;
            return Vouchers;
        }




        /// <summary>
        /// Get Inventory Vouchers
        /// </summary>
        /// <param name="StovkItemName">Specify the name of StockItem from which vouchers to be fetched from Tally</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
        /// <param name="toDate">Specify toDate if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// <returns>Returns List of Models.Voucher  with data from tally</returns>
        public async Task<List<Voucher>> GetInventoryVouchers(string ItemName,
                                                              string company = null,
                                                              string fromDate = null,
                                                              string toDate = null,
                                                              List<string> Nativelist = null,
                                                              List<string> Filters = null,
                                                              List<string> SystemFilter = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            Nativelist = Nativelist == null ? new() { "*" } : Nativelist;
            StaticVariables sv = new() { SVCompany = company,SVFromDate=fromDate,SVToDate=toDate };

            string xml = await GetNativeCollectionXML(rName: "Vouchers", colType: "Vouchers : Stockitem", Sv: sv,childof: ItemName,
                                                      NativeFields: Nativelist, Filters: Filters, SystemFilters: SystemFilter);

            List<Voucher> Vouchers = GetObjfromXml<VoucherEnvelope>(xml).Body.Data.Collection.Vouchers;
            return Vouchers;
        }


        #endregion
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
        public async Task<T> GetObjFromTally<T>(string ObjName,
                                                string ObjType,
                                                string company = null,
                                                string fromDate = null,
                                                string toDate = null,
                                                List<string> fetchList = null,
                                                string viewname = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            T Obj;
            try
            {
                string ReqXml = GetObjXML(objType: ObjType,
                                          ObjName: ObjName,
                                          company: company,
                                          fromDate:fromDate,
                                          toDate:toDate,
                                          fetchList: fetchList,
                                          viewname: viewname);
                string ResXml = await SendRequest(ReqXml);
                Obj = GetObjfromXml<T>(ResXml);
            }
            catch (Exception e)
            {
                throw;
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
                                 string company = null,
                                 string fromDate = null,
                                 string toDate = null,
                                 List<string> fetchList = null,
                                 string viewname = null)
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




        /// <summary>
        /// Generates XML for custom collection using TDL report
        /// </summary>
        /// <param name="rName">Custom Report Name to be used</param>
        /// <param name="Fields">Fields Dictionary with key as tally field and value as XML tag Name</param>
        /// <param name="colType">Specify Name of collection as per Tally</param>
        /// <param name="Sv">instance of Static vairiables</param>
        /// <param name="Filters">Filters if any</param>
        /// <param name="SystemFilters">Definition for filter</param>
        /// <returns>returns xml as string</returns>
        public async Task<string> GetCustomCollectionXML(string rName,
                                                         Dictionary<string, string> Fields,
                                                         string colType,
                                                         StaticVariables Sv = null,
                                                         List<string> Filters = null,
                                                         List<string> SystemFilters = null)
        {
            //LedgersList LedgList = new();
            string Resxml = null;
            await Check();
            if (Status == "Running")
            {
                Models.CusColEnvelope ColEnvelope = new(); //Collection Envelope
                string RName = rName;

                ColEnvelope.Header = new("Export", "Data", RName);  //Configuring Header To get Export data
                if (Sv != null)
                {
                    ColEnvelope.Body.Desc.StaticVariables = Sv;
                }

                Dictionary<string, string> LeftFields = Fields;
                Dictionary<string, string> RightFields = new();

                ColEnvelope.Body.Desc.TDL.TDLMessage = new(rName: RName, fName: RName, topPartName: RName,
                    rootXML: rName.Replace(" ", ""), colName: $"Form{RName}", lineName: RName, leftFields: LeftFields,
                    rightFields: RightFields, colType: colType, filters: Filters, SysFormulae: SystemFilters);

                string Reqxml = ColEnvelope.GetXML(); //Gets XML from Object
                Resxml = await SendRequest(Reqxml);
            }
            return Resxml;
        }


        /// <summary>
        /// Generates XML for custom collection using TDL report
        /// </summary>
        /// <param name="rName">Custom Report Name to be used</param>
        /// <param name="colType">Specify Name of collection as per Tally</param>
        /// <param name="Sv">instance of Static vairiables</param>
        /// <param name="NativeFields">Filters if any</param>
        /// <param name="Filters">Filters if any</param>
        /// <param name="SystemFilters">Definition for filter</param>
        /// <returns>returns xml as string</returns>
        public async Task<string> GetNativeCollectionXML(string rName,
                                                         string colType,
                                                         StaticVariables Sv = null,
                                                         string childof = null,
                                                         List<string> NativeFields = null,
                                                         List<string> Filters = null,
                                                         List<string> SystemFilters = null,
                                                         bool isInitialize = false)
        {
            //LedgersList LedgList = new();
            string Resxml = null;
            await Check();
            if (Status == "Running")
            {
                Models.CusColEnvelope ColEnvelope = new(); //Collection Envelope
                string RName = rName;

                ColEnvelope.Header = new("Export", "Collection", RName);  //Configuring Header To get Export data
                if (Sv != null)
                {
                    ColEnvelope.Body.Desc.StaticVariables = Sv;
                }

                ColEnvelope.Body.Desc.TDL.TDLMessage = new(colName: RName, colType: colType, nativeFields: NativeFields, Filters, SystemFilters);
                ColEnvelope.Body.Desc.TDL.TDLMessage.Collection.Childof = childof;
                if (isInitialize)
                {
                    ColEnvelope.Body.Desc.TDL.TDLMessage.Collection.SetAttributes(isInitialize: "Yes");
                }

                string Reqxml = ColEnvelope.GetXML(); //Gets XML from Object
                Resxml = await SendRequest(Reqxml);
            }
            return Resxml;
        }


        public async Task<string> GetReportXML(string reportname, StaticVariables Sv = null)
        {
            string Resxml = null;
            await Check();
            if (Status == "Running")
            {
                CusColEnvelope ColEnvelope = new(); //Collection Envelope
                string RName = reportname;

                ColEnvelope.Header = new("Export", "Data", RName);  //Configuring Header To get Export data

                if (Sv != null)
                {
                    ColEnvelope.Body.Desc.StaticVariables = Sv;
                }
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
        public async Task<string> SendRequest(string SXml)
        {
            string Resxml = "";
            //await Check();
            if (Status == "Running")
            {
                try
                {
                    CLogger.TallyRequest(SXml);
                    SXml = SXml.Replace("\t", "&#09;");
                    StringContent TXML = new(SXml, Encoding.UTF8, "application/xml");
                    HttpResponseMessage Res = await client.PostAsync(FullURL, TXML);
                    Res.EnsureSuccessStatusCode();
                    Resxml = await Res.Content.ReadAsStringAsync();
                    Resxml = ReplaceXMLText(Resxml);
                    CLogger.TallyResponse(Resxml);
                    return Resxml;
                }
                catch (Exception e)
                {
                    ReqStatus = e.Message;
                    CLogger.TallyReqError(e.Message);
                    return ReqStatus;
                }
            }
            return Resxml;
        }

        //Helper method to escape text for xml
        public static string ReplaceXML(string strText)
        {
            string result = null;
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
            string result = null;
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
        public static T GetObjfromXml<T>(string Xml)
        {
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            Xml = System.Text.RegularExpressions.Regex.Replace(Xml, re, "");
            XmlSerializer XMLSer = new(typeof(T));

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
                StringWriter textWriter = new StringWriter();
                XmlWriter xmlwriter = XmlWriter.Create(textWriter, new XmlWriterSettings() { OmitXmlDeclaration = true });
                xslTransform.Transform(rd, null, xmlwriter);
                rd = XmlReader.Create(new StringReader(textWriter.ToString()), xset, context);
            }
            try
            {
                T obj = (T)XMLSer.Deserialize(rd);


                return obj;
            }
            catch (Exception e)
            {

                throw;
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
                PropertyInfo destinationProperty = destinationType.GetProperty(sourceProperty.Name);
                if (destinationProperty != null)
                {
                    destinationProperty.SetValue(Derieved, sourceProperty.GetValue(Base, null), null);
                }
            }

        }

        public static PResult ParseResponse(string RespXml)
        {

            PResult result = new();

            if (!RespXml.Contains("RESPONSE")) //checks Unknown error
            {
                ResponseEnvelope Resp = Tally.GetObjfromXml<ResponseEnvelope>(RespXml); //Response from tally on sucess
                if (Resp.Body.Data.LineError != null)
                {
                    result.Status = RespStatus.Failure;
                    result.Result = Resp.Body.Data.LineError;

                }
                if (Resp.Body.Data.ImportResult !=null)
                {
                    if (Resp.Body.Data.ImportResult.LastVchId != null && Resp.Body.Data.ImportResult.LastVchId != 0)
                    {
                        result.Status = RespStatus.Sucess;
                        result.Result = Resp.Body.Data.ImportResult.LastVchId.ToString(); //Returns VoucherMaster ID
                    }
                    else
                    {
                        if (Resp.Body.Data.ImportResult.Created != 0)
                        {
                            result.Status = RespStatus.Sucess;
                            result.Result = "Created";
                        }
                    }
                }
                

            }
            else
            {
                FailureResponse resp = Tally.GetObjfromXml<FailureResponse>(RespXml); //Response from tally on Failure
                result.Status = RespStatus.Failure;
                result.Result = resp.ToString();
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
                Groups = null;
                Ledgers = null;
                CostCategories = null;
                CostCenters = null;
                StockCategories = null;
                StockGroups = null;
                StockItems = null;
                Units = null;
                Currencies = null;
                VoucherTypes = null;
                Employees = null;
                EmployeeGroups = null;
                AttendanceTypes = null;

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
}
