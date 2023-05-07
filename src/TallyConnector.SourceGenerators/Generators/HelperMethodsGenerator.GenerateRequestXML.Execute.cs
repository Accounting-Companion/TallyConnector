using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Xml.Serialization;
using TallyConnector.SourceGenerators.Extensions.Symbols;
using TallyConnector.SourceGenerators.Models;

namespace TallyConnector.SourceGenerators.Generators;

public partial class HelperMethodsGenerator
{
    public MethodDeclarationSyntax CreateGenerateGetRequestXML(INamedTypeSymbol requestEnvelopeType)
    {
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}
        XmlWriterArgs xmlWriterArgs = GetAllXmlWriterArgs(requestEnvelopeType, isRoot: true);

        MethodDeclarationSyntax methodDeclarationSyntax = GetMemberDeclarationSyntax(requestEnvelopeType.Name, requestEnvelopeType.ContainingNamespace.ToDisplayString(), xmlWriterArgs);

        return methodDeclarationSyntax;
    }

    private XmlWriterArgs GetAllXmlWriterArgs(INamedTypeSymbol requestEnvelopeType, int LastOrder = 0, bool isRoot = false)
    {
        XmlWriterArgs xmlWriterArgs = new();
        xmlWriterArgs.SortOrder = ++LastOrder;
        if (isRoot)
        {
            xmlWriterArgs.XmlTag = GetXmlRootfromClassSymbol(requestEnvelopeType);
        }
        IEnumerable<ISymbol> RequestEnvelopeProps = GetInfo(requestEnvelopeType);
        foreach (ISymbol symbol in RequestEnvelopeProps)
        {
            if (symbol is IPropertySymbol propertySymbol)
            {
                bool IsSpecialType = propertySymbol.Type.SpecialType != SpecialType.None;
                if (IsSpecialType)
                {
                    XmlWriterElement item = new();
                    item.SortOrder = ++LastOrder;
                    item.XmlTag = propertySymbol.Name.ToUpper();
                    string? xmlTag = GetXmlTagFromPropertyAttributes(propertySymbol);
                    item.XmlTag = xmlTag ?? item.XmlTag;
                    xmlWriterArgs.Elements.Add(item);
                }
                else
                {

                    if (propertySymbol.Type is INamedTypeSymbol namedTypeSymbol &&
                        namedTypeSymbol.IsGenericType &&
                        namedTypeSymbol.GetClassMetaName() == "System.Collections.Generic.List")
                    {

                    }
                    else
                    {
                        XmlWriterArgs item = GetAllXmlWriterArgs((INamedTypeSymbol)propertySymbol.Type, LastOrder);
                        LastOrder = item.SortOrder;
                        item.XmlTag = propertySymbol.Name;
                        item.XmlTag = GetXmlTagFromPropertyAttributes(propertySymbol) ?? propertySymbol.Name.ToUpper();
                        xmlWriterArgs.ComplexElements.Add(item);
                    }
                }

            }
        }
        return xmlWriterArgs;
    }

    private static string? GetXmlTagFromPropertyAttributes(IPropertySymbol propertySymbol)
    {
        System.Collections.Immutable.ImmutableArray<AttributeData> attributeData = propertySymbol.GetAttributes();
        foreach (AttributeData attributeDataAttribute in attributeData)
        {
            if (attributeDataAttribute.GetAttrubuteMetaName() == "System.Xml.Serialization.XmlElementAttribute")
            {
                if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
                {
                    var name = attributeDataAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (name != null)
                    {
                        return name;
                    }
                }
            }
        }
        return null;
    }

    private static string GetXmlRootfromClassSymbol(INamedTypeSymbol symbol)
    {
        System.Collections.Immutable.ImmutableArray<AttributeData> attributeDatas = symbol.GetAttributes();
        var Name = symbol.Name.ToUpper();
        foreach (AttributeData attributeData in attributeDatas)
        {
            if (attributeData.GetAttrubuteMetaName() == "System.Xml.Serialization.XmlRootAttribute")
            {
                if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
                {
                    Name = attributeData.ConstructorArguments.FirstOrDefault().Value?.ToString();
                }
            }


        }

        return Name;
    }

    private static void GetXmlWriterArgs(ISymbol symbol)
    {
        XmlWriterArgs writerArgs = new XmlWriterArgs();
        if (symbol is IPropertySymbol propertySymbol)
        {
            bool IsPrimitive = propertySymbol.Type.SpecialType == SpecialType.None;
            if (IsPrimitive)
            {

            }
            else
            {

            }

        }
    }

    private MethodDeclarationSyntax GetMemberDeclarationSyntax(string getObjectType, string containingNamespace, XmlWriterArgs xmlWriterArg)
    {
        List<StatementSyntax> statements = new List<StatementSyntax>();
        statements.AddRange(new StatementSyntax[]
        {
            LocalDeclarationStatement(VariableDeclaration(IdentifierName("global::System.IO.StringWriter"))
                      .WithVariables(SingletonSeparatedList<VariableDeclaratorSyntax>(VariableDeclarator(Identifier("stringWriter"))
                      .WithInitializer(EqualsValueClause(ImplicitObjectCreationExpression()

                  )))))
                  .WithUsingKeyword(Token(SyntaxKind.UsingKeyword)),

                  LocalDeclarationStatement(
                                  VariableDeclaration(
                                      IdentifierName("global::System.Xml.XmlWriterSettings"))
                                  .WithVariables(
                                      SingletonSeparatedList<VariableDeclaratorSyntax>(
                                          VariableDeclarator(
                                              Identifier("settings"))
                                          .WithInitializer(
                                              EqualsValueClause(
                                                  ImplicitObjectCreationExpression()
                                                  .WithInitializer(
                                                      InitializerExpression(
                                                          SyntaxKind.ObjectInitializerExpression,
                                                          SeparatedList<ExpressionSyntax>(
                                                              new SyntaxNodeOrToken[]{
                                                                AssignmentExpression(
                                                                    SyntaxKind.SimpleAssignmentExpression,
                                                                    IdentifierName("Indent"),
                                                                    LiteralExpression(
                                                                        SyntaxKind.TrueLiteralExpression)),
                                                                Token(SyntaxKind.CommaToken),
                                                                AssignmentExpression(
                                                                    SyntaxKind.SimpleAssignmentExpression,
                                                                    IdentifierName("Async"),
                                                                    LiteralExpression(
                                                                        SyntaxKind.TrueLiteralExpression)),
                                                                Token(SyntaxKind.CommaToken),
                                                                AssignmentExpression(
                                                                    SyntaxKind.SimpleAssignmentExpression,
                                                                    IdentifierName("OmitXmlDeclaration"),
                                                                    LiteralExpression(
                                                                        SyntaxKind.TrueLiteralExpression)),
                                                                Token(SyntaxKind.CommaToken),
                                                                AssignmentExpression(
                                                                    SyntaxKind.SimpleAssignmentExpression,
                                                                    IdentifierName("Encoding"),
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName("Encoding"),
                                                                        IdentifierName("Unicode"))),
                                                                Token(SyntaxKind.CommaToken),
                                                                AssignmentExpression(
                                                                    SyntaxKind.SimpleAssignmentExpression,
                                                                    IdentifierName("CheckCharacters"),
                                                                    LiteralExpression(
                                                                        SyntaxKind.FalseLiteralExpression))})))))))),

                  LocalDeclarationStatement(
                      VariableDeclaration(
                                      IdentifierName("global::System.Xml.XmlWriter"))
                                  .WithVariables(
                                      SingletonSeparatedList<VariableDeclaratorSyntax>(
                                          VariableDeclarator(
                                              Identifier("writer"))
                                          .WithInitializer(
                                              EqualsValueClause(
                                                  InvocationExpression(
                                                      MemberAccessExpression(
                                                          SyntaxKind.SimpleMemberAccessExpression,
                                                          IdentifierName("XmlWriter"),
                                                          IdentifierName("Create")))
                                                  .WithArgumentList(
                                                      ArgumentList(
                                                          SeparatedList<ArgumentSyntax>(
                                                              new SyntaxNodeOrToken[]{
                                                                Argument(
                                                                    IdentifierName("stringWriter")),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    IdentifierName("settings"))}))))))))
                              .WithUsingKeyword(
                                  Token(SyntaxKind.UsingKeyword)),
                              ExpressionStatement(
                                  AwaitExpression(
                                      InvocationExpression(
                                          MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              IdentifierName("writer"),
                                              IdentifierName("WriteStartDocumentAsync")))))
        });
        statements.AddRange(CreateExpressionForItem(xmlWriterArg));
        statements.AddRange(new StatementSyntax[]
        {
            ExpressionStatement(
                                  AwaitExpression(
                                      InvocationExpression(
                                          MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              IdentifierName("writer"),
                                              IdentifierName("WriteEndDocumentAsync"))))),
                              ExpressionStatement(
                                  AwaitExpression(
                                      InvocationExpression(
                                          MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              IdentifierName("writer"),
                                              IdentifierName("FlushAsync"))))),
                              ReturnStatement(
                                  InvocationExpression(
                                      MemberAccessExpression(
                                          SyntaxKind.SimpleMemberAccessExpression,
                                          IdentifierName("stringWriter"),
                                          IdentifierName("ToString"))))
        });
        return MethodDeclaration(GenericName("global::System.Threading.Tasks.Task")
                              .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(PredefinedType(Token(SyntaxKind.StringKeyword))))),
                            Identifier($"Get{getObjectType.Substring(0, 1).ToUpper()}{getObjectType.Substring(1, getObjectType.Length - 1)}XML"))
              .WithModifiers(TokenList(
                  new[]
                  {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.AsyncKeyword),
                  }))
              .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
              {
                Parameter(Identifier("value")).WithType(IdentifierName($"global::{containingNamespace}.{getObjectType}"))
              })))
              .WithBody(

            Block(statements));
    }

    private static List<ExpressionStatementSyntax> CreateExpressionforItems(XmlWriterArgs xmlWriterArg)
    {
        List<ExpressionStatementSyntax> expressionStatements = new List<ExpressionStatementSyntax>();
        foreach (var item in xmlWriterArg.ComplexElements)
        {
            expressionStatements.AddRange(CreateExpressionForItem(item));
        }
        return expressionStatements;
    }

    private static List<ExpressionStatementSyntax> CreateExpressionForItem(XmlWriterArgs item)
    {
        List<ExpressionStatementSyntax> expressionStatements = new List<ExpressionStatementSyntax>();
        List<IXmlWriterArg> tempArgs = new(item.Elements);
        tempArgs.AddRange(item.ComplexElements);
        tempArgs = tempArgs.OrderBy(x => x.SortOrder).ToList();
        expressionStatements.Add(CreateStartElement(item.XmlTag));
        foreach (var subItem in tempArgs)
        {
            if (subItem is XmlWriterElement element)
            {
                expressionStatements.Add(CreateStartElement(element.XmlTag));
                expressionStatements.Add(CreateEndElement());
            }
            if (subItem is XmlWriterArgs XmlWriterArg)
            {
                expressionStatements.AddRange(CreateExpressionForItem(XmlWriterArg));
            }

        }
        expressionStatements.Add(CreateEndElement());
        return expressionStatements;
    }

    private static ExpressionStatementSyntax CreateEndElement()
    {
        return ExpressionStatement(
                                    AwaitExpression(
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("writer"),
                                                IdentifierName("WriteEndElementAsync")))));
    }

    private static ExpressionStatementSyntax CreateStartElement(string xmlTag)
    {
        return ExpressionStatement(
                                    AwaitExpression(
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("writer"),
                                                IdentifierName("WriteStartElementAsync")))
                                        .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                Argument(
                                                    LiteralExpression(
                                                        SyntaxKind.NullLiteralExpression)),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(
                                                    LiteralExpression(
                                                        SyntaxKind.StringLiteralExpression,
                                                        Literal(xmlTag))),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(
                                                    LiteralExpression(
                                                        SyntaxKind.NullLiteralExpression))})))));
    }
}
