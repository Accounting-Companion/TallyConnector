using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TallyConnector.TDLReportSourceGenerator.Execute;
using TallyConnector.TDLReportSourceGenerator.Services;
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
        var data = context.SyntaxProvider
            .ForAttributeWithMetadataName("TallyConnector.Core.Attributes.SourceGenerator.ImplementTallyService",
                                          SyntaxPredicate,
                                          SematicTransformAttribute)
            .Where(static (c) => c.attr != null && c.Item2 != null)
            .Collect()!;


        IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> syntaxProvider = context.SyntaxProvider
          .CreateSyntaxProvider(SyntaxPredicate, SematicTransform)
          .Where(static (type) => type != null).Collect()!;
        context.RegisterSourceOutput(data.Combine(projectDirProvider), Execute);
        // context.RegisterSourceOutput(syntaxProvider.Combine(projectDirProvider), Execute);
    }



    private (AttributeData attr, INamedTypeSymbol?) SematicTransformAttribute(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        var attr = context.Attributes.First();
        return (attr, context.TargetSymbol as INamedTypeSymbol);
    }

    private bool SyntaxPredicate(SyntaxNode node, CancellationToken token)
    {
        if (node == null) return false;
        //if (node is ClassDeclarationSyntax classDeclaration)
        //{
        //    return classDeclaration.HasOrPotentiallyHasBaseTypes() || classDeclaration.HasOrPotentiallyHasAttributes();
        //}
        return true;
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
        }
        ;
        return null;
    }

    private void Execute(SourceProductionContext context,
                          (ImmutableArray<(AttributeData attr, INamedTypeSymbol)> Left, ProjectArgs Right) tuple)
    {
        try
        {

            var (symbolsData, projectArgs) = tuple;

            List<Dictionary<string, GenerateSymbolsArgs>> args = [];
            HashSet<string> names = [];

            foreach (var symbolData in symbolsData)
            {
                var symbol = symbolData.Item2;
                string fullName = symbol.OriginalDefinition.ToString();

                // this is required incase the class we are dealing is defined in multiple files
                if (names.Contains(fullName))
                {
                    continue;
                }
                var fieldName = symbolData.attr.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? "";
                names.Add(fullName);
                Dictionary<string, GenerateSymbolsArgs> generateSymbolsArgs = [];
                ImmutableArray<AttributeData> attributeDatas = symbol.GetAttributes();
                AttributeData activitySourceAttribute = attributeDatas.Where(c => c.HasFullyQualifiedMetadataName(ActivitySourceAttributeName)).FirstOrDefault();
                string? activitySourceName = null;
                if (activitySourceAttribute != null)
                {
                    activitySourceName = GetActivitySourceSymbol(activitySourceAttribute);
                }
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
                                                            (INamedTypeSymbol)typeargs[1], fieldName)
                        {
                            HelperAttributeData = helperAttributeData,
                            ActivitySourceName = activitySourceName
                        });
                    }
                }
                args.Add(generateSymbolsArgs);
            }
            var generateTDLReportsCommand = new GenerateTDLReportsCommand(args, projectArgs);
            generateTDLReportsCommand.Execute(context);
        }
        catch (Exception ex)
        {
            throw;
        }

    }

    private string? GetActivitySourceSymbol(AttributeData activitySourceAttribute)
    {
        if (activitySourceAttribute.NamedArguments != null && activitySourceAttribute.NamedArguments.Length > 0)
        {
            var namedArguments = activitySourceAttribute.NamedArguments;
            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "ActivitySource":
                        return (string?)namedArgument.Value.Value;
                }
            }
        }
        return null;
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
                        helperAttributeData.Args = [.. values.Where(c => c.Value != null).Select(c => (INamedTypeSymbol)c.Value!)];
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



    public GenerateSymbolsArgs(INamedTypeSymbol parentSymbol,
                               INamedTypeSymbol getSymbol,
                               INamedTypeSymbol requestEnvelope, string fieldName)
    {
        ParentSymbol = parentSymbol;
        GetSymbol = getSymbol;
        MethodName = getSymbol.Name;
        RequestEnvelope = requestEnvelope;
        FieldName = fieldName;
    }
    public INamedTypeSymbol ParentSymbol { get; }
    public INamedTypeSymbol GetSymbol { get; }
    public string Name { get; }
    public string NameSpace { get; }
    public INamedTypeSymbol RequestEnvelope { get; }
    public string FieldName { get; }
    public INamedTypeSymbol? PostEnvelope { get; }
    public string MethodName { get; internal set; }
    internal HelperAttributeData? HelperAttributeData { get; set; }

    public string? ActivitySourceName { get; set; }
}
public class ProjectArgs
{
    public string ProjectRoot { get; set; }
    public string RootNamespace { get; set; }
}
[Generator(LanguageNames.CSharp)]
public class TDLReportSourceGeneratorV2 : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<string> rootNameSpace = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, _) =>
            {
                provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? rootNamespace);
                return rootNamespace ?? string.Empty;
            });

        var modelsData = context.SyntaxProvider.ForAttributeWithMetadataName(Attributes.SourceGenerator.ImplementTallyRequestableObjectAttribute,
                                                                             SyntaxPredicate,
                                                                             Transform).Collect();

        context.RegisterSourceOutput(rootNameSpace.Combine(modelsData).Combine(context.CompilationProvider),
                                     async (context, src) => await Execute(context, src.Left.Left, src.Left.Right, src.Right));

    }
    private bool SyntaxPredicate(SyntaxNode node, CancellationToken token)
    {
        if (node is ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.HasPartialKeyword();
        }
        return false;
    }

    private INamedTypeSymbol Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        var symbol = (INamedTypeSymbol)context.TargetSymbol;
        return symbol;
    }


    private async Task Execute(SourceProductionContext context,
                         string rootNameSpace,
                         ImmutableArray<INamedTypeSymbol> modelSymbols,
                         Compilation compilation)
    {
        var token = context.CancellationToken;
        var assembly = compilation.Assembly;

        TDLReportTransformer tDLReportTransformer = new(assembly);
        foreach (var modelSymbol in modelSymbols)
        {
            token.ThrowIfCancellationRequested();
            await tDLReportTransformer.TransformAsync(modelSymbol, token);
        }

        var modelDataList = tDLReportTransformer.GetTransformedData();
        foreach (var modelData in modelDataList)
        {
            var tDLReportSourceGenerator = new TDLReportGenerator(modelData);
            string code = tDLReportSourceGenerator.Generate(token);
            context.AddSource($"{modelData.FullName}", code);
        }
    }












}