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

    public TDLCollectionAttribute(string? collectionName, string? type)
    {
        _collectionName = collectionName;
        _type = type;
    }
    public TDLCollectionAttribute(string? collectionName, string? type, bool include)
    {
        _collectionName = collectionName;
        _type = type;
        Include = include;
    }
    public TDLCollectionAttribute(string? collectionName, bool include)
    {
        _collectionName = collectionName;
        Include = include;
    }

    public string? Type
    {
        get { return _type; }
        set { _type = value; }
    }

    private bool _include;

    public bool Include
    {
        get { return _include; }
        set { _include = value; }
    }

}
