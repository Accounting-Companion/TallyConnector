namespace TallyConnector.Core.Models;
public class PaginatedResponse<T> : PaginationBase where T : BasicTallyObject
{
    public PaginatedResponse(int pageNum, int pageSize, int totalCount, int totalPages, List<T> data) : base(pageNum, pageSize, totalCount, totalPages)
    {
        Data = data;
    }

    public List<T> Data { get; set; }
}
