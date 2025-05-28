using TallyConnector.Core.Models.Interfaces;
using TallyConnector.Models.Common;
using TallyConnector.Models.Common.Pagination;

namespace TallyConnector.Services;
public interface ITallyCommonService
{
    Task<LastAlterIdsRoot> GetLastAlterIdsAsync(BaseRequestOptions? baseRequestOptions = null, CancellationToken token = default);
    Task<PaginatedResponse<T>> GetObjectsAsync<T>(PaginatedRequestOptions? options = null, CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject;
    Task<List<T>> GetObjectsAsync<T>(RequestOptions options, CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject;
    Task<List<AutoColVoucherTypeStat>> GetPeriodicVoucherStatisticsAsync(AutoColumnReportPeriodRequestOptions requestOptions, CancellationToken token = default);
    void SetupTallyService(string url, int port);
}