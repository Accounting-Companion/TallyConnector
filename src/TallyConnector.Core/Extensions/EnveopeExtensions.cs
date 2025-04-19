using TallyConnector.Core.Models;

namespace TallyConnector.Core.Extensions;
public static class EnveopeExtensions
{
    public static RequestEnvelope PopulateOptions(this RequestEnvelope requestEnvelope, BaseRequestOptions? requestOptions)
    {
        if (requestOptions != null)
        {
            var sv = requestEnvelope.Body.Desc.StaticVariables ??= new();
            sv.SVCompany = requestOptions.Company;
        }
        return requestEnvelope;
    }
    public static RequestEnvelope PopulateOptions(this RequestEnvelope requestEnvelope, DateFilterRequestOptions? requestOptions)
    {
        if (requestOptions != null)
        {
            var sv = requestEnvelope.Body.Desc.StaticVariables ??= new();
            sv.SVCompany = requestOptions.Company;
            sv.SVFromDate = requestOptions.FromDate;
            sv.SVToDate = requestOptions.ToDate;
        }
        return requestEnvelope;
    }
    public static RequestEnvelope PopulateOptions(this RequestEnvelope requestEnvelope, RequestOptions? requestOptions)
    {
        if (requestOptions != null)
        {
            var sv = requestEnvelope.Body.Desc.StaticVariables ??= new();
            sv.SVCompany = requestOptions.Company;
            sv.SVFromDate = requestOptions.FromDate;
            sv.SVToDate = requestOptions.ToDate;
            TDLMessage tDLMessage = requestEnvelope.Body.Desc.TDL.TDLMessage;
            Collection? collection = tDLMessage.Collection.FirstOrDefault();
            if (collection != null)
            {
                collection.Compute = requestOptions.Compute;
                collection.ComputeVar = requestOptions.ComputeVar;
                collection.Filters = requestOptions.Filters?.Select(c => c.FilterName!).ToList();
                collection.Childof = requestOptions.Childof;
            }

            if (requestOptions.Filters != null && requestOptions.Filters.Count > 0)
            {
                tDLMessage.System ??= [];
                tDLMessage.System.AddRange(requestOptions.Filters.Select(c => new Models.Request.System(c.FilterName!, c.FilterFormulae!)));
            }

        }
        return requestEnvelope;
    }
    public static RequestEnvelope PopulateOptions(this RequestEnvelope requestEnvelope, PaginatedRequestOptions? requestOptions)
    {
        if (requestOptions != null)
        {
            requestOptions.Filters ??= [];
            requestOptions.Compute ??= [];
            requestOptions.ComputeVar ??= [];

            int recordsPerPage = requestOptions.RecordsPerPage ?? 1000;
            int Start = recordsPerPage * (requestOptions.PageNum - 1);
            if (!requestOptions.DisableCountTag)
            {
                TDLMessage tDLMessage = requestEnvelope.Body.Desc.TDL.TDLMessage;
                Collection collection = tDLMessage.Collection.First();
                if (!string.IsNullOrWhiteSpace(requestOptions.Childof))
                {
                    collection.Childof = requestOptions.Childof;
                    collection.BelongsTo = requestOptions.BelongsTo ?? YesNo.None;
                    if (!string.IsNullOrWhiteSpace(requestOptions.CollectionType))
                    {
                        collection.Type = requestOptions.CollectionType;
                    }
                }
                string CollectionName = $"{collection.Name}_NonPaginated";
                tDLMessage.Collection.Add(new(CollectionName, collection.Type!, filters: [.. requestOptions.Filters.Select(c => c.FilterName!)]) { Childof = requestOptions.Childof, BelongsTo = requestOptions.BelongsTo ?? YesNo.None });
                const string objectCountName = "TC_ObjectsCount";
                Part part = tDLMessage.Part!.First();
                part.Lines = [.. part.Lines ?? [], objectCountName];
                tDLMessage.Part = [.. tDLMessage.Part ?? [], new(objectCountName, null)];
                tDLMessage.Line = [.. tDLMessage.Line ?? [], new(objectCountName, [objectCountName])];
                tDLMessage.Field = [.. tDLMessage.Field ?? [], new Field(objectCountName, "TC_TotalCount", $"$$NUMITEMS:{CollectionName}")];
            }


            requestOptions.Compute = [.. requestOptions.Compute, "LineIndex : ##vLineIndex"];
            requestOptions.ComputeVar = [.. requestOptions.ComputeVar, "vLineIndex: Number : IF $$IsEmpty:##vLineIndex THEN 1 ELSE ##vLineIndex + 1"];
            requestOptions.Filters = [.. requestOptions.Filters, new("TC_PaginationFilter", $"##vLineIndex <= {Start + recordsPerPage} AND ##vLineIndex > {Start}")];
            requestEnvelope.PopulateOptions((RequestOptions)requestOptions);

        }
        return requestEnvelope;
    }


    public static List<Models.Request.System> ToSystem(this IEnumerable<Filter> filters)
    {
        return [.. filters.Select(c => new Models.Request.System(c.FilterName!, c.FilterFormulae!))];
    }
    public static List<string> GetFilterNames(this IEnumerable<Filter> filters)
    {
        return [.. filters.Select(c => c.FilterName!)];
    }
}
