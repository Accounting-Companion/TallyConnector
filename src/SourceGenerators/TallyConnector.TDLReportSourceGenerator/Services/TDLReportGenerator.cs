using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Drawing;
using System.Reflection;
using System.Xml.Linq;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public class TDLReportGenerator
{
    private readonly string _reportVarName;
    private readonly string _collectionVariableName;
    private readonly string _xmlTagVariableName;
    private readonly ModelData _modelData;
    private readonly ImmutableList<PropertyData> _allChildProperties;
    private readonly ImmutableList<PropertyData> _properties;
    private readonly ImmutableList<PropertyData> _simpleProperties;
    private readonly ImmutableList<PropertyData> _allChildSimpleProperties;
    private readonly ImmutableList<PropertyData> _complexProperties;
    private readonly ImmutableList<PropertyData> _allChildComplexProperties;
    private readonly ImmutableList<PropertyData> _allSimpleProperties;
    private readonly ImmutableList<PropertyData> _allComplexProperties;
    private readonly string _uniqueNameSuffix;
    public TDLReportGenerator(ModelData modelData)
    {
        _modelData = modelData;
        _reportVarName = "ReportName";
        _collectionVariableName = "CollectionName";
        _xmlTagVariableName = "XMLTag";

        _properties = [.. _modelData.GetAllDirectProperties()];


        _simpleProperties = [.. _properties.Where(c => !c.IsComplex)];
        _complexProperties = [.. _properties.Where(c => c.IsComplex || c.IsList)];
        _allChildProperties = [.. Utils.GetAllProperties(_complexProperties)];
        _allChildSimpleProperties = [.. _allChildProperties.Where(c => !c.IsComplex)];
        _allChildComplexProperties = [.. _allChildProperties.Where(c => c.IsComplex || c.IsList)];
        _allSimpleProperties = [.. _simpleProperties.Concat(_allChildSimpleProperties)];
        _allComplexProperties = [.. _complexProperties.Concat(_allChildComplexProperties)];
        _uniqueNameSuffix = Utils.GenerateUniqueNameSuffix($"{_modelData.Symbol.ContainingAssembly.Name}\0{_modelData.FullName}");

    }



    public string Generate(CancellationToken token)
    {
        ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(_modelData.Name)
                    .WithModifiers(TokenList([Token(
                            TriviaList(
                                Comment($@"/*
* Generated based on {_modelData.FullName}
*/")),
                            SyntaxKind.PartialKeyword,
                            TriviaList())]));

        if (_modelData.IsRequestableObjectInterface)
        {

            classDeclarationSyntax = classDeclarationSyntax
                .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(GetGlobalNameforType(Constants.Models.Interfaces.TallyRequestableObjectInterfaceFullName)))));
        }

        List<UsingDirectiveSyntax> usings = [UsingDirective(IdentifierName(ExtensionsNameSpace))];
        if (_modelData.DefaultTDLFunctions.Count > 0)
        {
            usings.Add(UsingDirective(IdentifierName("TallyConnector.Core.Constants"))
                .WithStaticKeyword(Token(SyntaxKind.StaticKeyword)));
        }
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
                }))
          })).NormalizeWhitespace().ToFullString();

        return unit;
    }

    private IEnumerable<MemberDeclarationSyntax> GetClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        foreach (var property in _allSimpleProperties)
        {
            if (property.IsOveridden)
            {
                continue;
            }
            members.Add(CreateConstStringVar(GetFieldNameVariableName(property), property.FieldName));
        }
        foreach (var property in _allComplexProperties)
        {
            members.Add(CreateConstStringVar(GetPartNameVariableName(property), property.FieldName));
            if (property.XMLData.Count > 0)
            {
                foreach (var xMLData in property.XMLData)
                {
                    members.Add(CreateConstStringVar(GetPartNameVariableName(xMLData), xMLData.FieldName!));
                }
            }
        }

        members.Add(CreateConstStringVar(_reportVarName, $"{_modelData.Name}_{_uniqueNameSuffix}"));
        members.Add(CreateConstStringVar(_collectionVariableName, $"{_modelData.Name}sCollection_{_uniqueNameSuffix}"));
        members.Add(CreateConstStringVar(_xmlTagVariableName, _modelData.XMLTag!));


        members.Add(CreateConstIntVar(SimpleFieldsCountFieldName, _modelData.SimplePropertiesCount));
        members.Add(CreateConstIntVar(ComplexFieldsCountFieldName, _modelData.ComplexPropertiesCount));


        members.Add(CreateGetRequestEnvelopeMethod());
        members.Add(GenerateGetXmlAttributeOverridesMethodSyntax());

        members.AddRange(CreateGetPartsMethod());

        members.AddRange(CreateGetLinesMethods());
        members.Add(CreateGetFieldsMethod());
        members.Add(GenerateGetCollectionsMethodSyntax());
        members.Add(GenerateGetFetchListMethodSyntax());
        if (_modelData.ENumPropertiesCount > 0)
        {
            members.Add(CreateGetNameSetMethod());
        }
        return members;
    }



    private MemberDeclarationSyntax CreateGetRequestEnvelopeMethod()
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
                Argument(IdentifierName(_reportVarName))
            })))));
        // ctreate var for TDLMessage
        statements.Add(CreateVarInsideMethodWithExpression(tdlMsgVariableName,
                                                           MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                  IdentifierName(envelopeVariableName),
                                                                                  IdentifierName("Body.Desc.TDL.TDLMessage"))));


        statements.Add(CreateReportAndFormAsssignStatement(tdlMsgVariableName, "Report"));
        statements.Add(CreateReportAndFormAsssignStatement(tdlMsgVariableName, "Form"));

        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Part", [string.Format(GetTDLPartsMethodName, "")], [string.Format(GetMainTDLPartMethodName, "")]));

        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Line", [string.Format(GetTDLLinesMethodName, "")], [string.Format(GetMainTDLLineMethodName, "")]));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Field", [string.Format(GetTDLFieldsMethodName, "")]));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Collection", [string.Format(GetTDLCollectionsMethodName, "")]));

        if (_modelData.ENumPropertiesCount > 0)
        {
            statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "NameSet", [string.Format(GetTDLNameSetsMethodName, "")]));
        }
        if (_modelData.DefaultTDLFunctions.Count > 0)
        {
            statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Functions", [], _modelData.DefaultTDLFunctions.ToList()));
        }
        statements.Add(ReturnStatement(IdentifierName(envelopeVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(RequestEnvelopeFullTypeName),
                                                        Identifier(string.Format(GetRequestEnvelopeMethodName, "")))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;
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
                                Argument(CreateImplicitObjectExpression([Argument( IdentifierName(_xmlTagVariableName))]))
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

        foreach (var simpleProperty in _allSimpleProperties.Where(c => c.IsOveridden))
        {
            AddExpressionForOverridenChild(simpleProperty);
        }
        void AddExpressionForOverridenChild(PropertyData data)
        {

            statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlAttributeOverridesVarName), IdentifierName("Add")))
            .WithArgumentList(ArgumentList(
                                SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                {
                                         Argument(TypeOfExpression(GetGlobalNameforType(data.ModelData.FullName))),
                                          Token(SyntaxKind.CommaToken),
                                         Argument(CreateStringLiteral(data.Name)),
                                          Token(SyntaxKind.CommaToken),
                                         Argument(ObjectCreationExpression(GetGlobalNameforType(XmlAttributesClassName))
                                         .WithArgumentList(ArgumentList())
                                         .WithInitializer(InitializerExpression(
                                                        SyntaxKind.ObjectInitializerExpression,
                                                        SingletonSeparatedList<ExpressionSyntax>(
                                                            AssignmentExpression(
                                                                SyntaxKind.SimpleAssignmentExpression,
                                                                IdentifierName("XmlIgnore"),
                                                                LiteralExpression(
                                                                    SyntaxKind.TrueLiteralExpression)))))),
                                })))));
        }

        statements.Add(ReturnStatement(IdentifierName(xmlAttributeOverridesVarName)));
        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(XmlAttributeOverridesClassName),
                                                       Identifier(string.Format(GetXMLAttributeOveridesMethodName, "")))
           .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
           .WithParameterList(ParameterList())
           .WithBody(Block(statements));
        return methodDeclarationSyntax;
    }

    private MemberDeclarationSyntax[] CreateGetPartsMethod()
    {
        const string CollectionNameArgName = "collectionName";
        const string partNameArgName = "partName";
        const string XmlTagArgName = "xmlTag";
        const string PartsVariableName = "parts";
        List<StatementSyntax> statements = [];

        if (_modelData.ComplexPropertiesCount == 0)
        {
            statements.Add(ReturnStatement(CollectionExpression()));
        }
        else
        {
            statements.Add(CreateVarArrayWithCountVariable(PartFullTypeName, PartsVariableName, ComplexFieldsCountFieldName));
            int counter = 0;
            foreach (var complexProperty in _allComplexProperties)
            {
                List<SyntaxNodeOrToken> args = [];
                List<SyntaxNodeOrToken>? intializerArgs = null;

                args.SafeAddArgument((IdentifierName(GetPartNameVariableName(complexProperty))));
                args.SafeAddArgument(CreateStringLiteral(complexProperty.TDLCollectionData?.CollectionName ?? complexProperty.Name.ToString()));
                if (complexProperty.ListXMLTag != null)
                {
                    intializerArgs ??= [];
                    intializerArgs.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                                IdentifierName("XMLTag"),
                                                                CreateStringLiteral(complexProperty.ListXMLTag)));
                }
                statements.Add(CreateArrayAssignmentwithExpression(PartsVariableName,
                                                                   counter,
                                                                   CreateImplicitObjectExpression(args, intializerArgs)));

                counter++;
                foreach (var xMLData in complexProperty.XMLData)
                {
                    List<SyntaxNodeOrToken> xmlPartArgs = [];
                    xmlPartArgs.SafeAddArgument((IdentifierName(GetPartNameVariableName(xMLData))));
                    xmlPartArgs.SafeAddArgument(CreateStringLiteral(xMLData.ModelData?.TDLCollectionData?.CollectionName ?? complexProperty.Name.ToString()));
                    statements.Add(CreateArrayAssignmentwithExpression(PartsVariableName,
                                                                   counter,
                                                                   CreateImplicitObjectExpression(xmlPartArgs)));
                    counter++;
                }
            }
            statements.Add(ReturnStatement(IdentifierName(PartsVariableName)));
        }

        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(PartFullTypeName),
                                                       Identifier(string.Format(GetTDLPartsMethodName, "")))
           .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
           .WithBody(
           Block(statements));
        MethodDeclarationSyntax MainPartMethod = GenerateMainPartMethodSyntax(CollectionNameArgName,
                                                                                  partNameArgName,
                                                                                  XmlTagArgName,
                                                                                  PartsVariableName);
        return [MainPartMethod, methodDeclarationSyntax];
    }

    private MemberDeclarationSyntax[] CreateGetLinesMethods()
    {
        const string linesVariableName = "_lines";
        List<StatementSyntax> statements = [];
        if (_modelData.ComplexPropertiesCount == 0)
        {
            statements.Add(ReturnStatement(CollectionExpression()));
        }
        else
        {
            int counter = 0;
            statements.Add(CreateVarArrayWithCountVariable(LineFullTypeName, linesVariableName, ComplexFieldsCountFieldName));

            foreach (var complexProperty in _allComplexProperties)
            {
                List<SyntaxNodeOrToken> args = [];
                List<SyntaxNodeOrToken>? intializerArgs = null;

                args.SafeAddArgument(IdentifierName(GetPartNameVariableName(complexProperty)));
                if (!complexProperty.IsComplex && complexProperty.IsList)
                {
                    args.SafeAddArgument(CollectionExpression(SeparatedList<CollectionElementSyntax>([ExpressionElement(IdentifierName(GetFieldNameVariableName(complexProperty)))])));

                    statements.Add(CreateArrayAssignmentwithExpression(linesVariableName, counter, CreateImplicitObjectExpression(args, intializerArgs)));
                    counter++;
                }
                if (complexProperty.OriginalModelData == null)
                {
                    continue;
                }
                var simplePropertyVarNames = complexProperty.OriginalModelData.GetAllDirectProperties().Where(c => !c.IsComplex && !c.IsList && !c.IsAttribute).Select(c => GetFieldNameVariableName(c)).ToList();
                List<SyntaxNodeOrToken> collectionArgs = [];
                if (simplePropertyVarNames?.Count == 0)
                {
                    collectionArgs.Add(ExpressionElement(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("SimpleField"))));
                }
                else
                {
                    collectionArgs.SafeAdd(ExpressionElement(IdentifierName(string.Join(",", simplePropertyVarNames))));
                }

                args.SafeAddArgument(CollectionExpression(SeparatedList<CollectionElementSyntax>(collectionArgs)));
                args.SafeAddArgument(CreateStringLiteral(complexProperty.DefaultXMLData?.XmlTag ?? complexProperty.Name.ToString()));

                var childComplexProperties = complexProperty.OriginalModelData.Properties.Values.Where(c => c.IsComplex || c.IsList);
                List<SyntaxNodeOrToken> explodes = [];
                if (childComplexProperties.Any())
                {
                    foreach (var childcomplexProperty in childComplexProperties)
                    {
                        AddExplodeArgForComplexChild(childcomplexProperty.Name,
                                                     explodes,
                                                     GetPartNameVariableName(childcomplexProperty),
                                                     childcomplexProperty.TDLCollectionData?.ExplodeCondition);
                        foreach (var xMLData in childcomplexProperty.XMLData)
                        {
                            AddExplodeArgForComplexChild(complexProperty.Name,
                                                             explodes,
                                                             GetPartNameVariableName(xMLData),
                                                             complexProperty.TDLCollectionData?.ExplodeCondition);

                        }
                    }
                    intializerArgs = [
                    AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                          IdentifierName("Explode"),
                          CollectionExpression(SeparatedList<CollectionElementSyntax>(explodes)))
                    ];
                }

                statements.Add(CreateArrayAssignmentwithExpression(linesVariableName, counter, CreateImplicitObjectExpression(args, intializerArgs)));
                counter++;

                if (complexProperty.XMLData.Count > 0)
                {
                    HandleMultipleXMLProperties(complexProperty, statements, linesVariableName, ref counter);

                }
            }

            statements.Add(ReturnStatement(IdentifierName(linesVariableName)));
        }
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(LineFullTypeName),
                                                        Identifier(string.Format(GetTDLLinesMethodName, "")))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return [GenerateMainLineMethodSyntax(), methodDeclarationSyntax];
    }

    private void HandleMultipleXMLProperties(PropertyData complexProperty,
                                             List<StatementSyntax> statements,
                                             string linesVariableName,
                                             ref int counter)
    {

        foreach (var data in complexProperty.XMLData)
        {
            List<SyntaxNodeOrToken> innerArgs = [];
            List<SyntaxNodeOrToken>? innerIntializerArgs = null;
            innerArgs.SafeAddArgument(IdentifierName(GetPartNameVariableName(data)));

            var simplePropertyVarNames = data.ModelData?.GetAllDirectProperties().Where(c => !c.IsComplex && !c.IsList && !c.IsAttribute).Select(c => GetFieldNameVariableName(c)).ToList();
            List<SyntaxNodeOrToken> collectionArgs = [];
            if (simplePropertyVarNames?.Count == 0)
            {
                collectionArgs.Add(ExpressionElement(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("SimpleField"))));
            }
            else
            {
                collectionArgs.SafeAdd(ExpressionElement(IdentifierName(string.Join(",", simplePropertyVarNames))));
            }
            innerArgs.SafeAddArgument(CollectionExpression(SeparatedList<CollectionElementSyntax>(collectionArgs)));
            innerArgs.SafeAddArgument(CreateStringLiteral(data.XmlTag ?? data.ModelData?.ToString() ?? complexProperty.Name));
            statements.Add(CreateArrayAssignmentwithExpression(linesVariableName, counter, CreateImplicitObjectExpression(innerArgs, innerIntializerArgs)));
            counter++;
        }
    }

    private MethodDeclarationSyntax GenerateMainLineMethodSyntax()
    {
        List<StatementSyntax> statements = [];
        List<SyntaxNodeOrToken>? intializerArgs = null;

        List<SyntaxNodeOrToken> args = [];

        var propertyVarNames = _simpleProperties.Where(c => !c.IsList && !c.IsAttribute && !c.IsOveridden).Select(c => GetFieldNameVariableName(c)).ToList();

        if (propertyVarNames.Count == 0)
        {
            args.Add(ExpressionElement(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("SimpleField"))));
        }
        else
        {
            Utils.SafeAdd(args, ExpressionElement(IdentifierName(string.Join(",", propertyVarNames))));
        }
        if (_complexProperties.Any())
        {
            List<SyntaxNodeOrToken> explodes = [];
            foreach (var complexProperty in _complexProperties)
            {
                AddExplodeArgForComplexChild(complexProperty.Name,
                                             explodes,
                                             GetPartNameVariableName(complexProperty),
                                             complexProperty.TDLCollectionData?.ExplodeCondition);
                foreach (var xMLData in complexProperty.XMLData)
                {
                    AddExplodeArgForComplexChild(complexProperty.Name,
                                                     explodes,
                                                     GetPartNameVariableName(xMLData),
                                                     complexProperty.TDLCollectionData?.ExplodeCondition);

                }
            }

            intializerArgs = [
                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                          IdentifierName("Explode"),
                          CollectionExpression(SeparatedList<CollectionElementSyntax>(explodes)))
                ];
        }
        statements.Add(ReturnStatement(CreateImplicitObjectExpression(
            [Argument(IdentifierName(_reportVarName)),
                 Token(SyntaxKind.CommaToken),
                 Argument(CollectionExpression(SeparatedList<CollectionElementSyntax>( args))),
                 Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(_xmlTagVariableName))
                 ], intializerArgs)
            ));
        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(LineFullTypeName),
                                                        Identifier(string.Format(GetMainTDLLineMethodName, "")))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;

    }
    void AddExplodeArgForComplexChild(string name,
                                      List<SyntaxNodeOrToken> explodes,
                                      string partName,
                                      string? explodeCondition)
    {

        string text = ":YES";
        List<InterpolatedStringContentSyntax> nodes =
            [
                Interpolation(IdentifierName(partName)),
                        //InterpolatedStringText()
                        //.WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList()))
                    ];
        if (explodeCondition == null)
        {
            nodes.Add(InterpolatedStringText()
                .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
        }
        else
        {
            text = ":";
            nodes.Add(InterpolatedStringText()
                .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
            nodes.Add(Interpolation(CreateFormatExpression(explodeCondition, name)));
        }
        InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
            .WithContents(List(nodes));
        explodes.SafeAdd(ExpressionElement(interpolatedStringExpressionSyntax));

    }
    private MethodDeclarationSyntax GenerateMainPartMethodSyntax(string CollectionNameArgName,
                                                                 string partNameArgName,
                                                                 string XmlTagArgName,
                                                                 string PartsVariableName)
    {
        List<StatementSyntax> statements = [];

        List<SyntaxNodeOrToken> constructorArgs = [
                    Argument(IdentifierName(partNameArgName)),
            Token(SyntaxKind.CommaToken),
        ];

        constructorArgs.Add(Argument(IdentifierName(CollectionNameArgName)));

        List<SyntaxNodeOrToken>? firstPartIntializerArgs = null;
        constructorArgs.AddRange([Token(SyntaxKind.CommaToken), Argument(IdentifierName(partNameArgName))]);
        firstPartIntializerArgs = [AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("XMLTag"),
                                         IdentifierName(XmlTagArgName))];

        statements.Add(ReturnStatement(CreateImplicitObjectExpression(constructorArgs, firstPartIntializerArgs)));

        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(PartFullTypeName),
                                                        Identifier(string.Format(GetMainTDLPartMethodName, "")))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        methodDeclarationSyntax = methodDeclarationSyntax
                 .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
                 {
                    Parameter(Identifier(partNameArgName))
                    .WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                    .WithDefault(EqualsValueClause(IdentifierName(_reportVarName))),
                    Token(SyntaxKind.CommaToken),
                    Parameter(Identifier(CollectionNameArgName))
                        .WithType(NullableType( PredefinedType(Token(SyntaxKind.StringKeyword))))
                        .WithDefault(EqualsValueClause(IdentifierName(_collectionVariableName))),
                    Token(SyntaxKind.CommaToken),
                     Parameter(Identifier(XmlTagArgName))
                        .WithType(NullableType( PredefinedType(Token(SyntaxKind.StringKeyword))))
                        .WithDefault(EqualsValueClause(LiteralExpression(
                                        SyntaxKind.NullLiteralExpression))),
                 })));

        return methodDeclarationSyntax;
    }

    private MemberDeclarationSyntax CreateGetFieldsMethod()
    {
        const string FieldsVariableName = "_fields";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCountVariable(FieldFullTypeName, FieldsVariableName, SimpleFieldsCountFieldName));
        int counter = 0;

        foreach (var property in _allSimpleProperties)
        {
            if (property.IsOveridden) continue;
            //if (property.Exclude) continue;
            string name = GetFieldNameVariableName(property);
            List<SyntaxNodeOrToken> constructerArgs = [];
            List<SyntaxNodeOrToken>? intializerArgs = null;
            constructerArgs.SafeAddArgument(IdentifierName(name));
            constructerArgs.SafeAddArgument(CreateStringLiteral(property.DefaultXMLData?.XmlTag ?? property.Name.ToUpper()));
            if (property.IsEnum)
            {
                constructerArgs.SafeAddArgument(CreateStringLiteral($"$$NameGetValue:{property.TDLFieldData?.Set}:{GetNameSetName(property)}"));
               
            }
            else
            {
                constructerArgs.SafeAddArgument(CreateStringLiteral(property.TDLFieldData?.Set ?? ""));
            }
            if (property.IsNullable || property.IsEnum)
            {
                intializerArgs ??= [];
                intializerArgs.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                     IdentifierName("Invisible"),
                                     CreateStringLiteral("$$ISEmpty:$$value")));
            }

            var expressionStatementSyntax = GetArrayAssignmentExppressionImplicit(FieldsVariableName, counter, constructerArgs, intializerArgs);
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


    private MemberDeclarationSyntax GenerateGetCollectionsMethodSyntax()
    {
        const string CollectionsVariableName = "collections";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(CollectionFullTypeName, CollectionsVariableName, 1));
        List<SyntaxNodeOrToken> nodesAndTokens =
            [
              SpreadElement(  InvocationExpression(IdentifierName(string.Format(GetFetchListMethodName,""))).WithArgumentList(ArgumentList()))
            ];

        statements.Add(GetArrayAssignmentExppressionImplicit(CollectionsVariableName, 0,
            [
                Argument(IdentifierName(_collectionVariableName)),
                Token(SyntaxKind.CommaToken),
                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(_modelData.TDLCollectionData?.Type ?? _modelData.XMLTag ?? ""))),
                Token(SyntaxKind.CommaToken),
                Argument(CollectionExpression(SeparatedList<CollectionElementSyntax>(nodesAndTokens)))
                .WithNameColon(NameColon(IdentifierName("nativeFields")))
            ]));
        statements.Add(ReturnStatement(IdentifierName(CollectionsVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(CollectionFullTypeName),
                                                        Identifier(string.Format(GetTDLCollectionsMethodName, "")))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        return methodDeclarationSyntax;


    }

    private MemberDeclarationSyntax GenerateGetFetchListMethodSyntax()
    {
        List<SyntaxNodeOrToken> nodesAndTokens = [];


        foreach (var property in _simpleProperties)
        {
            if (property.IsOveridden) continue;
            if (property.TDLFieldData?.ExcludeInFetch ?? false)
            {
                continue;
            }
            nodesAndTokens.SafeAddExpressionElement(CreateStringLiteral(property.TDLFieldData?.FetchText ?? property.Name));

        }
        foreach (var property in _allComplexProperties)
        {
            if (property.IsOveridden) continue;
            if (property.IsTallyComplexObject)
            {
                nodesAndTokens.SafeAddExpressionElement(CreateStringLiteral(property.TDLFieldData?.FetchText ?? property.Name));
                continue;
            }
            if (property.OriginalModelData == null)
            {
                continue;
            }

            AddFetchListofComplexProperties(nodesAndTokens, property.OriginalModelData, property.CollectionPrefix);
            foreach (var xMLData in property.XMLData)
            {
                if (xMLData.ModelData == null)
                {
                    continue;
                }
                AddFetchListofComplexProperties(nodesAndTokens, xMLData.ModelData, xMLData.CollectionPrefix);
            }
        }

        List<StatementSyntax> statements = [ReturnStatement(CollectionExpression(SeparatedList<CollectionElementSyntax>(nodesAndTokens)))];
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                        Identifier(string.Format(GetFetchListMethodName, "")))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        //if (_symbol.IsChild && (!_symbol.IsBaseSymbol || _symbol.IsParentChild))
        //{
        //    methodDeclarationSyntax = methodDeclarationSyntax.WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
        //          {
        //            Parameter(Identifier(prefixParamName))
        //            .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))) })));



        //}
        return methodDeclarationSyntax;

        static void AddFetchListofComplexProperties(List<SyntaxNodeOrToken> nodesAndTokens, ModelData modelData, string? collectionPrefix)
        {
            var simpleProperties = modelData.GetAllDirectProperties()
                .Where(c => !c.IsComplex && !(c.TDLFieldData?.ExcludeInFetch ?? false));
            string fetchText;
            if (collectionPrefix is null)
            {
                fetchText = string.Join(",", simpleProperties.Select(c => c.TDLFieldData!.FetchText));
            }
            else
            {
                fetchText = string.Join(",", simpleProperties.Select(c => $"{collectionPrefix}.{c.TDLFieldData!.FetchText}"));

            }
            if (!string.IsNullOrWhiteSpace(fetchText))
            {
                nodesAndTokens.SafeAddExpressionElement(CreateStringLiteral(fetchText));
            }
        }
    }

    private MemberDeclarationSyntax CreateGetNameSetMethod()
    {
        const string varName = "namesets";
        List<StatementSyntax> statements = [];
        if (_modelData.ENumPropertiesCount == 0)
        {
            statements.Add(ReturnStatement(CollectionExpression()));
        }
        else
        {
            statements.Add(CreateVarArrayWithCount(TDLNameSetFullTypeName, varName, _modelData.ENumPropertiesCount));
            int counter = 0;
            foreach (var simpleProperty in _allSimpleProperties)
            {
                if (!simpleProperty.IsEnum || simpleProperty.Exclude)
                {
                    continue;
                }
                List<SyntaxNodeOrToken> args = [];
                List<SyntaxNodeOrToken> intializerArgs = [];
                var items = simpleProperty.OriginalModelData?.Properties.Values.SelectMany(c => c.DefaultXMLData?.EnumChoices
                .Where(choice => !string.IsNullOrWhiteSpace(choice.Choice)).Select(ch => $"{ch.Choice}:\"{c.Name}\"")).Select(c => ExpressionElement(CreateStringLiteral(c)));
                intializerArgs.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                          IdentifierName("List"),
                          CollectionExpression(SeparatedList<CollectionElementSyntax>(items))));
                args.SafeAddArgument(CreateStringLiteral(GetNameSetName(simpleProperty)));
                statements.Add(CreateArrayAssignmentwithExpression(varName, counter, CreateImplicitObjectExpression(args, intializerArgs)));
                counter++;
            }
            statements.Add(ReturnStatement(IdentifierName(varName)));
        }

        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(TDLNameSetFullTypeName),
                                                        Identifier(string.Format(GetTDLNameSetsMethodName, "")))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;


    }
    private string GetNameSetName(PropertyData simpleProperty)
    {
        if (simpleProperty.OriginalModelData is null)
        {
            return string.Empty;
        }
        return $"{simpleProperty.OriginalModelData.Name}_{Utils.GenerateUniqueNameSuffix(simpleProperty.OriginalModelData.FullName)}";
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
                                                                 Argument(IdentifierName(_reportVarName)),
                                                            }))))
                                                        }))));
    }


    private StatementSyntax CreateAssignFromMethodStatement(string varName,
                                                            string propName,
                                                            List<string> methodNames,
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
        for (int i = 0; i < methodNames.Count; i++)
        {
            var methodName = methodNames[i];
            Utils.SafeAdd(nodes, SpreadElement(InvocationExpression(IdentifierName(methodName))));
        }

        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(varName),
                                    IdentifierName(propName)),
                                    CollectionExpression(SeparatedList<CollectionElementSyntax>(nodes))));
    }


    private static string GetFieldNameVariableName(PropertyData property) => $"{property.FieldName}_FieldName";

    private static string GetPartNameVariableName(PropertyData property) => $"{property.FieldName}_PartName";
    private static string GetPartNameVariableName(XMLData data) => $"{data.FieldName}_PartName";

    static InvocationExpressionSyntax CreateFormatExpression(string val, string? name = null)
    {
        return InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                      PredefinedType(Token(SyntaxKind.StringKeyword)),
                                      IdentifierName("Format")))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
            {
                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(val))),
                    Token(SyntaxKind.CommaToken),
                    Argument(name == null ?  IdentifierName(setValueParamName) :CreateStringLiteral(name))
            })));

    }
    private static ExpressionStatementSyntax GetArrayAssignmentExppressionImplicit(string FieldsVariableName,
                                                                                   int index,
                                                                                   List<SyntaxNodeOrToken> constructorArgs,
                                                                                   List<SyntaxNodeOrToken>? intializerArgs = null)
    {
        ExpressionSyntax implicitObjectCreationExpressionSyntax = CreateImplicitObjectExpression(constructorArgs, intializerArgs);
        return CreateArrayAssignmentwithExpression(FieldsVariableName, index, implicitObjectCreationExpressionSyntax);
    }
    private static ExpressionStatementSyntax CreateArrayAssignmentwithExpression(string FieldsVariableName,
                                                                                 int index,
                                                                                 ExpressionSyntax expression)
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

    private static ImplicitObjectCreationExpressionSyntax CreateImplicitObjectExpression(List<SyntaxNodeOrToken> constructorArgs,
                                                                                         List<SyntaxNodeOrToken>? intializerArgs = null)
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


    internal LocalDeclarationStatementSyntax CreateVarArrayWithCountVariable(string typeName,
                                                                             string varName,
                                                                             string countVariableName)
    {

        return CreateVarInsideMethodWithExpression(varName, ArrayCreationExpression(CreateArrayTypewithCountVariable(typeName, countVariableName)));
    }
    internal LocalDeclarationStatementSyntax CreateVarArrayWithCount(string typeName, string varName, int count)
    {

        return CreateVarInsideMethodWithExpression(varName, ArrayCreationExpression(CreateArrayTypewithCount(typeName, count)));
    }
    internal ArrayTypeSyntax CreateArrayTypewithCountVariable(string typeName, string countVariableName)
    {
        var arrayTypeSyntax = ArrayType(GetGlobalNameforType(typeName))
            .WithRankSpecifiers(List([ArrayRankSpecifier(SingletonSeparatedList((ExpressionSyntax)IdentifierName(countVariableName)))]));
        return arrayTypeSyntax;
    }
    internal ArrayTypeSyntax CreateArrayTypewithCount(string typeName, int count)
    {
        var arrayTypeSyntax = ArrayType(GetGlobalNameforType(typeName))
            .WithRankSpecifiers(List([ArrayRankSpecifier(SingletonSeparatedList((ExpressionSyntax)LiteralExpression(
                                                                SyntaxKind.NumericLiteralExpression,
                                                                Literal(count))))]));
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
    private static FieldDeclarationSyntax CreatePublicConstStringVar(string varName, string varText)
    {
        TypeSyntax strSyntax = PredefinedType(Token(SyntaxKind.StringKeyword));

        VariableDeclarationSyntax variableDeclarationSyntax = CreateVariableDelaration(strSyntax,
                                                                                       varName,
                                                                                       LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(varText)));
        List<SyntaxToken> tokens = [];
        tokens.Add(Token(SyntaxKind.PublicKeyword));

        tokens.Add(Token(SyntaxKind.ConstKeyword));
        return FieldDeclaration(variableDeclarationSyntax)
            .WithModifiers(TokenList(tokens));
    }
    private static FieldDeclarationSyntax CreateConstIntVar(string varName, int value, bool isInternal = false)
    {
        TypeSyntax strSyntax = PredefinedType(Token(SyntaxKind.IntKeyword));

        VariableDeclarationSyntax variableDeclarationSyntax = CreateVariableDelaration(strSyntax,
                                                                                       varName,
                                                                                       LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)));
        List<SyntaxToken> tokens = [Token(SyntaxKind.ConstKeyword)];

        return FieldDeclaration(variableDeclarationSyntax)
            .WithModifiers(TokenList(tokens));
    }
}
