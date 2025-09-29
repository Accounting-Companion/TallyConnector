using TallyConnector.Abstractions.Models;
using static TallyConnector.TDLReportSourceGenerator.Constants;

namespace TallyConnector.Core.Models.Request;
public class BaseRequestOptions
{
    public string? Company { get; set; }

    public BaseRequestOptions SetCompany(string Company)
    {
        this.Company = Company;
        return this;
    }
}
public class PostRequestOptions : BaseRequestOptions
{
    public bool StopatFirstError { get; set; } = false;
}

public class DateFilterRequestOptions : BaseRequestOptions
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public new DateFilterRequestOptions SetCompany(string Company)
    {
        this.Company = Company;
        return this;
    }

    public DateFilterRequestOptions From(DateTime fromDate)
    {
        this.FromDate = fromDate;
        return this;
    }

    public DateFilterRequestOptions To(DateTime toDate)
    {
        this.ToDate = toDate;
        return this;
    }


}
public class AutoColumnReportPeriodRequestOptions : DateFilterRequestOptions
{
    public PeriodicityType Periodicity { get; set; }
}


public class RequestOptions : DateFilterRequestOptions
{
    public List<Filter>? Filters { get; set; } = [];
    public List<string>? Compute { get; set; } = [];
    public List<string>? ComputeVar { get; set; } = [];
    public string? Childof { get; set; }
    public string? CollectionType { get; set; }
    public YesNo? BelongsTo { get; set; }

    public new RequestOptions SetCompany(string Company)
    {
        this.Company = Company;
        return this;
    }

}
public class RequestOptions<T, TMeta> : RequestOptions where T : BaseObject, IMetaGenerated where TMeta : MetaObject
{
    TMeta _meta;
    public RequestOptions(TMeta meta)
    {
        _meta = meta;
    }

    public RequestOptions<T, TMeta> FilterBy(Func<TMeta, FilterCondition> builder, string? name = null)
    {
        Filters ??= [];

        FilterCondition filterCondition = builder(_meta);
        name ??= $"{filterCondition.Name}";
        Filters.Add(new(name, filterCondition.ToString()!));
        return this;
    }
}

public class PaginatedRequestOptions : RequestOptions
{
    public int PageNum { get; set; } = 1;
    public int? RecordsPerPage { get; set; }

    /// <summary>
    /// If set to true, Count request is not send  receiving data from tally
    /// </summary>
    public bool DisableCountTag { get; set; }
}
public class MasterRequestOptions : RequestOptions
{
    public MasterLookupField LookupField { get; set; } = MasterLookupField.Name;
}


public class CollectionRequestOptions : PaginatedRequestOptions
{
    public CollectionRequestOptions()
    {
    }

    public CollectionRequestOptions(string collectionType)
    {
        CollectionType = collectionType;
    }

    public bool Pagination { get; set; }
    public string? ChildOf { get; set; }

}

