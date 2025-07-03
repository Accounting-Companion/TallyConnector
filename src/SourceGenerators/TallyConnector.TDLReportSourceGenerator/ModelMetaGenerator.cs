using System.Collections.Immutable;
using System.Diagnostics;
using TallyConnector.TDLReportSourceGenerator.Services;
namespace TallyConnector.TDLReportSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class ModelMetaGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //Debugger.Launch();
        IncrementalValueProvider<string> rootNameSpace = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, _) =>
            {
                provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? rootNamespace);
                return rootNamespace ?? string.Empty;
            });

        var modelsData = context.SyntaxProvider.ForAttributeWithMetadataName(Attributes.Abstractions.GenerateMetaAttributeName,
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

            ModelTransformer tDLReportTransformer = ModelTransformer.Instance;
            foreach (var modelSymbol in modelSymbols)
            {
                token.ThrowIfCancellationRequested();
                await tDLReportTransformer.TransformAsync(modelSymbol, token: token);
            }
            var modelDataList = tDLReportTransformer.GetSymbols();
            foreach (var modelData in modelDataList)
            {
                new MetaDataGenerator(modelData, context, token)
                    .GenerateMeta()
                    .GenerateMetaField();
                if (modelData.GenerateITallyRequestableObectAttribute)
                {
                    new TDLEnvelopeGenerator(modelData, context, token)
                    .Generate();
                }
            }
        }
        catch (Exception ex)
        {
            //throw;
        }
    }
}

