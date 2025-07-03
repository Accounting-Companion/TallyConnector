namespace TallyConnector.TDLReportSourceGenerator.Execute;
public static class FactoryMethods
{
    public const string CancellationTokenArgName = "token";

    internal static ExpressionSyntax GetEmptyStringSyntax() => MemberAccessExpression(
                                                      SyntaxKind.SimpleMemberAccessExpression,
                                                      PredefinedType(
                                                          Token(SyntaxKind.StringKeyword)),
                                                      IdentifierName("Empty"));

    internal static AliasQualifiedNameSyntax GetGlobalNameforType(string typeName)
    {
        return AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)),
                                                                   IdentifierName(typeName));
    }
    internal static LiteralExpressionSyntax CreateStringLiteral(string name)
    {
        return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(name));
    }
    internal static LiteralExpressionSyntax CreateNullLiteral()
    {
        return LiteralExpression(SyntaxKind.NullLiteralExpression);
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
    internal static FieldDeclarationSyntax CreateConstStringVar(string varName, string varText)
    {
        TypeSyntax strSyntax = PredefinedType(Token(SyntaxKind.StringKeyword));

        VariableDeclarationSyntax variableDeclarationSyntax = CreateVariableDelaration(strSyntax,
                                                                                       varName,
                                                                                       LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(varText)));

        return FieldDeclaration(variableDeclarationSyntax);
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

    internal static ImplicitObjectCreationExpressionSyntax CreateImplicitObjectExpression(List<SyntaxNodeOrToken> constructorArgs,
                                                                                         List<SyntaxNodeOrToken>? intializerArgs = null)
    {
        var implicitObjectCreationExpressionSyntax = ImplicitObjectCreationExpression().WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(constructorArgs)));
        if (intializerArgs != null)
        {
            implicitObjectCreationExpressionSyntax = implicitObjectCreationExpressionSyntax
                .WithInitializer(InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                                                       SeparatedList<ExpressionSyntax>(intializerArgs)));
        }

        return implicitObjectCreationExpressionSyntax;
    }
}
