
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
        const string Name = BaseInterfaceName;
        if (symbol.HasInterfaceWithFullyQualifiedMetadataName(Name) || symbol.HasOrInheritsFromFullyQualifiedMetadataName(Name))
        {
            return symbol;
        };
        return null;
    }

    private void Execute(SourceProductionContext context, ImmutableArray<INamedTypeSymbol> symbols)
    {
        var generateTDLReportsCommand = new GenerateTDLReportsCommand(symbols);
        generateTDLReportsCommand.Execute(context);
        
    }

}
public class UniqueSymbol(string Name, INamedTypeSymbol Symbol)
{
    public string Name { get; } = Name;
    public INamedTypeSymbol Symbol { get; } = Symbol;
}
