using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Execute;
public class PostRequestEnvelopeHelper
{
    internal static string GetPostRequestEnvelopeCompilationUnit(Dictionary<string, SymbolData> data, string nameSpace, string name)
    {
        List<MemberDeclarationSyntax> members = [];

        var items = data.Select(c => c.Value).Where(c => !c.IsChild && c.GenerationMode is GenerationMode.All or GenerationMode.Post);
        List<MemberDeclarationSyntax> memberDeclarationSyntaxes = [];

        foreach (var item in items)
        {
            PropertyDeclarationSyntax propertyDeclarationSyntax = GetPropertyMemberSyntax(QualifiedName(GetGlobalNameforType(CollectionsNameSpace), GenericName(ListClassName)

.WithTypeArgumentList(
    TypeArgumentList(
        SingletonSeparatedList(
            (TypeSyntax)IdentifierName($"{item.Name}DTO"))))), item.MethodNameSuffixPlural)
                .WithInitializer(
                        EqualsValueClause(
                            CollectionExpression()))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken));
            propertyDeclarationSyntax = propertyDeclarationSyntax.WithAttributeLists(List(new AttributeListSyntax[] 
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
            }));;
            memberDeclarationSyntaxes.Add(propertyDeclarationSyntax);
        }
        ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(string.Format(PostRequestEnvelopeMessageName, name))
                        .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword)]))
                        .WithMembers(List(memberDeclarationSyntaxes));
        members.Add(classDeclarationSyntax);

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
}
