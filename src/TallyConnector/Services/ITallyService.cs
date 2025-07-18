﻿namespace TallyConnector.Services;

/// <summary>
/// contains API to interact with Tally
/// </summary>
public interface ITallyService : IBaseTallyService
{

}


//    /// <summary>
//    /// Gets Active company from tally
//    /// </summary>
//    /// <returns></returns>
//    Task<BaseCompany?> GetActiveCompanyAsync(CancellationToken token = default);



//    /// <summary>
//    /// Gets List of companies that are opened in Tally
//    /// </summary>
//    /// <typeparam name="CompanyType"></typeparam>
//    /// <returns></returns>
//    Task<List<CompanyType>?> GetCompaniesAsync<CompanyType>(CancellationToken token = default) where CompanyType : BaseCompany;
//    /// <summary>
//    /// Gets List of companies that are opened in Tally
//    /// </summary>
//    /// <param name="token">cancellation Token</param>
//    /// <returns></returns>
//    Task<List<Company>?> GetCompaniesAsync(CancellationToken token = default);
//    /// <summary>
//    /// Gets Last AlterId in Master and Voucher
//    /// </summary>
//    /// <returns>instance of LastMasterIdsRoot</returns>
//    Task<LastAlterIdsRoot?> GetLastAlterIdsAsync(CancellationToken token = default);

//    /// <summary>
//    /// Gets List of companies that are in Default Data Path of Tally
//    /// </summary>
//    /// <returns></returns>
//    Task<List<CompanyOnDisk>?> GetCompaniesinDefaultPathAsync(CancellationToken token = default);



//    Task<AutoVoucherStatisticsEnvelope> GetVoucherStatisticsAsync(AutoColumnReportPeriodRequestOptions? requestOptions = null, CancellationToken token = default);

//    #region Accounting Masters
//    /*
//     * Get and Post Methods of Accounting Masters - Currency, Group, Ledger,VoucherType, Cost Category, CostCetre
//     * To Get an Item use Get[BaseServiceActivityName]Async<Type>(lookupvalue,options);
//     * Ex:To Get Group use - await GetGroupAsync<Group>("BaseServiceActivityName");
//     */
//    #region Get - Methods 

//    Task<CurrencyType> GetCurrencyAsync<CurrencyType>(string lookupValue, MasterRequestOptions? currencyOptions = null, CancellationToken token = default) where CurrencyType : Currency;
//    Task<Currency> GetCurrencyAsync(string lookupValue, MasterRequestOptions? currencyOptions = null, CancellationToken token = default);

//    Task<GroupType> GetGroupAsync<GroupType>(string lookupValue, MasterRequestOptions? groupOptions = null, CancellationToken token = default) where GroupType : Group;
//    Task<Group> GetGroupAsync(string lookupValue, MasterRequestOptions? groupOptions = null, CancellationToken token = default);

//    Task<LedgerType> GetLedgerAsync<LedgerType>(string LookupValue, MasterRequestOptions? ledgerOptions = null, CancellationToken token = default) where LedgerType : Ledger;
//    Task<Ledger> GetLedgerAsync(string LookupValue, MasterRequestOptions? ledgerOptions = null, CancellationToken token = default);

//    Task<VChTypeType> GetVoucherTypeAsync<VChTypeType>(string LookupValue, MasterRequestOptions? voucherTypeOptions = null, CancellationToken token = default) where VChTypeType : VoucherType;
//    Task<VoucherType> GetVoucherTypeAsync(string LookupValue, MasterRequestOptions? voucherTypeOptions = null, CancellationToken token = default);

//    Task<CostCategoryType> GetCostCategoryAsync<CostCategoryType>(string LookupValue, MasterRequestOptions? costCategoryOptions = null, CancellationToken token = default) where CostCategoryType : CostCategory;
//    Task<CostCategory> GetCostCategoryAsync(string LookupValue, MasterRequestOptions? costCategoryOptions = null, CancellationToken token = default);
//    Task<CostCentreType> GetCostCenterAsync<CostCentreType>(string LookupValue, MasterRequestOptions? costCenterOptions = null, CancellationToken token = default) where CostCentreType : CostCentre;
//    Task<CostCentre> GetCostCenterAsync(string LookupValue, MasterRequestOptions? costCenterOptions = null, CancellationToken token = default);
//    Task<TaxUnitType> GetTaxUnitAsync<TaxUnitType>(string LookupValue, MasterRequestOptions? costCenterOptions = null, CancellationToken token = default) where TaxUnitType : TaxUnit;
//    Task<TaxUnit> GetTaxUnitAsync(string LookupValue, MasterRequestOptions? costCenterOptions = null, CancellationToken token = default);

//    #endregion

//    /*
//     * Bulk Get Methods of Accounting Masters by Pagination - Currency, Group, Ledger,VoucherType, Cost Category, CostCetre
//     * To Get Items with default type use Get[BaseServiceActivityName]sPaginatedAsync<Type>(options);
//     * To Get Items with custom type use Get[BaseServiceActivityName]sPaginatedAsync(options);
//     * Ex:To Get Groups with default type use - await GetGroupsPaginatedAsync(options);
//     * Ex:To Get Groups with custom type use - await GetGroupsPaginatedAsync<Group>(options);
//     */
//    #region Bulk GetPaginated - Methods 

//    /// <inheritdoc cref="GetCurrenciesAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<CurrencyType>?> GetCurrenciesAsync<CurrencyType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where CurrencyType : Currency;

//    /// <inheritdoc cref="GetCurrenciesAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<Currency>?> GetCurrenciesAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);

//    /// <inheritdoc cref="GetGroupsAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<GroupType>?> GetGroupsAsync<GroupType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where GroupType : Group;

//    /// <inheritdoc cref="GetGroupsAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<Group>?> GetGroupsAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);

//    /// <inheritdoc cref="GetLedgersAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<LedgerType>?> GetLedgersAsync<LedgerType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where LedgerType : Ledger;

//    /// <summary>
//    /// Get All Ledgers
//    /// </summary>
//    /// <param name="ReqOptions">Options to Get Ledgers</param>
//    /// <returns></returns>
//    Task<PaginatedResponse<Ledger>?> GetLedgersAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);

//    /// <inheritdoc cref="GetVoucherTypesAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<VChTypeType>?> GetVoucherTypesAsync<VChTypeType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where VChTypeType : VoucherType;

//    /// <inheritdoc cref="GetVoucherTypesAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<VoucherType>?> GetVoucherTypesAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);
//    /// <inheritdoc cref="GetCostCategoriesAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<CostCategoryType>?> GetCostCategoriesAsync<CostCategoryType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where CostCategoryType : CostCategory;
//    /// <inheritdoc cref="GetCostCategoriesAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<CostCategory>?> GetCostCategoriesAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);

//    /// <inheritdoc cref="GetCostCentersAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<CostCentreType>?> GetCostCentersAsync<CostCentreType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where CostCentreType : CostCentre;

//    /// <inheritdoc cref="GetCostCentersAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<CostCentre>?> GetCostCentersAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);

//    /// <inheritdoc cref="GetTaxUnitsAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<TaxUnitType>?> GetTaxUnitsAsync<TaxUnitType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where TaxUnitType : TaxUnit;

//    /// <inheritdoc cref="GetTaxUnitsAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<PaginatedResponse<TaxUnit>?> GetTaxUnitsAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);

//    #endregion

//    /*
//     * Bulk Get Methods of Accounting Masters without Pagination - Currency, Group, Ledger,VoucherType, Cost Category, CostCetre
//     * To Get Items use Get[BaseServiceActivityName]sAsync<Type>(options);
//     * Ex:To Get Groups use - await GetGroupsAsync<Group>(options);
//     */
//    #region Bulk Get - Methods

//    /// <typeparam name="CurrencyType">type of currency</typeparam>
//    /// <returns cref="List{CurrencyType}"></returns>
//    /// <inheritdoc cref="GetCurrenciesAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<List<CurrencyType>?> GetCurrenciesAsync<CurrencyType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where CurrencyType : Currency;

//    /// <summary>
//    /// Gets All Currencies
//    /// </summary>
//    /// <param name="ReqOptions">options to configure this request</param>
//    /// <param name="token"></param>
//    /// <returns cref="List{Currency}">List of Currencies</returns>
//    Task<List<Currency>?> GetCurrenciesAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);

//    /// <inheritdoc cref="GetGroupsAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<List<GroupType>?> GetGroupsAsync<GroupType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where GroupType : Group;

//    /// <summary>
//    /// Gets all Groups
//    /// </summary>
//    /// <param name="ReqOptions">options to configure this request</param>
//    /// <returns></returns>
//    Task<List<Group>?> GetGroupsAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);

//    /// <inheritdoc cref="GetLedgersAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<List<LedgerType>?> GetLedgersAsync<LedgerType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where LedgerType : Ledger;
//    /// <summary>
//    /// Gets all Ledgers
//    /// </summary>
//    /// <param name="ReqOptions">options to configure this request</param>
//    /// <param name="token"></param>
//    /// <returns></returns>
//    Task<List<Ledger>?> GetLedgersAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);

//    /// <inheritdoc cref="GetVoucherTypesAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<List<VChTypeType>?> GetVoucherTypesAsync<VChTypeType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where VChTypeType : VoucherType;
//    /// <summary>
//    /// Gets all VoucherTypes
//    /// </summary>
//    /// <param name="ReqOptions">options to configure this request</param>
//    /// <param name="token"></param>
//    /// <returns></returns>
//    Task<List<VoucherType>?> GetVoucherTypesAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);

//    /// <inheritdoc cref="GetCostCategoriesAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<List<CostCategoryType>?> GetCostCategoriesAsync<CostCategoryType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where CostCategoryType : CostCategory;
//    /// <summary>
//    /// Gets all CostCategories
//    /// </summary>
//    /// <param name="ReqOptions">options to configure this request</param>
//    /// <param name="token"></param>
//    /// <returns></returns>
//    Task<List<CostCategory>?> GetCostCategoriesAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);

//    /// <inheritdoc cref="GetCostCentersAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<List<CostCentreType>?> GetCostCentersAsync<CostCentreType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where CostCentreType : CostCentre;
//    /// <summary>
//    /// Gets all CostCentres
//    /// </summary>
//    /// <param name="ReqOptions">options to configure this request</param>
//    /// <param name="token"></param>
//    /// <returns></returns>
//    Task<List<CostCentre>?> GetCostCentersAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);


//    /// <inheritdoc cref="GetTaxUnitsAsync(RequestOptionsBuilder,CancellationToken)" />
//    Task<List<TaxUnitType>?> GetTaxUnitsAsync<TaxUnitType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where TaxUnitType : TaxUnit;
//    /// <summary>
//    /// Gets all CostCentres
//    /// </summary>
//    /// <param name="ReqOptions">options to configure this request</param>
//    /// <param name="token"></param>
//    /// <returns></returns>
//    Task<List<TaxUnit>?> GetTaxUnitsAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);


//    #endregion

//    #region Post-Methods
//    Task<TallyResult> PostCurrencyAsync<CurrencyType>(CurrencyType currency, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where CurrencyType : Currency;

//    Task<TallyResult> PostGroupAsync<GroupType>(GroupType group, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where GroupType : Group;
//    Task<TallyResult> PostLedgerAsync<LedgerType>(LedgerType ledger, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where LedgerType : Ledger;

//    Task<TallyResult> PostVoucherTypeAsync<VChTypeType>(VChTypeType voucherType, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where VChTypeType : VoucherType;

//    Task<TallyResult> PostCostCategoryAsync<CostCategoryType>(CostCategoryType costCategory, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where CostCategoryType : CostCategory;
//    Task<TallyResult> PostCostCenterAsync<CostCentreType>(CostCentreType costCenter, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where CostCentreType : CostCentre;
//    Task<TallyResult> PostTaxUnitAsync<TaxUnitType>(TaxUnitType costCenter, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where TaxUnitType : TaxUnit;

//    #endregion

//    #endregion

//    #region Inventory Masters

//    #region Get - Methods 
//    Task<UnitType> GetUnitAsync<UnitType>(string LookupValue, MasterRequestOptions? unitOptions = null, CancellationToken token = default) where UnitType : Unit;
//    Task<Unit> GetUnitAsync(string LookupValue, MasterRequestOptions? unitOptions = null, CancellationToken token = default);
//    Task<GodownType> GetGodownAsync<GodownType>(string LookupValue, MasterRequestOptions? godownOptions = null, CancellationToken token = default) where GodownType : Godown;
//    Task<Godown> GetGodownAsync(string LookupValue, MasterRequestOptions? godownOptions = null, CancellationToken token = default);
//    Task<StockCategoryType> GetStockCategoryAsync<StockCategoryType>(string LookupValue, MasterRequestOptions? stockCategoryOptions = null, CancellationToken token = default) where StockCategoryType : StockCategory;
//    Task<StockCategory> GetStockCategoryAsync(string LookupValue, MasterRequestOptions? stockCategoryOptions = null, CancellationToken token = default);
//    Task<StockGroupType> GetStockGroupAsync<StockGroupType>(string LookupValue, MasterRequestOptions? stockGroupOptions = null, CancellationToken token = default) where StockGroupType : StockGroup;
//    Task<StockGroup> GetStockGroupAsync(string LookupValue, MasterRequestOptions? stockGroupOptions = null, CancellationToken token = default);
//    Task<StckItmType> GetStockItemAsync<StckItmType>(string LookupValue, MasterRequestOptions? stockItemOptions = null, CancellationToken token = default) where StckItmType : StockItem;
//    Task<StockItem> GetStockItemAsync(string LookupValue, MasterRequestOptions? stockItemOptions = null, CancellationToken token = default);

//    #region Bulk GetPaginated - Methods 
//    Task<PaginatedResponse<UnitType>?> GetUnitsAsync<UnitType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where UnitType : Unit;
//    Task<PaginatedResponse<Unit>?> GetUnitsAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);
//    Task<PaginatedResponse<GodownType>?> GetGodownsAsync<GodownType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where GodownType : Godown;
//    Task<PaginatedResponse<Godown>?> GetGodownsAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);
//    Task<PaginatedResponse<StockCategoryType>?> GetStockCategoriesAsync<StockCategoryType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where StockCategoryType : StockCategory;
//    Task<PaginatedResponse<StockCategory>?> GetStockCategoriesAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);
//    Task<PaginatedResponse<StockGroupType>?> GetStockGroupsAsync<StockGroupType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where StockGroupType : StockGroup;
//    Task<PaginatedResponse<StockGroup>?> GetStockGroupsAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);
//    Task<PaginatedResponse<StckItmType>?> GetStockItemsAsync<StckItmType>(PaginatedRequestOptions ReqOptions, CancellationToken token = default) where StckItmType : StockItem;
//    Task<PaginatedResponse<StockItem>?> GetStockItemsAsync(PaginatedRequestOptions ReqOptions, CancellationToken token = default);

//    #endregion

//    #region Bulk Get - Methods 
//    /// <summary>
//    /// Get All Units
//    /// </summary>
//    /// <param name="ReqOptions"></param>
//    /// <param name="token"></param>
//    /// <returns></returns>
//    Task<List<UnitType>?> GetUnitsAsync<UnitType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where UnitType : Unit;

//    Task<List<Unit>?> GetUnitsAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);
//    Task<List<GodownType>?> GetGodownsAsync<GodownType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where GodownType : Godown;

//    Task<List<Godown>?> GetGodownsAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);
//    Task<List<StockCategoryType>?> GetStockCategoriesAsync<StockCategoryType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where StockCategoryType : StockCategory;

//    Task<List<StockCategory>?> GetStockCategoriesAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);
//    Task<List<StockGroupType>?> GetStockGroupsAsync<StockGroupType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where StockGroupType : StockGroup;

//    Task<List<StockGroup>?> GetStockGroupsAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);
//    Task<List<StckItmType>?> GetStockItemsAsync<StckItmType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where StckItmType : StockItem;

//    Task<List<StockItem>?> GetStockItemsAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);

//    #endregion

//    #endregion

//    #region Post - Methods 

//    Task<TallyResult> PostUnitAsync<UnitType>(UnitType unit, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where UnitType : Unit;

//    Task<TallyResult> PostGodownAsync<GodownType>(GodownType godown, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where GodownType : Godown;

//    Task<TallyResult> PostStockCategoryAsync<StockCategoryType>(StockCategoryType stockCategory, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where StockCategoryType : StockCategory;
//    Task<TallyResult> PostStockGroupAsync<StockGroupType>(StockGroupType stockGroup, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where StockGroupType : StockGroup;
//    Task<TallyResult> PostStockItemAsync<StckItmType>(StckItmType stockItem, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where StckItmType : StockItem;

//    #endregion

//    #endregion

//    #region Payroll Masters

//    #region Get - Methods 
//    Task<AttendanceTypType> GetAttendanceTypeAsync<AttendanceTypType>(string LookupValue, MasterRequestOptions? attendanceTypeOptions = null, CancellationToken token = default) where AttendanceTypType : AttendanceType;
//    Task<AttendanceType> GetAttendanceTypeAsync(string LookupValue, MasterRequestOptions? attendanceTypeOptions = null, CancellationToken token = default);
//    Task<EmployeeGroupType> GetEmployeeGroupAsync<EmployeeGroupType>(string LookupValue, MasterRequestOptions? employeeGroupOptions = null, CancellationToken token = default) where EmployeeGroupType : EmployeeGroup;

//    Task<EmployeeType> GetEmployeeAsync<EmployeeType>(string LookupValue, MasterRequestOptions? employeeOptions = null, CancellationToken token = default) where EmployeeType : Employee;
//    #endregion

//    #region Bulk Get - Methods 
//    Task<List<AtndTypeType>?> GetAttendanceTypesAsync<AtndTypeType>(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default) where AtndTypeType : AttendanceType;
//    Task<List<AttendanceType>?> GetAttendanceTypesAsync(RequestOptionsBuilder? ReqOptions = null, CancellationToken token = default);

//    #endregion

//    #region Bulk GetPaginated - Methods 
//    Task<PaginatedResponse<AtndTypeType>?> GetAttendanceTypesAsync<AtndTypeType>(PaginatedRequestOptions? options, CancellationToken token = default) where AtndTypeType : AttendanceType;
//    Task<PaginatedResponse<AttendanceType>?> GetAttendanceTypesAsync(PaginatedRequestOptions? options, CancellationToken token = default);

//    #endregion
//    #region Post - Methods 
//    Task<TallyResult> PostAttendanceTypeAsync<AttendanceTypType>(AttendanceTypType attendanceType, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where AttendanceTypType : AttendanceType;

//    Task<TallyResult> PostEmployeeGroupAsync<EmployeeGroupType>(EmployeeGroupType employeeGroup, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where EmployeeGroupType : EmployeeGroup;

//    Task<TallyResult> PostEmployeeAsync<EmployeeType>(EmployeeType employee, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where EmployeeType : Employee;

//    #endregion

//    #endregion

//    #region Voucher

//    #region Get - Methods 
//    Task<Vchtype> GetVoucherAsync<Vchtype>(string lookupValue, VoucherRequestOptions? voucherRequestOptions = null, CancellationToken token = default) where Vchtype : Voucher;
//    Task<Voucher> GetVoucherAsync(string lookupValue, VoucherRequestOptions? voucherRequestOptions = null, CancellationToken token = default);

//    #endregion

//    #region Post - Methods 
//    Task<TallyResult> PostVoucherAsync<TVch>(TVch voucher, PostRequestOptions? postRequestOptions = null, CancellationToken token = default) where TVch : Voucher;

//    #endregion

//    #endregion

//    #region Generic Methods
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="collectionOptions"></param>
//    /// <param name="indented"></param>
//    /// <returns></returns>
//    Task<string> GenerateCollectionXML(CollectionRequestOptions collectionOptions, bool indented = false);
//    Task<PaginatedResponse<ObjType>?> GetCustomCollectionAsync<ObjType>(CollectionRequestOptions collectionOptions, string? requestType = null, CancellationToken token = default) where ObjType : TallyBaseObject;

//    Task<ObjType> GetObjectAsync<ObjType>(string lookupValue, MasterRequestOptions? requestOptions = null, CancellationToken token = default) where ObjType : TallyBaseObject, INamedTallyObject;
//    Task<ObjType> GetObjectAsync<ObjType>(string lookupValue, VoucherRequestOptions? requestOptions = null, CancellationToken token = default) where ObjType : Voucher;
//    Task<PaginatedResponse<ObjType>?> GetObjectsAsync<ObjType>(PaginatedRequestOptions? objectOptions = null, CancellationToken token = default) where ObjType : TallyBaseObject;
//    Task<List<ObjType>> GetAllObjectsAsync<ObjType>(RequestOptionsBuilder? objectOptions = null, IProgress<ReportProgressHelper>? progress = null, CancellationToken token = default) where ObjType : TallyBaseObject;

//    Task<ReturnType?> GetTDLReportAsync<ReportType, ReturnType>(DateFilterRequestOptions? requestOptions = null, string? requestType = null, CancellationToken token = default);
//    Task<ReturnType?> GetTDLReportAsync<ReturnType>(DateFilterRequestOptions? requestOptions = null, CancellationToken token = default) where ReturnType : TallyBaseObject;

//    Task<TallyResult> PostObjectToTallyAsync<ObjType>(ObjType Object, PostRequestOptions? postRequestOptions = null, string? requestType = null, CancellationToken token = default) where ObjType : TallyXml, ITallyObject;


//    #endregion

//    /// <summary>
//    /// Get Count of collection Type
//    /// </summary>
//    /// <param name="options">options to configure this requests</param>
//    /// <param name="requestType">options to configure this requests</param>
//    /// <param name="token"></param>
//    /// <returns></returns>
//    Task<int?> GetObjectCountAync(CountRequestOptions options, string? requestType = null, CancellationToken token = default);


//}
