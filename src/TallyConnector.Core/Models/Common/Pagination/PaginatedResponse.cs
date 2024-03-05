namespace TallyConnector.Core.Models.Common.Pagination;
public class PaginatedResponse<T> : PaginationBase where T : IBaseObject
{
    public PaginatedResponse(int totalCount, int pageSize, List<T>? data, int pageNum) : base(totalCount, pageSize, pageNum)
    {
        Data = data;
    }

    public List<T>? Data { get; set; }
}
