using TallyConnector.Core.Models.Masters;
using TallyConnector.Core.Models.Masters.CostCenter;

namespace TallyConnector.Services;
public partial class TallyService
{

    public async Task<CurrencyType> GetCurrencyAsync<CurrencyType>(string lookupValue,
                                                                   MasterRequestOptions? currencyOptions = null) where CurrencyType : Currency
    {
        return await GetObjectAsync<CurrencyType>(lookupValue, currencyOptions);
    }
    public async Task<TallyResult> PostCurrencyAsync<CurrencyType>(CurrencyType currency,
                                                                   PostRequestOptions? postRequestOptions = null) where CurrencyType : Currency
    {
        return await PostObjectToTallyAsync(currency, postRequestOptions);
    }

    public async Task<GroupType> GetGroupAsync<GroupType>(string lookupValue,
                                                          MasterRequestOptions? groupOptions = null) where GroupType : Group
    {
        return await GetObjectAsync<GroupType>(lookupValue, groupOptions);
    }
    public async Task<TallyResult> PostGroupAsync<GroupType>(GroupType group,
                                                             PostRequestOptions? postRequestOptions = null) where GroupType : Group
    {
        return await PostObjectToTallyAsync(group, postRequestOptions);
    }

    public async Task<LedgerType> GetLedgerAsync<LedgerType>(string LookupValue,
                                                             MasterRequestOptions? ledgerOptions = null) where LedgerType : Ledger
    {
        return await GetObjectAsync<LedgerType>(LookupValue, ledgerOptions);
    }

    public async Task<TallyResult> PostLedgerAsync<LedgerType>(LedgerType ledger,
                                                               PostRequestOptions? postRequestOptions = null) where LedgerType : Ledger
    {

        return await PostObjectToTallyAsync(ledger, postRequestOptions);
    }

    //Related to CostCenter
    public async Task<CostCategoryType> GetCostCategoryAsync<CostCategoryType>(string LookupValue,
                                                             MasterRequestOptions? costCategoryOptions = null) where CostCategoryType : CostCategory
    {
        return await GetObjectAsync<CostCategoryType>(LookupValue, costCategoryOptions);
    }

    public async Task<TallyResult> PostCostCategoryAsync<CostCategoryType>(CostCategoryType costCategory,
                                                                           PostRequestOptions? postRequestOptions = null) where CostCategoryType : CostCategory
    {

        return await PostObjectToTallyAsync(costCategory, postRequestOptions);
    }

    public async Task<CostCentreType> GetCostCentreAsync<CostCentreType>(string LookupValue,
                                                                         MasterRequestOptions? costCenterOptions = null) where CostCentreType : CostCenter
    {
        return await GetObjectAsync<CostCentreType>(LookupValue, costCenterOptions);
    }

    public async Task<TallyResult> PostCostCentreAsync<CostCentreType>(CostCentreType costCenter,
                                                                       PostRequestOptions? postRequestOptions = null) where CostCentreType : CostCenter
    {

        return await PostObjectToTallyAsync(costCenter, postRequestOptions);
    }


    public async Task<VChTypeType> GetVoucherTypeAsync<VChTypeType>(string LookupValue,
                                                                    MasterRequestOptions? voucherTypeOptions = null) where VChTypeType : VoucherType
    {
        return await GetObjectAsync<VChTypeType>(LookupValue, voucherTypeOptions);
    }

    public async Task<TallyResult> PostVoucherTypeAsync<VChTypeType>(VChTypeType voucherType,
                                                                     PostRequestOptions? postRequestOptions = null) where VChTypeType : VoucherType
    {

        return await PostObjectToTallyAsync(voucherType, postRequestOptions);
    }
}
