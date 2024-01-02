
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TC.TDLReportSourceGenerator;
[Generator(LanguageNames.CSharp)]
public class TDLReportSourceGenerator : IIncrementalGenerator
{
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
        return symbol;
    }

    private void Execute(SourceProductionContext context, ImmutableArray<INamedTypeSymbol> array)
    {
        throw new NotImplementedException();
    }
}
