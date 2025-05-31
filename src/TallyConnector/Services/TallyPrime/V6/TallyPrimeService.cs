using TallyConnector.Models.Common;
using TallyConnector.Models.Common.Pagination;
using TallyConnector.Models.TallyPrime.V6;
using TallyConnector.Models.TallyPrime.V6.Masters;

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
}