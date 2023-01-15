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
    /// Gets Last AlterId in Master and Voucher
    /// </summary>
    /// <returns>instance of LastMasterIdsRoot</returns>
    Task<LastAlterIdsRoot?> GetLastAlterIdsAsync();

    /// <summary>
    /// Gets List of companies that are in Default Data Path of Tally
    /// </summary>
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
    Task<CostCentreType> GetCostCenterAsync<CostCentreType>(string LookupValue, MasterRequestOptions? costCenterOptions = null) where CostCentreType : CostCenter;
    #endregion

    /*
     * Bulk Get Methods of Accounting Masters by Pagination - Currency, Group, Ledger,VoucherType, Cost Category, CostCetre
     * To Get Items with default type use Get[Name]sPaginatedAsync<Type>(options);
     * To Get Items with custom type use Get[Name]sPaginatedAsync(options);
     * Ex:To Get Groups with default type use - await GetGroupsPaginatedAsync(options);
     * Ex:To Get Groups with custom type use - await GetGroupsPaginatedAsync<Group>(options);
     */
    #region Bulk GetPaginated - Methods 

    /// <inheritdoc cref="GetCurrenciesAsync(RequestOptions)" />
    Task<PaginatedResponse<CurrencyType>?> GetCurrenciesAsync<CurrencyType>(PaginatedRequestOptions ReqOptions) where CurrencyType : Currency;

    /// <inheritdoc cref="GetCurrenciesAsync(RequestOptions)" />
    Task<PaginatedResponse<Currency>?> GetCurrenciesAsync(PaginatedRequestOptions ReqOptions);

    /// <inheritdoc cref="GetGroupsAsync(RequestOptions)" />
    Task<PaginatedResponse<GroupType>?> GetGroupsAsync<GroupType>(PaginatedRequestOptions ReqOptions) where GroupType : Group;

    /// <inheritdoc cref="GetGroupsAsync(RequestOptions)" />
    Task<PaginatedResponse<Group>?> GetGroupsAsync(PaginatedRequestOptions ReqOptions);

    /// <inheritdoc cref="GetLedgersAsync(RequestOptions)" />
    Task<PaginatedResponse<LedgerType>?> GetLedgersAsync<LedgerType>(PaginatedRequestOptions ReqOptions) where LedgerType : Ledger;

    /// <summary>
    /// Get All Ledgers
    /// </summary>
    /// <param name="ReqOptions">Options to Get Ledgers</param>
    /// <returns></returns>
    Task<PaginatedResponse<Ledger>?> GetLedgersAsync(PaginatedRequestOptions ReqOptions);

    /// <inheritdoc cref="GetVoucherTypesAsync(RequestOptions)" />
    Task<PaginatedResponse<VChTypeType>?> GetVoucherTypesAsync<VChTypeType>(PaginatedRequestOptions ReqOptions) where VChTypeType : VoucherType;

    /// <inheritdoc cref="GetVoucherTypesAsync(RequestOptions)" />
    Task<PaginatedResponse<VoucherType>?> GetVoucherTypesAsync(PaginatedRequestOptions ReqOptions);
    /// <inheritdoc cref="GetCostCategoriesAsync(RequestOptions)" />
    Task<PaginatedResponse<CostCategoryType>?> GetCostCategoriesAsync<CostCategoryType>(PaginatedRequestOptions ReqOptions) where CostCategoryType : CostCategory;
    /// <inheritdoc cref="GetCostCategoriesAsync(RequestOptions)" />
    Task<PaginatedResponse<CostCategory>?> GetCostCategoriesAsync(PaginatedRequestOptions ReqOptions);

    /// <inheritdoc cref="GetCostCentersAsync(RequestOptions)" />
    Task<PaginatedResponse<CostCentreType>?> GetCostCentersAsync<CostCentreType>(PaginatedRequestOptions ReqOptions) where CostCentreType : CostCenter;

    /// <inheritdoc cref="GetCostCentersAsync(RequestOptions)" />
    Task<PaginatedResponse<CostCenter>?> GetCostCentersAsync(PaginatedRequestOptions ReqOptions);

    #endregion

    /*
     * Bulk Get Methods of Accounting Masters without Pagination - Currency, Group, Ledger,VoucherType, Cost Category, CostCetre
     * To Get Items use Get[Name]sAsync<Type>(options);
     * Ex:To Get Groups use - await GetGroupsAsync<Group>(options);
     */
    #region Bulk Get - Methods

    /// <typeparam name="CurrencyType">type of currency</typeparam>
    /// <returns cref="List{CurrencyType}"></returns>
    /// <inheritdoc cref="GetCurrenciesAsync(RequestOptions)" />
    Task<List<CurrencyType>?> GetCurrenciesAsync<CurrencyType>(RequestOptions? ReqOptions = null) where CurrencyType : Currency;

    /// <summary>
    /// Gets All Currencies
    /// </summary>
    /// <param name="ReqOptions">options to configure this request</param>
    /// <returns cref="List{Currency}">List of Currencies</returns>
    Task<List<Currency>?> GetCurrenciesAsync(RequestOptions? ReqOptions = null);

    /// <inheritdoc cref="GetGroupsAsync(RequestOptions)" />
    Task<List<GroupType>?> GetGroupsAsync<GroupType>(RequestOptions? ReqOptions = null) where GroupType : Group;

    /// <summary>
    /// Gets all Groups
    /// </summary>
    /// <param name="ReqOptions">options to configure this request</param>
    /// <returns></returns>
    Task<List<Group>?> GetGroupsAsync(RequestOptions? ReqOptions = null);

    /// <inheritdoc cref="GetLedgersAsync(RequestOptions)" />
    Task<List<LedgerType>?> GetLedgersAsync<LedgerType>(RequestOptions? ReqOptions = null) where LedgerType : Ledger;
    /// <summary>
    /// Gets all Ledgers
    /// </summary>
    /// <param name="ReqOptions">options to configure this request</param>
    /// <returns></returns>
    Task<List<Ledger>?> GetLedgersAsync(RequestOptions? ReqOptions = null);

    /// <inheritdoc cref="GetVoucherTypesAsync(RequestOptions)" />
    Task<List<VChTypeType>?> GetVoucherTypesAsync<VChTypeType>(RequestOptions? ReqOptions = null) where VChTypeType : VoucherType;
    /// <summary>
    /// Gets all VoucherTypes
    /// </summary>
    /// <param name="ReqOptions">options to configure this request</param>
    /// <returns></returns>
    Task<List<VoucherType>?> GetVoucherTypesAsync(RequestOptions? ReqOptions = null);

    /// <inheritdoc cref="GetCostCategoriesAsync(RequestOptions)" />
    Task<List<CostCategoryType>?> GetCostCategoriesAsync<CostCategoryType>(RequestOptions? ReqOptions = null) where CostCategoryType : CostCategory;
    /// <summary>
    /// Gets all CostCategories
    /// </summary>
    /// <param name="ReqOptions">options to configure this request</param>
    /// <returns></returns>
    Task<List<CostCategory>?> GetCostCategoriesAsync(RequestOptions? ReqOptions = null);

    /// <inheritdoc cref="GetCostCentersAsync(RequestOptions)" />
    Task<List<CostCentreType>?> GetCostCentersAsync<CostCentreType>(RequestOptions? ReqOptions = null) where CostCentreType : CostCenter;
    /// <summary>
    /// Gets all CostCentres
    /// </summary>
    /// <param name="ReqOptions">options to configure this request</param>
    /// <returns></returns>
    Task<List<CostCenter>?> GetCostCentersAsync(RequestOptions? ReqOptions = null);


    #endregion

    #region Post-Methods
    Task<TallyResult> PostCurrencyAsync<CurrencyType>(CurrencyType currency, PostRequestOptions? postRequestOptions = null) where CurrencyType : Currency;

    Task<TallyResult> PostGroupAsync<GroupType>(GroupType group, PostRequestOptions? postRequestOptions = null) where GroupType : Group;
    Task<TallyResult> PostLedgerAsync<LedgerType>(LedgerType ledger, PostRequestOptions? postRequestOptions = null) where LedgerType : Ledger;

    Task<TallyResult> PostVoucherTypeAsync<VChTypeType>(VChTypeType voucherType, PostRequestOptions? postRequestOptions = null) where VChTypeType : VoucherType;

    Task<TallyResult> PostCostCategoryAsync<CostCategoryType>(CostCategoryType costCategory, PostRequestOptions? postRequestOptions = null) where CostCategoryType : CostCategory;
    Task<TallyResult> PostCostCenterAsync<CostCentreType>(CostCentreType costCenter, PostRequestOptions? postRequestOptions = null) where CostCentreType : CostCenter;
    #endregion

    #endregion

    #region Inventory Masters

    #region Get - Methods 
    Task<UnitType> GetUnitAsync<UnitType>(string LookupValue, MasterRequestOptions? unitOptions = null) where UnitType : Unit;
    Task<GodownType> GetGodownAsync<GodownType>(string LookupValue, MasterRequestOptions? godownOptions = null) where GodownType : Godown;
    Task<StockCategoryType> GetStockCategoryAsync<StockCategoryType>(string LookupValue, MasterRequestOptions? stockCategoryOptions = null) where StockCategoryType : StockCategory;
    Task<StockGroupType> GetStockGroupAsync<StockGroupType>(string LookupValue, MasterRequestOptions? stockGroupOptions = null) where StockGroupType : StockGroup;
    Task<StckItmType> GetStockItemAsync<StckItmType>(string LookupValue, MasterRequestOptions? stockItemOptions = null) where StckItmType : StockItem;

    #region Bulk GetPaginated - Methods 
    Task<PaginatedResponse<UnitType>?> GetUnitsAsync<UnitType>(PaginatedRequestOptions ReqOptions) where UnitType : Unit;
    Task<PaginatedResponse<Unit>?> GetUnitsAsync(PaginatedRequestOptions ReqOptions);

    #endregion
    #region Bulk Get - Methods 
    /// <summary>
    /// Get All Units
    /// </summary>
    /// <param name="ReqOptions"></param>
    /// <returns></returns>
    Task<List<UnitType>?> GetUnitsAsync<UnitType>(RequestOptions? ReqOptions = null) where UnitType : Unit;

    Task<List<Unit>?> GetUnitsAsync(RequestOptions? ReqOptions = null);

    #endregion

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
    /// <param name="indented"></param>
    /// <returns></returns>
    string GenerateCollectionXML(CollectionRequestOptions collectionOptions, bool indented = false);
    Task<PaginatedResponse<ObjType>?> GetCustomCollectionAsync<ObjType>(CollectionRequestOptions collectionOptions) where ObjType : TallyBaseObject;

    Task<ObjType> GetObjectAsync<ObjType>(string lookupValue, MasterRequestOptions? requestOptions = null) where ObjType : TallyBaseObject, INamedTallyObject;
    Task<ObjType> GetObjectAsync<ObjType>(string lookupValue, VoucherRequestOptions? requestOptions = null) where ObjType : Voucher;
    Task<PaginatedResponse<ObjType>?> GetObjectsAsync<ObjType>(PaginatedRequestOptions? objectOptions = null) where ObjType : TallyBaseObject;
    Task<List<ObjType>> GetAllObjectsAsync<ObjType>(RequestOptions? objectOptions = null, IProgress<ReportProgressHelper>? progress = null) where ObjType : TallyBaseObject;

    Task<ReturnType?> GetTDLReportAsync<ReportType, ReturnType>(DateFilterRequestOptions? requestOptions = null);

    Task<TallyResult> PostObjectToTallyAsync<ObjType>(ObjType Object, PostRequestOptions? postRequestOptions = null) where ObjType : TallyXmlJson, ITallyObject;


    #endregion

    /// <summary>
    /// Get Count of collection Type
    /// </summary>
    /// <param name="options">options to configure this requests</param>
    /// <returns></returns>
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

    /// <summary>
    /// Parse response in XML and return TallyResult
    /// </summary>
    /// <param name="tallyResult"></param>
    /// <returns></returns>
    TallyResult ParseResponse(TallyResult tallyResult);
}