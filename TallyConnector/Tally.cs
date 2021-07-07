using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TallyConnector.Models;
namespace TallyConnector
{
    public class Tally : IDisposable
    {
        static readonly HttpClient client = new();

        private int Port;
        private string BaseURL;

        public string Status;
        public string ReqStatus;

        private bool disposedValue;

        //Gets Full Url from Baseurl and Port
        private string FullURL { get { return BaseURL + ":" + Port; } }

        public List<string> Groups { get; private set; }
        public List<string> Ledgers { get; private set; }
        public List<string> Parents { get; private set; }
        public List<string> CostCategories { get; private set; }
        public List<string> CostCenters { get; private set; }
        public List<string> StockGroups { get; private set; }
        public List<string> StockCategories { get; private set; }

        public List<string> StockItems { get; private set; }
        public List<string> Godowns { get; private set; }
        public List<string> VoucherTypes { get; private set; }
        public List<string> Units { get; private set; }
        public List<string> Currencies { get; private set; }

        public List<string> AttendanceTypes { get; private set; }
        public List<string> EmployeeGroups { get; private set; }
        public List<string> Employees { get; private set; }

        public List<Company> CompaniesList { get; private set; }

        public string Company { get; private set; }
        public string FromDate { get; private set; }
        public string ToDate { get; private set; }

        /// <summary>
        /// Intiate Tally with <strong>baseURL</strong> and <strong>port</strong>
        /// </summary>
        /// <param name="baseURL">Url on which Tally is Running</param>
        /// <param name="port">Port on which Tally is Running</param>
        public Tally(string baseURL, int port)
        {
            Port = port;
            BaseURL = baseURL;
            client.Timeout = TimeSpan.FromSeconds(30);
        }


        /// <summary>
        /// If nothing Specified during Intialisation default Url will be <strong>http://localhost</strong> running on port <strong>9000</strong>
        /// </summary>
        public Tally()
        {
            client.Timeout = TimeSpan.FromSeconds(30);
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
        public void Setup(string baseURL, int port, string company = null, string fromDate = null, string toDate = null)
        {
            BaseURL = baseURL;
            Port = port;
            Company = company;
            FromDate = fromDate;
            ToDate = toDate;
            
        }

        

        /// <summary>
        /// Checks whether Tally is running in given URL and port
        /// </summary>
        /// <returns>Return true if running,else false</returns>
        public async Task<bool> Check()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(FullURL);
                response.EnsureSuccessStatusCode();
                string res = await response.Content.ReadAsStringAsync();

                Status = "Running";
                return true;
            }
            catch (HttpRequestException ex)
            {
                HttpRequestException e = ex;
                Status = $"Tally is not opened \n or Tally is not running in given port - { Port} )\n or Given URL - {BaseURL} \n" +
                    e.Message;
            }
            catch (Exception e)
            {

            }
            return false;
        }



        /// <summary>
        /// Gets List of Companies opened in tally and saves in Model.Company List
        /// </summary>
        /// <returns>return list of Model.Company List</returns>
        public async Task<List<Company>> GetCompaniesList()
        {
            await Check(); //Checks Whether Tally is running
            if (Status == "Running")
            {
                List<string> NativeFields = new() { "Name", "StartingFrom", "GUID" ,"*"};
                string xml = await GetNativeCollectionXML(rName: "ListofCompanies",
                                                      colType: "Company",NativeFields: NativeFields,isInitialize:true);
                try
                {
                    CompaniesList = Tally.GetObjfromXml<ComListEnvelope>(xml).Body.Data.Collection.CompaniesList;

                }
                catch (Exception e) 
                {
                    //throw;
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
            List<CompanyOnDisk> Companies = new List<CompanyOnDisk>();
            await Check(); //Checks Whether Tally is running
            if (Status == "Running")
            {
                List<string> NativeFields = new() { "*" };
                List<string> Filters = new() { "NonGroupFilter" };
                List<string> SystemFilter = new() { $"$ISAGGREGATE = \"No\"" };
                string xml = await GetNativeCollectionXML(rName: "ListofCompaniesOpened",
                                                      colType: "Company On Disk", NativeFields: NativeFields,Filters:Filters,SystemFilters:SystemFilter);
                try
                {
                    Companies = Tally.GetObjfromXml<ComListinpathEnvelope>(xml).Body.Data.Collection.CompaniesList;

                }
                catch{}

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
            company ??= Company;
            //If Company Name is provided, fetch information related to particular company
            //- Useful when multiple companies are opened in Tally
            StaticVariables staticVariables = new()
            {
                SVCompany = company,
                SVExportFormat = "XML",
            };

            //Gets Groups from Tally
            Dictionary<string, string> fields = new() { { "$NAME", "NAME" } };
            string GrpXml = await GetCustomCollectionXML("List Of Groups", fields, "Group", staticVariables);
            Groups = GetObjfromXml<GroupsList>(GrpXml).GroupNames;

            //Gets Ledger from Tally
            string LedXml = await GetCustomCollectionXML("List Of Ledgers", fields, "Ledger", staticVariables);
            Ledgers = GetObjfromXml<LedgersList>(LedXml).LedgerNames;

            //Gets Cost Categories from Tally
            string CostCategoryXml = await GetCustomCollectionXML("List Of CostCategories", fields, "CostCategory", staticVariables);
            CostCategories = GetObjfromXml<CostCategoriesList>(CostCategoryXml).CostCategories;

            //Gets Cost Centers from Tally
            List<string> Filters = new() { "IsEmployeeGroup", "Payroll" };
            List<string> SystemFilters = new() { "Not $ISEMPLOYEEGROUP", "Not $FORPAYROLL" };
            string CostCenetrXml = await GetCustomCollectionXML("List Of CostCenters", fields, "CostCenter",
                staticVariables, Filters, SystemFilters);
            CostCenters = GetObjfromXml<CostCentersList>(CostCenetrXml).CostCenters;

            //Gets Stock Groups from Tally
            string StockGroupXml = await GetCustomCollectionXML("List Of StockGroups", fields, "StockGroups", staticVariables);
            StockGroups = GetObjfromXml<StockGroupsList>(StockGroupXml).StockGroups;

            //Gets Stock Categories from Tally
            string StockCategoryXml = await GetCustomCollectionXML("List Of StockCategories", fields, "StockCategory", staticVariables);
            StockCategories = GetObjfromXml<StockCategoriesList>(StockCategoryXml).StockCategories;

            //Gets Stock Items from Tally
            string StockItemsXml = await GetCustomCollectionXML("List Of StockItems", fields, "StockItems", staticVariables);
            StockItems = GetObjfromXml<StockItemsList>(StockItemsXml).StockItems;

            //Gets Godowns from Tally
            string GodownsXml = await GetCustomCollectionXML("List Of Godowns", fields, "Godown", staticVariables);
            Godowns = GetObjfromXml<GodownsList>(GodownsXml).Godowns;


            //Gets Voucher Types from Tally
            string VoucherTypesXml = await GetCustomCollectionXML("List Of VoucherTypes", fields, "VoucherTypes", staticVariables);
            VoucherTypes = GetObjfromXml<VoucherTypesList>(VoucherTypesXml).VoucherTypes;

            //Gets Voucher Types from Tally
            string UnitsXml = await GetCustomCollectionXML("List Of Units", fields, "Units", staticVariables);
            Units = GetObjfromXml<UnitsList>(UnitsXml).Units;

            //Gets Currencies from Tally
            Dictionary<string, string> Currenciesfields = new() { { "$EXPANDEDSYMBOL", "NAME" } };
            string CurrenciesXml = await GetCustomCollectionXML("List Of Currencies", Currenciesfields, "Currencies", staticVariables);
            Currencies = GetObjfromXml<CurrenciesList>(CurrenciesXml).Currencies;



            //Gets AttendanceType from Tally
            string AttendanceTypesXml = await GetCustomCollectionXML("List Of AttendanceTypes", fields, "AttendanceType", staticVariables);
            AttendanceTypes = GetObjfromXml<AttendanceTypesList>(AttendanceTypesXml).AttendanceTypes;

            //Gets EmployeeGroups from Tally
            List<string> EmployeeGroupFilters = new() { "IsEmployeeGroup" };
            List<string> EmployeeGroupSystemFilters = new() { "$ISEMPLOYEEGROUP" };
            string EmployeeGroupsXml = await GetCustomCollectionXML("List Of EmployeeGroups", fields, "CostCenter", staticVariables,
                EmployeeGroupFilters, EmployeeGroupSystemFilters);
            EmployeeGroups = GetObjfromXml<EmployeeGroupList>(EmployeeGroupsXml).EmployeeGroups;

            //Gets Employeees from Tally
            List<string> EmployeeFilters = new() { "IsEmployeeGroup", "Payroll" };
            List<string> EmployeeSystemFilters = new() { "Not $ISEMPLOYEEGROUP", "$FORPAYROLL" };
            string EmployeeesXml = await GetCustomCollectionXML("List Of Employees", fields, "CostCenter", staticVariables,
                EmployeeFilters, EmployeeSystemFilters);
            Employees = GetObjfromXml<EmployeesList>(EmployeeesXml).Employees;


        }



        /// <summary>
        /// Gets Existing Group from Tally based on group name
        /// </summary>
        /// <param name="GroupName">Specify the name of group to be fetched from Tally</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
        /// <param name="toDate">Specify toDate if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// if field is in tally but it is not shown in Groupinstance then you need to extend Group model and specify that field</param>
        /// <returns>Returns instance of Models.Group instance with data from tally</returns>
        public async Task<Group> GetGroup(String GroupName,
                                          string company = null,
                                          string fromDate = null,
                                          string toDate = null,
                                          List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;

            Group group = (await GetObjFromTally<GroupEnvelope>(ObjName: GroupName,
                                                                ObjType: "Group",
                                                                company: company,
                                                                fromDate: fromDate,
                                                                toDate: toDate,
                                                                fetchList: fetchList,
                                                                viewname: null)).Body.Data.Message.Group;

            return group;
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
        public async Task<Ledger> GetLedgerDynamic(string ledgerName,
                                            string company = null,
                                            string fromDate = null,
                                            string toDate = null,
                                            List<string> Nativelist = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            Nativelist ??= new() { "Address", "InterestCollection", "*" };
            StaticVariables sv = new() { SVCompany = company,SVFromDate=fromDate,SVToDate=toDate };
            List<string> Filters = new() { "Ledgerfilter" };
            List<string> SystemFilter = new() { $"$Name = \"{ledgerName}\"" };

            string xml = await GetNativeCollectionXML(rName: "Ledgers",
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
            List<string> Filters = new() { "Ledgerfilter" };
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
        public async Task<CostCategory> GetCostCategory(String CostCategoryName,
                                                        string company = null,
                                                        string fromDate = null,
                                                        string toDate = null,
                                                        List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;

            CostCategory costCategory = (await GetObjFromTally<CostCatEnvelope>(ObjName: CostCategoryName,
                                                                                ObjType: "CostCategory",
                                                                                company: company,
                                                                                fromDate: fromDate,
                                                                                toDate: toDate,
                                                                                fetchList: fetchList,
                                                                                viewname: null)).Body.Data.Message.CostCategory;

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
        public async Task<CostCenter> GetCostCenter(String CostCenterName,
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

            CostCenter costCenter = (await GetObjFromTally<CostCentEnvelope>(ObjName: CostCenterName,
                                                                             ObjType: "CostCenter",
                                                                             company: company,
                                                                             fromDate: fromDate,
                                                                             toDate: toDate,
                                                                             fetchList: fetchList,
                                                                             viewname: null)).Body.Data.Message.CostCenter;

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
        public async Task<StockGroup> GetStockGroup(String StockGroupName,
                                                    string company = null,
                                                    string fromDate = null,
                                                    string toDate = null,
                                                    List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;

            StockGroup stockGroup = (await GetObjFromTally<StockGrpEnvelope>(ObjName: StockGroupName,
                                                                             ObjType: "StockGroup",
                                                                             company: company,
                                                                             fromDate: fromDate,
                                                                             toDate: toDate,
                                                                             fetchList: fetchList,
                                                                             viewname: null)).Body.Data.Message.StockGroup;

            return stockGroup;
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
        public async Task<StockCategory> GetStockCategory(String StockCategoryName,
                                                          string company = null,
                                                          string fromDate = null,
                                                          string toDate = null,
                                                          List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;

            StockCategory stockCategory = (await GetObjFromTally<StockCatEnvelope>(ObjName: StockCategoryName,
                                                                                   ObjType: "StockCategory",
                                                                                   company: company,
                                                                                   fromDate: fromDate,
                                                                                   toDate: toDate,
                                                                                   fetchList: fetchList,
                                                                                   viewname: null)).Body.Data.Message.StockCategory;

            return stockCategory;
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
        public async Task<StockItem> GetStockItem(String StockItemName,
                                                  string company = null,
                                                  string fromDate = null,
                                                  string toDate = null,
                                                  List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;

            StockItem stockItem = (await GetObjFromTally<StockItemEnvelope>(ObjName: StockItemName,
                                                                            ObjType: "StockItem",
                                                                            company: company,
                                                                            fromDate: fromDate,
                                                                            toDate: toDate,
                                                                            fetchList: fetchList,
                                                                            viewname: null)).Body.Data.Message.StockItem;

            return stockItem;
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
        public async Task<Unit> GetUnit(String UnitName,
                                        string company = null,
                                        string fromDate = null,
                                        string toDate = null,
                                        List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;

            Unit unit = (await GetObjFromTally<UnitEnvelope>(ObjName: UnitName,
                                                             ObjType: "Unit",
                                                             company: company,
                                                             fromDate: fromDate,
                                                             toDate: toDate,
                                                             fetchList: fetchList,
                                                             viewname: null)).Body.Data.Message.Unit;

            return unit;
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
        public async Task<Godown> GetGodown(String GodownName,
                                            string company = null,
                                            string fromDate = null,
                                            string toDate = null,
                                            List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;

            Godown godown = (await GetObjFromTally<GodownEnvelope>(ObjName: GodownName,
                                                                   ObjType: "Godown",
                                                                   company: company,
                                                                   fromDate: fromDate,
                                                                   toDate: toDate,
                                                                   fetchList: fetchList,
                                                                   viewname: null)).Body.Data.Message.Godown;

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
        public async Task<VoucherType> GetVoucherType(String VoucherTypeName,
                                                      string company = null,
                                                      string fromDate = null,
                                                      string toDate = null,
                                                      List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;

            VoucherType voucherType = (await GetObjFromTally<VoucherTypeEnvelope>(ObjName: VoucherTypeName,
                                                                                  ObjType: "VoucherType",
                                                                                  company: company,
                                                                                  fromDate: fromDate,
                                                                                  toDate: toDate,
                                                                                  fetchList: fetchList,
                                                                                  viewname: null)).Body.Data.Message.VoucherType;

            return voucherType;
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
        public async Task<Currency> GetCurrency(String CurrencyName,
                                                  string company = null,
                                                  string fromDate = null,
                                                  string toDate = null,
                                                  List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;

            Currency currency = (await GetObjFromTally<CurrencyEnvelope>(ObjName: CurrencyName,
                                                                           ObjType: "Currencies",
                                                                           company: company,
                                                                           fromDate: fromDate,
                                                                           toDate: toDate,
                                                                           fetchList: fetchList,
                                                                           viewname: null)).Body.Data.Message.Currency;

            return currency;
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
        /// Gets CostCenter from Tally based on CostCenter name
        /// </summary>
        /// <param name="EmployeeGroupName">Specify the name of EmployeeGroupName to be fetched from Tally</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fromDate">Specify fromDate if not specified in Setup</param>
        /// <param name="toDate">Specify toDate if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// </param>
        /// <returns>Returns instance of Models.CostCenter instance with data from tally</returns>
        public async Task<EmployeeGroup> GetEmployeeGroup(String EmployeeGroupName,
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

            EmployeeGroup EmpGrp = (await GetObjFromTally<EmployeeGroupEnvelope>(ObjName: EmployeeGroupName,
                                                                             ObjType: "CostCenter",
                                                                             company: company,
                                                                             fromDate: fromDate,
                                                                             toDate: toDate,
                                                                             fetchList: fetchList,
                                                                             viewname: null)).Body.Data.Message.EmployeeGroup;

            return EmpGrp;
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
        public async Task<Employee> GetEmployee(String EmployeeName,
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
            fetchList = new() { "TaxRegimeDetails","*" };
            Employee Employee = (await GetObjFromTally<EmployeeEnvelope>(ObjName: EmployeeName,
                                                                             ObjType: "CostCenter",
                                                                             company: company,
                                                                             fromDate: fromDate,
                                                                             toDate: toDate,
                                                                             fetchList: fetchList,
                                                                             viewname: null)).Body.Data.Message.Employee;

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
        /// Gets Voucher based on <strong>Voucher MasterID</strong>
        /// </summary>
        /// <param name="VoucherMasterID">Specify MasterID based on which vouchers to be fetched</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// </param>
        /// <returns>Returns instance of Models.Voucher with data from tally</returns>
        public async Task<Voucher> GetVoucherByMasterID(String VoucherMasterID,
                                                               string company = null,
                                                               List<string> fetchList = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;

            Models.Voucher voucher = (await GetObjFromTally<VoucherEnvelope>(ObjName: $"ID: {VoucherMasterID}",
                                                                             ObjType: "Voucher",
                                                                             company: company,
                                                                             fetchList: fetchList,
                                                                             viewname: "Accounting Voucher View")).Body.Data.Message.Voucher;

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
        public async Task<Voucher> GetVoucherByVoucherNumber(String VoucherNumber, string Date,
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
        /// </param>
        /// <returns>Returns instance of Models.Ledger instance with data from tally</returns>
        public async Task<List<Voucher>> GetLedgerVouchers(string ledgerName,
                                            string company = null,
                                            string fromDate = null,
                                            string toDate = null,
                                            List<string> Nativelist = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            Nativelist = Nativelist == null?new() { "*" }: Nativelist;
            StaticVariables sv = new() { SVCompany = company,SVFromDate=fromDate,SVToDate=toDate };

            string xml = await GetNativeCollectionXML(rName: "Vouchers", colType: "Vouchers : Ledger", Sv: sv,childof:ledgerName,
                                                      NativeFields: Nativelist);

            List<Voucher> Vouchers = GetObjfromXml<VoucherEnvelope>(xml).Body.Data.Collection.Vouchers;
            return Vouchers;
        }




        /// <summary>
        /// Get Group Vouchers
        /// </summary>
        /// <param name="ledgerName">Specify the name of Group from which vouchers to be fetched from Tally</param>
        /// <param name="company">Specify Company if not specified in Setup</param>
        /// <param name="fetchList">You can select the list of fields to be fetched from tally if nothing specified it pulls all fields availaible in Tally
        /// </param>
        /// <returns>Returns instance of Models.Ledger instance with data from tally</returns>
        public async Task<List<Voucher>> GetGroupVouchers(string GroupName,
                                            string company = null,
                                            string fromDate = null,
                                            string toDate = null,
                                            List<string> Nativelist = null)
        {
            //If parameter is null Get value from instance
            company ??= Company;
            fromDate ??= FromDate;
            toDate ??= ToDate;
            Nativelist = Nativelist == null?new() { "*" }: Nativelist;
            StaticVariables sv = new() { SVCompany = company,SVFromDate=fromDate,SVToDate=toDate };

            string xml = await GetNativeCollectionXML(rName: "Vouchers", colType: "Vouchers : Group", Sv: sv,childof: GroupName,
                                                      NativeFields: Nativelist);

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



        public async Task<LedgersList> GetLedgersList()
        {
            LedgersList LedgList = new();

            if (Status == "Running")
            {
                Models.CusColEnvelope ColEnvelope = new(); //Collection Envelope
                string RName = "List of Ledgers";

                ColEnvelope.Header = new("Export", "Data", RName);  //Configuring Header To get Export data

                Dictionary<string, string> LeftFields = new() //Left Fields
                {
                    { "$NAME", "NAME" }

                };
                Dictionary<string, string> RightFields = new() //Right Fields
                {

                };

                ColEnvelope.Body.Desc.TDL.TDLMessage = new(rName: RName, fName: RName, topPartName: RName,
                    rootXML: "LISTOFLEDGERS", colName: $"Form{RName}", lineName: RName, leftFields: LeftFields,
                    rightFields: RightFields, colType: "Ledger");

                string Reqxml = ColEnvelope.GetXML(); //Gets XML from Object
                string Resxml = await SendRequest(Reqxml);

                try
                {
                    LedgList = Tally.GetObjfromXml<LedgersList>(Resxml);
                    Ledgers = LedgList.LedgerNames;

                }
                catch (Exception e)
                {

                    //throw;
                }
            }
            return LedgList;
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
                    SXml = SXml.Replace("\t", "&#09;");
                    StringContent TXML = new(SXml, Encoding.UTF8, "application/xml");
                    HttpResponseMessage Res = await client.PostAsync(FullURL, TXML);
                    Res.EnsureSuccessStatusCode();
                    Resxml = await Res.Content.ReadAsStringAsync();
                    Resxml = ReplaceXMLText(Resxml);
                    return Resxml;
                }
                catch (Exception e)
                {
                    ReqStatus = e.Message;
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


        public static PResult ParseResponse(string RespXml)
        {

            PResult result = new();

            if (!RespXml.Contains("RESPONSE")) //checks Unknown error
            {
                ResponseEnvelope Resp = Tally.GetObjfromXml<ResponseEnvelope>(RespXml); //Response from tally on sucess
                if (Resp.Body.Data.LineError != null)
                {
                    result.Status = RespStatus.Failure;


                }
                if (Resp.Body.Data.ImportResult.LastVchId != null)
                {
                    result.Status = RespStatus.Sucess;
                    result.Result = Resp.Body.Data.ImportResult.LastVchId.ToString(); //Returns VoucherMaster ID
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
