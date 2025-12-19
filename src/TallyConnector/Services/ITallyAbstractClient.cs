using TallyConnector.Core.Models.Interfaces;

namespace TallyConnector.Services;
public interface ITallyAbstractClient
{
    Task<ulong> GetCountAsync<T>(BaseRequestOptions? options = null, CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject;
    Task<LastAlterIdsRoot> GetLastAlterIdsAsync(BaseRequestOptions? baseRequestOptions = null, CancellationToken token = default);
    /// <summary>
    /// Get paginated data from Tally
    /// </summary>

    /// <remarks>
    /// Tally does not support pagination, so what we implemented is in-memory pagination
    /// just to make sure tally is not hanged <br/> 
    /// also dont mention bigger page size usually 1000 is better for vouchers
    ///</remarks>
    /// <typeparam name="T">
    /// The type of objects to retrieve. Must implement both <see cref="ITallyRequestableObject"/> 
    /// and <see cref="IBaseObject"/>.
    ///</typeparam>
    /// <param name="options">The pagination options including page size, filters, and sort criteria</param>
    /// <param name="token">An optional <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>
    ///  a  <see cref="PaginatedResponse{T}"/> with the retrieved objects and pagination metadata.
    /// </returns>
    Task<PaginatedResponse<T>> GetObjectsAsync<T>(PaginatedRequestOptions options, CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject;
    Task<List<T>> GetObjectsAsync<T>(BaseRequestOptions? options = null, CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject;
    Task<List<AutoColVoucherTypeStat>> GetPeriodicVoucherStatisticsAsync(AutoColumnReportPeriodRequestOptions requestOptions, CancellationToken token = default);
    void SetupTallyService(string url, int port);
}