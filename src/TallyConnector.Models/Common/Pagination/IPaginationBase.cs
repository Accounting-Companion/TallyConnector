namespace TallyConnector.Models.Common.Pagination;

public interface IPaginationBase
{
    int PageNum { get; }
    int PageSize { get; }
    ulong TotalCount { get; }
    int TotalPages { get; }
}