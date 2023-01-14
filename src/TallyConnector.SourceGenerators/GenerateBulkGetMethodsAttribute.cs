namespace TallyConnector.SourceGenerators.Attributes;
public static class GenerateBulkGetMethodsAttribute
{
    /// <summary>
    /// Generates 
    //#nullable disable
    //namespace TallyConnector.Services;
    //[global::System.CodeDom.Compiler.GeneratedCode("TallyConnector.SourceGenerators", "1.0.0")]
    //[global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = true)]
    //public class GenerateGetMethodsAttribute<T> : global::System.Attribute where T : global::TallyConnector.Core.Models.BasicTallyObject
    //{
    //    public string PluralName { get; set; }

    //    public string TypeName { get; set; }
    //}
    /// </summary>
    /// <returns></returns>
    public static string Generate()
    {
        string AttributeName = AttrubuteName + "Attribute";
        CompilationUnitSyntax compilationUnitSyntax = CompilationUnit()
          .WithMembers(
            SingletonList<MemberDeclarationSyntax>(
              FileScopedNamespaceDeclaration(
                QualifiedName(
                  IdentifierName("TallyConnector"),
                  IdentifierName("Services")))
              .WithNamespaceKeyword(
                Token(
                  TriviaList(
                    Trivia(
                      NullableDirectiveTrivia(
                        Token(SyntaxKind.DisableKeyword),
                        true))),
                  SyntaxKind.NamespaceKeyword,
                  TriviaList()))
              .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                  ClassDeclaration(AttributeName)
                  .WithAttributeLists(
                    List<AttributeListSyntax>(
                      new AttributeListSyntax[] {
                    AttributeList(
                        SingletonSeparatedList < AttributeSyntax > (
                          Attribute(
                            QualifiedName(
                              QualifiedName(
                                QualifiedName(
                                  AliasQualifiedName(
                                    IdentifierName(
                                      Token(SyntaxKind.GlobalKeyword)),
                                    IdentifierName("System")),
                                  IdentifierName("CodeDom")),
                                IdentifierName("Compiler")),
                              IdentifierName("GeneratedCode")))
                          .WithArgumentList(
                            AttributeArgumentList(
                              SeparatedList < AttributeArgumentSyntax > (
                                new SyntaxNodeOrToken[] {
                                  AttributeArgument(
                                      LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal("TallyConnector.SourceGenerators"))),
                                    Token(SyntaxKind.CommaToken),
                                    AttributeArgument(
                                      LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal("1.0.0")))
                                }))))),
                      AttributeList(
                        SingletonSeparatedList < AttributeSyntax > (
                          Attribute(
                            QualifiedName(
                              AliasQualifiedName(
                                IdentifierName(
                                  Token(SyntaxKind.GlobalKeyword)),
                                IdentifierName("System")),
                              IdentifierName("AttributeUsage")))
                          .WithArgumentList(
                            AttributeArgumentList(
                              SeparatedList < AttributeArgumentSyntax > (
                                new SyntaxNodeOrToken[] {
                                  AttributeArgument(
                                      MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        MemberAccessExpression(
                                          SyntaxKind.SimpleMemberAccessExpression,
                                          AliasQualifiedName(
                                            IdentifierName(
                                              Token(SyntaxKind.GlobalKeyword)),
                                            IdentifierName("System")),
                                          IdentifierName("AttributeTargets")),
                                        IdentifierName("Class"))),
                                    Token(SyntaxKind.CommaToken),
                                    AttributeArgument(
                                      LiteralExpression(
                                        SyntaxKind.TrueLiteralExpression))
                                    .WithNameEquals(
                                      NameEquals(
                                        IdentifierName("AllowMultiple")))
                                })))))
                      }))
                  .WithModifiers(
                    TokenList(
                      Token(SyntaxKind.PublicKeyword)))
                  .WithTypeParameterList(
                    TypeParameterList(
                      SingletonSeparatedList<TypeParameterSyntax>(
                        TypeParameter(
                          Identifier(
                            TriviaList(),
                            SyntaxKind.TypeKeyword,
                            "T",
                            "T",
                            TriviaList())))))
                  .WithBaseList(
                    BaseList(
                      SingletonSeparatedList<BaseTypeSyntax>(
                        SimpleBaseType(
                          QualifiedName(
                            AliasQualifiedName(
                              IdentifierName(
                                Token(SyntaxKind.GlobalKeyword)),
                              IdentifierName("System")),
                            IdentifierName("Attribute"))))))
                  .WithConstraintClauses(
                    SingletonList<TypeParameterConstraintClauseSyntax>(
                      TypeParameterConstraintClause(
                        IdentifierName(
                          Identifier(
                            TriviaList(),
                            SyntaxKind.TypeKeyword,
                            "T",
                            "T",
                            TriviaList())))
                      .WithConstraints(
                        SingletonSeparatedList<TypeParameterConstraintSyntax>(
                          TypeConstraint(
                            QualifiedName(
                              QualifiedName(
                                QualifiedName(
                                  AliasQualifiedName(
                                    IdentifierName(
                                      Token(SyntaxKind.GlobalKeyword)),
                                    IdentifierName("TallyConnector")),
                                  IdentifierName("Core")),
                                IdentifierName("Models")),
                              IdentifierName("BasicTallyObject")))))))
                  .WithMembers(
                    List<MemberDeclarationSyntax>(
                      new MemberDeclarationSyntax[] {
                      PropertyDeclaration(
                        PredefinedType(
                          Token(SyntaxKind.StringKeyword)),
                        Identifier("PluralName"))
                      .WithModifiers(
                        TokenList(
                          Token(SyntaxKind.PublicKeyword)))
                      .WithAccessorList(
                        AccessorList(
                          List < AccessorDeclarationSyntax > (
                            new AccessorDeclarationSyntax[] {
                              AccessorDeclaration(
                                  SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(
                                  Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(
                                  SyntaxKind.SetAccessorDeclaration)
                                .WithSemicolonToken(
                                  Token(SyntaxKind.SemicolonToken))
                            }))),
                      PropertyDeclaration(
                        PredefinedType(
                          Token(SyntaxKind.StringKeyword)),
                        Identifier("TypeName"))
                      .WithModifiers(
                        TokenList(
                          Token(SyntaxKind.PublicKeyword)))
                      .WithAccessorList(
                        AccessorList(
                          List < AccessorDeclarationSyntax > (
                            new AccessorDeclarationSyntax[] {
                              AccessorDeclaration(
                                  SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(
                                  Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(
                                  SyntaxKind.SetAccessorDeclaration)
                                .WithSemicolonToken(
                                  Token(SyntaxKind.SemicolonToken))
                            })))
                      }))))))
          .NormalizeWhitespace();
        return compilationUnitSyntax.ToFullString();
    }
}