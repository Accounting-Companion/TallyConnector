using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Execute;
public class PostObjectsHelper
{
    internal static string GetPostObjectsCompilationUnit(Dictionary<string, SymbolData> data, string nameSpace, string name)
    {
        const string objectsArgName = "objects";
        const string objectVarName = "obj";
        const string MesagevarName = "msg";
        const string selectedObj = "selectedObj";
        const string envelopevarName = "envp";
        string reqTypeVarName = "reqType";

        string xmlVarName = "reqXml";
        string xmlRespVarName = "resp";
        string xmlRespObjectVarName = "postResults";

        QualifiedNameSyntax node = QualifiedName(GetGlobalNameforType($"{nameSpace}.Models"), IdentifierName(string.Format(PostRequestEnvelopeMessageName, name)));
        QualifiedNameSyntax type = QualifiedName(GetGlobalNameforType(TallyConnectorRequestModelsNameSpace), GenericName(TallyResponseEnvelopeTypeName).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)node))));

        List<StatementSyntax> statements = [];
        statements.Add(CreateVarInsideMethodWithExpression(reqTypeVarName, CreateStringLiteral($"Posting Objects to Tally")));
        statements.Add(CreateVarInsideMethodWithExpression(envelopevarName, ObjectCreationExpression(type)
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[] {
                Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,GetGlobalNameforType(RequestTypeEnumName),IdentifierName("Import"))),
                Token(SyntaxKind.CommaToken),
                Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,GetGlobalNameforType(HeaderTypeEnumName),IdentifierName("Data"))),
                Token(SyntaxKind.CommaToken),
                Argument(CreateStringLiteral("AllMasters")) })))));



        statements.Add(CreateVarInsideMethodWithExpression(MesagevarName, ObjectCreationExpression(node)
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[] { })))));

        List<StatementSyntax> expressionStatements = [];

        expressionStatements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.CoalesceAssignmentExpression, MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName(objectVarName),
                                                    IdentifierName("RemoteId")),
                                                    InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Guid"), IdentifierName("NewGuid().ToString"))))));

        MemberAccessExpressionSyntax left = MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName(objectVarName),
                                                            IdentifierName("Action"));
        expressionStatements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left,
                                                        ConditionalExpression(BinaryExpression(SyntaxKind.IsExpression, left, QualifiedName(GetGlobalNameforType(ActionEnumFullTypeName), IdentifierName("None"))),
                                                        QualifiedName(GetGlobalNameforType(ActionEnumFullTypeName), IdentifierName("Create")), left))));

        List<SwitchSectionSyntax> SwitcNnodes = [];

        var fieldName = "";
        foreach (var member in data.Select(c => c.Value).Where(c => !c.IsChild && c.GenerationMode is GenerationMode.All or GenerationMode.Post))
        {
            fieldName = member.ServiceFieldName;
            SwitcNnodes.Add(SwitchSection()
                .WithLabels(SingletonList<SwitchLabelSyntax>(
                                                            CasePatternSwitchLabel(
                                                                DeclarationPattern(
                                                                    IdentifierName($"{member.Name}DTO"),
                                                                    SingleVariableDesignation(
                                                                        Identifier(selectedObj))),
                                                                Token(SyntaxKind.ColonToken))))
                 .WithStatements(List<StatementSyntax>(new StatementSyntax[]
                 {
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(MesagevarName),
                                        IdentifierName(member.MethodNameSuffixPlural)),
                                    IdentifierName("Add")))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList<ArgumentSyntax>(
                                        Argument(
                                            IdentifierName(selectedObj)))))),
                        BreakStatement()
                 })));
        }


        //statements.Add(ExpressionStatement(InvocationExpression(IdentifierName(AddCustomResponseReportForPostMethodName))
        //    .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
        //    {
        //       Argument(  IdentifierName(envelopevarName)),
        //    })))));
        expressionStatements.Add(SwitchStatement(IdentifierName(objectVarName))
            .WithSections(List(SwitcNnodes)));
        statements.Add(ForEachStatement(IdentifierName(
                                        Identifier(
                                            TriviaList(),
                                            SyntaxKind.VarKeyword,
                                            "var",
                                            "var",
                                            TriviaList())), Identifier(objectVarName),
                                    IdentifierName(objectsArgName), Block(expressionStatements)));

        statements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(envelopevarName), IdentifierName("Body.RequestData.RequestMessage")), IdentifierName(MesagevarName))));

        statements.Add(CreateVarInsideMethodWithExpression(xmlVarName, InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(envelopevarName), IdentifierName("GetXML")))));

        statements.Add(CreateVarInsideMethodWithExpression(xmlRespVarName, AwaitExpression(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(fieldName), IdentifierName(SendRequestMethodName)))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
            new SyntaxNodeOrToken[]{
                Argument(IdentifierName(xmlVarName)),
                Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(reqTypeVarName)),
                 Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(CancellationTokenArgName))
            }))))));

        statements.Add(CreateVarInsideMethodWithExpression(xmlRespObjectVarName,
                                                          InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                      GetGlobalNameforType(XMLToObjectClassName),
                                                                                                      GenericName(GetObjfromXmlMethodName)
           .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)GetGlobalNameforType(PostResultsFullTypeName))))))
                                                          .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                                          {
                                                               Argument(PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression,MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespVarName), IdentifierName("Response")))),
                                                               Token(SyntaxKind.CommaToken),
                                                            Argument(LiteralExpression(
                                                                    SyntaxKind.NullLiteralExpression))  , //Argument(InvocationExpression(IdentifierName(string.Format(GetXMLAttributeOveridesMethodName,_symbol.TypeName)))),
                                                               Token(SyntaxKind.CommaToken),
                                                               Argument( IdentifierName("_logger"))
                                                          })))));

        statements.Add(ReturnStatement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespObjectVarName), IdentifierName("Results"))));


        var unit = CompilationUnit()
           .WithUsings(List([UsingDirective(IdentifierName(ExtensionsNameSpace)), UsingDirective(IdentifierName($"{nameSpace}.Models"))]))
           .WithMembers(List(new MemberDeclarationSyntax[]
           {
                FileScopedNamespaceDeclaration(IdentifierName(nameSpace))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                      true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List((List<MemberDeclarationSyntax>)(
                [
                    ClassDeclaration(name)
                        .WithModifiers(TokenList([Token(SyntaxKind.PartialKeyword)]))

                        .WithMembers(List((List<MemberDeclarationSyntax>)(
                        [
                            MethodDeclaration(QualifiedName(GetGlobalNameforType("System.Threading.Tasks"), GenericName("Task")
                            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(QualifiedName(GetGlobalNameforType(CollectionsNameSpace), GenericName(ListClassName).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(PostResultFullTypeName))))))))),"PostObjectsAsync")
                                .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
                                {
                                    Parameter(Identifier(objectsArgName)).WithType(QualifiedName( GetGlobalNameforType(CollectionsNameSpace),GenericName(IEnumerableClassName)
                                    .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(GetGlobalNameforType(TallyObjectDTOInterfaceName)))))),
                                    Token(SyntaxKind.CommaToken),
                                    GetCancellationTokenParameterSyntax(),
                                })))
                                .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword),Token(SyntaxKind.AsyncKeyword)]))
                               .WithBody(Block(statements)),


                        //GetPostReqEnvelope(node),
                               ]))),

                        ])))
           })).NormalizeWhitespace().ToFullString();

        return unit;
    }

    private static MethodDeclarationSyntax GetPostReqEnvelope(QualifiedNameSyntax node)
    {
        List<StatementSyntax> statements = [];
        const string envelopevarName = "envp";
        QualifiedNameSyntax type = QualifiedName(GetGlobalNameforType(TallyConnectorRequestModelsNameSpace), GenericName(TallyResponseEnvelopeTypeName).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)node))));
        statements.Add(CreateVarInsideMethodWithExpression(envelopevarName, ObjectCreationExpression(type)
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[] {
                Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,GetGlobalNameforType(RequestTypeEnumName),IdentifierName("Import"))),
                Token(SyntaxKind.CommaToken),
                Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,GetGlobalNameforType(HeaderTypeEnumName),IdentifierName("Data"))),
                Token(SyntaxKind.CommaToken),
                Argument(CreateStringLiteral("AllMasters")) })))));

        statements.Add(ReturnStatement(IdentifierName(envelopevarName)));
        var methodDeclaration = MethodDeclaration(type, GetPostRequerstEnvelopeMethodName)
            .WithBody(Block(statements));
        return methodDeclaration;
    }

}
