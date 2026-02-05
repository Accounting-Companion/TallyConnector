using TallyConnector.Core.Models.Interfaces;
using TallyConnector.Models.TallyPrime.V7;
using TallyConnector.Models.TallyPrime.V7.Masters;
using TallyConnector.Models.TallyPrime.V7.Masters.Inventory;

namespace TallyConnector.Services.TallyPrime.V7;

[GenerateHelperMethod<Group>()]
[GenerateHelperMethod<Ledger>()]

[GenerateHelperMethod<CostCategory>(MethodNameSuffixPlural = "CostCategories")]
[GenerateHelperMethod<CostCentre>()]

[GenerateHelperMethod<Unit>()]
[GenerateHelperMethod<StockCategory>(MethodNameSuffixPlural = "StockCategories")]
[GenerateHelperMethod<StockGroup>()]
[GenerateHelperMethod<StockItem>()]
[GenerateHelperMethod<Godown>()]

[GenerateHelperMethod<VoucherType>()]

[GenerateHelperMethod<Voucher>()]
public partial class TallyPrimeService : TallyAbstractClient
{
    public TallyPrimeService()
    {
    }

    public TallyPrimeService(IBaseTallyService baseTallyService) : base(baseTallyService)
    {
    }

    public TallyPrimeService(ILogger logger, IBaseTallyService baseTallyService) : base(logger, baseTallyService)
    {
        
    }

    

    public Task<List<Company>> GetCompaniesAsync(CancellationToken cancellationToken = default) => GetObjectsAsync<Company>(token: cancellationToken);

    public Task<List<MasterStatistics>> GetMasterStatisticsAsync(BaseRequestOptions requestOptions, CancellationToken cancellationToken = default) => GetObjectsAsync<MasterStatistics>(requestOptions, cancellationToken);

    public Task<List<VoucherStatistics>> GetVoucherStatisticsAsync(DateFilterRequestOptions requestOptions, CancellationToken cancellationToken = default) => GetObjectsAsync<VoucherStatistics>(requestOptions, cancellationToken);

    public Task<List<GSTRegistration>> GetGSTRegistrations(CancellationToken cancellationToken = default) => GetObjectsAsync<GSTRegistration>(token: cancellationToken);
}