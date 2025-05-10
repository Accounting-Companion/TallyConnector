namespace TallyConnector.Models.Common.Pagination;

public class PaginationBase
{
    public PaginationBase()
    {
    }

    public PaginationBase(int totalCount, int pageSize)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        PageNum = 1;
        TotalPages = totalCount != 0 ? (int)Math.Ceiling((decimal)TotalCount / PageSize) : 0;
    }

    public PaginationBase(int totalCount, int pageSize, int pageNum) : this(totalCount, pageSize)
    {
        TotalCount = totalCount;
        PageNum = pageNum;
        PageSize = pageSize;
    }

    public PaginationBase(int pageNum, int pageSize, int totalCount, int totalPages)
    {
        PageNum = pageNum;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
    }

    public int PageNum { get; internal set; }
    public int PageSize { get; }


    public int TotalCount { get; }
    public int TotalPages { get; internal set; }


}