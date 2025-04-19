using TallyConnector.TDLReportSourceGenerator.Models;
namespace TallyConnector.TDLReportSourceGenerator.Execute;
public class GetReportHelper
{
    internal static string GetReportResponseEnvelopeCompilationUnit(Dictionary<string, Models.SymbolData> data, string nameSpace, string name)
    {
        List<MemberDeclarationSyntax> members = [];
        var items = data.Select(c => c.Value).Where(c => !c.IsChild && c.GenerationMode is GenerationMode.All or GenerationMode.Get or GenerationMode.GetMultiple);

        foreach (var item in items)
        {
            ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(GetReportResponseEnvelopeName(item))
                        .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword)]))
                        .WithAttributeLists(
                SingletonList(
                    AttributeList(
                        SingletonSeparatedList(
                            Attribute(
                                GetGlobalNameforType(XmlRootAttributeName))
                            .WithArgumentList(
                                AttributeArgumentList(
                                    SingletonSeparatedList(
                                        AttributeArgument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal("ENVELOPE"))))))))))
                        .WithMembers(List(new MemberDeclarationSyntax[]
                        {
                            GetPropertyMemberSyntax(QualifiedName(GetGlobalNameforType(CollectionsNameSpace),GenericName(ListClassName)

                            .WithTypeArgumentList(
                                TypeArgumentList(
                                    SingletonSeparatedList(
                                        (TypeSyntax)GetGlobalNameforType(item.FullName))))),"Objects")
                            .WithInitializer(
                    EqualsValueClause(
                        CollectionExpression())).WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken))
                            .WithAttributeLists(List(new AttributeListSyntax[]
                            {
                                AttributeList(SingletonSeparatedList<AttributeSyntax>(Attribute(
                                        IdentifierName(XMLElementAttributeName))
                .WithArgumentList(AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    AttributeArgument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal(item.RootXmlTag)))
                                                    .WithNameEquals(
                                                        NameEquals(
                                                            IdentifierName("ElementName"))),
                                                    })))))
                            })),
                            GetPropertyMemberSyntax(NullableType(PredefinedType(Token( SyntaxKind.IntKeyword))),"TotalCount") 
                            .WithAttributeLists(List(new AttributeListSyntax[]
                            {
                                AttributeList(SingletonSeparatedList<AttributeSyntax>(Attribute(
                                        IdentifierName(XMLElementAttributeName))
                .WithArgumentList(AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    AttributeArgument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal("TC_TOTALCOUNT")))
                                                    .WithNameEquals(
                                                        NameEquals(
                                                            IdentifierName("ElementName"))),
                                                    })))))
                            }))
                        }));

            members.Add(classDeclarationSyntax);
        }


        var unit = CompilationUnit()
            .WithUsings(List([UsingDirective(IdentifierName(ExtensionsNameSpace))]))
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                FileScopedNamespaceDeclaration(IdentifierName($"{nameSpace}.Models"))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                      true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(members))
            })).NormalizeWhitespace().ToFullString();
        return unit;
    }



    internal static string GetReportResponseEnvelopeHelperCompilationUnit(string nameSpace, string name)
    {
        string xmlArgName = "xml";
        string xmlRespEnvlopeVarName = "respEnv";
        List<StatementSyntax> statements = [];
        const string typeName = "T";
        QualifiedNameSyntax genericNameforEnv = QualifiedName(GetGlobalNameforType($"{nameSpace}.Models"), GenericName($"{name}ReportResponseEnvelope")
                    .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)IdentifierName(typeName)))));

        statements.Add(CreateVarInsideMethodWithExpression(xmlRespEnvlopeVarName,
                                                           InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                       GetGlobalNameforType(XMLToObjectClassName),
                                                                                                       GenericName(GetObjfromXmlMethodName)
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)genericNameforEnv)))))
                                                           .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                                           {
                                                               Argument(IdentifierName(xmlArgName)),
                                                               Token(SyntaxKind.CommaToken),
                                                                Argument(LiteralExpression(SyntaxKind.NullLiteralExpression)),
                                                               Token(SyntaxKind.CommaToken),
                                                               Argument( IdentifierName("_logger"))
                                                           })))));
        statements.Add(ReturnStatement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespEnvlopeVarName), IdentifierName("Objects"))));
        var unit = CompilationUnit()
            .WithUsings(List([UsingDirective(IdentifierName(ExtensionsNameSpace))]))
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                FileScopedNamespaceDeclaration(IdentifierName(nameSpace))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                      true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    ClassDeclaration(name)
                    .WithModifiers(TokenList([Token(SyntaxKind.PartialKeyword)]))
                    .WithMembers(List(new MemberDeclarationSyntax[]
                    {
                        MethodDeclaration(QualifiedName(IdentifierName(CollectionsNameSpace),GenericName(ListClassName)
                        .WithTypeArgumentList(TypeArgumentList(
                            SingletonSeparatedList(
                                (TypeSyntax)IdentifierName(typeName))))),Identifier(ParseReportMethodName))
                        .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>( new SyntaxNodeOrToken[]
                        {
                            Parameter(Identifier(xmlArgName))
                            .WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                        })))
                        .WithTypeParameterList(TypeParameterList(
                        SingletonSeparatedList(
                            TypeParameter(
                                Identifier(typeName)))))
                        .WithConstraintClauses(SingletonList(
                TypeParameterConstraintClause(
                    IdentifierName(typeName))
                .WithConstraints(
                    SingletonSeparatedList(
                        (TypeParameterConstraintSyntax)TypeConstraint(
                            GetGlobalNameforType(BaseInterfaceName))))))
                        .WithBody( Block(statements))
                    }))

                }))
            })).NormalizeWhitespace().ToFullString();
        return unit;
    }
}
