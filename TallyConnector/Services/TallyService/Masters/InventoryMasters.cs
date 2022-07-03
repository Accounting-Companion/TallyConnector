using TallyConnector.Core.Models.Masters.Inventory;

namespace TallyConnector.Services;
public partial class TallyService
{
    public async Task<StckItmType> GetStockItemAsync<StckItmType>(string LookupValue,
                                                        MasterRequestOptions? StockItemOptions = null) where StckItmType : StockItem
    {
        await SendRequestAsync();
        return (StckItmType)new StockItem();
    }
    public async Task<PResult> PostStockItemAsync<StckItmType>(StckItmType StockItem,
                                                              PostRequestOptions? StockItemOptions = null) where StckItmType : StockItem
    {
        await SendRequestAsync();
        return new PResult();
    }
}
