using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Reflection;
using System.Xml.Linq;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public class TDLReportGenerator
{
    private readonly ModelData _modelData;
    private ImmutableList<PropertyData> _allProperties;
    private ImmutableList<PropertyData> _simpleProperties;
    private ImmutableList<PropertyData> _complexProperties;
    public TDLReportGenerator(ModelData modelData)
    {
        _modelData = modelData;
        _allProperties = [.. _modelData.Properties.Values];
        _simpleProperties = [.. _allProperties.Where(c => !c.IsComplex)];
        _complexProperties = [.. _allProperties.Where(c => c.IsComplex)];

    }

    public string Generate(CancellationToken token)
    {
        var unit = CompilationUnit()
          .WithUsings(List([UsingDirective(IdentifierName(ExtensionsNameSpace))]))
          .WithMembers(List(new MemberDeclarationSyntax[]
          {
                FileScopedNamespaceDeclaration(IdentifierName(_modelData.Namespace))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    ClassDeclaration(_modelData.Name)
                    .WithModifiers(TokenList([Token(
                            TriviaList(
                                Comment($@"/*
* Generated based on {_modelData.FullName}
*/")),
                            SyntaxKind.PartialKeyword,
                            TriviaList())]))
                    .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(GetGlobalNameforType(Constants.Models.Interfaces.TallyRequestableObjectInterfaceFullName)))))
                    .WithMembers(List(GetClassMembers()))
                }))
          })).NormalizeWhitespace().ToFullString();

        return unit;
    }

    private IEnumerable<MemberDeclarationSyntax> GetClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        foreach (var property in _modelData.Properties.Values)
        {
            members.Add(CreateConstStringVar(GetFieldNameVariableName(property), property.FieldName, true));
        }
        members.Add(CreatePublicConstIntVar(SimpleFieldsCountFieldName, _modelData.SimplePropertiesCount));

        members.Add(CreateGetRequestEnvelopeMethod());

        members.Add(CreateGetFieldsMethod());
        
        return members;
    }

    private MemberDeclarationSyntax CreateGetRequestEnvelopeMethod()
    {
        const string FieldsVariableName = "_fields";
        List<StatementSyntax> statements = [];
        statements.Add(ReturnStatement(ImplicitObjectCreationExpression()));
        var methodDeclarationSyntax = MethodDeclaration(QualifiedName(GetGlobalNameforType("System.Threading.Tasks"), GenericName( "Task").WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(GetGlobalNameforType(RequestEnvelopeFullTypeName)))) ),
                                                        Identifier(string.Format(GetRequestEnvelopeMethodName, "")))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword),Token(SyntaxKind.AsyncKeyword)]))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;
    }

    private static string GetFieldNameVariableName(PropertyData property) => $"{property.Name}_FieldName";

    private MemberDeclarationSyntax CreateGetFieldsMethod()
    {
        const string FieldsVariableName = "_fields";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(FieldFullTypeName, FieldsVariableName, SimpleFieldsCountFieldName));
        int counter = 0;

        if (_modelData.BaseData != null && _modelData.BaseData.IsTallyRequestableObject)
        {
            string name = _modelData.BaseData.Name;
            //var suffix = Utils.GenerateUniqueNameSuffix($"{name}\0{FieldsVariableName}");
            string ComplexFieldsVariableName = $"{name.Substring(0, 1).ToLower()}{name.Substring(1)}Fields";
            statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                       InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                   GetGlobalNameforType(_modelData.BaseData.FullName),
                                                                                                   IdentifierName(string.Format(GetTDLFieldsMethodName, ""))))));

            statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                               IdentifierName(FieldsVariableName),
                                                                                               IdentifierName(AddToArrayExtensionMethodName)))
                    .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                         new SyntaxNodeOrToken[]{
                            Argument(IdentifierName(ComplexFieldsVariableName)),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(counter))),
                         })))
                    ));
            counter += _modelData.BaseData.ModelData?.SimplePropertiesCount ?? 0;
        }

        foreach (var property in _simpleProperties)
        {
            string name = GetFieldNameVariableName(property);
            List<SyntaxNodeOrToken> constructerArgs = [Argument(IdentifierName(name))];
            var expressionStatementSyntax = GetArrayAssignmentExppressionImplicit(FieldsVariableName, counter, constructerArgs);
            statements.Add(expressionStatementSyntax);
            counter++;
        }


        statements.Add(ReturnStatement(IdentifierName(FieldsVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(FieldFullTypeName),
                                                        Identifier(string.Format(GetTDLFieldsMethodName, "")))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;

    }

    private static ExpressionStatementSyntax GetArrayAssignmentExppressionImplicit(string FieldsVariableName,
                                                                           int index,
                                                                           List<SyntaxNodeOrToken> constructorArgs,
                                                                           List<SyntaxNodeOrToken>? intializerArgs = null)
    {
        ExpressionSyntax implicitObjectCreationExpressionSyntax = CreateImplicitObjectExpression(constructorArgs, intializerArgs);
        return CreateArrayAssignmentwithExpression(FieldsVariableName, index, implicitObjectCreationExpressionSyntax);
    }
    private static ExpressionStatementSyntax CreateArrayAssignmentwithExpression(string FieldsVariableName, int index, ExpressionSyntax expression)
    {
        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                                 ElementAccessExpression(IdentifierName(FieldsVariableName))
                                                                 .WithArgumentList(BracketedArgumentList(
                                                                     SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                                                                                                       Literal(index)))))),
                                                                 expression
                                                                 // ObjectCreationExpression(GetGlobalNameforType(FieldFullTypeName))
                                                                 ));
    }

    private static ImplicitObjectCreationExpressionSyntax CreateImplicitObjectExpression(List<SyntaxNodeOrToken> constructorArgs, List<SyntaxNodeOrToken>? intializerArgs)
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


    internal LocalDeclarationStatementSyntax CreateVarArrayWithCount(string typeName, string varName, string countVariableName)
    {

        return CreateVarInsideMethodWithExpression(varName, ArrayCreationExpression(CreateArrayTypewithCount(typeName, countVariableName)));
    }
    internal ArrayTypeSyntax CreateArrayTypewithCount(string typeName, string countVariableName)
    {
        var arrayTypeSyntax = ArrayType(GetGlobalNameforType(typeName))
            .WithRankSpecifiers(List([ArrayRankSpecifier(SingletonSeparatedList((ExpressionSyntax)IdentifierName(countVariableName)))]));
        return arrayTypeSyntax;
    }

    internal static ArrayTypeSyntax CreateEmptyArrayType(string typeName)
    {
        TypeSyntax elementType = GetGlobalNameforType(typeName);
        var arrayTypeSyntax = CreateEmptyArrayType(elementType);
        return arrayTypeSyntax;
    }
    internal static ArrayTypeSyntax CreateEmptyArrayType(TypeSyntax elementType)
    {
        return ArrayType(elementType)
                    .WithRankSpecifiers(List([ArrayRankSpecifier(SingletonSeparatedList((ExpressionSyntax)OmittedArraySizeExpression()))]));
    }
    private static FieldDeclarationSyntax CreateConstStringVar(string varName, string varText, bool isInternal = false)
    {
        TypeSyntax strSyntax = PredefinedType(Token(SyntaxKind.StringKeyword));

        VariableDeclarationSyntax variableDeclarationSyntax = CreateVariableDelaration(strSyntax,
                                                                                       varName,
                                                                                       LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(varText)));
        List<SyntaxToken> tokens = [];
        if (isInternal)
        {
            tokens.Add(Token(SyntaxKind.InternalKeyword));
        }
        tokens.Add(Token(SyntaxKind.ConstKeyword));
        return FieldDeclaration(variableDeclarationSyntax)
            .WithModifiers(TokenList(tokens));
    }
    private static FieldDeclarationSyntax CreatePublicConstIntVar(string varName, int value, bool isInternal = false)
    {
        TypeSyntax strSyntax = PredefinedType(Token(SyntaxKind.IntKeyword));

        VariableDeclarationSyntax variableDeclarationSyntax = CreateVariableDelaration(strSyntax,
                                                                                       varName,
                                                                                       LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)));
        List<SyntaxToken> tokens = [Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ConstKeyword)];

        return FieldDeclaration(variableDeclarationSyntax)
            .WithModifiers(TokenList(tokens));
    }
}
