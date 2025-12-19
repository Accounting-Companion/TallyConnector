
namespace TallyConnector.Models.Common.Pagination;
public class PaginatedResponse<T> : PaginationBase, IPaginatedResponse<T> where T : IBaseObject
{
    public PaginatedResponse(ulong totalCount, int pageSize, List<T>? data, int pageNum) : base(totalCount, pageSize, pageNum)
    {
        Data = data ?? [];
    }

    public IEnumerable<T> Data { get;  }

}
