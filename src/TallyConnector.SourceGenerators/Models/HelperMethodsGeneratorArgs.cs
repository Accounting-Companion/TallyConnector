using System.Diagnostics;
using System.Xml.Linq;

namespace TallyConnector.SourceGenerators.Models;
public class HelperMethodArgs : IEquatable<HelperMethodArgs>
{
    public HelperMethodArgs(string nameSpace,
                            string className)
    {
        NameSpace = nameSpace;
        ClassName = className;
    }

    public string NameSpace { get; }
    public string ClassName { get; }

    public List<AttributeArgs> AttributeArgs = new();
    //public INamedTypeSymbol ReturnType { get; }
    //public INamedTypeSymbol GetObjType { get; }
    //public INamedTypeSymbol PostType { get; }
    //public INamedTypeSymbol RequestEnvelopeType { get; }


    public override bool Equals(object? obj)
    {
        return obj is HelperMethodArgs customObject
               && Equals(customObject);
    }

    public bool Equals(HelperMethodArgs other)
    {
        return ClassName == other.ClassName;
    }
    public override int GetHashCode()
    {
        return ClassName.GetHashCode();
    }
}
public class AttributeArgs
{
    public AttributeArgs(INamedTypeSymbol returnType, INamedTypeSymbol getType, INamedTypeSymbol postType, INamedTypeSymbol requestEnvelopeType)
    {
        ReturnType = returnType;
        GetObjType = getType;
        PostType = postType;
        RequestEnvelopeType = requestEnvelopeType;
    }

    public INamedTypeSymbol ReturnType { get; }
    public INamedTypeSymbol GetObjType { get; }
    public INamedTypeSymbol PostType { get; }
    public INamedTypeSymbol RequestEnvelopeType { get; }
}
