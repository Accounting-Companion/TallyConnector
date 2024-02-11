
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using TallyConnector.TDLReportSourceGenerator.Execute;
using TallyConnector.TDLReportSourceGenerator.Extensions;
using TallyConnector.TDLReportSourceGenerator.Extensions.Symbols;
using static TallyConnector.TDLReportSourceGenerator.Constants;
namespace TallyConnector.TDLReportSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class TDLReportSourceGenerator : IIncrementalGenerator
{

    private IEqualityComparer<INamedTypeSymbol> c;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}
        IncrementalValueProvider<ProjectArgs> projectDirProvider = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, _) =>
            {
                provider.GlobalOptions.TryGetValue("build_property.projectdir", out string? projectDirectory);
                provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? rootNamespace);
                return new ProjectArgs() { RootNamespace = rootNamespace!, ProjectRoot = projectDirectory! };
            });
        IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> syntaxProvider = context.SyntaxProvider
          .CreateSyntaxProvider(SyntaxPredicate, SematicTransform)
          .Where(static (type) => type != null).Collect()!;
        context.RegisterSourceOutput(syntaxProvider.Combine(projectDirProvider), Execute);
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
        if (symbol.HasFullyQualifiedMetadataName(Name) || symbol.HasInterfaceWithFullyQualifiedMetadataName(Name) || symbol.HasOrInheritsFromFullyQualifiedMetadataName(Name))
        {
            return symbol;
        };
        return null;
    }

    private void Execute(SourceProductionContext context,
                          (ImmutableArray<INamedTypeSymbol> Left, ProjectArgs Right) tuple)
    {
        var (symbols, projectArgs) = tuple;
        List<Dictionary<string, GenerateSymbolsArgs>> args = [];
        HashSet<string> names = [];
        foreach (var symbol in symbols)
        {
            string fullName = symbol.OriginalDefinition.ToString();
            if (names.Contains(fullName))
            {
                continue;
            }
            names.Add(fullName);
            Dictionary<string, GenerateSymbolsArgs> generateSymbolsArgs = [];
            ImmutableArray<AttributeData> attributeDatas = symbol.GetAttributes();
            foreach (var attributeData in attributeDatas)
            {
                string attrName = attributeData.GetAttrubuteMetaName();
                if (attrName == GenerateHelperMethodAttrName)
                {
                    INamedTypeSymbol? attributeClass = attributeData.AttributeClass;
                    HelperAttributeData? helperAttributeData = GetHelperAttributeData(attributeData);
                    var typeargs = attributeClass!.TypeArguments;
                    switch (typeargs.Length)
                    {
                        case 1:
                            typeargs = attributeClass.BaseType!.TypeArguments;
                            break;
                        default:
                            break;

                    }
                    var getTypeSymbol = (INamedTypeSymbol)typeargs[0];
                    generateSymbolsArgs.Add(helperAttributeData?.MethodNameSuffix ?? getTypeSymbol.Name, new(symbol, getTypeSymbol,
                                                        (INamedTypeSymbol)typeargs[1])
                    {
                        HelperAttributeData = helperAttributeData
                    });
                }
            }
            args.Add(generateSymbolsArgs);
        }
        var generateTDLReportsCommand = new GenerateTDLReportsCommand(args, projectArgs);
        generateTDLReportsCommand.Execute(context);

    }

    private HelperAttributeData? GetHelperAttributeData(AttributeData attributeDataAttribute)
    {
        HelperAttributeData? helperAttributeData = null;
        //if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
        //{
        //}
        if (attributeDataAttribute.NamedArguments != null && attributeDataAttribute.NamedArguments.Length > 0)
        {
            var namedArguments = attributeDataAttribute.NamedArguments;
            helperAttributeData ??= new();
            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "GenerationMode":
                        helperAttributeData.GenerationMode = (int)namedArgument.Value.Value!;
                        break;
                    case "MethodNameSuffix":
                        helperAttributeData.MethodNameSuffix = (string)namedArgument.Value.Value!;
                        break;
                    case "MethodNameSuffixPlural":
                        helperAttributeData.MethodNameSuffixPlural = (string)namedArgument.Value.Value!;
                        break;
                    case "Args":
                        var values = namedArgument.Value.Values;
                        helperAttributeData.Args = values.Where(c => c.Value != null).Select(c => (INamedTypeSymbol)c.Value!).ToList();
                        break;
                }
            }

        }
        return helperAttributeData;
    }
}

internal class HelperAttributeData
{
    public int? GenerationMode { get; internal set; }
    public string? MethodNameSuffix { get; internal set; }
    public string? MethodNameSuffixPlural { get; internal set; }
    public List<INamedTypeSymbol>? Args { get; internal set; } = [];
}

public class UniqueSymbol(string Name, INamedTypeSymbol Symbol)
{
    public string Name { get; } = Name;
    public INamedTypeSymbol Symbol { get; } = Symbol;
}
public class GenerateSymbolsArgs
{
    public GenerateSymbolsArgs(INamedTypeSymbol parentSymbol, INamedTypeSymbol getSymbol) : this(parentSymbol, getSymbol, null) { }


    public GenerateSymbolsArgs(INamedTypeSymbol parentSymbol,
                               INamedTypeSymbol getSymbol,
                               INamedTypeSymbol? requestEnvelope = null)
    {
        ParentSymbol = parentSymbol;
        GetSymbol = getSymbol;
        MethodName = getSymbol.Name;
        RequestEnvelope = requestEnvelope;
    }
    public INamedTypeSymbol ParentSymbol { get; }
    public INamedTypeSymbol GetSymbol { get; }
    public string Name { get; }
    public string NameSpace { get; }
    public INamedTypeSymbol RequestEnvelope { get; }
    public INamedTypeSymbol? PostEnvelope { get; }
    public string MethodName { get; internal set; }
    internal HelperAttributeData? HelperAttributeData { get; set; }
}
public class ProjectArgs
{
    public string ProjectRoot { get; set; }
    public string RootNamespace { get; set; }
}