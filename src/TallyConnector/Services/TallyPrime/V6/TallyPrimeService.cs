using TallyConnector.Models.Common;
using TallyConnector.Models.Common.Pagination;
using TallyConnector.Models.TallyPrime.V6;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Models.TallyPrime.V6.Masters.Inventory;

namespace TallyConnector.Services.TallyPrime.V6;

public partial class TallyPrimeService : TallyCommonService
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


    public Task<List<Group>> GetGroupsAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<Group>(options, cancellationToken);
    public Task<PaginatedResponse<Group>> GetGroupsAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<Group>(options, cancellationToken);
    public Task<List<Ledger>> GetLedgersAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<Ledger>(options, cancellationToken);
    public Task<PaginatedResponse<Ledger>> GetLedgersAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<Ledger>(options, cancellationToken);

    public Task<List<CostCategory>> GetCostCategoriesAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<CostCategory>(options, cancellationToken);
    public Task<PaginatedResponse<CostCategory>> GetCostCategoriesAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<CostCategory>(options, cancellationToken);

    public Task<List<CostCentre>> GetCostCentresAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<CostCentre>(options, cancellationToken);
    public Task<PaginatedResponse<CostCentre>> GetCostCentresAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<CostCentre>(options, cancellationToken);


    public Task<List<Unit>> GetUnitsAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<Unit>(options, cancellationToken);
    public Task<PaginatedResponse<Unit>> GetUnitsAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<Unit>(options, cancellationToken);
    public Task<List<StockCategory>> GetStockCategoriesAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<StockCategory>(options, cancellationToken);
    public Task<PaginatedResponse<StockCategory>> GetStockCategoriesAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<StockCategory>(options, cancellationToken);
    public Task<List<StockGroup>> GetStockGroupsAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<StockGroup>(options, cancellationToken);
    public Task<PaginatedResponse<StockGroup>> GetStockGroupsAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<StockGroup>(options, cancellationToken);
    public Task<List<StockItem>> GetStockItemsAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<StockItem>(options, cancellationToken);
    public Task<PaginatedResponse<StockItem>> GetStockItemsAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<StockItem>(options, cancellationToken);

    public Task<List<VoucherType>> GetVoucherTypesAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<VoucherType>(options, cancellationToken);
    public Task<PaginatedResponse<VoucherType>> GetVoucherTypesAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<VoucherType>(options, cancellationToken);

    public Task<List<Voucher>> GetVouchersAsync(RequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<Voucher>(options, cancellationToken);
    public Task<PaginatedResponse<Voucher>> GetVouchersAsync(PaginatedRequestOptions options, CancellationToken cancellationToken = default) => GetObjectsAsync<Voucher>(options, cancellationToken);


}