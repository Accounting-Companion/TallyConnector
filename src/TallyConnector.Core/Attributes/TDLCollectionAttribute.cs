namespace TallyConnector.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class TDLCollectionAttribute : Attribute
{
    private string? _collectionName;
    private string? _type;
    public string CollectionName
    {
        get { return _collectionName ?? string.Empty; }
        set { _collectionName = value; }
    }
    public TDLCollectionAttribute() { }

    public TDLCollectionAttribute(string? collectionName, string? type = null)
    {
        _collectionName = collectionName;
        _type = type;
    }
    public TDLCollectionAttribute(string? collectionName, string? type, bool exclude)
    {
        _collectionName = collectionName;
        _type = type;
        Exclude = exclude;
    }
    public TDLCollectionAttribute(string? collectionName, bool exclude)
    {
        _collectionName = collectionName;
        Exclude = exclude;
    }

    public string? Type
    {
        get { return _type; }
        set { _type = value; }
    }

    private bool _include;

    public bool Exclude
    {
        get { return _include; }
        set { _include = value; }
    }

    public string? ExplodeCondition { get; set; }
    public Type? TypeInfo { get; set; }

}
