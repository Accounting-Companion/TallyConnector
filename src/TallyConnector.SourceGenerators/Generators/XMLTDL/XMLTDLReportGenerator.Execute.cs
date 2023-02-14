using System;
using System.Collections.Generic;
using System.Text;
using TallyConnector.SourceGenerators.Models;

namespace TallyConnector.SourceGenerators.Generators.XMLTDL;
public partial class XMLTDLReportGenerator
{
    private void Execute(SourceProductionContext sourceProductionContext, XMLTDLReportGeneratorArgs xMLTDLReport)
    {
        //Debugger.Launch();
        //throw new NotImplementedException();

        sourceProductionContext.AddSource($"TC.{xMLTDLReport.ClassName}.cs", CreateTDLReportMethod(xMLTDLReport));
    }
    public string CreateTDLReportMethod(XMLTDLReportGeneratorArgs generatorArgs)
    {
        CompilationUnitSyntax compilationUnitSyntax = CompilationUnit()
          .WithMembers(
            SingletonList<MemberDeclarationSyntax>(
              FileScopedNamespaceDeclaration(IdentifierName(generatorArgs.NameSpace))
              .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                  ClassDeclaration(generatorArgs.ClassName)
                  .WithModifiers(
                    TokenList(
                      new[] {
                    Token(SyntaxKind.PublicKeyword),
                      Token(SyntaxKind.PartialKeyword)
                      }))
                  .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                      MethodDeclaration(
                        PredefinedType(
                          Token(SyntaxKind.VoidKeyword)),
                        Identifier("GetTdlReport"))
                      .WithModifiers(
                        TokenList(
                          new[] {
                        Token(SyntaxKind.PublicKeyword),
                          Token(SyntaxKind.StaticKeyword)
                          }))
                      .WithBody(
                        Block(
                            LocalDeclarationStatement(
                              VariableDeclaration(
                                IdentifierName("Field"))
                              .WithVariables(
                                SingletonSeparatedList<VariableDeclaratorSyntax>(
                                  VariableDeclarator(
                                    Identifier(
                                      TriviaList(),
                                      SyntaxKind.FieldKeyword,
                                      "field",
                                      "field",
                                      TriviaList()))
                                  .WithInitializer(
                                    EqualsValueClause(
                                      ObjectCreationExpression(
                                        IdentifierName("Field"))
                                      .WithArgumentList(
                                        ArgumentList())
                                      .WithInitializer(
                                        InitializerExpression(
                                          SyntaxKind.ObjectInitializerExpression,
                                          SeparatedList<ExpressionSyntax>(
                                            new SyntaxNodeOrToken[] {
                                          AssignmentExpression(
                                              SyntaxKind.SimpleAssignmentExpression,
                                              IdentifierName("Name"),
                                              LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal("name"))),
                                            Token(SyntaxKind.CommaToken),
                                            AssignmentExpression(
                                              SyntaxKind.SimpleAssignmentExpression,
                                              IdentifierName("Set"),
                                              LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal("set"))),
                                            Token(SyntaxKind.CommaToken),
                                            AssignmentExpression(
                                              SyntaxKind.SimpleAssignmentExpression,
                                              IdentifierName("XMLTag"),
                                              LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal("vg"))),
                                            Token(SyntaxKind.CommaToken)
                                            }))))))))
                            ))))))))
          .NormalizeWhitespace();
        string v = compilationUnitSyntax.ToFullString();
        return v;
    }
}