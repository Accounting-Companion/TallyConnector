using TallyConnector.Core.Models.Request;

namespace TallyConnector.Core.Extensions;
public static class RequestOptionsExtensions
{
    public static RequestOptions ToRequestOptions(this PaginatedRequestOptions? paginatedRequestOptions, int defaultPaginationCount=1000)
    {
        RequestOptions requestOptions = new();
        requestOptions.XMLAttributeOverrides = paginatedRequestOptions?.XMLAttributeOverrides;
        requestOptions.Filters = paginatedRequestOptions?.Filters;
        requestOptions.FromDate = paginatedRequestOptions?.FromDate;
        requestOptions.ToDate = paginatedRequestOptions?.ToDate;
        requestOptions.Compute = paginatedRequestOptions?.Compute;
        requestOptions.ComputeVar = paginatedRequestOptions?.ComputeVar;
        requestOptions.Company = paginatedRequestOptions?.Company;
        //requestOptions.Filters ??= [];
        //requestOptions.Compute ??= [];
        //requestOptions.ComputeVar ??= [];

        int? recordsPerPage = paginatedRequestOptions?.RecordsPerPage ?? defaultPaginationCount;
        int? Start = recordsPerPage * ((paginatedRequestOptions?.PageNum ?? 1) - 1);

        requestOptions.Compute = [.. requestOptions.Compute ?? [], "LineIndex : ##vLineIndex"];
        requestOptions.ComputeVar = [.. requestOptions.ComputeVar ?? [], "vLineIndex: Number : IF $$IsEmpty:##vLineIndex THEN 1 ELSE ##vLineIndex + 1"];
        requestOptions.Filters = [.. requestOptions.Filters ?? [], new("TC_PaginationFilter", $"##vLineIndex <= {Start + recordsPerPage} AND ##vLineIndex > {Start}")];
        return requestOptions;
    }
}
