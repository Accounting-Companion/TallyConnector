using System.Collections.Immutable;
using TallyConnector.TDLReportSourceGenerator.Services;
namespace TallyConnector.TDLReportSourceGenerator;


[Generator(LanguageNames.CSharp)]
public class TDLReportSourceGenerator : IIncrementalGenerator
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
        try
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
        catch (Exception ex)
        {
            //throw;
        }
    }

}