using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TallyConnector.SourceGenerators.Extensions.Symbols;
using TallyConnector.SourceGenerators.Models;

namespace TallyConnector.SourceGenerators.Generators.XMLTDL;
[Generator(LanguageNames.CSharp)]
public partial class XMLTDLReportGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<XMLTDLReportGeneratorArgs> syntaxProvider = context.SyntaxProvider
            .CreateSyntaxProvider(SyntaxPredicate, SematicTransform)
            .Where(static (type) => type != null)!;
        // .Select(static (INamedTypeSymbol? type,CancellationToken _)=>type);

        context.RegisterSourceOutput(syntaxProvider, Execute);

    }



    private object TransformType((INamedTypeSymbol, object) value)
    {
        throw new NotImplementedException();
    }

    private bool SyntaxPredicate(SyntaxNode node, CancellationToken arg)
    {
        //Debugger.Launch();
        return node is ClassDeclarationSyntax
        {

        } candidate
        && candidate.Modifiers.Any(SyntaxKind.PartialKeyword)
        && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword);
    }
    private XMLTDLReportGeneratorArgs? SematicTransform(GeneratorSyntaxContext context, CancellationToken cancellation)
    {

        var classDeclaration = Unsafe.As<ClassDeclarationSyntax>(context.Node);
        ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellation);
        if (symbol is INamedTypeSymbol type && (type.CheckInterface("TallyConnector.Core.Models.Interfaces.ITallyObject") || type.CheckInterface("TallyConnector.Core.Models.Interfaces.ITallyReport")) && type.MetadataName == "BaseGroup")
        {
            //Debugger.Launch();
            string ClassName = type.Name;
            string ObjectType = ClassName;
            XMLTDLReportGeneratorArgs result = new();
            result.ReportName = $"TC ListOf{ObjectType}".ToUpperInvariant();
            System.Collections.Immutable.ImmutableArray<AttributeData> classAttributes = type.GetAttributes();

            foreach (AttributeData attributeData in classAttributes)
            {
                string attributeMetaName = attributeData.GetAttrubuteMetaName();
                if (attributeMetaName == "System.Xml.Serialization.XmlRootAttribute")
                {
                    ObjectType = attributeData.ConstructorArguments.First().Value?.ToString() ?? ObjectType;
                }
            }
            IEnumerable<ISymbol> symbols = type.GetProperties();

            foreach (IPropertySymbol classChildSymbol in symbols.Cast<IPropertySymbol>())
            {
                SpecialType Stype = classChildSymbol.Type.SpecialType;
                bool IsPrimitive = Stype != SpecialType.None;
                var attributes = classChildSymbol.GetAttributes();
                string FieldName = $"TC {ClassName} {classChildSymbol.Name} Field";
                string FieldXmlTag = classChildSymbol.Name.ToUpper();
                string FiledSet = $"${FieldXmlTag}";

                if (attributes != null)
                {
                    foreach (AttributeData attribute in attributes)
                    {
                        string attributeMetaName = attribute.AttributeClass!.OriginalDefinition.ToString();

                        if (attributeMetaName == "System.Xml.Serialization.XmlElementAttribute")
                        {
                            if (attribute.ConstructorArguments.Length > 0)
                            {

                            }
                            if (attribute.NamedArguments.Length > 1)
                            {
                            }
                            continue;
                        }
                        if (attributeMetaName == "TallyConnector.Core.Attributes.TDLXMLSetAttribute")
                        {
                            if (attribute.ConstructorArguments.Length > 0)
                            {

                            }
                            if (attribute.NamedArguments.Length > 1)
                            {

                            }
                            continue;
                        }
                        if (attributeMetaName == "TallyConnector.Core.Attributes.TDLXMLSetAttribute")
                        {
                            if (attribute.ConstructorArguments.Length > 0)
                            {

                            }
                            if (attribute.NamedArguments.Length > 1)
                            {

                            }
                            continue;
                        }

                    }
                }
                result.Fields.Add(new() { FieldName = FieldName, XMLTag = FieldXmlTag, Set = FiledSet });
            }

            bool k = type.CheckInterface("TallyConnector.Core.Models.Interfaces.ITallyObject");
            //context.SemanticModel.Compilation.GetTypeByMetadataName();
            return result;
        }

        return null;
    }
}
