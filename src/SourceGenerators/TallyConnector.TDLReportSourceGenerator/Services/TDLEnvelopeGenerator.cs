using Microsoft.CodeAnalysis.CSharp.Syntax;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public class TDLEnvelopeGenerator
{
    private readonly ClassData _modelData;
    private readonly SourceProductionContext context;
    private readonly CancellationToken token;
    SyntaxToken[] modifiers;
    public TDLEnvelopeGenerator(ClassData modelData, SourceProductionContext context, CancellationToken token)
    {
        this._modelData = modelData;
        this.context = context;
        this.token = token;
        if(_modelData.IsBaseIRequestableObject)
        {
            modifiers =
            [
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.NewKeyword),
                Token(SyntaxKind.StaticKeyword)
            ];
        }
        else
        {
            modifiers =
            [
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.StaticKeyword)
            ];
        }
        
    }
    public void Generate()
    {
        List<UsingDirectiveSyntax> usings = [
            UsingDirective(IdentifierName(ExtensionsNameSpace)),
            UsingDirective(IdentifierName(Constants.Models.Abstractions.PREFIX))
            ];
        ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(_modelData.Name)
          .WithModifiers(TokenList([Token(
                            TriviaList(
                                Comment($@"/*
* Generated based on {_modelData.FullName}
*/")),
                            SyntaxKind.PartialKeyword,
                            TriviaList())]));
        var unit = CompilationUnit()
          .WithUsings(List(usings))
          .WithMembers(List(new MemberDeclarationSyntax[]
          {
                FileScopedNamespaceDeclaration(IdentifierName(_modelData.Namespace))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    classDeclarationSyntax
                    .WithMembers(List(GetClassMembers()))
                     .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(GetGlobalNameforType(Constants.Models.Interfaces.TallyRequestableObjectInterfaceFullName)))))
    }))
          })).NormalizeWhitespace().ToFullString();
        context.AddSource($"Envelope.{_modelData.Name}_{_modelData.Namespace}.g.cs", unit);
    }

    private IEnumerable<MemberDeclarationSyntax> GetClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        members.Add(GetReqEnvelopeMethod());
        members.Add(GenerateGetXmlAttributeOverridesMethodSyntax());
        return members;
    }

    private MemberDeclarationSyntax GetReqEnvelopeMethod()
    {

        const string envelopeVariableName = "reqEnvelope";
        const string tdlMsgVariableName = "tdlMsg";

        List<StatementSyntax> statements = [];

        // Declaring Variable for RequestEnvelope
        statements.Add(CreateVarInsideMethodWithExpression(envelopeVariableName, ObjectCreationExpression(GetGlobalNameforType(RequestEnvelopeFullTypeName))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
            {
                Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,GetGlobalNameforType(HeaderTypeEnumName),IdentifierName("Data"))),
                Token(SyntaxKind.CommaToken),
                Argument(IdentifierName(Meta.IdentifierPropPath))
            })))));
        // ctreate var for TDLMessage
        statements.Add(CreateVarInsideMethodWithExpression(tdlMsgVariableName,
                                                           MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                  IdentifierName(envelopeVariableName),
                                                                                  IdentifierName("Body.Desc.TDL.TDLMessage"))));

        statements.Add(CreateReportAndFormAsssignStatement(tdlMsgVariableName, "Report"));
        statements.Add(CreateReportAndFormAsssignStatement(tdlMsgVariableName, "Form"));

        statements.Add(CreateAssignFromPropertyStatement(tdlMsgVariableName, "Part", [Meta.AllPartsPropPath], [Meta.TDLDefaultPartPropPath]));

        statements.Add(CreateAssignFromPropertyStatement(tdlMsgVariableName, "Line", [Meta.AllLinesPropPath], [Meta.TDLDefaultLinePropPath]));

        statements.Add(CreateAssignFromPropertyStatement(tdlMsgVariableName, "Field", [Meta.FieldsPropPath]));

        if (!(_modelData.TDLCollectionData?.Exclude ?? false))
        {
            statements.Add(CreateAssignFromPropertyStatement(tdlMsgVariableName, "Collection", [], [Meta.DefaultCollectionPropPath]));

        }
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Functions", [], [.. _modelData.DefaultTDLFunctions]));


        statements.Add(CreateAssignFromPropertyStatement(tdlMsgVariableName, "NameSet", [Meta.NameSetsPropPath]));
        statements.Add(ReturnStatement(IdentifierName(envelopeVariableName)));


        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(RequestEnvelopeFullTypeName),
                                                        Identifier(string.Format(GetRequestEnvelopeMethodName, "")))
            .WithModifiers(TokenList(modifiers))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;
    }

    private ExpressionStatementSyntax CreateReportAndFormAsssignStatement(string tdlMsgVariableName,
                                                                         string name)
    {
        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                               IdentifierName(tdlMsgVariableName),
                                                                               IdentifierName(name)),
                                                        CollectionExpression(SeparatedList<CollectionElementSyntax>(new SyntaxNodeOrToken[]
                                                        {
                                                            ExpressionElement(ImplicitObjectCreationExpression()
                                                            .WithArgumentList(   ArgumentList(SeparatedList<ArgumentSyntax>( new SyntaxNodeOrToken[]
                                                            {
                                                                 Argument(IdentifierName($"Meta.{Meta.IdentifierNameVarName}")),
                                                            }))))
                                                        }))));
    }


    private StatementSyntax CreateAssignFromPropertyStatement(string varName,
                                                            string propName,
                                                            List<string> propNames,
                                                            List<string>? singleReturnMethodNames = null)
    {
        List<SyntaxNodeOrToken> nodes = [];
        if (singleReturnMethodNames != null)
        {
            foreach (var methodName in singleReturnMethodNames)
            {
                Utils.SafeAdd(nodes, ExpressionElement(IdentifierName(methodName)));
            }
        }
        for (int i = 0; i < propNames.Count; i++)
        {
            var methodName = propNames[i];
            Utils.SafeAdd(nodes, SpreadElement((IdentifierName(methodName))));
        }

        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(varName),
                                    IdentifierName(propName)),
                                    CollectionExpression(SeparatedList<CollectionElementSyntax>(nodes))));
    }

    private StatementSyntax CreateAssignFromMethodStatement(string varName,
                                                            string propName,
                                                            List<string> propNames,
                                                            List<string>? singleReturnMethodNames = null)
    {
        List<SyntaxNodeOrToken> nodes = [];
        if (singleReturnMethodNames != null)
        {
            foreach (var methodName in singleReturnMethodNames)
            {
                Utils.SafeAdd(nodes, ExpressionElement(InvocationExpression(IdentifierName(methodName))));
            }
        }
        for (int i = 0; i < propNames.Count; i++)
        {
            var methodName = propNames[i];
            Utils.SafeAdd(nodes, SpreadElement(InvocationExpression(IdentifierName(methodName))));
        }

        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(varName),
                                    IdentifierName(propName)),
                                    CollectionExpression(SeparatedList<CollectionElementSyntax>(nodes))));
    }


    private MemberDeclarationSyntax GenerateGetXmlAttributeOverridesMethodSyntax()
    {
        var xmlAttributeOverridesVarName = "xmlAttributeOverrides";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarInsideMethodWithExpression(xmlAttributeOverridesVarName, ObjectCreationExpression(GetGlobalNameforType(XmlAttributeOverridesClassName)).WithArgumentList(ArgumentList())));


        HashSet<string> typeNames = [];
        const string _varAttrs = "XmlAttributes";
        statements.Add(CreateVarInsideMethodWithExpression(_varAttrs, ObjectCreationExpression(GetGlobalNameforType(XmlAttributesClassName)).WithArgumentList(ArgumentList())));
        statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(_varAttrs), IdentifierName("XmlElements.Add")))
            .WithArgumentList(ArgumentList(
                            SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                            {
                                Argument(CreateImplicitObjectExpression([Argument( IdentifierName("Meta.XMLTag"))]))
                            })))));

        statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                       IdentifierName(xmlAttributeOverridesVarName),
                                                                                       IdentifierName("Add")))
            .WithArgumentList(ArgumentList(
                                SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                {
                                         Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                         GenericName(Identifier(Constants.Models.Response.ReportResponseEnvelopeClassName))
                                         .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(GetGlobalNameforType( _modelData.FullName)))),
                                         IdentifierName("TypeInfo"))),
                                          Token(SyntaxKind.CommaToken),
                                         Argument(CreateStringLiteral("Objects")),
                                          Token(SyntaxKind.CommaToken),
                                         Argument(IdentifierName(_varAttrs)
                                         ),
                                })))));



        statements.Add(ReturnStatement(IdentifierName(xmlAttributeOverridesVarName)));
        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(XmlAttributeOverridesClassName),
                                                       Identifier(string.Format(GetXMLAttributeOveridesMethodName, "")))
           .WithModifiers(TokenList(modifiers))
           .WithParameterList(ParameterList())
           .WithBody(Block(statements));
        return methodDeclarationSyntax;
    }
}
