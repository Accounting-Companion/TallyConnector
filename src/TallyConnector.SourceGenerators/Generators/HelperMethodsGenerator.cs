using TallyConnector.SourceGenerators.Models;

namespace TallyConnector.SourceGenerators.Generators;
public static class HelperMethodsGenerator
{
    public static string Generate(HelperMethodArgs helperMethodArgs)
    {
        var objectNamespace = helperMethodArgs.ObjectNameSpace;
        var objectName = helperMethodArgs.ObjectName;
        string BulkGetMethodName = $"Get{helperMethodArgs.PluralName ?? (objectName + "s")}Async";
        string GetMethodName = $"Get{objectName}Async";
        string PostMethodName = $"Post{objectName}Async";
        string TypeName = $"{helperMethodArgs.GenericTypeName ?? objectName}Type";

        CompilationUnitSyntax compilationUnitSyntax = CompilationUnit()
          .WithMembers(
            SingletonList(
              (MemberDeclarationSyntax)FileScopedNamespaceDeclaration(
                  IdentifierName(helperMethodArgs.NameSpace))
              .WithNamespaceKeyword(
                Token(
                  TriviaList(
                    Trivia(
                      NullableDirectiveTrivia(
                        Token(SyntaxKind.EnableKeyword),
                        true))),
                  SyntaxKind.NamespaceKeyword,
                  TriviaList()))
              .WithMembers(
                SingletonList(
                  (MemberDeclarationSyntax)ClassDeclaration(helperMethodArgs.ClassName)
                  .WithModifiers(
                    TokenList(
                      new[] {
                    Token(SyntaxKind.PublicKeyword),
                      Token(SyntaxKind.PartialKeyword)
                      }))
                  .WithMembers(
                    List(
                      new MemberDeclarationSyntax[] {
                          PostMethod(objectNamespace, objectName,PostMethodName,TypeName),
                          GetMethod(objectNamespace, objectName,GetMethodName,TypeName),
                          GetMethodWithExactType(objectNamespace, objectName,GetMethodName,TypeName),
                          GetBulkMethod(objectNamespace, objectName,BulkGetMethodName,TypeName),
                          GetBulkMethodWithExactType(objectNamespace, objectName,BulkGetMethodName,TypeName),
                          GetBulkPaginatedMethod(objectNamespace, objectName,BulkGetMethodName,TypeName),
                          GetBulkPaginatedMethodWithExactType(objectNamespace, objectName,BulkGetMethodName,TypeName)
                      }))))))
          .NormalizeWhitespace();
        return compilationUnitSyntax.ToFullString();
    }

    private static MethodDeclarationSyntax PostMethod(string objectNamespace, string objectName, string methodName, string typeName)
    {
        string LowerobjectName = objectName.ToLower();
        return MethodDeclaration(
                                GenericName(Identifier("Task"))
                                .WithTypeArgumentList(TypeArgumentList(
                                  SingletonSeparatedList<TypeSyntax>(
                                            QualifiedName(IdentifierName($"global::TallyConnector.Core.Models"), IdentifierName("TallyResult"))))),
                                Identifier(methodName))
                              .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                  AttributeList(
                                    SingletonSeparatedList<AttributeSyntax>(
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
                                          SeparatedList<AttributeArgumentSyntax>(
                                            new SyntaxNodeOrToken[] {
                                      AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorName))),
                                        Token(SyntaxKind.CommaToken),
                                        AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorVersion)))
                                            })))))))
                              .WithModifiers(
                                TokenList(
                                  new[] {
                            Token(
                                TriviaList(
                                  Trivia(
                                    DocumentationCommentTrivia(
                                      SyntaxKind.SingleLineDocumentationCommentTrivia,
                                      List < XmlNodeSyntax > (
                                        new XmlNodeSyntax[] {
                                          XmlText()
                                            .WithTextTokens(
                                              TokenList(
                                                XmlTextLiteral(
                                                  TriviaList(
                                                    DocumentationCommentExterior("///")),
                                                  " ",
                                                  " ",
                                                  TriviaList()))),
                                            XmlEmptyElement("inheritdoc"),
                                            XmlText()
                                            .WithTextTokens(
                                              TokenList(
                                                XmlTextNewLine(
                                                  TriviaList(),
                                                  Environment.NewLine,
                                                  Environment.NewLine,
                                                  TriviaList())))
                                        })))),
                                SyntaxKind.PublicKeyword,
                                TriviaList()),
                              Token(SyntaxKind.AsyncKeyword)
                                  }))
                              .WithTypeParameterList(
                                TypeParameterList(
                                  SingletonSeparatedList<TypeParameterSyntax>(
                                    TypeParameter(
                                      Identifier(typeName)))))
                              .WithParameterList(
                                ParameterList(
                                  SeparatedList<ParameterSyntax>(
                                    new SyntaxNodeOrToken[] {
                                        Parameter(
                                Identifier(LowerobjectName))
                            .WithType(
                                IdentifierName(typeName)),
                            Token(SyntaxKind.CommaToken),
                            Parameter(
                                Identifier("postRequestOptions"))
                            .WithType(
                                NullableType(
                                    QualifiedName(IdentifierName($"global::TallyConnector.Core.Models"), IdentifierName("PostRequestOptions"))))
                            .WithDefault(
                                EqualsValueClause(
                                    LiteralExpression(
                                        SyntaxKind.NullLiteralExpression)))
                             })))
                              .WithConstraintClauses(
                                SingletonList<TypeParameterConstraintClauseSyntax>(
                                  TypeParameterConstraintClause(
                                    IdentifierName(typeName))
                                  .WithConstraints(
                                    SingletonSeparatedList<TypeParameterConstraintSyntax>(
                                      TypeConstraint(QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName)))))))
                              .WithBody(
                                Block(
                                  SingletonList<StatementSyntax>(
                                    ReturnStatement(
                                      AwaitExpression(
                                        InvocationExpression(
                                          GenericName(
                                            Identifier("PostObjectToTallyAsync"))
                                          .WithTypeArgumentList(
                                            TypeArgumentList(
                                              SingletonSeparatedList<TypeSyntax>(
                                                IdentifierName(typeName)))))
                                        .WithArgumentList(
                                          ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                              new SyntaxNodeOrToken[] {
                                        Argument(IdentifierName(LowerobjectName)),
                                                  Token(SyntaxKind.CommaToken),
                                                  Argument(
                                                    IdentifierName("postRequestOptions"))
                                              }))))))));
    }
    private static MethodDeclarationSyntax GetMethod(string objectNamespace, string objectName, string methodName, string typeName)
    {
        return MethodDeclaration(
                                GenericName(Identifier("Task"))
                                .WithTypeArgumentList(TypeArgumentList(
                                  SingletonSeparatedList<TypeSyntax>(
                                            IdentifierName(typeName)))),
                                Identifier(methodName))
                              .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                  AttributeList(
                                    SingletonSeparatedList<AttributeSyntax>(
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
                                          SeparatedList<AttributeArgumentSyntax>(
                                            new SyntaxNodeOrToken[] {
                                      AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorName))),
                                        Token(SyntaxKind.CommaToken),
                                        AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorVersion)))
                                            })))))))
                              .WithModifiers(
                                TokenList(
                                  new[] {
                            Token(
                                TriviaList(
                                  Trivia(
                                    DocumentationCommentTrivia(
                                      SyntaxKind.SingleLineDocumentationCommentTrivia,
                                      List < XmlNodeSyntax > (
                                        new XmlNodeSyntax[] {
                                          XmlText()
                                            .WithTextTokens(
                                              TokenList(
                                                XmlTextLiteral(
                                                  TriviaList(
                                                    DocumentationCommentExterior("///")),
                                                  " ",
                                                  " ",
                                                  TriviaList()))),
                                            XmlEmptyElement("inheritdoc"),
                                            XmlText()
                                            .WithTextTokens(
                                              TokenList(
                                                XmlTextNewLine(
                                                  TriviaList(),
                                                  Environment.NewLine,
                                                  Environment.NewLine,
                                                  TriviaList())))
                                        })))),
                                SyntaxKind.PublicKeyword,
                                TriviaList()),
                              Token(SyntaxKind.AsyncKeyword)
                                  }))
                              .WithTypeParameterList(
                                TypeParameterList(
                                  SingletonSeparatedList<TypeParameterSyntax>(
                                    TypeParameter(
                                      Identifier(typeName)))))
                              .WithParameterList(
                                ParameterList(
                                  SeparatedList<ParameterSyntax>(
                                    new SyntaxNodeOrToken[] {
                                        Parameter(
                                Identifier("lookupValue"))
                            .WithType(
                                PredefinedType(
                                    Token(SyntaxKind.StringKeyword))),
                            Token(SyntaxKind.CommaToken),
                            Parameter(
                                Identifier("options"))
                            .WithType(
                                NullableType(
                                    IdentifierName(objectName == "Voucher" ?"VoucherRequestOptions" :"MasterRequestOptions")))
                            .WithDefault(
                                EqualsValueClause(
                                    LiteralExpression(
                                        SyntaxKind.NullLiteralExpression)))
                             })))
                              .WithConstraintClauses(
                                SingletonList<TypeParameterConstraintClauseSyntax>(
                                  TypeParameterConstraintClause(
                                    IdentifierName(typeName))
                                  .WithConstraints(
                                    SingletonSeparatedList<TypeParameterConstraintSyntax>(
                                      TypeConstraint(QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName)))))))
                              .WithBody(
                                Block(
                                  SingletonList<StatementSyntax>(
                                    ReturnStatement(
                                      AwaitExpression(
                                        InvocationExpression(
                                          GenericName(
                                            Identifier("GetObjectAsync"))
                                          .WithTypeArgumentList(
                                            TypeArgumentList(
                                              SingletonSeparatedList<TypeSyntax>(
                                                IdentifierName(typeName)))))
                                        .WithArgumentList(
                                          ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                              new SyntaxNodeOrToken[] {
                                        Argument(IdentifierName("lookupValue")),
                                                  Token(SyntaxKind.CommaToken),
                                                  Argument(
                                                    IdentifierName("options"))
                                              }))))))));
    }
    private static MethodDeclarationSyntax GetBulkMethod(string objectNamespace, string objectName, string methodName, string typeName)
    {
        return MethodDeclaration(
                                GenericName(Identifier("Task"))
                                .WithTypeArgumentList(TypeArgumentList(
                                  SingletonSeparatedList<TypeSyntax>(
                                    NullableType(
                                      GenericName(
                                        Identifier("List"))
                                      .WithTypeArgumentList(
                                        TypeArgumentList(
                                          SingletonSeparatedList<TypeSyntax>(
                                            IdentifierName(typeName)))))))),
                                Identifier(methodName))
                              .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                  AttributeList(
                                    SingletonSeparatedList<AttributeSyntax>(
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
                                          SeparatedList<AttributeArgumentSyntax>(
                                            new SyntaxNodeOrToken[] {
                                      AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorName))),
                                        Token(SyntaxKind.CommaToken),
                                        AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorVersion)))
                                            })))))))
                              .WithModifiers(
                                TokenList(
                                  new[] {
                            Token(
                                TriviaList(
                                  Trivia(
                                    DocumentationCommentTrivia(
                                      SyntaxKind.SingleLineDocumentationCommentTrivia,
                                      List < XmlNodeSyntax > (
                                        new XmlNodeSyntax[] {
                                          XmlText()
                                            .WithTextTokens(
                                              TokenList(
                                                XmlTextLiteral(
                                                  TriviaList(
                                                    DocumentationCommentExterior("///")),
                                                  " ",
                                                  " ",
                                                  TriviaList()))),
                                            XmlEmptyElement("inheritdoc"),
                                            XmlText()
                                            .WithTextTokens(
                                              TokenList(
                                                XmlTextNewLine(
                                                  TriviaList(),
                                                  Environment.NewLine,
                                                  Environment.NewLine,
                                                  TriviaList())))
                                        })))),
                                SyntaxKind.PublicKeyword,
                                TriviaList()),
                              Token(SyntaxKind.AsyncKeyword)
                                  }))
                              .WithTypeParameterList(
                                TypeParameterList(
                                  SingletonSeparatedList<TypeParameterSyntax>(
                                    TypeParameter(
                                      Identifier(typeName)))))
                              .WithParameterList(
                                ParameterList(
                                  SeparatedList<ParameterSyntax>(
                                    new SyntaxNodeOrToken[] {
                              Parameter(
                                  Identifier("options"))
                                .WithType(
                                  NullableType(
                                    QualifiedName(QualifiedName(
                                        QualifiedName(
                                          AliasQualifiedName(
                                            IdentifierName(
                                              Token(SyntaxKind.GlobalKeyword)),
                                            IdentifierName("TallyConnector")),
                                          IdentifierName("Core")),
                                        IdentifierName("Models")),
                                      IdentifierName("RequestOptions"))))
                                .WithDefault(
                                  EqualsValueClause(
                                    LiteralExpression(
                                      SyntaxKind.NullLiteralExpression)))
                                    })))
                              .WithConstraintClauses(
                                SingletonList<TypeParameterConstraintClauseSyntax>(
                                  TypeParameterConstraintClause(
                                    IdentifierName(typeName))
                                  .WithConstraints(
                                    SingletonSeparatedList<TypeParameterConstraintSyntax>(
                                      TypeConstraint(QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName)))))))
                              .WithBody(
                                Block(
                                  SingletonList<StatementSyntax>(
                                    ReturnStatement(
                                      AwaitExpression(
                                        InvocationExpression(
                                          GenericName(
                                            Identifier("GetAllObjectsAsync"))
                                          .WithTypeArgumentList(
                                            TypeArgumentList(
                                              SingletonSeparatedList<TypeSyntax>(
                                                IdentifierName(typeName)))))
                                        .WithArgumentList(
                                          ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                              new SyntaxNodeOrToken[] {
                                        Argument(
                                          IdentifierName("options")) }))))))));
    }
    private static MethodDeclarationSyntax GetBulkPaginatedMethod(string objectNamespace, string objectName, string methodName, string typeName)
    {
        return MethodDeclaration(
                                GenericName(Identifier("Task"))
                                .WithTypeArgumentList(TypeArgumentList(
                                  SingletonSeparatedList<TypeSyntax>(
                                    NullableType(
                                      GenericName(
                                        Identifier("global::TallyConnector.Core.Models.Pagination.PaginatedResponse"))
                                      .WithTypeArgumentList(
                                        TypeArgumentList(
                                          SingletonSeparatedList<TypeSyntax>(
                                            IdentifierName(typeName)))))))),
                                Identifier(methodName))
                              .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                  AttributeList(
                                    SingletonSeparatedList<AttributeSyntax>(
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
                                          SeparatedList<AttributeArgumentSyntax>(
                                            new SyntaxNodeOrToken[] {
                                      AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorName))),
                                        Token(SyntaxKind.CommaToken),
                                        AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorVersion)))
                                            })))))))
                              .WithModifiers(
                                TokenList(
                                  new[] {
                            Token(
                                TriviaList(
                                  Trivia(
                                    DocumentationCommentTrivia(
                                      SyntaxKind.SingleLineDocumentationCommentTrivia,
                                      List < XmlNodeSyntax > (
                                        new XmlNodeSyntax[] {
                                          XmlText()
                                            .WithTextTokens(
                                              TokenList(
                                                XmlTextLiteral(
                                                  TriviaList(
                                                    DocumentationCommentExterior("///")),
                                                  " ",
                                                  " ",
                                                  TriviaList()))),
                                            XmlEmptyElement("inheritdoc"),
                                            XmlText()
                                            .WithTextTokens(
                                              TokenList(
                                                XmlTextNewLine(
                                                  TriviaList(),
                                                  Environment.NewLine,
                                                  Environment.NewLine,
                                                  TriviaList())))
                                        })))),
                                SyntaxKind.PublicKeyword,
                                TriviaList()),
                              Token(SyntaxKind.AsyncKeyword)
                                  }))
                              .WithTypeParameterList(
                                TypeParameterList(
                                  SingletonSeparatedList<TypeParameterSyntax>(
                                    TypeParameter(
                                      Identifier(typeName)))))
                              .WithParameterList(
                                ParameterList(
                                  SeparatedList<ParameterSyntax>(
                                    new SyntaxNodeOrToken[] {
                              Parameter(
                                  Identifier("options"))
                                .WithType(
                                    QualifiedName(QualifiedName(
                                        QualifiedName(
                                          AliasQualifiedName(
                                            IdentifierName(
                                              Token(SyntaxKind.GlobalKeyword)),
                                            IdentifierName("TallyConnector")),
                                          IdentifierName("Core")),
                                        IdentifierName("Models")),
                                      IdentifierName("PaginatedRequestOptions")))
                                    })))
                              .WithConstraintClauses(
                                SingletonList<TypeParameterConstraintClauseSyntax>(
                                  TypeParameterConstraintClause(
                                    IdentifierName(typeName))
                                  .WithConstraints(
                                    SingletonSeparatedList<TypeParameterConstraintSyntax>(
                                      TypeConstraint(QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName)))))))
                              .WithBody(
                                Block(
                                  SingletonList<StatementSyntax>(
                                    ReturnStatement(
                                      AwaitExpression(
                                        InvocationExpression(
                                          GenericName(
                                            Identifier("GetObjectsAsync"))
                                          .WithTypeArgumentList(
                                            TypeArgumentList(
                                              SingletonSeparatedList<TypeSyntax>(
                                                IdentifierName(typeName)))))
                                        .WithArgumentList(
                                          ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                              new SyntaxNodeOrToken[] {
                                        Argument(
                                          IdentifierName("options")) }))))))));
    }

    private static MethodDeclarationSyntax GetMethodWithExactType(string objectNamespace, string objectName, string methodName, string typeName)
    {
        return MethodDeclaration(
                                GenericName(
                                    Identifier("Task"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName))))),
                                Identifier(methodName))
            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                  AttributeList(
                                    SingletonSeparatedList<AttributeSyntax>(
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
                                          SeparatedList<AttributeArgumentSyntax>(
                                            new SyntaxNodeOrToken[] {
                                      AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorName))),
                                        Token(SyntaxKind.CommaToken),
                                        AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorVersion)))
                                            })))))))
                            .WithModifiers(
                                TokenList(
                                    new[]{
                                        Token(
                                            TriviaList(
                                                Trivia(
                                                    DocumentationCommentTrivia(
                                                        SyntaxKind.SingleLineDocumentationCommentTrivia,
                                                        List<XmlNodeSyntax>(
                                                            new XmlNodeSyntax[]{
                                                                XmlText()
                                                                .WithTextTokens(
                                                                    TokenList(
                                                                        XmlTextLiteral(
                                                                            TriviaList(
                                                                                DocumentationCommentExterior("///")),
                                                                            " ",
                                                                            " ",
                                                                            TriviaList()))),
                                                                XmlEmptyElement("inheritdoc"),
                                                                XmlText()
                                                                .WithTextTokens(
                                                                    TokenList(
                                                                        XmlTextNewLine(
                                                                            TriviaList(),
                                                                            Environment.NewLine,
                                                                            Environment.NewLine,
                                                                            TriviaList())))})))),
                                            SyntaxKind.PublicKeyword,
                                            TriviaList()),
                                        Token(SyntaxKind.AsyncKeyword)}))
                            .WithParameterList(
                                ParameterList(
                                  SeparatedList<ParameterSyntax>(
                                    new SyntaxNodeOrToken[] {
                                        Parameter(
                                Identifier("lookupValue"))
                            .WithType(
                                PredefinedType(
                                    Token(SyntaxKind.StringKeyword))),
                            Token(SyntaxKind.CommaToken),
                            Parameter(
                                Identifier("options"))
                            .WithType(
                                NullableType(
                                    IdentifierName(objectName == "Voucher" ?"VoucherRequestOptions" :"MasterRequestOptions")))
                            .WithDefault(
                                EqualsValueClause(
                                    LiteralExpression(
                                        SyntaxKind.NullLiteralExpression)))
                             })))
                            .WithBody(
                                Block(
                                    SingletonList<StatementSyntax>(
                                        ReturnStatement(
                                            AwaitExpression(
                                                InvocationExpression(
                                                    GenericName(
                                                        Identifier(methodName))
                                                    .WithTypeArgumentList(
                                                        TypeArgumentList(
                                                            SingletonSeparatedList<TypeSyntax>(
                                                                QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName))))))
                                                .WithArgumentList(
                                          ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                              new SyntaxNodeOrToken[] {
                                        Argument(IdentifierName("lookupValue")),
                                                  Token(SyntaxKind.CommaToken),
                                                  Argument(
                                                    IdentifierName("options"))
                                              }))))))));
    }
    private static MethodDeclarationSyntax GetBulkMethodWithExactType(string objectNamespace, string objectName, string methodName, string typeName)
    {
        return MethodDeclaration(
                                GenericName(
                                    Identifier("Task"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            NullableType(
                                                GenericName(
                                                    Identifier("List"))
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SingletonSeparatedList<TypeSyntax>(
                                                            QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName))))))))),
                                Identifier(methodName))
            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                  AttributeList(
                                    SingletonSeparatedList<AttributeSyntax>(
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
                                          SeparatedList<AttributeArgumentSyntax>(
                                            new SyntaxNodeOrToken[] {
                                      AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorName))),
                                        Token(SyntaxKind.CommaToken),
                                        AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorVersion)))
                                            })))))))
                            .WithModifiers(
                                TokenList(
                                    new[]{
                                        Token(
                                            TriviaList(
                                                Trivia(
                                                    DocumentationCommentTrivia(
                                                        SyntaxKind.SingleLineDocumentationCommentTrivia,
                                                        List<XmlNodeSyntax>(
                                                            new XmlNodeSyntax[]{
                                                                XmlText()
                                                                .WithTextTokens(
                                                                    TokenList(
                                                                        XmlTextLiteral(
                                                                            TriviaList(
                                                                                DocumentationCommentExterior("///")),
                                                                            " ",
                                                                            " ",
                                                                            TriviaList()))),
                                                                XmlEmptyElement("inheritdoc"),
                                                                XmlText()
                                                                .WithTextTokens(
                                                                    TokenList(
                                                                        XmlTextNewLine(
                                                                            TriviaList(),
                                                                            Environment.NewLine,
                                                                            Environment.NewLine,
                                                                            TriviaList())))})))),
                                            SyntaxKind.PublicKeyword,
                                            TriviaList()),
                                        Token(SyntaxKind.AsyncKeyword)}))
                            .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList<ParameterSyntax>(
                                        Parameter(
                                            Identifier("options"))
                                        .WithType(
                                           NullableType(QualifiedName(QualifiedName(
                                        QualifiedName(
                                          AliasQualifiedName(
                                            IdentifierName(
                                              Token(SyntaxKind.GlobalKeyword)),
                                            IdentifierName("TallyConnector")),
                                          IdentifierName("Core")),
                                        IdentifierName("Models")),
                                      IdentifierName("RequestOptions"))))
                                        .WithDefault(
                                  EqualsValueClause(
                                    LiteralExpression(
                                      SyntaxKind.NullLiteralExpression))))))
                            .WithBody(
                                Block(
                                    SingletonList<StatementSyntax>(
                                        ReturnStatement(
                                            AwaitExpression(
                                                InvocationExpression(
                                                    GenericName(
                                                        Identifier(methodName))
                                                    .WithTypeArgumentList(
                                                        TypeArgumentList(
                                                            SingletonSeparatedList<TypeSyntax>(
                                                                QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName))))))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("options"))))))))));
    }
    private static MethodDeclarationSyntax GetBulkPaginatedMethodWithExactType(string objectNamespace, string objectName, string methodName, string typeName)
    {
        return MethodDeclaration(
                                GenericName(
                                    Identifier("Task"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            NullableType(
                                                GenericName(
                                                    Identifier("global::TallyConnector.Core.Models.Pagination.PaginatedResponse"))
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SingletonSeparatedList<TypeSyntax>(
                                                            QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName))))))))),
                                Identifier(methodName))
            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                  AttributeList(
                                    SingletonSeparatedList<AttributeSyntax>(
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
                                          SeparatedList<AttributeArgumentSyntax>(
                                            new SyntaxNodeOrToken[] {
                                      AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorName))),
                                        Token(SyntaxKind.CommaToken),
                                        AttributeArgument(
                                          LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(SourceGeneratorVersion)))
                                            })))))))
                            .WithModifiers(
                                TokenList(
                                    new[]{
                                        Token(
                                            TriviaList(
                                                Trivia(
                                                    DocumentationCommentTrivia(
                                                        SyntaxKind.SingleLineDocumentationCommentTrivia,
                                                        List<XmlNodeSyntax>(
                                                            new XmlNodeSyntax[]{
                                                                XmlText()
                                                                .WithTextTokens(
                                                                    TokenList(
                                                                        XmlTextLiteral(
                                                                            TriviaList(
                                                                                DocumentationCommentExterior("///")),
                                                                            " ",
                                                                            " ",
                                                                            TriviaList()))),
                                                                XmlEmptyElement("inheritdoc"),
                                                                XmlText()
                                                                .WithTextTokens(
                                                                    TokenList(
                                                                        XmlTextNewLine(
                                                                            TriviaList(),
                                                                            Environment.NewLine,
                                                                            Environment.NewLine,
                                                                            TriviaList())))})))),
                                            SyntaxKind.PublicKeyword,
                                            TriviaList()),
                                        Token(SyntaxKind.AsyncKeyword)}))
                            .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList<ParameterSyntax>(
                                        Parameter(
                                            Identifier("options"))
                                        .WithType(
                                           QualifiedName(QualifiedName(
                                        QualifiedName(
                                          AliasQualifiedName(
                                            IdentifierName(
                                              Token(SyntaxKind.GlobalKeyword)),
                                            IdentifierName("TallyConnector")),
                                          IdentifierName("Core")),
                                        IdentifierName("Models")),
                                      IdentifierName("PaginatedRequestOptions")))
                                        )))
                            .WithBody(
                                Block(
                                    SingletonList<StatementSyntax>(
                                        ReturnStatement(
                                            AwaitExpression(
                                                InvocationExpression(
                                                    GenericName(
                                                        Identifier(methodName))
                                                    .WithTypeArgumentList(
                                                        TypeArgumentList(
                                                            SingletonSeparatedList<TypeSyntax>(
                                                                QualifiedName(IdentifierName($"global::{objectNamespace}"), IdentifierName(objectName))))))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("options"))))))))));
    }


}