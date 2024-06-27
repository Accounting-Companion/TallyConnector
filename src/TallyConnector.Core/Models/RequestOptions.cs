


namespace TallyConnector.Core.Models;
public class BaseRequestOptions
{
    public string? Company { get; set; }
    public XmlAttributeOverrides? XMLAttributeOverrides { get; set; }
}
public class PostRequestOptions : BaseRequestOptions
{

}

public class DateFilterRequestOptions : BaseRequestOptions
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
public class AutoColumnReportPeriodRequestOprions : DateFilterRequestOptions
{
    public PeriodicityType Periodicity { get; set; }
}

public class CountRequestOptions : DateFilterRequestOptions
{
    public List<Filter>? Filters { get; set; }
    public YesNo IsInitialize { get; set; } = YesNo.No;

    public string ChildOf { get; set; }

    public string CollectionType { get; set; }
}
public class RequestOptions : DateFilterRequestOptions
{
    public List<Filter>? Filters { get; set; }
    public List<string>? Compute { get; set; } = new();
    public List<string>? ComputeVar { get; set; } = new();
    public string? Childof { get; set; }
    public string? CollectionType { get; set; }
    public YesNo? BelongsTo { get; set; }
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
public class VoucherRequestOptions : RequestOptions
{
    public VoucherLookupField LookupField { get; set; } = VoucherLookupField.VoucherNumber;
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
    public string CollectionType { get; set; }
    public string? ChildOf { get; set; }

}

