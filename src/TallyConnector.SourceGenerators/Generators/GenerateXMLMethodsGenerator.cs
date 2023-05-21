using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using TallyConnector.SourceGenerators.Extensions.Symbols;
using TallyConnector.SourceGenerators.Models;

namespace TallyConnector.SourceGenerators.Generators;
public class GenerateXMLMethodsGenerator
{
    private SourceProductionContext _sourceProductionContext;
    private readonly string _className;
    private readonly string _nameSpace;
    private List<XMLMethodGenerateArgs> _generateArgs = new();
    public GenerateXMLMethodsGenerator(SourceProductionContext sourceProductionContext,
                                       string className,
                                       string nameSpace)
    {
        _sourceProductionContext = sourceProductionContext;
        _className = className;
        _nameSpace = nameSpace;
    }

    public void GenerateEnvelopeXmlGenerator(List<INamedTypeSymbol> requestEnvelopeTypes)
    {
        foreach (var envelopeType in requestEnvelopeTypes)
        {
            GenerateEnvelopeXmlGenerator(envelopeType);
        }
        CreateSerializeXmlMethods();
        _generateArgs = new();
    }

    private void CreateSerializeXmlMethods()
    {
        foreach (var _generateArg in _generateArgs)
        {
            GenerateSerlizeMethod(_generateArg);
        }
    }


    private void GenerateEnvelopeXmlGenerator(INamedTypeSymbol envelopeType, bool isList = false)
    {
        string typeFullName = envelopeType.OriginalDefinition.ToString();


        IEnumerable<ISymbol> RequestEnvelopeProps = envelopeType.GetAllPropertiesAndFields();
        XMLMethodGenerateArgs xMLMethodGenerateArgs = new XMLMethodGenerateArgs();
        xMLMethodGenerateArgs.FullName = envelopeType.OriginalDefinition.ToString();
        var rootXmlTag = envelopeType.GetXmlRootFromClassSymbol();
        xMLMethodGenerateArgs.Name = envelopeType.Name;
        xMLMethodGenerateArgs.XmlTag = rootXmlTag ?? xMLMethodGenerateArgs.Name.ToUpper();
        int sortOrder = 0;
        foreach (var envelopeProp in RequestEnvelopeProps)
        {
            if (envelopeProp is IPropertySymbol propertySymbol)
            {
                bool IsSpecialType = propertySymbol.Type.SpecialType != SpecialType.None;
                if (IsSpecialType)
                {
                    XMlProperties? xMlProperties = propertySymbol.GetXmlProperties();
                    string? xmlTag = xMlProperties?.XMLTag;
                    string name = propertySymbol.Name;
                    xMLMethodGenerateArgs.XmlFields.Add(new(name, xmlTag, ++sortOrder, xMlProperties?.IsAttribute ?? false));
                }
                else
                {
                    if (propertySymbol.Type is INamedTypeSymbol namedTypeSymbol &&
                    namedTypeSymbol.IsGenericType)
                    {
                        if (namedTypeSymbol.GetClassMetaName() == "System.Collections.Generic.List")
                        {
                            INamedTypeSymbol? typeSymbol = (INamedTypeSymbol?)namedTypeSymbol.TypeArguments.FirstOrDefault();
                            if (typeSymbol != null)
                            {
                                GenerateEnvelopeXmlGenerator((INamedTypeSymbol)typeSymbol, true);
                                string? xmlTag = propertySymbol.GetXmlTagFromPropertyAttributes();
                                string name = propertySymbol.Name;
                                xMLMethodGenerateArgs.XmlFields.Add(new(name, xmlTag, ++sortOrder, false, typeSymbol.Name, typeSymbol.OriginalDefinition.ToString(), true));
                            }
                        }
                    }
                    else
                    {
                        GenerateEnvelopeXmlGenerator((INamedTypeSymbol)propertySymbol.Type);
                        string? xmlTag = propertySymbol.GetXmlTagFromPropertyAttributes();
                        string name = propertySymbol.Name;
                        xMLMethodGenerateArgs.XmlFields.Add(new(name, xmlTag, ++sortOrder, false, propertySymbol.Type.Name, propertySymbol.Type.OriginalDefinition.ToString()));
                    }
                }
            }
        }

        _generateArgs.Add(xMLMethodGenerateArgs);
    }

    private void GenerateSerlizeMethod(XMLMethodGenerateArgs generateArg)
    {
        var methodDeclarationSyntax = GetMemberDeclarationSyntax(generateArg);

        CompilationUnitSyntax compilationUnitSyntax = CompilationUnit()
            .WithMembers(SingletonList<MemberDeclarationSyntax>(FileScopedNamespaceDeclaration(IdentifierName(_nameSpace))
            .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                   true))),
                  SyntaxKind.NamespaceKeyword,
                  TriviaList()))
            .WithMembers(List<MemberDeclarationSyntax>(
                new MemberDeclarationSyntax[]
                {
                        ClassDeclaration(_className)
                        .WithModifiers(TokenList(new[]{Token(SyntaxKind.PublicKeyword),Token(SyntaxKind.PartialKeyword)}))
                        .WithMembers(List<MemberDeclarationSyntax>(methodDeclarationSyntax))
                }))
            )
            ).NormalizeWhitespace();
        string source = compilationUnitSyntax.ToFullString();
        _sourceProductionContext.AddSource($"{_className}.{generateArg.Name}.g.cs", source);
    }

    private List<MethodDeclarationSyntax> GetMemberDeclarationSyntax(XMLMethodGenerateArgs generateArg)
    {
        string name = $"{generateArg.Name.Substring(0, 1).ToUpper()}{generateArg.Name.Substring(1, generateArg.Name.Length - 1)}";
        string AsyncMethodName = $"Get{name}XMLAsync";
        string MethodName = $"Get{name}XML";

        List<MethodDeclarationSyntax> methodDeclarationSyntaxes = new();
        IEnumerable<StatementSyntax> collectionAsync = CreateExpressionForItem(generateArg, true);
        IEnumerable<StatementSyntax> collection = CreateExpressionForItem(generateArg);

        List<StatementSyntax> asyncStatements = new();
        List<StatementSyntax> Statements = new();

        asyncStatements.AddRange(new StatementSyntax[]
        {
            LocalDeclarationStatement(VariableDeclaration(IdentifierName("global::System.IO.StringWriter"))
                      .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("stringWriter"))
                      .WithInitializer(EqualsValueClause(ImplicitObjectCreationExpression()

                  )))))
                  .WithUsingKeyword(Token(SyntaxKind.UsingKeyword)),
                  GetXmlSettingsDeclaration(true),
                  LocalDeclarationStatement(
                      VariableDeclaration(
                                      IdentifierName("global::System.Xml.XmlWriter"))
                                  .WithVariables(
                                      SingletonSeparatedList(
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
                                      AddConfigureAwait(
                                             InvocationExpression( MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              IdentifierName("writer"),
                                              IdentifierName("WriteStartDocumentAsync"))))
                                      ))
        });
        Statements.AddRange(new StatementSyntax[]
        {
            LocalDeclarationStatement(VariableDeclaration(IdentifierName("global::System.IO.StringWriter"))
                      .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("stringWriter"))
                      .WithInitializer(EqualsValueClause(ImplicitObjectCreationExpression()

                  )))))
                  .WithUsingKeyword(Token(SyntaxKind.UsingKeyword)),
                  GetXmlSettingsDeclaration(),
                  LocalDeclarationStatement(
                      VariableDeclaration(
                                      IdentifierName("global::System.Xml.XmlWriter"))
                                  .WithVariables(
                                      SingletonSeparatedList(
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
                                      InvocationExpression(
                                          MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              IdentifierName("writer"),
                                              IdentifierName("WriteStartDocument"))))
        });

        asyncStatements.Add(CreateStartElement(generateArg.XmlTag, true));
        Statements.Add(CreateStartElement(generateArg.XmlTag));
        asyncStatements.Add(ExpressionStatement(
                                AwaitExpression(
                                     AddConfigureAwait(
                                    InvocationExpression(
                                        IdentifierName(AsyncMethodName))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Argument(
                                                        IdentifierName("writer")),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(
                                                        IdentifierName("value"))})))))));
        Statements.Add(ExpressionStatement(
                                    InvocationExpression(
                                        IdentifierName(MethodName))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Argument(
                                                        IdentifierName("writer")),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(
                                                        IdentifierName("value"))})))));

        asyncStatements.Add(CreateEndElement(true));
        Statements.Add(CreateEndElement());
        asyncStatements.AddRange(new StatementSyntax[]
        {
            ExpressionStatement(
                                  AwaitExpression(
                                     AddConfigureAwait( InvocationExpression(
                                          MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              IdentifierName("writer"),
                                              IdentifierName("WriteEndDocumentAsync")))))),
                              ExpressionStatement(
                                  AwaitExpression(
                                    AddConfigureAwait(  InvocationExpression(
                                          MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              IdentifierName("writer"),
                                              IdentifierName("FlushAsync")))))),
                              ReturnStatement(
                                  InvocationExpression(
                                      MemberAccessExpression(
                                          SyntaxKind.SimpleMemberAccessExpression,
                                          IdentifierName("stringWriter"),
                                          IdentifierName("ToString"))))
        });
        Statements.AddRange(new StatementSyntax[]
        {
            ExpressionStatement(
                                      InvocationExpression(
                                          MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              IdentifierName("writer"),
                                              IdentifierName("WriteEndDocument")))),
                              ExpressionStatement(
                                      InvocationExpression(
                                          MemberAccessExpression(
                                              SyntaxKind.SimpleMemberAccessExpression,
                                              IdentifierName("writer"),
                                              IdentifierName("Flush")))),
                              ReturnStatement(
                                  InvocationExpression(
                                      MemberAccessExpression(
                                          SyntaxKind.SimpleMemberAccessExpression,
                                          IdentifierName("stringWriter"),
                                          IdentifierName("ToString"))))
        });

        MethodDeclarationSyntax FullSerializeMethodAsync = MethodDeclaration(GenericName("global::System.Threading.Tasks.Task")
                              .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)PredefinedType(Token(SyntaxKind.StringKeyword))))),
                            Identifier(AsyncMethodName))
              .WithModifiers(TokenList(
                  new[]
                  {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.AsyncKeyword),
                  }))
              .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
              {
                Parameter(Identifier("value")).WithType(IdentifierName($"global::{generateArg.FullName}"))
              })))
              .WithBody(

            Block(asyncStatements));

        MethodDeclarationSyntax FullSerializeMethod = MethodDeclaration((TypeSyntax)PredefinedType(Token(SyntaxKind.StringKeyword)),
                            Identifier(MethodName))
              .WithModifiers(TokenList(
                  new[]
                  {
                    Token(SyntaxKind.PublicKeyword)
                  }))
              .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
              {
                Parameter(Identifier("value")).WithType(IdentifierName($"global::{generateArg.FullName}"))
              })))
              .WithBody(

            Block(Statements));



        MethodDeclarationSyntax ChildSerializeMethodAsync = MethodDeclaration(IdentifierName("global::System.Threading.Tasks.Task"),
                            Identifier(AsyncMethodName))
              .WithModifiers(TokenList(
                  new[]
                  {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.AsyncKeyword),
                  }))
              .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
              {
                Parameter(Identifier("writer")).WithType(IdentifierName($"global::System.Xml.XmlWriter")),
                 Token(SyntaxKind.CommaToken),
                Parameter(Identifier("value")).WithType(IdentifierName($"global::{generateArg.FullName}"))
              })))
              .WithBody(
            Block(collectionAsync));

        MethodDeclarationSyntax ChildSerializeMethod = MethodDeclaration(IdentifierName("void"),
                            Identifier(MethodName))
              .WithModifiers(TokenList(
                  new[]
                  {
                    Token(SyntaxKind.PublicKeyword),
                  }))
              .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
              {
                Parameter(Identifier("writer")).WithType(IdentifierName($"global::System.Xml.XmlWriter")),
                 Token(SyntaxKind.CommaToken),
                Parameter(Identifier("value")).WithType(IdentifierName($"global::{generateArg.FullName}"))
              })))
              .WithBody(
            Block(collection));

        MethodDeclarationSyntax IEnumerableSerializeMethodAsync = MethodDeclaration(IdentifierName("global::System.Threading.Tasks.Task")
            , Identifier(AsyncMethodName))
             .WithModifiers(TokenList(
                  new[]
                  {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.AsyncKeyword),
                  }))
            .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
              {
                Parameter(Identifier("writer")).WithType(IdentifierName($"global::System.Xml.XmlWriter")),
                 Token(SyntaxKind.CommaToken),
                Parameter(Identifier("values")).WithType(IdentifierName($"global::System.Collections.Generic.IEnumerable<{generateArg.FullName}>"))
              }))).WithBody(
            Block(IfStatement(
                                BinaryExpression(
                                    SyntaxKind.EqualsExpression,
                                    IdentifierName("values"),
                                    LiteralExpression(
                                        SyntaxKind.NullLiteralExpression)),
                                Block(ReturnStatement())),
                ForEachStatement(
                                IdentifierName(
                                    Identifier(
                                        TriviaList(),
                                        SyntaxKind.VarKeyword,
                                        "var",
                                        "var",
                                        TriviaList())),
                                Identifier("value"),
                                IdentifierName("values"),
                                Block(
                                    CreateStartElement(generateArg.XmlTag, true),
                                    ExpressionStatement(
                                AwaitExpression(
                                  AddConfigureAwait(InvocationExpression(
                                        IdentifierName(AsyncMethodName))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Argument(
                                                        IdentifierName("writer")),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(
                                                        IdentifierName("value"))})))))),
                                    CreateEndElement(true)))));

        MethodDeclarationSyntax IEnumerableSerializeMethod = MethodDeclaration(IdentifierName("void")
            , Identifier(MethodName))
             .WithModifiers(TokenList(
                  new[]
                  {
                    Token(SyntaxKind.PublicKeyword),
                  }))
            .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
              {
                Parameter(Identifier("writer")).WithType(IdentifierName($"global::System.Xml.XmlWriter")),
                 Token(SyntaxKind.CommaToken),
                Parameter(Identifier("values")).WithType(IdentifierName($"global::System.Collections.Generic.IEnumerable<{generateArg.FullName}>"))
              }))).WithBody(
            Block(IfStatement(
                                BinaryExpression(
                                    SyntaxKind.EqualsExpression,
                                    IdentifierName("values"),
                                    LiteralExpression(
                                        SyntaxKind.NullLiteralExpression)),
                                Block(ReturnStatement())),
                ForEachStatement(
                                IdentifierName(
                                    Identifier(
                                        TriviaList(),
                                        SyntaxKind.VarKeyword,
                                        "var",
                                        "var",
                                        TriviaList())),
                                Identifier("value"),
                                IdentifierName("values"),
                                Block(
                                    CreateStartElement(generateArg.XmlTag),
                                    ExpressionStatement(
                                    InvocationExpression(
                                        IdentifierName(MethodName))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Argument(
                                                        IdentifierName("writer")),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(
                                                        IdentifierName("value"))})))),
                                    CreateEndElement()))));

        methodDeclarationSyntaxes.Add(FullSerializeMethodAsync);
        methodDeclarationSyntaxes.Add(FullSerializeMethod);
        methodDeclarationSyntaxes.Add(ChildSerializeMethodAsync);
        methodDeclarationSyntaxes.Add(ChildSerializeMethod);
        methodDeclarationSyntaxes.Add(IEnumerableSerializeMethodAsync);
        methodDeclarationSyntaxes.Add(IEnumerableSerializeMethod);
        return methodDeclarationSyntaxes;
    }

    private static LocalDeclarationStatementSyntax GetXmlSettingsDeclaration(bool isAsync = false)
    {
        return LocalDeclarationStatement(
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
                                                                        isAsync ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)),
                                                                Token(SyntaxKind.CommaToken),
                                                                AssignmentExpression(
                                                                    SyntaxKind.SimpleAssignmentExpression,
                                                                    IdentifierName("OmitXmlDeclaration"),
                                                                    LiteralExpression(SyntaxKind.TrueLiteralExpression)),
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
                                                                        SyntaxKind.FalseLiteralExpression))}))))))));
    }

    private IEnumerable<StatementSyntax> CreateExpressionForItem(XMLMethodGenerateArgs generateArg, bool isAsync = false)
    {
        List<StatementSyntax> expressionStatements = new();
        expressionStatements.Add(IfStatement(
                                BinaryExpression(
                                    SyntaxKind.EqualsExpression,
                                    IdentifierName("value"),
                                    LiteralExpression(
                                        SyntaxKind.NullLiteralExpression)),
                                Block(ReturnStatement())));
        foreach (var xmlField in generateArg.XmlFields)
        {
            if (!xmlField.IsMethod)
            {
                expressionStatements.Add(CreateElement(xmlField, isAsync));

            }
            else
            {
                string name = xmlField.TypeName;
                string MethodName = $"Get{name.Substring(0, 1).ToUpper()}{name.Substring(1, name.Length - 1)}XML{(isAsync ? "Async" : "")}";
                if (!xmlField.IsList)
                {
                    expressionStatements.Add(CreateStartElement(xmlField.XmlTag, isAsync && !xmlField.IsAttribute, xmlField.IsAttribute));
                }

                InvocationExpressionSyntax methodExpression = InvocationExpression(
                                                        IdentifierName(MethodName))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SeparatedList<ArgumentSyntax>(
                                                                new SyntaxNodeOrToken[]{
                                                    Argument(
                                                        IdentifierName("writer")),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("value"),
                                                            IdentifierName(xmlField.Name)))})));
                expressionStatements.Add(ExpressionStatement(
                                isAsync ? AwaitExpression(
                                    AddConfigureAwait(methodExpression)) : methodExpression));
                if (!xmlField.IsList)
                {
                    expressionStatements.Add(CreateEndElement(isAsync && !xmlField.IsAttribute, xmlField.IsAttribute));
                }
            }
        }
        return expressionStatements;
    }

    private StatementSyntax CreateElement(XmlField xmlField, bool isAsync = false)
    {
        string Name = xmlField.IsAttribute ? $"WriteAttributeString{(isAsync ? "Async" : "")}" : $"WriteElementString{(isAsync ? "Async" : "")}";
        MemberAccessExpressionSyntax memberExpression = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("value"), IdentifierName(xmlField.Name));
        InvocationExpressionSyntax expression = InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("writer"),
                                                        IdentifierName(Name)))
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
                                                        Literal(xmlField.XmlTag))),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(
                                                    LiteralExpression(
                                                        SyntaxKind.NullLiteralExpression)),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(
                                               MemberAccessExpression( SyntaxKind.SimpleMemberAccessExpression,
                                               memberExpression,IdentifierName("ToString()")))
                                                            })));
        IfStatementSyntax ifStatementSyntax = IfStatement(BinaryExpression(
                                    SyntaxKind.NotEqualsExpression,
                                    memberExpression,
                                    LiteralExpression(
                                        SyntaxKind.NullLiteralExpression)),
                                       Block(isAsync ? ExpressionStatement(AwaitExpression(expression)) : ExpressionStatement(expression)));
        return ifStatementSyntax;
        //return isAsync ? ExpressionStatement(AwaitExpression(expression)) : ExpressionStatement(expression);
    }

    private StatementSyntax CreateValueElement(string name, bool isAsync = false, bool isAttribute = false)
    {
        string Name = isAttribute ? $"WriteAttributeString{(isAsync ? "Async" : "")}" : $"WriteValue{(isAsync ? "Async" : "")}";
        InvocationExpressionSyntax expression = InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("writer"),
                                                        IdentifierName(Name)))
                                                .WithArgumentList(
                                                    ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                                    {
                                                Argument(MemberAccessExpression( SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("value"),
                                                            IdentifierName(name)))

                                                    })));
        return isAsync ? ExpressionStatement(AwaitExpression(expression)) : ExpressionStatement(expression);
    }

    public static ExpressionStatementSyntax CreateStartElement(string xmlTag, bool isAsync = false, bool isAttribute = false)
    {
        string Name = isAttribute ? $"WriteStartAttribute{(isAsync ? "Async" : "")}" : $"WriteStartElement{(isAsync ? "Async" : "")}";
        InvocationExpressionSyntax expression = InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("writer"),
                                                        IdentifierName(Name)))
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
                                                        SyntaxKind.NullLiteralExpression))})));

        return isAsync ? ExpressionStatement(AwaitExpression(AddConfigureAwait(expression))) : ExpressionStatement(expression);
    }

    private static InvocationExpressionSyntax AddConfigureAwait(InvocationExpressionSyntax expression)
    {
        return InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, IdentifierName("ConfigureAwait")))
                        .WithArgumentList(ArgumentList(
                            SingletonSeparatedList<ArgumentSyntax>(Argument(LiteralExpression(SyntaxKind.FalseLiteralExpression)))));
    }

    public static ExpressionStatementSyntax CreateEndElement(bool isAsync = false, bool isAttribute = false)
    {
        string Name = isAttribute ? $"WriteEndAttribute{(isAsync ? "Async" : "")}" : $"WriteEndElement{(isAsync ? "Async" : "")}";
        InvocationExpressionSyntax expression = InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("writer"),
                                                        IdentifierName(Name)));
        return isAsync ? ExpressionStatement(AwaitExpression(AddConfigureAwait(expression))) : ExpressionStatement(expression);
    }
}

public class XMLMethodGenerateArgs
{
    public XMLMethodGenerateArgs()
    {
    }

    public string FullName { get; set; }

    public string Name { get; set; }
    public string XmlTag { get; set; }

    public List<XmlField> XmlFields { get; set; } = new();
}
public interface IXmlField
{
    string Name { get; set; }

    string XmlTag { get; set; }

    string TypeName { get; set; }

    int SortOrder { get; set; }
}
public class XmlField : IXmlField
{
    public XmlField(string name, string? xmlTag, int sortOrder, bool isAttribute)
    {
        Name = name;
        XmlTag = xmlTag ?? name.ToUpper();
        SortOrder = sortOrder;
        IsAttribute = isAttribute;
    }
    public XmlField(string name, string? xmlTag, int sortOrder, bool isAttribute, string typeName, string typeFullName, bool isList = false)
    {
        Name = name;
        TypeName = typeName;
        XmlTag = xmlTag ?? name.ToUpper();
        SortOrder = sortOrder;
        IsAttribute = isAttribute;
        TypeName = typeName;
        TypeFullName = typeFullName;
        IsList = isList;
        IsMethod = true;
    }

    public string Name { get; set; }
    public string XmlTag { get; set; }
    public string TypeName { get; set; }
    public string TypeFullName { get; set; }
    public bool IsList { get; set; }
    public int SortOrder { get; set; }
    public bool IsAttribute { get; }
    public bool IsMethod { get; set; }
}

