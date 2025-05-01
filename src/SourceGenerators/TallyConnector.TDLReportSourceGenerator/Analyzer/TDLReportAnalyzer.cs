using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Text;

namespace TallyConnector.TDLReportSourceGenerator.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TDLReportAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            "TC_GEN_001",
            "",
            "Class '{0}' is marked with [{1}] but is not declared 'partial'",
            "Source Generation",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Classes using the {Attributes.SourceGenerator.ImplementTallyRequestableObjectAttribute} must be declared as partial to allow for code generation or extensions.");
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);

    }

    private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var semanticModel = context.SemanticModel;
        INamedTypeSymbol? classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);
        if (classSymbol == null)
        {
            return;
        }
        var attributes = classSymbol.GetAttributes();


        CheckWhetherClassisPartialAndHasAttribute(classDeclaration, classSymbol, attributes, context);

    }

    private static void CheckWhetherClassisPartialAndHasAttribute(ClassDeclarationSyntax classDeclaration, INamedTypeSymbol classSymbol, ImmutableArray<AttributeData> attributes, SyntaxNodeAnalysisContext context)
    {
        bool hasTargetAttribute = false;
        foreach (var attribute in attributes)
        {
            if (attribute.HasFullyQualifiedMetadataName(Attributes.SourceGenerator.ImplementTallyRequestableObjectAttribute))
            {
                hasTargetAttribute = true;
                break;
            }
        }

        if (!hasTargetAttribute)
        {
            return;
        }
        bool isPartial = classDeclaration.HasPartialKeyword();
        if (isPartial)
        {
            return;
        }
        var diagnostic = Diagnostic.Create(
                    Rule,
                    classDeclaration.Identifier.GetLocation(),
                    classSymbol.Name,
                    Attributes.SourceGenerator.ImplementTallyRequestableObjectAttribute
                );
        context.ReportDiagnostic(diagnostic);
    }
}
