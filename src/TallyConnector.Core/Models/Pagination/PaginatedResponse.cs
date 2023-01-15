namespace TallyConnector.Core.Models.Pagination;
public class PaginatedResponse<T> : PaginationBase where T : TallyBaseObject
{
    public PaginatedResponse(int totalCount, int pageSize, List<T>? data) : base(totalCount, pageSize)
    {
        Data = data;
    }

    public List<T>? Data { get; set; }
}
