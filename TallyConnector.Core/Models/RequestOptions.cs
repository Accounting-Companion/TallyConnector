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

public class RequestOptions : DateFilterRequestOptions
{
    
    public List<string>? FetchList { get; set; }
    public List<Filter>? Filters { get; set; }
    public List<string>? Compute { get; set; } = new();
    public List<string>? ComputeVar { get; set; } = new();
    public YesNo IsInitialize { get; set; } = YesNo.No;
}

public class PaginatedRequestOptions: RequestOptions
{
    public Pagination? Pagination { get; set; }
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

    public string CollectionType { get; set; }
    public string? ChildOf { get; set; }    
}

