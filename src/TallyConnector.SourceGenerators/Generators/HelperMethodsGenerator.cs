using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using TallyConnector.SourceGenerators.Extensions.Symbols;
using TallyConnector.SourceGenerators.Models;

namespace TallyConnector.SourceGenerators.Generators;
public class HelperMethodArgsComparer
   : IEqualityComparer<HelperMethodArgs>
{
    public bool Equals(
       HelperMethodArgs x,
       HelperMethodArgs y)
    {
        return x.ClassName.Equals(y.ClassName);
    }

    public int GetHashCode(HelperMethodArgs obj)
    {
        return obj.ClassName.GetHashCode();
    }
}
[Generator(LanguageNames.CSharp)]
public partial class HelperMethodsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}
        IncrementalValuesProvider<HelperMethodArgs> syntaxProvider = context.SyntaxProvider
           .CreateSyntaxProvider(SyntaxPredicate, SematicTransform)
           .Where(static (type) => type != null)!;
        //var c = context.CompilationProvider.Combine(syntaxProvider.Collect());
        context.RegisterSourceOutput(syntaxProvider, Execute);
    }

    private void Execute(SourceProductionContext sourceProductionContext, HelperMethodArgs helperMethodArgs)
    {
        List<INamedTypeSymbol> RequestEnvelopeTypes = new List<INamedTypeSymbol>();
        List<INamedTypeSymbol> GetTypes = new List<INamedTypeSymbol>();
        GenerateXMLMethodsGenerator generateXMLMethodsGenerator = new(sourceProductionContext, helperMethodArgs.ClassName, helperMethodArgs.NameSpace);

        foreach (var item in helperMethodArgs.AttributeArgs)
        {
            if (!RequestEnvelopeTypes.Contains(item.RequestEnvelopeType))
            {
                RequestEnvelopeTypes.Add(item.RequestEnvelopeType);
            }
            if (!GetTypes.Contains(item.GetObjType))
            {
                GetTypes.Add(item.GetObjType);
            }
            //Execute(sourceProductionContext, item);
        }
        generateXMLMethodsGenerator.GenerateEnvelopeXmlGeneratorArgs(RequestEnvelopeTypes);
        generateXMLMethodsGenerator.CreateSerializeXmlMethodsforGetEnvelopeTypes();
       // generateXMLMethodsGenerator.GenerateTDLReportsForGettingObject(GetTypes);
        //generateXMLMethodsGenerator.CreateGetTDLReportHelperMethods();
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

    private HelperMethodArgs? SematicTransform(GeneratorSyntaxContext context, CancellationToken cancellation)
    {
        var classDeclaration = Unsafe.As<ClassDeclarationSyntax>(context.Node);
        ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellation);
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}
        if (symbol is INamedTypeSymbol type && (type.CheckInterface("TallyConnector.Services.IBaseTallyService")))
        {
            if (classDeclaration.BaseList?.Types == null)
            {
                return null;
            }
            else
            {
                if (classDeclaration.BaseList.Types.Count > 0)
                {
                    bool match = false;
                    foreach (BaseTypeSyntax baseTypeSyntax in classDeclaration.BaseList.Types)
                    {
                        var candidate = context.SemanticModel.GetSymbolInfo(baseTypeSyntax.Type).Symbol as INamedTypeSymbol;
                        if (candidate != null && candidate.GetClassMetaName() == "TallyConnector.Services.BaseTallyService")
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            System.Collections.Immutable.ImmutableArray<AttributeData> Attributes = symbol.GetAttributes();
            HelperMethodArgs args = new(type.ContainingNamespace.ToString(), type.MetadataName.ToString());
            foreach (var Attribute in Attributes)
            {
                if (Attribute.GetAttrubuteMetaName() == "TallyConnector.Core.Attributes.GenerateHelperMethodAttribute")
                {
                    ImmutableArray<ITypeSymbol> typeArguments = Attribute.AttributeClass!.TypeArguments;
                    //Debugger.Launch();
                    INamedTypeSymbol ReturnType = (INamedTypeSymbol)typeArguments[0];
                    INamedTypeSymbol GetType = (INamedTypeSymbol)typeArguments[1];
                    INamedTypeSymbol PostType = (INamedTypeSymbol)typeArguments[2];

                    if (typeArguments.Count() == 3)
                    {
                        INamedTypeSymbol RequestEnvelopeType = (INamedTypeSymbol)Attribute.AttributeClass.BaseType!.TypeArguments[3];
                        args.AttributeArgs.Add(new(ReturnType, GetType, PostType, RequestEnvelopeType));
                    }
                    else
                    {
                        INamedTypeSymbol RequestEnvelopeType = (INamedTypeSymbol)typeArguments[3];
                        args.AttributeArgs.Add(new(ReturnType, GetType, PostType, RequestEnvelopeType));
                    }

                }
            }
            return args;
        }
        return null;
    }

    //public void Execute(SourceProductionContext sourceProductionContext, HelperMethodArgs args)
    //{

    //    INamedTypeSymbol getType = args.GetObjType;

    //    IEnumerable<ISymbol> info = GetInfo(getType);
    //    INamedTypeSymbol RequestEnvelopeType = args.RequestEnvelopeType;
    //    MethodDeclarationSyntax methodDeclarationSyntax = CreateGenerateGetRequestXML(RequestEnvelopeType);


    //    TDLReportParams tDLReportParams = new();

    //    foreach (ISymbol symbol in info)
    //    {
    //        GenerateTDLReportParams(tDLReportParams, symbol);
    //    }
    //    CompilationUnitSyntax compilationUnitSyntax = CompilationUnit()
    //        .WithMembers(SingletonList<MemberDeclarationSyntax>(FileScopedNamespaceDeclaration(IdentifierName(args.NameSpace))
    //        .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
    //                                                                               true))),
    //              SyntaxKind.NamespaceKeyword,
    //              TriviaList()))
    //        .WithMembers(List<MemberDeclarationSyntax>(
    //            new MemberDeclarationSyntax[]
    //            {
    //                ClassDeclaration(args.ClassName)
    //                .WithModifiers(TokenList(new[]{Token(SyntaxKind.PublicKeyword),Token(SyntaxKind.PartialKeyword)}))
    //                .WithMembers(List<MemberDeclarationSyntax>(new MemberDeclarationSyntax[]
    //                {
    //                    methodDeclarationSyntax
    //                }))
    //            }))
    //        )
    //        ).NormalizeWhitespace();
    //    string source = compilationUnitSyntax.ToFullString();
    //    //if(!Debugger.IsAttached)
    //    //{
    //    //   // Debugger.Launch();
    //    //}
    //    //sourceProductionContext.AddSource($"Test_{Guid.NewGuid()}.g.cs", $"//sdsfd{args.ClassName}.{getType.Name}.g.cs");
    //    sourceProductionContext.AddSource($"{args.ClassName}.{getType.Name}.g.cs", source);
    //}



    private void GenerateTDLReportParams(TDLReportParams tDLReportParams, ISymbol symbol)
    {
        if (symbol is IPropertySymbol propertySymbol && !propertySymbol.IsReadOnly)
        {
            bool isPrimitive = propertySymbol.Type.SpecialType != SpecialType.None;
            if (isPrimitive)
            {
                TDLField TField = GetTDLFieldFromPrimitivePropertySymbol(symbol);
                tDLReportParams.Fields.Add(TField);
            }
            else
            {
                TDLPart tDLPart = new();
                // Debugger.Launch();
                if (propertySymbol.Type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsGenericType)
                {
                    INamedTypeSymbol typeSymbol = (INamedTypeSymbol)namedTypeSymbol.TypeArguments.First();
                    bool isGenericTypePrimitive = typeSymbol.SpecialType != SpecialType.None;
                    if (isGenericTypePrimitive)
                    {
                        tDLPart.Fields.Add(GetTDLFieldFromPrimitivePropertySymbol(typeSymbol));
                    }
                    else
                    {
                        tDLPart.Parts.Add(GetTDLPart(typeSymbol));
                    }
                }
                else
                {
                    tDLPart.Parts.Add(GetTDLPart((INamedTypeSymbol)propertySymbol));
                }
                tDLReportParams.Parts.Add(tDLPart);
            }
        }
    }

    private TDLPart GetTDLPart(INamedTypeSymbol namedTypeSymbol)
    {
        TDLPart tDLPart = new();
        IEnumerable<ISymbol> symbols = GetInfo(namedTypeSymbol);
        foreach (ISymbol symbol in symbols)
        {
            if (symbol is IPropertySymbol propertySymbol && !propertySymbol.IsReadOnly)
            {
                if (propertySymbol.Type is INamedTypeSymbol propertynamedTypeSymbol && propertynamedTypeSymbol.IsGenericType)
                {
                    INamedTypeSymbol typeSymbol = (INamedTypeSymbol)propertynamedTypeSymbol.TypeArguments.First();
                    bool isGenericTypePrimitive = typeSymbol.SpecialType != SpecialType.None;
                    if (isGenericTypePrimitive)
                    {
                        tDLPart.Fields.Add(GetTDLFieldFromPrimitivePropertySymbol(typeSymbol));
                    }
                    else
                    {
                        tDLPart.Parts.Add(GetTDLPart(typeSymbol));
                    }
                }
                else
                {
                    bool isPrimitive = propertySymbol.Type.SpecialType != SpecialType.None;
                    if (isPrimitive)
                    {
                        TDLField TField = GetTDLFieldFromPrimitivePropertySymbol(symbol);
                        tDLPart.Fields.Add(TField);
                    }
                    else
                    {
                        tDLPart.Parts.Add(GetTDLPart((INamedTypeSymbol)propertySymbol.Type));
                    }
                }

            }
        }
        return tDLPart;

    }

    private static TDLField GetTDLFieldFromPrimitivePropertySymbol(ISymbol symbol)
    {
        var xmlTag = symbol.Name;
        TDLField TField = new()
        {
            Name = $"{TDLPrefix}{symbol.Name}",
            XmlTag = xmlTag,
            Set = xmlTag,
        };
        return TField;
    }

    private IEnumerable<ISymbol> GetInfo(INamedTypeSymbol getType)
    {
        IEnumerable<ISymbol> info = getType.GetPropertiesAndFields();
        if (getType.BaseType != null)
        {
            info = info.Concat(GetInfo(getType.BaseType));
        }
        return info;
    }
}
