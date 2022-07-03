using TallyConnector.Core.Exceptions;

namespace TallyConnector.Services;
public partial class TallyService
{

    public async Task<LedgerType> GetLedgerAsync<LedgerType>(string LookupValue,
                                                         MasterRequestOptions? ledgerOptions = null) where LedgerType : Ledger
    {
        LedgerType ledger = await GetObjectAsync<LedgerType>(LookupValue, ledgerOptions);
        return ledger;
    }

    public async Task<TallyResult> PostLedgerAsync<LedgerType>(LedgerType ledger,
                                                               PostRequestOptions? postRequestOptions = null) where LedgerType : Ledger
    {
        var result = await PostObjectToTallyAsync(Object: ledger, postRequestOptions);

        return result;
    }

}
