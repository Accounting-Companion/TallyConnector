using System;
using System.Collections.Generic;
using System.Text;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Execute;
public static class FactoryMethods
{
    public const string CancellationTokenArgName = "token";
    internal static bool CheckSymbolHasProperty(SymbolData symbol, string name)
    {
        bool hasProperty = symbol.Children.ContainsKey(name);
        if (symbol.BaseSymbolData == null)
        {
            return hasProperty;
        }
        else
        {
            return hasProperty || CheckSymbolHasProperty(symbol.BaseSymbolData.SymbolData, name);
        }

    }
    internal static ExpressionSyntax GetEmptyStringSyntax() => MemberAccessExpression(
                                                      SyntaxKind.SimpleMemberAccessExpression,
                                                      PredefinedType(
                                                          Token(SyntaxKind.StringKeyword)),
                                                      IdentifierName("Empty"));
    internal static ChildSymbolData GetPropertyWithName(SymbolData symbol, string name)
    {
        symbol.Children.TryGetValue(name, out var property);
        if (symbol.BaseSymbolData == null)
        {
            return property;
        }
        else
        {
            return property ?? GetPropertyWithName(symbol.BaseSymbolData.SymbolData, name);
        }

    }
    internal static AliasQualifiedNameSyntax GetGlobalNameforType(string typeName)
    {
        return AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)),
                                                                   IdentifierName(typeName));
    }
    internal static LiteralExpressionSyntax CreateStringLiteral(string name)
    {
        return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(name));
    }
    internal static ParameterSyntax GetCancellationTokenParameterSyntax()
    {
        return Parameter(Identifier(CancellationTokenArgName))
                    .WithType(GetGlobalNameforType(CancellationTokenStructName))
                    .WithDefault(EqualsValueClause(LiteralExpression(
                                                        SyntaxKind.DefaultLiteralExpression,
                                                        Token(SyntaxKind.DefaultKeyword))));
    }
    internal static PropertyDeclarationSyntax GetPropertyMemberSyntax(TypeSyntax typeSyntax, string Name, bool isOverriden = false)
    {
        List<SyntaxToken> tokens = [Token(SyntaxKind.PublicKeyword)];
        if (isOverriden)
        {
            tokens.Add(Token(SyntaxKind.NewKeyword));
        }
        PropertyDeclarationSyntax item = PropertyDeclaration(typeSyntax, Identifier(Name))
                                    .WithModifiers(TokenList(tokens))
                                    .WithAccessorList(AccessorList(
                                                    List(
                                                        new AccessorDeclarationSyntax[]{
                                        AccessorDeclaration(
                                            SyntaxKind.GetAccessorDeclaration)
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken)),
                                        AccessorDeclaration(
                                            SyntaxKind.SetAccessorDeclaration)
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken))})));

        return item;
    }


    internal static LocalDeclarationStatementSyntax CreateVarInsideMethodWithExpression(string varName, ExpressionSyntax expressionSyntax)
    {
        var varSyntax = LocalDeclarationStatement(
            CreateVariableDeclaration(varName, expressionSyntax));
        return varSyntax;
    }

    internal static VariableDeclarationSyntax CreateVariableDeclaration(string varName, ExpressionSyntax expressionSyntax)
    {
        IdentifierNameSyntax varSyntax = IdentifierName(Identifier(TriviaList(),
                                                                                  SyntaxKind.VarKeyword,
                                                                                  "var",
                                                                                  "var",
                                                                                  TriviaList()));
        return CreateVariableDelaration(varSyntax, varName, expressionSyntax);
    }
    internal static VariableDeclarationSyntax CreateVariableDelaration(TypeSyntax varSyntax, string varName, ExpressionSyntax expressionSyntax)
    {
        return VariableDeclaration(varSyntax)
            .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(varName))
            .WithInitializer(EqualsValueClause(expressionSyntax))
            ));
    }
    internal static string GetReportResponseEnvelopeName(SymbolData item)
    {
        return $"{item.MainSymbol.Name}ReportResponseEnvelopeFor{item.TypeName}";
    }

}
