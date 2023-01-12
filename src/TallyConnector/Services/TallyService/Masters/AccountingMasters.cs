using TallyConnector.Core.Models.Masters;
using TallyConnector.Core.Models.Masters.CostCenter;

namespace TallyConnector.Services;
public partial class TallyService
{

    #region Currencies
    /// <inheritdoc/>
    public async Task<CurrencyType> GetCurrencyAsync<CurrencyType>(string lookupValue,
                                                                   MasterRequestOptions? currencyOptions = null) where CurrencyType : Currency
    {
        return await GetObjectAsync<CurrencyType>(lookupValue, currencyOptions);
    }
    /// <inheritdoc/>
    public async Task<List<CurrencyType>?> GetCurrenciesAsync<CurrencyType>(PaginatedRequestOptions? options = null) where CurrencyType : Currency
    {
        return await GetObjectsAsync<CurrencyType>(options);
    }
    /// <inheritdoc/>
    public async Task<List<Currency>?> GetCurrenciesAsync(PaginatedRequestOptions? options = null)
    {
        return await GetCurrenciesAsync<Currency>(options);
    }
    /// <inheritdoc/>
    public async Task<List<CurrencyType>?> GetCurrenciesAsync<CurrencyType>(RequestOptions? options = null) where CurrencyType : Currency
    {
        return await GetAllObjectsAsync<CurrencyType>(options);
    }
    /// <inheritdoc/>
    public async Task<List<Currency>?> GetCurrenciesAsync(RequestOptions? options = null)
    {
        return await GetCurrenciesAsync<Currency>(options);
    }

    /// <inheritdoc/>
    public async Task<TallyResult> PostCurrencyAsync<CurrencyType>(CurrencyType currency,
                                                                   PostRequestOptions? postRequestOptions = null) where CurrencyType : Currency
    {
        return await PostObjectToTallyAsync(currency, postRequestOptions);
    }
    #endregion

    #region Groups

    /// <inheritdoc/>
    public async Task<GroupType> GetGroupAsync<GroupType>(string lookupValue,
                                                          MasterRequestOptions? options = null) where GroupType : Group
    {
        return await GetObjectAsync<GroupType>(lookupValue, options);
    }
    /// <inheritdoc/>
    public async Task<List<GroupType>?> GetGroupsAsync<GroupType>(PaginatedRequestOptions options) where GroupType : Group
    {
        return await GetObjectsAsync<GroupType>(options);
    }
    /// <inheritdoc/>
    public async Task<List<Group>?> GetGroupsAsync(PaginatedRequestOptions options)
    {
        return await GetObjectsAsync<Group>(options);
    }
    /// <inheritdoc/>
    public async Task<List<GroupType>?> GetGroupsAsync<GroupType>(RequestOptions? ReqOptions = null) where GroupType : Group
    {
        return await GetAllObjectsAsync<GroupType>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<Group>?> GetGroupsAsync(RequestOptions? ReqOptions = null)
    {
        return await GetAllObjectsAsync<Group>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<TallyResult> PostGroupAsync<GroupType>(GroupType group,
                                                             PostRequestOptions? postRequestOptions = null) where GroupType : Group
    {
        return await PostObjectToTallyAsync(group, postRequestOptions);
    }
    #endregion

    #region Ledgers
    /// <inheritdoc/>
    public async Task<LedgerType> GetLedgerAsync<LedgerType>(string LookupValue,
                                                             MasterRequestOptions? options = null) where LedgerType : Ledger
    {
        return await GetObjectAsync<LedgerType>(LookupValue, options);
    }

    /// <inheritdoc/>
    public async Task<List<LedgerType>?> GetLedgersAsync<LedgerType>(PaginatedRequestOptions options) where LedgerType : Ledger
    {
        return await GetObjectsAsync<LedgerType>(options);
    }
    /// <inheritdoc/>
    public async Task<List<Ledger>?> GetLedgersAsync(PaginatedRequestOptions options)
    {
        return await GetObjectsAsync<Ledger>(options);
    }
    /// <inheritdoc/>
    public async Task<List<LedgerType>?> GetLedgersAsync<LedgerType>(RequestOptions? ReqOptions = null) where LedgerType : Ledger
    {
        return await GetAllObjectsAsync<LedgerType>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<Ledger>?> GetLedgersAsync(RequestOptions? ReqOptions = null)
    {
        return await GetAllObjectsAsync<Ledger>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<TallyResult> PostLedgerAsync<LedgerType>(LedgerType ledger,
                                                               PostRequestOptions? postRequestOptions = null) where LedgerType : Ledger
    {

        return await PostObjectToTallyAsync(ledger, postRequestOptions);
    }
    #endregion

    #region Related to CostCenter

    #region CostCategory
    /// <inheritdoc/>
    public async Task<CostCategoryType> GetCostCategoryAsync<CostCategoryType>(string LookupValue,
                                                             MasterRequestOptions? costCategoryOptions = null) where CostCategoryType : CostCategory
    {
        return await GetObjectAsync<CostCategoryType>(LookupValue, costCategoryOptions);
    }
    /// <inheritdoc/>
    public async Task<List<CostCategoryType>?> GetCostCategoriesAsync<CostCategoryType>(PaginatedRequestOptions ReqOptions) where CostCategoryType : CostCategory
    {
        return await GetObjectsAsync<CostCategoryType>(ReqOptions);
    }
    /// <inheritdoc/>
    public Task<List<CostCategory>?> GetCostCategoriesAsync(PaginatedRequestOptions ReqOptions)
    {
        return GetCostCategoriesAsync<CostCategory>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<CostCategoryType>?> GetCostCategoriesAsync<CostCategoryType>(RequestOptions? ReqOptions = null) where CostCategoryType : CostCategory
    {
        return await GetAllObjectsAsync<CostCategoryType>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<CostCategory>?> GetCostCategoriesAsync(RequestOptions? ReqOptions = null)
    {
        return await GetCostCategoriesAsync<CostCategory>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<TallyResult> PostCostCategoryAsync<CostCategoryType>(CostCategoryType costCategory,
                                                                           PostRequestOptions? postRequestOptions = null) where CostCategoryType : CostCategory
    {

        return await PostObjectToTallyAsync(costCategory, postRequestOptions);
    }
    #endregion

    #region CostCentre
    /// <inheritdoc/>
    public async Task<CostCentreType> GetCostCentreAsync<CostCentreType>(string LookupValue,
                                                                         MasterRequestOptions? costCenterOptions = null) where CostCentreType : CostCenter
    {
        return await GetObjectAsync<CostCentreType>(LookupValue, costCenterOptions);
    }
    /// <inheritdoc/>
    public async Task<List<CostCentreType>?> GetCostCentresAsync<CostCentreType>(PaginatedRequestOptions ReqOptions) where CostCentreType : CostCenter
    {
        return await GetObjectsAsync<CostCentreType>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<CostCenter>?> GetCostCentresAsync(PaginatedRequestOptions ReqOptions)
    {
        return await GetCostCentresAsync<CostCenter>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<CostCentreType>?> GetCostCentresAsync<CostCentreType>(RequestOptions? ReqOptions = null) where CostCentreType : CostCenter
    {
        return await GetAllObjectsAsync<CostCentreType>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<CostCenter>?> GetCostCentresAsync(RequestOptions? ReqOptions = null)
    {
        return await GetCostCentresAsync<CostCenter>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<TallyResult> PostCostCentreAsync<CostCentreType>(CostCentreType costCenter,
                                                                       PostRequestOptions? postRequestOptions = null) where CostCentreType : CostCenter
    {
        return await PostObjectToTallyAsync(costCenter, postRequestOptions);
    }

    #endregion

    #endregion

    #region VoucherType
    /// <inheritdoc/>
    public async Task<VChTypeType> GetVoucherTypeAsync<VChTypeType>(string LookupValue,
                                                                    MasterRequestOptions? voucherTypeOptions = null) where VChTypeType : VoucherType
    {
        return await GetObjectAsync<VChTypeType>(LookupValue, voucherTypeOptions);
    }
    /// <inheritdoc/>
    public async Task<List<VChTypeType>?> GetVoucherTypesAsync<VChTypeType>(PaginatedRequestOptions ReqOptions) where VChTypeType : VoucherType
    {
        return await GetObjectsAsync<VChTypeType>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<VoucherType>?> GetVoucherTypesAsync(PaginatedRequestOptions ReqOptions)
    {
        return await GetVoucherTypesAsync<VoucherType>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<VChTypeType>?> GetVoucherTypesAsync<VChTypeType>(RequestOptions? ReqOptions = null) where VChTypeType : VoucherType
    {
        return await GetAllObjectsAsync<VChTypeType>(ReqOptions);
    }
    /// <inheritdoc/>
    public async Task<List<VoucherType>?> GetVoucherTypesAsync(RequestOptions? ReqOptions = null)
    {
        return await GetVoucherTypesAsync<VoucherType>(ReqOptions);
    }

    /// <inheritdoc/>
    public async Task<TallyResult> PostVoucherTypeAsync<VChTypeType>(VChTypeType voucherType,
                                                                     PostRequestOptions? postRequestOptions = null) where VChTypeType : VoucherType
    {
        return await PostObjectToTallyAsync(voucherType, postRequestOptions);
    }

    #endregion
}
