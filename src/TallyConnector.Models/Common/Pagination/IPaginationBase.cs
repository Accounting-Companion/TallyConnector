namespace TallyConnector.Models.Common.Pagination;

public interface IPaginationBase
{
    int PageNum { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int TotalPages { get; }
}