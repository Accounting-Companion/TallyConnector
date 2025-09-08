using System.Linq.Expressions;
using TallyConnector.Abstractions.Models;

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
public class RequestOptions<T> : RequestOptions where T : class, new()
{
    public RequestOptions<T> Where(Func<T, bool> func)
    {
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

