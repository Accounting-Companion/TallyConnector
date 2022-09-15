using TallyConnector.Core.Models.Masters;
using TallyConnector.Core.Models.Masters.CostCenter;
using TallyConnector.Core.Models.Masters.Inventory;
using TallyConnector.Core.Models.Masters.Payroll;

namespace TallyConnector.Services;

/// <summary>
/// contains API to interact with Tally
/// </summary>
public interface ITallyService
{
    /// <summary>
    /// Coonfigure Tally Url and port
    /// </summary>
    /// <param name="url">Url on which Tally is running</param>
    /// <param name="port">Port on Which Tally is runnig</param>
    void Setup(string url, int port);

    /// <summary>
    /// Checks Whether Tally is running at Given Url and port
    /// </summary>
    /// <returns>true or false</returns>
    Task<bool> CheckAsync();

    /// <summary>
    /// Get License Information and Other Basic Info from Tally
    /// </summary>
    /// <returns></returns>
    Task<LicenseInfo?> GetLicenseInfoAsync();

    /// <summary>
    /// Gets Active company from tally
    /// </summary>
    /// <returns></returns>
    Task<BaseCompany?> GetActiveCompanyAsync();

    /// <summary>
    /// Gets List of companies that are opened in Tally
    /// </summary>
    /// <typeparam name="CompanyType"></typeparam>
    /// <returns></returns>
    Task<List<CompanyType>?> GetCompaniesAsync<CompanyType>() where CompanyType : BaseCompany;

    /// <summary>
    /// Gets List of companies that are in Default Data Path of Tally
    /// </summary>
    /// <typeparam name="CompanyType"></typeparam>
    /// <returns></returns>
    Task<List<CompanyOnDisk>?> GetCompaniesinDefaultPathAsync();

    /// <summary>
    /// Set company , all future requests will be send to company mentioned here
    /// irrespective of active company in Tally
    /// you can overide company by mentioning in request options
    /// </summary>
    /// <param name="company"></param>
    void SetCompany(Company company);

    /// <summary>
    /// Get Statistics of Masters form Tally
    /// Request options specified here overirdes one setup using Setup() method
    /// </summary>
    /// <param name="requestOptions">Request options to configure tally</param>
    /// <returns></returns>
    Task<List<MasterTypeStat>?> GetMasterStatisticsAsync(BaseRequestOptions? requestOptions = null);

    /// <summary>
    /// Get Statistics of Vouchers from Tally
    /// We can metion specific period using From and To Date in request options
    /// </summary>
    /// <param name="requestOptions">Request options to configure tally</param>
    /// <returns></returns>
    Task<List<VoucherTypeStat>?> GetVoucherStatisticsAsync(DateFilterRequestOptions? requestOptions = null);


    #region Accounting Masters
    /*
     * Get and Post Methods of Accounting Masters - Currency, Group, Ledger,VoucherType, Cost Category, CostCetre
     * To Get an Item use Get[Name]Async<Type>(lookupvalue,options);
     * Ex:To Get Group use - await GetGroupAsync<Group>("Name");
     */
    #region Get - Methods 

    Task<CurrencyType> GetCurrencyAsync<CurrencyType>(string lookupValue, MasterRequestOptions? currencyOptions = null) where CurrencyType : Currency;

    Task<GroupType> GetGroupAsync<GroupType>(string lookupValue, MasterRequestOptions? groupOptions = null) where GroupType : Group;
    Task<LedgerType> GetLedgerAsync<LedgerType>(string LookupValue, MasterRequestOptions? ledgerOptions = null) where LedgerType : Ledger;
    Task<VChTypeType> GetVoucherTypeAsync<VChTypeType>(string LookupValue, MasterRequestOptions? voucherTypeOptions = null) where VChTypeType : VoucherType;

    Task<CostCategoryType> GetCostCategoryAsync<CostCategoryType>(string LookupValue, MasterRequestOptions? costCategoryOptions = null) where CostCategoryType : CostCategory;
    Task<CostCentreType> GetCostCentreAsync<CostCentreType>(string LookupValue, MasterRequestOptions? costCenterOptions = null) where CostCentreType : CostCenter;
    #endregion


    #region Post-Methods
    Task<TallyResult> PostCurrencyAsync<CurrencyType>(CurrencyType currency, PostRequestOptions? postRequestOptions = null) where CurrencyType : Currency;

    Task<TallyResult> PostGroupAsync<GroupType>(GroupType group, PostRequestOptions? postRequestOptions = null) where GroupType : Group;
    Task<TallyResult> PostLedgerAsync<LedgerType>(LedgerType ledger, PostRequestOptions? postRequestOptions = null) where LedgerType : Ledger;

    Task<TallyResult> PostVoucherTypeAsync<VChTypeType>(VChTypeType voucherType, PostRequestOptions? postRequestOptions = null) where VChTypeType : VoucherType;

    Task<TallyResult> PostCostCategoryAsync<CostCategoryType>(CostCategoryType costCategory, PostRequestOptions? postRequestOptions = null) where CostCategoryType : CostCategory;
    Task<TallyResult> PostCostCentreAsync<CostCentreType>(CostCentreType costCenter, PostRequestOptions? postRequestOptions = null) where CostCentreType : CostCenter;
    #endregion

    #endregion

    #region Inventory Masters

    #region Get - Methods 
    Task<UnitType> GetUnitAsync<UnitType>(string LookupValue, MasterRequestOptions? unitOptions = null) where UnitType : Unit;
    Task<GodownType> GetGodownAsync<GodownType>(string LookupValue, MasterRequestOptions? godownOptions = null) where GodownType : Godown;
    Task<StockCategoryType> GetStockCategoryAsync<StockCategoryType>(string LookupValue, MasterRequestOptions? stockCategoryOptions = null) where StockCategoryType : StockCategory;
    Task<StockGroupType> GetStockGroupAsync<StockGroupType>(string LookupValue, MasterRequestOptions? stockGroupOptions = null) where StockGroupType : StockGroup;
    Task<StckItmType> GetStockItemAsync<StckItmType>(string LookupValue, MasterRequestOptions? stockItemOptions = null) where StckItmType : StockItem;

    #endregion

    #region Post - Methods 

    Task<TallyResult> PostUnitAsync<UnitType>(UnitType unit, PostRequestOptions? postRequestOptions = null) where UnitType : Unit;

    Task<TallyResult> PostGodownAsync<GodownType>(GodownType godown, PostRequestOptions? postRequestOptions = null) where GodownType : Godown;

    Task<TallyResult> PostStockCategoryAsync<StockCategoryType>(StockCategoryType stockCategory, PostRequestOptions? postRequestOptions = null) where StockCategoryType : StockCategory;
    Task<TallyResult> PostStockGroupAsync<StockGroupType>(StockGroupType stockGroup, PostRequestOptions? postRequestOptions = null) where StockGroupType : StockGroup;
    Task<TallyResult> PostStockItemAsync<StckItmType>(StckItmType stockItem, PostRequestOptions? postRequestOptions = null) where StckItmType : StockItem;

    #endregion

    #endregion

    #region Payroll Masters

    #region Get - Methods 
    Task<AttendanceTypType> GetAttendanceTypeAsync<AttendanceTypType>(string LookupValue, MasterRequestOptions? attendanceTypeOptions = null) where AttendanceTypType : AttendanceType;
    Task<EmployeeGroupType> GetEmployeeGroupAsync<EmployeeGroupType>(string LookupValue, MasterRequestOptions? employeeGroupOptions = null) where EmployeeGroupType : EmployeeGroup;

    Task<EmployeeType> GetEmployeeAsync<EmployeeType>(string LookupValue, MasterRequestOptions? employeeOptions = null) where EmployeeType : Employee;
    #endregion

    #region Post - Methods 
    Task<TallyResult> PostAttendanceTypeAsync<AttendanceTypType>(AttendanceTypType attendanceType, PostRequestOptions? postRequestOptions = null) where AttendanceTypType : AttendanceType;

    Task<TallyResult> PostEmployeeGroupAsync<EmployeeGroupType>(EmployeeGroupType employeeGroup, PostRequestOptions? postRequestOptions = null) where EmployeeGroupType : EmployeeGroup;

    Task<TallyResult> PostEmployeeAsync<EmployeeType>(EmployeeType employee, PostRequestOptions? postRequestOptions = null) where EmployeeType : Employee;

    #endregion

    #endregion

    #region Voucher

    #region Get - Methods 
    Task<Vchtype> GetVoucherAsync<Vchtype>(string lookupValue, VoucherRequestOptions? voucherRequestOptions = null) where Vchtype : Voucher;

    #endregion

    #region Post - Methods 
    Task<TallyResult> PostVoucherAsync<TVch>(TVch voucher, PostRequestOptions? postRequestOptions = null) where TVch : Voucher;

    #endregion

    #endregion

    #region Generic Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collectionOptions"></param>
    /// <returns></returns>
    string GenerateCollectionXML(CollectionRequestOptions collectionOptions);
    Task<List<ObjType>?> GetCustomCollectionAsync<ObjType>(CollectionRequestOptions collectionOptions) where ObjType : TallyBaseObject;

    Task<ObjType> GetObjectAsync<ObjType>(string lookupValue, MasterRequestOptions? requestOptions = null) where ObjType : TallyBaseObject, ITallyObject;
    Task<ObjType> GetObjectAsync<ObjType>(string lookupValue, VoucherRequestOptions? requestOptions = null) where ObjType : Voucher;
    Task<List<ObjType>?> GetObjectsAsync<ObjType>(PaginatedRequestOptions? objectOptions = null) where ObjType : TallyBaseObject;
    Task<List<ObjType>> GetAllObjectsAsync<ObjType>(RequestOptions? objectOptions = null, IProgress<ReportProgressHelper>? progress = null) where ObjType : TallyBaseObject;

    Task<ReturnType?> GetTDLReportAsync<ReportType, ReturnType>(DateFilterRequestOptions? requestOptions = null) where ReturnType : TallyBaseObject;

    Task<TallyResult> PostObjectToTallyAsync<ObjType>(ObjType Object, PostRequestOptions? postRequestOptions = null) where ObjType : TallyXmlJson, ITallyObject;


    #endregion

    Task<int?> GetObjectCountAync(CountRequestOptions options);

    /// <summary>
    /// A helper function to send request to Tally
    /// </summary>
    /// <param name="xml">xml that is required to send</param>
    /// <returns></returns>
    /// <returns></returns>
    Task<TallyResult> SendRequestAsync(string? xml = null);

    /// <summary>
    /// Checks whether xml as linerror and returns error
    /// if noerror return null
    /// </summary>
    /// <param name="ResXml"></param>
    /// <returns></returns>
    string? CheckTallyError(string ResXml);


    TallyResult ParseResponse(TallyResult tallyResult);
}