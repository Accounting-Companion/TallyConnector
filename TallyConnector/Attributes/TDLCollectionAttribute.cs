using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Attributes;

[AttributeUsage(AttributeTargets.Class| AttributeTargets.Property)]
public class TDLCollectionAttribute : Attribute
{
    private string _collectionName;
    private string? _type;
    public string CollectionName
    {
        get { return _collectionName == null ? string.Empty : _collectionName; }
        set { _collectionName = value; }
    }
    public TDLCollectionAttribute() { }

    public TDLCollectionAttribute(string? collectionName,string? type)
    {
        _collectionName = collectionName;
        _type = type;
    }

    public string? Type
    {
        get { return _type; }
        set { _type = value; }
    }
}
