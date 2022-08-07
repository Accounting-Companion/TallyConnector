using TallyConnector.Core.Models.Masters.Inventory;

namespace TallyConnector.Services;
public partial class TallyService
{
    public async Task<UnitType> GetUnitAsync<UnitType>(string LookupValue,
                                                       MasterRequestOptions? unitOptions = null) where UnitType : Unit
    {
        return await GetObjectAsync<UnitType>(LookupValue, unitOptions);
    }
    public async Task<TallyResult> PostUnitAsync<UnitType>(UnitType unit,
                                                           PostRequestOptions? postRequestOptions = null) where UnitType : Unit
    {

        return await PostObjectToTallyAsync(unit, postRequestOptions);
    }

    public async Task<GodownType> GetGodownAsync<GodownType>(string LookupValue,
                                                             MasterRequestOptions? godownOptions = null) where GodownType : Godown
    {
        return await GetObjectAsync<GodownType>(LookupValue, godownOptions);
    }
    public async Task<TallyResult> PostGodownAsync<GodownType>(GodownType godown,
                                                               PostRequestOptions? postRequestOptions = null) where GodownType : Godown
    {

        return await PostObjectToTallyAsync(godown, postRequestOptions);
    }

    public async Task<StockGroupType> GetStockGroupAsync<StockGroupType>(string LookupValue,
                                                             MasterRequestOptions? stockGroupOptions = null) where StockGroupType : StockGroup
    {
        return await GetObjectAsync<StockGroupType>(LookupValue, stockGroupOptions);
    }
    public async Task<TallyResult> PostStockGroupAsync<StockGroupType>(StockGroupType stockGroup,
                                                               PostRequestOptions? postRequestOptions = null) where StockGroupType : StockGroup
    {
        return await PostObjectToTallyAsync(stockGroup, postRequestOptions);
    }
    public async Task<StockCategoryType> GetStockCategoryAsync<StockCategoryType>(string LookupValue,
                                                                                  MasterRequestOptions? stockCategoryOptions = null) where StockCategoryType : StockCategory
    {
        return await GetObjectAsync<StockCategoryType>(LookupValue, stockCategoryOptions);
    }
    public async Task<TallyResult> PostStockCategoryAsync<StockCategoryType>(StockCategoryType stockCategory,
                                                                             PostRequestOptions? postRequestOptions = null) where StockCategoryType : StockCategory
    {
        return await PostObjectToTallyAsync(stockCategory, postRequestOptions);
    }

    public async Task<StckItmType> GetStockItemAsync<StckItmType>(string LookupValue,
                                                                  MasterRequestOptions? stockItemOptions = null) where StckItmType : StockItem
    {
        return await GetObjectAsync<StckItmType>(LookupValue, stockItemOptions);
    }
    public async Task<TallyResult> PostStockItemAsync<StckItmType>(StckItmType stockItem,
                                                                   PostRequestOptions? postRequestOptions = null) where StckItmType : StockItem
    {

        return await PostObjectToTallyAsync(stockItem, postRequestOptions);
    }
}
