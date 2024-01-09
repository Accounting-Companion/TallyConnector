
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using TC.TDLReportSourceGenerator.Execute;
using static TC.TDLReportSourceGenerator.Constants;
namespace TC.TDLReportSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class TDLReportSourceGenerator : IIncrementalGenerator
{
    private IEqualityComparer<INamedTypeSymbol> c;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> syntaxProvider = context.SyntaxProvider
          .CreateSyntaxProvider(SyntaxPredicate, SematicTransform)
          .Where(static (type) => type != null).Collect()!;

        context.RegisterSourceOutput(syntaxProvider, Execute);
    }

    private bool SyntaxPredicate(SyntaxNode node, CancellationToken token)
    {
        if (node == null) return false;
        if (node is ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.HasOrPotentiallyHasBaseTypes() || classDeclaration.HasOrPotentiallyHasAttributes();
        }
        return false;
    }


    private INamedTypeSymbol? SematicTransform(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (!context.SemanticModel.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp8))
        {
            return null;
        }
        var classDeclaration = Unsafe.As<ClassDeclarationSyntax>(context.Node);
        INamedTypeSymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, token);
        if (symbol == null)
        {
            return null;
        }
        const string Name = BaseClassName;
        if (symbol.HasInterfaceWithFullyQualifiedMetadataName(Name) || symbol.HasOrInheritsFromFullyQualifiedMetadataName(Name))
        {
            return symbol;
        };
        return null;
    }

    private void Execute(SourceProductionContext context,
                         ImmutableArray<INamedTypeSymbol> symbols)
    {
        List<Dictionary<string, GenerateSymbolsArgs>> args = [];
        foreach (var symbol in symbols)
        {
            Dictionary<string, GenerateSymbolsArgs> generateSymbolsArgs = [];
            ImmutableArray<AttributeData> attributeDatas = symbol.GetAttributes();
            foreach (var attributeData in attributeDatas)
            {
                string attrName = attributeData.GetAttrubuteMetaName();
                if (attrName == GenerateHelperMethodAttrName)
                {
                    INamedTypeSymbol? attributeClass = attributeData.AttributeClass;
                    var typeargs = attributeClass!.TypeArguments;
                    INamedTypeSymbol getTypeSymbol;
                    switch (typeargs.Length)
                    {
                        case 1:
                            getTypeSymbol = (INamedTypeSymbol)typeargs[0];
                            generateSymbolsArgs.Add(getTypeSymbol.Name, new(symbol, getTypeSymbol) );
                            break;
                        case 2:
                            getTypeSymbol = (INamedTypeSymbol)typeargs[0];
                            generateSymbolsArgs.Add(getTypeSymbol.Name, new(symbol,getTypeSymbol,
                                                        (INamedTypeSymbol)typeargs[1]));
                            break;
                        case 4:
                            getTypeSymbol = (INamedTypeSymbol)typeargs[0];
                            generateSymbolsArgs.Add(getTypeSymbol.Name, new(symbol,getTypeSymbol,
                                                        (INamedTypeSymbol)typeargs[1],
                                                         (INamedTypeSymbol)typeargs[2],
                                                          (INamedTypeSymbol)typeargs[3]));
                            break;
                        default:
                            break;
                    }
                }
            }
            args.Add(generateSymbolsArgs);
        }
        var generateTDLReportsCommand = new GenerateTDLReportsCommand(args);
        generateTDLReportsCommand.Execute(context);

    }

}
public class UniqueSymbol(string Name, INamedTypeSymbol Symbol)
{
    public string Name { get; } = Name;
    public INamedTypeSymbol Symbol { get; } = Symbol;
}
public class GenerateSymbolsArgs
{
    public GenerateSymbolsArgs(INamedTypeSymbol parentSymbol, INamedTypeSymbol getSymbol) : this(parentSymbol, getSymbol, getSymbol) { }


    public GenerateSymbolsArgs(INamedTypeSymbol parentSymbol,
                               INamedTypeSymbol getSymbol,
                               INamedTypeSymbol postSymbol,
                               INamedTypeSymbol? requestEnvelope = null,
                               INamedTypeSymbol? postEnvelope = null)
    {
        ParentSymbol = parentSymbol;
        GetSymbol = getSymbol;
        MethodName = getSymbol.Name;
        PostSymbol = postSymbol;
        RequestEnvelope = requestEnvelope;
        PostEnvelope = postEnvelope;
    }
    public INamedTypeSymbol ParentSymbol { get; }
    public INamedTypeSymbol GetSymbol { get; }
    public INamedTypeSymbol PostSymbol { get; }
    public string Name { get; }
    public string NameSpace { get; }
    public INamedTypeSymbol? RequestEnvelope { get; }
    public INamedTypeSymbol? PostEnvelope { get; }
    public string MethodName { get; internal set; }
}