
namespace TallyConnector.Models.Common.Pagination;

public interface IPaginatedResponse<out T> : IPaginationBase where T : IBaseObject
{
    IEnumerable<T> Data { get;  }
}