namespace TallyConnector.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class TDLCollectionAttribute : Attribute
{
    private string? _collectionName;

    public string CollectionName
    {
        get { return _collectionName ?? string.Empty; }
        set { _collectionName = value; }
    }
    public TDLCollectionAttribute() { }

    public TDLCollectionAttribute(string? collectionName, string? type)
    {
        _collectionName = collectionName;
        Type = type;
    }
    public TDLCollectionAttribute(string? collectionName, string? type, bool include)
    {
        _collectionName = collectionName;
        Type = type;
        Include = include;
    }
    public TDLCollectionAttribute(string? collectionName, bool include)
    {
        _collectionName = collectionName;
        Include = include;
    }

    public string? Type { get; set; }

    public bool Include { get; set; }
    public bool Initialize { get; set; }

}
