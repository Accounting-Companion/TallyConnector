using TallyConnector.Models.TallyPrime.V3.Masters;
using TallyConnector.Models.TallyPrime.V3;
using TallyConnector.Models.TallyPrime.V3.Masters.Inventory;

namespace TallyConnector.Services.TallyPrime.V3;

[GenerateHelperMethod<Currency>()]
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
}