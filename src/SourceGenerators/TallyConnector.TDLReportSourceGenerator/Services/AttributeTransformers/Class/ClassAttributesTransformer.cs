using System.Collections.Immutable;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Class;
public class ClassAttributesTransformer
{
    private static readonly Dictionary<string, ClassPropertyAttributeTransformer> _classattributeTransformers = new()
    {
        {TDLCollectionAttributeName,new ClassCollectionAttributeTransformer() },
        {XmlRootAttributeName,new XMLRootAttributeTransformer() },
        {MaptoDTOAttributeName,new MaptoDTOAttributeTransformer() },
        {Attributes.Abstractions.GenerateITallyRequestableObectAttributeeName,new GenerateITallyRequestableObectAttributeTransformer() },
        {TDLFunctionsMethodNameAttributeName,new FunctionNameExtractor(c=>c.TDLFunctions) },
    };

    /// <summary>
    /// Propogates data properties based on attributes of class
    /// </summary>
    /// <param name="data"></param>
    /// <param name="memberAttributes"></param>
    internal static void TransformAsync(ClassData data, ImmutableArray<AttributeData> memberAttributes)
    {
        foreach (var memberAttribute in memberAttributes)
        {
            var fullName = memberAttribute.GetAttrubuteMetaName();
            _classattributeTransformers.TryGetValue(fullName, out var attributeTransformer);

            attributeTransformer?.TransformAsync(data, memberAttribute);
        }

        // if class doesnot have xml root attribute
        data.XMLTag ??= data.Name.ToUpper();
    }
}

internal class MaptoDTOAttributeTransformer : ClassPropertyAttributeTransformer
{
    public override void TransformAsync(ClassData data, AttributeData attributeData)
    {
        if (attributeData.AttributeClass?.TypeArguments is { Length: 1 } args &&
     args[0] is INamedTypeSymbol symbol)
        {
            data.DTOName = symbol.Name;
            data.DTOFullName = symbol.GetClassMetaName();
            data.IgnoreForGenerateDTO = true;
        }
    }
}

public abstract class ClassPropertyAttributeTransformer
{
    public abstract void TransformAsync(ClassData data, AttributeData attributeData);
}
