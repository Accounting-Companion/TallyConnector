using System.Collections.Immutable;
using TallyConnector.TDLReportSourceGenerator.Services;
namespace TallyConnector.TDLReportSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class ModelMetaGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!System.Diagnostics.Debugger.IsAttached)
        //{
        //    System.Diagnostics.Debugger.Launch();
        //}
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
        if (node is EnumDeclarationSyntax enumDeclaration)
        {
            return true;
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
                if (modelData.Symbol.CheckInterface(Constants.Models.Abstractions.IMetaGeneratedFullTypeName))
                {
                    if (modelData.BaseData == null || !modelData.BaseData.Symbol.CheckInterface(Constants.Models.Abstractions.IMetaGeneratedFullTypeName))
                    {
                        continue;
                    }
                }
                if (modelData.Symbol.ContainingAssembly.MetadataName != assembly.MetadataName)
                {
                    continue;
                }
                new MetaDataGenerator(modelData, context, token)
                    .GenerateMeta()
                    .GenerateMetaField();
                if (modelData.GenerateITallyRequestableObject)
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

