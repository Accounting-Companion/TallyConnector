using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TallyConnector.TDLReportSourceGenerator.Extensions.Symbols;
using TallyConnector.TDLReportSourceGenerator.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TallyConnector.TDLReportSourceGenerator.Execute;
internal class Helper
{

    private readonly SymbolData _symbol;
    readonly string _reportVariableName;
    readonly string _collectionVariableName;
    private readonly string _paginatedCollectionVariableName;
    readonly string ReportName;
    readonly string CollectionName;
    private readonly string _paginatedCollectionName;

    const string _reqOptionsArgName = "reqOptions";
    private readonly List<ChildSymbolData> _complexChildren;
    private readonly List<ChildSymbolData> _simpleChildren;
    private readonly bool _includeCollection;
    private readonly bool _isMasterObject;
    private readonly bool _isVoucherObject;
    private readonly bool _isTallyObject;
    private readonly bool _hasActivitySource;
    private readonly bool _hasDefaultFilters;

    public Helper(SymbolData symbol)
    {
        _symbol = symbol;
        ReportName = $"TC_{_symbol.TypeName}List";
        _reportVariableName = $"{_symbol.TypeName}{nameof(ReportName)}";
        _collectionVariableName = $"{_symbol.TypeName}{nameof(CollectionName)}";
        _paginatedCollectionVariableName = $"{_collectionVariableName}Paginated";
        CollectionName = _symbol.IsChild ? _symbol.Name : $"TC_{_symbol.Name}Collection";
        _paginatedCollectionName = _symbol.IsChild ? _symbol.Name : $"{CollectionName}_Paginated";
        _complexChildren = symbol.Children.Values.Where(c => c.IsComplex || c.IsList).ToList();
        _simpleChildren = symbol.Children.Values.Where(c => !c.IsComplex).ToList();
        _includeCollection = !(_symbol.TDLCollectionDetails?.Exclude ?? false);
        _isMasterObject = _symbol.Symbol.HasInterfaceWithFullyQualifiedMetadataName(MasterObjectInterfaceName);
        _isVoucherObject = _symbol.Symbol.HasInterfaceWithFullyQualifiedMetadataName(VoucherObjectInterfaceName);
        _isTallyObject = _isMasterObject || _isVoucherObject;
        _hasActivitySource = !string.IsNullOrWhiteSpace(symbol.ActivitySourceName);
        _hasDefaultFilters = _symbol.DefaultFilterMethods.Any();
    }

    public string GetPostRequestEnvelopeCompilationUnit()
    {
        var unit = CompilationUnit()
            .WithUsings(List([UsingDirective(IdentifierName(ExtensionsNameSpace))]))
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                FileScopedNamespaceDeclaration(IdentifierName($"{_symbol.MainNameSpace}.Models"))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                      true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    ClassDeclaration("PostRequestEnvelope")
                    .WithModifiers(TokenList([Token(SyntaxKind.PartialKeyword)]))
                    .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                    SimpleBaseType(
                        IdentifierName(_symbol.ReqEnvelopeSymbol.OriginalDefinition.ToString())))))
                    .WithMembers(List(new MemberDeclarationSyntax[]
                    {
                        GetPropertyMemberSyntax(IdentifierName("Body"),"Body"),
                    }))
                }))
            })).NormalizeWhitespace().ToFullString();

        return unit;
    }


    public string GetTDLReportCompilationUnit()
    {
        var unit = CompilationUnit()
            .WithUsings(List([UsingDirective(IdentifierName(ExtensionsNameSpace))]))
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                FileScopedNamespaceDeclaration(IdentifierName(_symbol.MainNameSpace))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                      true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    ClassDeclaration(_symbol.MainSymbol.Name)
                    .WithModifiers(TokenList([Token(
                            TriviaList(
                                Comment($@"/*
 * Generated based on {_symbol.FullName}
 */")),
                            SyntaxKind.PartialKeyword,
                            TriviaList())]))
                    .WithMembers(List(GetTDLReportClassMembers()))
                }))
            })).NormalizeWhitespace().ToFullString();

        return unit;
    }

    private IEnumerable<MemberDeclarationSyntax> GetTDLReportClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        // creates const variables for field names
        if (!_symbol.IsEnum)
        {
            foreach (var child in _simpleChildren)
            {
                members.Add(CreateConstStringVar(GetFieldName(child), $"TC_{child.Parent.Name}_{child.Name}", true));
            }
            foreach (var child in _complexChildren)
            {
                members.Add(CreateConstStringVar(child.ReportVarName, $"TC_{_symbol.TypeName}{child.Name}List", true));

                if (child.DefaultTDLCollectionDetails != null && child.DefaultTDLCollectionDetails.CollectionName != null)
                {
                    members.Add(CreateConstStringVar(GetCollectionName(child), child.DefaultTDLCollectionDetails.CollectionName));
                }
                int Counter = 1;
                foreach (var xMLData in child.XMLData)
                {
                    if (xMLData.ChildSymbolData != null)
                    {
                        members.Add(CreateConstStringVar(xMLData.ChildSymbolData.ReportVarName, $"TC_{_symbol.TypeName}{child.Name}{Counter}List", true));
                        if (xMLData.ChildSymbolData?.DefaultTDLCollectionDetails != null && xMLData.ChildSymbolData.DefaultTDLCollectionDetails.CollectionName != null)
                        {
                            members.Add(CreateConstStringVar(GetCollectionName(xMLData.ChildSymbolData, Counter), xMLData.ChildSymbolData.DefaultTDLCollectionDetails.CollectionName));
                        }
                        Counter++;
                    }


                }
            }
            members.AddRange(
                [
                    CreateConstStringVar(_reportVariableName, ReportName, true),
                    CreateConstStringVar(_collectionVariableName, CollectionName),
                    CreateConstStringVar(_paginatedCollectionVariableName, _paginatedCollectionName),
                ]);
        }
        if (!_symbol.IsChild)
        {
            members.Add(GenerateGetObjectsMethodSyntax());

            if (_isTallyObject)
            {
                members.Add(GenerateGetObjectsPaginatedMethodSyntax());
                if (_symbol.GenerationMode is GenerationMode.Post or GenerationMode.All)
                {
                    members.Add(GeneratePostObjectsMethodSyntax());
                }
            }
            members.Add(GenerateGetXmlAttributeOverridesMethodSyntax());
            members.Add(GenerateGetRequestEnvolopeMethodSyntax());
        }
        if (!_symbol.IsEnum)
        {
            members.AddRange(GenerateGetPartsMethodSyntax());
            members.AddRange(
                [
                    GenerateGetLinesMethodSyntax(),
                    GenerateGetFieldsMethodSyntax(),
                ]);
        }

        if (!_symbol.IsChild)
        {
            if (_includeCollection && _symbol.TDLCollectionMethods.Count == 0)
            {
                members.Add(GenerateGetCollectionsMethodSyntax());
            }
        }
        if (_symbol.IsEnum)
        {
            if (_symbol.TDLNameSetMethods.Count == 0)
            {
                members.Add(GenerateGetNamesetsMethodSyntax());
            }
            members.Add(GenerateGetFunctionsMethodSyntax());
            members.Add(GenerateGetTallyStringMethodSyntax());
        }
        else
        {
            members.Add(GenerateGetFetchListMethodSyntax());
        }

        return members;
    }

    
    private MemberDeclarationSyntax GenerateGetObjectsPaginatedMethodSyntax()
    {
        string reqEnvelopeVarName = "reqEnvelope";
        string xmlVarName = "reqXml";
        string xmlRespVarName = "resp";
        string xmlRespEnvlopeVarName = "respEnv";
        string activityVarName = "activity";
        string reqTypeVarName = "reqType";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarInsideMethodWithExpression(reqTypeVarName, CreateStringLiteral($"Getting {_symbol.MethodNameSuffixPlural}")));
        if (_hasActivitySource)
        {
            statements.Add(LocalDeclarationStatement(CreateVariableDeclaration(activityVarName,
                                                                              InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                      IdentifierName(_symbol.ActivitySourceName!),
                                                                                                      IdentifierName(StartActivityMethodName)))
                                                                              .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                                                              {
                                                                              Argument(IdentifierName(reqTypeVarName)),
                                                                              }))))).WithUsingKeyword(
                                Token(SyntaxKind.UsingKeyword)));
        }
        InvocationExpressionSyntax mainExpression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                               GetGlobalNameforType(_symbol.MainFullName),
                                                                                                               IdentifierName(string.Format(GetRequestEnvelopeMethodName, _symbol.TypeName))));
        statements.Add(CreateVarInsideMethodWithExpression(reqEnvelopeVarName, mainExpression));

        statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(reqEnvelopeVarName), IdentifierName(PopulateOptionsExtensionMethodName)))
                .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                {
                    Argument(IdentifierName(_reqOptionsArgName))
                })))));
        statements.Add(ExpressionStatement(AwaitExpression(InvocationExpression(IdentifierName(PopulateDefaultOptionsMethodName))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
            {
                Argument(IdentifierName(reqEnvelopeVarName)),
                Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(CancellationTokenArgName)),
            }))))));
        statements.Add(CreateVarInsideMethodWithExpression(xmlVarName,
                                                           InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                           IdentifierName(reqEnvelopeVarName), IdentifierName("GetXML")))));
        statements.Add(CreateVarInsideMethodWithExpression(xmlRespVarName, AwaitExpression(InvocationExpression(IdentifierName(SendRequestMethodName))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
            new SyntaxNodeOrToken[]{
                Argument(IdentifierName(xmlVarName)),
                Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(reqTypeVarName)),
                 Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(CancellationTokenArgName))
            }))))));

        statements.Add(CreateVarInsideMethodWithExpression(xmlRespEnvlopeVarName,
                                                           InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                       GetGlobalNameforType(XMLToObjectClassName),
                                                                                                       GenericName(GetObjfromXmlMethodName)
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)GetGlobalNameforType($"{_symbol.MainNameSpace}.Models.{GetReportResponseEnvelopeName(_symbol)}"))))))
                                                           .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                                           {
                                                               Argument(PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression,MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespVarName), IdentifierName("Response")))),
                                                               Token(SyntaxKind.CommaToken),
                                                               Argument(InvocationExpression(IdentifierName(string.Format(GetXMLAttributeOveridesMethodName,_symbol.TypeName)))),
                                                               Token(SyntaxKind.CommaToken),
                                                               Argument( IdentifierName("_logger"))
                                                           })))));


        //switchStatements.Add(ReturnStatement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespEnvlopeVarName), IdentifierName("Objects"))));

        statements.Add(ReturnStatement(ImplicitObjectCreationExpression().WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
        {
           Argument(BinaryExpression(SyntaxKind.CoalesceExpression,MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespEnvlopeVarName), IdentifierName("TotalCount")),LiteralExpression(
                                                                                SyntaxKind.NumericLiteralExpression,
                                                                                Literal(0)))),
           Token(SyntaxKind.CommaToken),
            Argument(BinaryExpression(SyntaxKind.CoalesceExpression,ConditionalAccessExpression(IdentifierName(_reqOptionsArgName),MemberBindingExpression( IdentifierName("RecordsPerPage"))),LiteralExpression(
                                                                                SyntaxKind.NumericLiteralExpression,
                                                                                Literal(1_000)))),
             Token(SyntaxKind.CommaToken),
            Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespEnvlopeVarName), IdentifierName("Objects"))),
            Token(SyntaxKind.CommaToken),
            Argument(BinaryExpression(SyntaxKind.CoalesceExpression,ConditionalAccessExpression(IdentifierName(_reqOptionsArgName),MemberBindingExpression( IdentifierName("PageNum"))),LiteralExpression(
                                                                                SyntaxKind.NumericLiteralExpression,
                                                                                Literal(1)))),

        })))));
        var methodDeclarationSyntax = MethodDeclaration(QualifiedName(GetGlobalNameforType("System.Threading.Tasks"), GenericName("Task")
         .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(QualifiedName(GetGlobalNameforType(TallyConnectorPaginationNameSpace), GenericName(PaginatedResponseClassName)
         .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(GetGlobalNameforType(_symbol.FullName))))))))),
                                                     Identifier(string.Format(GetObjectsMethodName, _symbol.MethodNameSuffixPlural)))
         .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword)]))
         .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[] {Parameter(Identifier(_reqOptionsArgName))
                .WithType(NullableType(GetGlobalNameforType(PaginatedRequestOptionsClassName)))
                .WithDefault(EqualsValueClause(LiteralExpression(
                                                SyntaxKind.NullLiteralExpression))) , Token(SyntaxKind.CommaToken), GetCancellationTokenParameterSyntax()})))
         .WithBody(Block(statements));
        return methodDeclarationSyntax;
    }
    private MemberDeclarationSyntax GenerateGetObjectsMethodSyntax()
    {
        string reqEnvelopeVarName = "reqEnvelope";
        string xmlVarName = "reqXml";
        string xmlRespVarName = "resp";
        string xmlRespEnvlopeVarName = "respEnv";
        string activityVarName = "activity";
        string reqTypeVarName = "reqType";
        List<StatementSyntax> statements = [];
        List<SyntaxNodeOrToken> methodArgs = [];
        statements.Add(CreateVarInsideMethodWithExpression(reqTypeVarName, CreateStringLiteral($"Getting {_symbol.MethodNameSuffixPlural}")));
        if (_hasActivitySource)
        {
            statements.Add(LocalDeclarationStatement(CreateVariableDeclaration(activityVarName,
                                                                              InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                      IdentifierName(_symbol.ActivitySourceName!),
                                                                                                      IdentifierName(StartActivityMethodName)))
                                                                              .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                                                              {
                                                                              Argument(IdentifierName(reqTypeVarName)),
                                                                              }))))).WithUsingKeyword(Token(SyntaxKind.UsingKeyword)));
        }
        InvocationExpressionSyntax mainExpression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                               GetGlobalNameforType(_symbol.MainFullName),
                                                                                                               IdentifierName(string.Format(GetRequestEnvelopeMethodName, _symbol.TypeName))));
        statements.Add(CreateVarInsideMethodWithExpression(reqEnvelopeVarName, mainExpression));

        bool haveAnyArgs = _symbol.Args.Any();
        if (haveAnyArgs)
        {
            INamedTypeSymbol namedTypeSymbol = _symbol.Args.First();

            methodArgs.Add(Parameter(Identifier(_reqOptionsArgName))
                .WithType(NullableType(GetGlobalNameforType(namedTypeSymbol.OriginalDefinition.ToString())))
                .WithDefault(EqualsValueClause(LiteralExpression(
                                                SyntaxKind.NullLiteralExpression))));
        }
        if (!haveAnyArgs && (_isMasterObject || _isVoucherObject))
        {
            methodArgs.Add(Parameter(Identifier(_reqOptionsArgName))
                .WithType(GetGlobalNameforType(RequestOptionsClassName)));
            //.WithDefault(EqualsValueClause(LiteralExpression(
            //                                SyntaxKind.NullLiteralExpression))));
        }
        if (haveAnyArgs || _isMasterObject || _isVoucherObject)
        {
            var extensionMethodInvokeSyntax = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(reqEnvelopeVarName), IdentifierName(PopulateOptionsExtensionMethodName)))
                .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                {
                    Argument(IdentifierName(_reqOptionsArgName))
                })));
            statements.Add(IfStatement(BinaryExpression(SyntaxKind.NotEqualsExpression,
                                                        IdentifierName(_reqOptionsArgName),
                                                        LiteralExpression(SyntaxKind.NullLiteralExpression)), Block(
                                                            ExpressionStatement(extensionMethodInvokeSyntax))));
        }
        if (!(_symbol.Symbol.HasOrInheritsFromFullyQualifiedMetadataName(BaseCompanyTypeName) || _symbol.Symbol.HasFullyQualifiedMetadataName(BaseCompanyTypeName)))
        {
            statements.Add(ExpressionStatement(AwaitExpression(InvocationExpression(IdentifierName(PopulateDefaultOptionsMethodName))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
            {
                Argument(IdentifierName(reqEnvelopeVarName)),
                Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(CancellationTokenArgName)),
            }))))));
        }

        statements.Add(CreateVarInsideMethodWithExpression(xmlVarName,
                                                           InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                           IdentifierName(reqEnvelopeVarName), IdentifierName("GetXML")))));
        statements.Add(CreateVarInsideMethodWithExpression(xmlRespVarName, AwaitExpression(InvocationExpression(IdentifierName(SendRequestMethodName))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
            new SyntaxNodeOrToken[]{
                Argument(IdentifierName(xmlVarName)),
                Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(reqTypeVarName)),
                 Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(CancellationTokenArgName))
            }))))));


        statements.Add(CreateVarInsideMethodWithExpression(xmlRespEnvlopeVarName,
                                                           InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                       GetGlobalNameforType(XMLToObjectClassName),
                                                                                                       GenericName(GetObjfromXmlMethodName)
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)GetGlobalNameforType($"{_symbol.MainNameSpace}.Models.{GetReportResponseEnvelopeName(_symbol)}"))))))
                                                           .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                                           {
                                                               Argument(PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression,MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespVarName), IdentifierName("Response")))),
                                                               Token(SyntaxKind.CommaToken),
                                                               Argument(InvocationExpression(IdentifierName(string.Format(GetXMLAttributeOveridesMethodName,_symbol.TypeName)))),
                                                               Token(SyntaxKind.CommaToken),
                                                               Argument( IdentifierName("_logger"))
                                                           })))));


        statements.Add(ReturnStatement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespEnvlopeVarName), IdentifierName("Objects"))));

        SafeAdd(methodArgs, GetCancellationTokenParameterSyntax());
        var methodDeclarationSyntax = MethodDeclaration(QualifiedName(GetGlobalNameforType("System.Threading.Tasks"), GenericName("Task")
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(QualifiedName(GetGlobalNameforType(CollectionsNameSpace), GenericName(ListClassName)
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(GetGlobalNameforType(_symbol.FullName))))))))),
                                                        Identifier(string.Format(GetObjectsMethodName, _symbol.MethodNameSuffixPlural)))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword)]))
            .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(methodArgs)))
            .WithBody(Block(statements));

        return methodDeclarationSyntax;
    }

    private MemberDeclarationSyntax GeneratePostObjectsMethodSyntax()
    {
        const string envelopeVariableName = "reqEnvelope";
        const string reqMsgVariableName = "reqMsg";
        List<StatementSyntax> statements = [];

        QualifiedNameSyntax node = QualifiedName(GetGlobalNameforType($"{_symbol.MainNameSpace}.Models"), IdentifierName(string.Format(PostRequestEnvelopeMessageName, _symbol.MainSymbol.Name)));
        statements.Add(CreateVarInsideMethodWithExpression(envelopeVariableName, ObjectCreationExpression(QualifiedName(GetGlobalNameforType(TallyConnectorModelsNameSpace), GenericName(TallyEnvelopeTypeName).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)node)))))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[] { })))));

        statements.Add(CreateVarInsideMethodWithExpression(reqMsgVariableName,
                                                           ObjectCreationExpression(node).WithArgumentList(ArgumentList())));
        statements.Add(ExpressionStatement(AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                  IdentifierName(envelopeVariableName),
                                                                                  IdentifierName("Body.RequestData.RequestMessage")), IdentifierName(reqMsgVariableName))));
        statements.Add(ReturnStatement(ImplicitObjectCreationExpression()));
        var methodDeclarationSyntax = MethodDeclaration(QualifiedName(GetGlobalNameforType("System.Threading.Tasks"), GenericName("Task")
         .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(QualifiedName(GetGlobalNameforType(CollectionsNameSpace), GenericName(ListClassName).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(PostResultFullTypeName))))))))),
                                                     Identifier(string.Format(PostObjectsMethodName, _symbol.MethodNameSuffixPlural)))
         .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword)]))
         .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[] {Parameter(Identifier(_reqOptionsArgName))
                .WithType(NullableType(GetGlobalNameforType(RequestOptionsClassName)))
                .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression))) , Token(SyntaxKind.CommaToken), GetCancellationTokenParameterSyntax()})))
         .WithBody(Block(statements));
        return methodDeclarationSyntax;
    }
    private MemberDeclarationSyntax GenerateGetXmlAttributeOverridesMethodSyntax()
    {
        var xmlAttributeOverridesVarName = "xmlAttributeOverrides";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarInsideMethodWithExpression(xmlAttributeOverridesVarName, ObjectCreationExpression(GetGlobalNameforType(XmlAttributeOverridesClassName)).WithArgumentList(ArgumentList())));
        HashSet<string> typeNames = [];
        AddExpressionForOverridenChilds(_symbol);

        void AddExpressionForOverridenChilds(SymbolData symbol)
        {
            typeNames.Add(symbol.FullName);
            if (symbol.BaseSymbolData != null && !typeNames.Contains(symbol.BaseSymbolData.SymbolData!.FullName))
            {

                AddExpressionForOverridenChilds(symbol.BaseSymbolData.SymbolData);
            }
            foreach (var child in symbol.Children.Values.Where(c => c.IsOverridden))
            {
                ChildSymbolData overriddenChild = child.OverriddenChild!;
                if (overriddenChild == null)
                {

                }
                AddExpressionForOverridenChild(overriddenChild);
                if (child.IsComplex && !typeNames.Contains(child.SymbolData!.FullName))
                {

                    AddExpressionForOverridenChilds(child.SymbolData!);
                }

                void AddExpressionForOverridenChild(ChildSymbolData overriddenChild)
                {
                    statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlAttributeOverridesVarName), IdentifierName("Add")))
                    .WithArgumentList(ArgumentList(
                                        SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                        {
                                         Argument(TypeOfExpression(GetGlobalNameforType(overriddenChild.Parent.FullName))),
                                          Token(SyntaxKind.CommaToken),
                                         Argument(CreateStringLiteral(overriddenChild.Name)),
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

            }


        }

        statements.Add(ReturnStatement(IdentifierName(xmlAttributeOverridesVarName)));
        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(XmlAttributeOverridesClassName),
                                                       Identifier(string.Format(GetXMLAttributeOveridesMethodName, _symbol.TypeName)))
           .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword)]))
           .WithParameterList(ParameterList())
           .WithBody(Block(statements));
        return methodDeclarationSyntax;
    }





    private MemberDeclarationSyntax GenerateGetRequestEnvolopeMethodSyntax()
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
                Argument(IdentifierName(_reportVariableName))
            })))));

        statements.Add(CreateVarInsideMethodWithExpression(tdlMsgVariableName,
                                                           MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                  IdentifierName(envelopeVariableName),
                                                                                  IdentifierName("Body.Desc.TDL.TDLMessage"))));
        statements.Add(CreateReportAndFormAsssignStatement(tdlMsgVariableName, "Report"));
        statements.Add(CreateReportAndFormAsssignStatement(tdlMsgVariableName, "Form"));

        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Part", [string.Format(GetTDLPartsMethodName, _symbol.TypeName)]));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Line", [string.Format(GetTDLLinesMethodName, _symbol.TypeName)]));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Field", [string.Format(GetTDLFieldsMethodName, _symbol.TypeName)]));
        if (_includeCollection)
        {
            statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Collection", [string.Format(GetTDLCollectionsMethodName, _symbol.TypeName)]));

        }

        List<ExpressionSyntax> functionMethodNames = [InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                           GetGlobalNameforType(_symbol.MainFullName), IdentifierName(GetDefaultTDLFunctionsMethodName)))];
        GetMethodNames(_symbol, c => c.TDLFunctionMethods, functionMethodNames);
        //foreach (var child in _simpleChildren)
        //{
        //    if (child.IsEnum && child.SymbolData!.TDLFunctionMethods.Count == 0)
        //    {
        //        functionMethodNames.Add(InvocationExpression(
        //            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
        //                                   GetGlobalNameforType(symbol.MainFullName),
        //                                   IdentifierName(string.Format(GetTDLFunctionsMethodName,
        //                                                                child.SymbolData!.TypeName)))));
        //    }
        //}
        AddFunctions(_symbol, functionMethodNames, GetTDLFunctionsMethodName, c => c.TDLFunctionMethods.Count);
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Functions", functionMethodNames));

        List<ExpressionSyntax> NameSetMethodNames = [];
        AddFunctions(_symbol, NameSetMethodNames, GetTDLNameSetsMethodName, c => c.TDLNameSetMethods.Count);
        if (_symbol.TDLNameSetMethods.Count > 0 || NameSetMethodNames.Count > 0)
        {
            statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "NameSet", GetMethodNames(_symbol, c => c.TDLNameSetMethods, NameSetMethodNames)));
        }

        List<ExpressionSyntax> tdlObjectMethodNames = [];

        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Object", GetMethodNames(_symbol, c => c.TDLGetObjectMethods, tdlObjectMethodNames)));
        if (_hasDefaultFilters)
        {
            //switchStatements.Add(Create());
            const string filtersVarName = "defaultFilters";
            statements.Add(LocalDeclarationStatement(CreateVariableDelaration(QualifiedName(GetGlobalNameforType(CollectionsNameSpace), GenericName(Identifier(ListClassName))
                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(GetGlobalNameforType(FilterFullTypeName))))), filtersVarName, CreateAssignFromMethodStatementRightExp(GetMethodNames(_symbol, c => c.DefaultFilterMethods, [])))));
            //switchStatements.Add(ExpressionStatement( AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,IdentifierName(filtersVarName), CreateAssignFromMethodStatementRightExp(GetMethodNames(_symbol, c => c.DefaultFilterMethods, [])))));
            statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "System", InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(filtersVarName), IdentifierName("ToSystem")))));
            statements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ElementAccessExpression(MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(tdlMsgVariableName),
                                            IdentifierName("Collection"))).WithArgumentList(BracketedArgumentList(SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                LiteralExpression(
                                                    SyntaxKind.NumericLiteralExpression,
                                                    Literal(0)))))), IdentifierName("Filters")),
                                            InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(filtersVarName), IdentifierName("GetFilterNames"))))));
        }
        // Return Statement
        statements.Add(ReturnStatement(IdentifierName(envelopeVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(RequestEnvelopeFullTypeName),
                                                        Identifier(string.Format(GetRequestEnvelopeMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;


        List<ExpressionSyntax> GetMethodNames(SymbolData symbol, Func<SymbolData, FunctionDetails> func,
                                              List<ExpressionSyntax>? methodNames = null)
        {
            methodNames ??= [];
            foreach (var dictvalue in func(symbol))
            {
                FunctionDetail value = dictvalue.Value;
                methodNames.Add(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                            GetGlobalNameforType(value.SymbolData.FullName),
                                                                            IdentifierName(value.FunctionName))));

            }
            if (symbol.BaseSymbolData != null)
            {
                GetMethodNames(symbol.BaseSymbolData, func, methodNames);
            }
            return methodNames;
        }

        static void AddFunctions(SymbolData symbol,
                                 List<ExpressionSyntax> methodNames,
                                 string methodFormatString,
                                 Func<SymbolData, int> action, HashSet<string>? ComplexTypeNames = null)
        {
            ComplexTypeNames ??= [];
            ComplexTypeNames.Add(symbol.FullName);
            IEnumerable<ChildSymbolData> children = symbol.Children.Values.Where(c => !c.IsComplex);
            IEnumerable<ChildSymbolData> complexchildren = symbol.Children.Values.Where(c => c.IsComplex);
            foreach (var child in children)
            {
                if (child.IsEnum && action(child.SymbolData!) == 0)
                {
                    methodNames.Add(InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                               GetGlobalNameforType(symbol.MainFullName),
                                               IdentifierName(string.Format(methodFormatString,
                                                                            child.SymbolData!.TypeName)))));
                }
            }
            foreach (var child in complexchildren)
            {
                if (!ComplexTypeNames.Contains(child.SymbolData!.FullName))
                {
                    AddFunctions(child.SymbolData!, methodNames, methodFormatString, action, ComplexTypeNames);
                }

            }
            if (symbol.BaseSymbolData != null)
            {
                if (!ComplexTypeNames.Contains(symbol.BaseSymbolData.SymbolData.FullName))
                {
                    AddFunctions(symbol.BaseSymbolData.SymbolData, methodNames, methodFormatString, action, ComplexTypeNames);
                }
            }
        }
    }

    private ExpressionStatementSyntax CreateReportAndFormAsssignStatement(string tdlMsgVariableName, string name)
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
                                                                 Argument(IdentifierName(_reportVariableName)),
                                                            }))))
                                                        }))));
    }

    private IEnumerable<MemberDeclarationSyntax> GenerateGetPartsMethodSyntax()
    {
        const string CollectionNameArgName = "collectionName";
        const string partNameArgName = "partName";
        const string XmlTagArgName = "xmlTag";
        const string PartsVariableName = "parts";
        List<StatementSyntax> statements = [];
        bool IsBaseSymbol = _symbol.IsBaseSymbol;
        statements.Add(CreateVarArrayWithCount(PartFullTypeName, PartsVariableName, IsBaseSymbol ? _symbol.TDLPartsCount : _symbol.TDLPartsCount + 1));


        MethodDeclarationSyntax MainPartMethod = GenerateMainPartMethodSyntax(CollectionNameArgName,
                                                                                  partNameArgName,
                                                                                  XmlTagArgName,
                                                                                  PartsVariableName);

        if (!IsBaseSymbol)
        {
            List<SyntaxNodeOrToken> nodesAndTokens = [];
            if (_symbol.IsChild)
            {
                nodesAndTokens =
                [
                    Argument(IdentifierName(partNameArgName)),
                    Token(SyntaxKind.CommaToken),
                    Argument(IdentifierName(CollectionNameArgName))
                ];
            }
            statements.Add(CreateArrayAssignmentwithExpression(PartsVariableName,
                                                                   0,
                                                                 InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                          GetGlobalNameforType(_symbol.MainFullName),
                                                                                          IdentifierName(string.Format(GetMainTDLPartMethodName, _symbol.TypeName))))
                                                                 .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                                                                                              nodesAndTokens)))));
        }
        int counter = IsBaseSymbol ? 0 : 1;
        if (_symbol.BaseSymbolData != null && _symbol.BaseSymbolData.Exclude == false)
        {
            string ComplexFieldsVariableName = $"{_symbol.BaseSymbolData.Name.Substring(0, 1).ToLower()}{_symbol.BaseSymbolData.Name.Substring(1)}{PartsVariableName.Substring(0, 1).ToUpper()}{PartsVariableName.Substring(1)}";
            statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                   InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                               GetGlobalNameforType(_symbol.MainFullName),
                                                                                               IdentifierName(string.Format(GetTDLPartsMethodName, _symbol.BaseSymbolData.SymbolData.TypeName))))));

            statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                           IdentifierName(PartsVariableName),
                                                                                           IdentifierName(AddToArrayExtensionMethodName)))
                .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                     new SyntaxNodeOrToken[]{
                            Argument(IdentifierName(ComplexFieldsVariableName)),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(counter))),
                     })))
                ));
            counter += _symbol.BaseSymbolData.SymbolData?.TDLPartsCount ?? 0;

        }
        foreach (var child in _complexChildren)
        {
            if (child.IsList && !child.IsComplex)
            {
                //var name = $"TC_{_symbol.TypeName}{child.Name}List";
                List<SyntaxNodeOrToken>? intializerArgs = null;
                List<SyntaxNodeOrToken> lineConstructorArgs =
                    [
                        Argument(IdentifierName(child.ReportVarName)),
                        Token(SyntaxKind.CommaToken),
                    ];
                if (child.DefaultTDLCollectionDetails == null || child.DefaultTDLCollectionDetails.CollectionName == null)
                {
                    intializerArgs
                    =
                    [
                        AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("Repeat"),
                                         IdentifierName(child.ReportVarName)),
                    ];
                    lineConstructorArgs.Add(Argument(LiteralExpression(SyntaxKind.NullLiteralExpression)));
                }
                else
                {
                    if (child.ListXmlTag != null)
                    {
                        intializerArgs
                            =
                            [
                                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("XMLTag"),
                                         LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.ListXmlTag))),
                            ];
                    }
                    lineConstructorArgs.Add(Argument(IdentifierName(GetCollectionName(child))));
                }


                statements.Add(GetArrayAssignmentExppressionImplicit(PartsVariableName, counter,
                    lineConstructorArgs, intializerArgs));
                counter += 1;
            }
            else
            {
                AddPartSyntaxForComplexField(child);
                int localCounter = 1;
                foreach (var xMLData in child.XMLData)
                {
                    if (xMLData.ChildSymbolData != null)
                    {
                        AddPartSyntaxForComplexField(xMLData.ChildSymbolData, localCounter);
                        localCounter++;
                    }
                }
                void AddPartSyntaxForComplexField(ChildSymbolData child, int? localCounter = null)
                {
                    string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{PartsVariableName.Substring(0, 1).ToUpper()}{PartsVariableName.Substring(1)}";
                    List<SyntaxNodeOrToken> methodArgs =
                    [
                        Argument(IdentifierName(child.ReportVarName)),
                    Token(SyntaxKind.CommaToken),
                    Argument(child.DefaultTDLCollectionDetails != null && child.DefaultTDLCollectionDetails.CollectionName != null ? IdentifierName(GetCollectionName(child,localCounter)) : LiteralExpression(SyntaxKind.NullLiteralExpression))
                    ];
                    if (child.ListXmlTag != null)
                    {
                        methodArgs.AddRange([Token(SyntaxKind.CommaToken), Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.ListXmlTag)))]);
                    }
                    if (child.Exclude)
                    {
                        statements.Add(CreateArrayAssignmentwithExpression(PartsVariableName, counter, InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                       GetGlobalNameforType(_symbol.MainFullName),
                                                                                                       IdentifierName(string.Format(GetMainTDLPartMethodName, child.SymbolData?.TypeName ?? child.Name))))
                            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(methodArgs)))));
                        counter += 1;
                        return;
                    }
                    statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                           InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                       GetGlobalNameforType(_symbol.MainFullName),
                                                                                                       IdentifierName(string.Format(GetTDLPartsMethodName, child.SymbolData?.TypeName ?? child.Name))))
                                                           .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(methodArgs)))));

                    statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                   IdentifierName(PartsVariableName),
                                                                                                   IdentifierName(AddToArrayExtensionMethodName)))

                        .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                             new SyntaxNodeOrToken[]{
                            Argument(IdentifierName(ComplexFieldsVariableName)),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(counter))),
                             })))
                        ));
                    counter += (child.SymbolData?.TDLPartsCount ?? 0) + 1;
                    return;
                }
            }

        }
        statements.Add(ReturnStatement(IdentifierName(PartsVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(PartFullTypeName),
                                                        Identifier(string.Format(GetTDLPartsMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        if (_symbol.IsChild)
        {
            methodDeclarationSyntax = methodDeclarationSyntax
                .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
                {
                    Parameter(Identifier(partNameArgName))
                    .WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                    .WithDefault(EqualsValueClause(IdentifierName(_reportVariableName))),
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
        }
        return [MainPartMethod, methodDeclarationSyntax];


    }

    private string GetCollectionName(ChildSymbolData child, int? localCounter)
    {
        string v = GetCollectionName(child);
        if (localCounter == null)
        {
            return v;
        }
        return $"{v}{localCounter}";
    }

    private MethodDeclarationSyntax GenerateMainPartMethodSyntax(string CollectionNameArgName,
                                              string partNameArgName,
                                              string XmlTagArgName,
                                              string PartsVariableName)
    {
        List<StatementSyntax> statements = [];
        List<SyntaxNodeOrToken> constructorArgs = [
                    Argument(IdentifierName(_symbol.IsChild ? partNameArgName : _reportVariableName)),
            Token(SyntaxKind.CommaToken),
        ];

        if (_includeCollection)
        {
            if (_symbol.IsChild)
            {
                constructorArgs.Add(Argument(IdentifierName(CollectionNameArgName)));
            }
            else
            {
                constructorArgs.Add(Argument(IdentifierName(_collectionVariableName)));
            }
        }
        else
        {
            constructorArgs.Add(Argument(CreateStringLiteral(_symbol.TDLCollectionDetails.CollectionName)));
        }
        List<SyntaxNodeOrToken>? firstPartIntializerArgs = null;
        if (_symbol.IsChild)
        {
            constructorArgs.AddRange([Token(SyntaxKind.CommaToken), Argument(IdentifierName(_symbol.IsChild ? partNameArgName : _reportVariableName))]);
            firstPartIntializerArgs = [AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("XMLTag"),
                                         IdentifierName(XmlTagArgName))];
        }
        statements.Add(ReturnStatement(CreateImplicitObjectExpression(constructorArgs, firstPartIntializerArgs)));

        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(PartFullTypeName),
                                                        Identifier(string.Format(GetMainTDLPartMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        if (_symbol.IsChild)
        {
            methodDeclarationSyntax = methodDeclarationSyntax
                .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
                {
                    Parameter(Identifier(partNameArgName))
                    .WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                    .WithDefault(EqualsValueClause(IdentifierName(_reportVariableName))),
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
        }
        return methodDeclarationSyntax;
    }

    private MemberDeclarationSyntax GenerateGetLinesMethodSyntax()
    {
        const string xmlTagArgName = "xmlTag";
        const string LinesVariableName = "lines";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(LineFullTypeName, LinesVariableName, _symbol.TDLLinesCount + 1));
        List<SyntaxNodeOrToken> args = [];

        var childSymbolDatas = _simpleChildren.Where(c => !c.IsList && !c.IsAttribute).Select(c => GetFieldName(c)).ToList();
        const int count = 5;
        if (childSymbolDatas.Count == 0 && _symbol.BaseSymbolData == null)
        {
            args.Add(ExpressionElement(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("SimpleField"))));
        }
        else
        {
            for (int i = 0; i < childSymbolDatas.Count; i += count)
            {
                if (i != 0)
                {
                    args.Add(Token(SyntaxKind.CommaToken));
                }
                string v = string.Join(",", childSymbolDatas.Skip(i).Take(count));
                args.Add(ExpressionElement(
                    IdentifierName(v)));

            }
        }

        List<SyntaxNodeOrToken>? intializerArgs = null;
        if (_complexChildren.Count > 0)
        {
            List<SyntaxNodeOrToken> explodes = [];
            for (int i = 0; i < _complexChildren.Count; i++)
            {
                var child = _complexChildren[i];

                AddExplodeArgForComplexChild(child);
                int Counter = 1;
                foreach (var xMLData in child.XMLData)
                {
                    if (xMLData.ChildSymbolData != null)
                    {
                        AddExplodeArgForComplexChild(xMLData.ChildSymbolData);
                        Counter++;
                    }
                }
                void AddExplodeArgForComplexChild(ChildSymbolData child)
                {
                    if (explodes.Count != 0)
                    {
                        explodes.Add(Token(SyntaxKind.CommaToken));
                    }
                    string text = ":YES";
                    List<InterpolatedStringContentSyntax> nodes =
                        [
                            Interpolation(IdentifierName(child.ReportVarName)),
                        //InterpolatedStringText()
                        //.WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList()))
                    ];
                    if (child.DefaultTDLCollectionDetails?.ExplodeCondition == null)
                    {
                        nodes.Add(InterpolatedStringText()
                            .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
                    }
                    else
                    {
                        text = ":";
                        nodes.Add(InterpolatedStringText()
                            .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
                        nodes.Add(Interpolation(CreateFormatExpression(child.DefaultTDLCollectionDetails.ExplodeCondition, child.TDLFieldDetails?.Set ?? "")));
                    }
                    InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
                        .WithContents(List(nodes));
                    explodes.Add(ExpressionElement(interpolatedStringExpressionSyntax));
                }
            }

            intializerArgs =
                [
                    AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("Explode"),
                                         CollectionExpression(SeparatedList<CollectionElementSyntax>(explodes)))
                ];
        }
        if (_symbol.BaseSymbolData != null)
        {
            if (intializerArgs == null)
            {
                intializerArgs = [];
            }
            else
            {
                intializerArgs.Add(Token(SyntaxKind.CommaToken));
            }
            intializerArgs.AddRange(
                [
                    AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("Use"), IdentifierName($"{_symbol.BaseSymbolData.SymbolData.TypeName}{nameof(ReportName)}"))
                ]);
        }
        List<SyntaxNodeOrToken> lineIntializerDeleteArgs = [];
        foreach (var item in _simpleChildren.Where(c => c.IsOverridden))
        {
            List<InterpolatedStringContentSyntax> nodes = [];
            var prefixText = "Field:";
            nodes.Add(InterpolatedStringText().WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, prefixText, prefixText, TriviaList())));
            nodes.Add(Interpolation(IdentifierName(GetFieldName(item.OverriddenChild!))));
            InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken)).WithContents(List(nodes));
            SafeAdd(lineIntializerDeleteArgs, ExpressionElement(interpolatedStringExpressionSyntax));
        }
        foreach (var item in _complexChildren.Where(c => c.IsOverridden))
        {
            ChildSymbolData childSymbolData = item.OverriddenChild!;
            AddExplodeArgForComplexChild(childSymbolData);
            int Counter = 1;
            foreach (var xMLData in childSymbolData.XMLData)
            {
                if (xMLData.ChildSymbolData != null)
                {
                    AddExplodeArgForComplexChild(xMLData.ChildSymbolData);
                    Counter++;
                }
            }
            void AddExplodeArgForComplexChild(ChildSymbolData childSymbolData)
            {
                List<InterpolatedStringContentSyntax> nodes = [];
                var prefixText = "Explode:";
                nodes.Add(InterpolatedStringText().WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, prefixText, prefixText, TriviaList())));
                nodes.Add(Interpolation(IdentifierName(childSymbolData.ReportVarName)));
                InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken)).WithContents(List(nodes));
                SafeAdd(lineIntializerDeleteArgs, ExpressionElement(interpolatedStringExpressionSyntax));
            }

        }
        if (lineIntializerDeleteArgs.Count > 0)
        {
            intializerArgs ??= [];
            SafeAdd(intializerArgs, AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                             IdentifierName("Delete"), CollectionExpression(SeparatedList<CollectionElementSyntax>(lineIntializerDeleteArgs))));
        }
        var childSymbolAttributeDatas = _simpleChildren.Where(c => !c.IsList && c.IsAttribute).ToList();
        if (childSymbolAttributeDatas.Count > 0)
        {
            intializerArgs ??= [];
            List<SyntaxNodeOrToken> xmlAttrArgs = [];
            foreach (var attrData in childSymbolAttributeDatas)
            {
                SafeAdd(xmlAttrArgs, ExpressionElement(CreateStringLiteral($"{attrData.XmlTag}:{attrData.TDLFieldDetails!.Set}")));
            }
            SafeAdd(intializerArgs, AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                            IdentifierName("XMLAttributes"), CollectionExpression(SeparatedList<CollectionElementSyntax>(xmlAttrArgs))));
        }
        statements.Add(GetArrayAssignmentExppressionImplicit(LinesVariableName,
                                                 0,
                                                 [
                                                     Argument(IdentifierName(_reportVariableName)),
                                                     Token(SyntaxKind.CommaToken),
                                                     Argument(CollectionExpression(
                                                         SeparatedList<CollectionElementSyntax>(
                                                             args))),
                                                     Token(SyntaxKind.CommaToken),
                                                     Argument(_symbol.IsChild ? IdentifierName(xmlTagArgName) : LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(_symbol.RootXmlTag)))
                                                 ],
                                                 intializerArgs));
        int counter = 1;
        if (_symbol.BaseSymbolData != null && _symbol.BaseSymbolData.Exclude == false)
        {
            string ComplexFieldsVariableName = $"{_symbol.BaseSymbolData.Name.Substring(0, 1).ToLower()}{_symbol.BaseSymbolData.Name.Substring(1)}{LinesVariableName.Substring(0, 1).ToUpper()}{LinesVariableName.Substring(1)}";
            statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                   InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                               GetGlobalNameforType(_symbol.MainFullName),
                                                                                               IdentifierName(string.Format(GetTDLLinesMethodName, _symbol.BaseSymbolData.SymbolData.TypeName))))));

            statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                           IdentifierName(LinesVariableName),
                                                                                           IdentifierName(AddToArrayExtensionMethodName)))
                .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                     new SyntaxNodeOrToken[]{
                            Argument(IdentifierName(ComplexFieldsVariableName)),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(counter))),
                     })))
                ));
            counter += (_symbol.BaseSymbolData?.SymbolData.TDLLinesCount ?? 0) + 1;

        }
        foreach (var child in _complexChildren)
        {
            if (child.IsList && !child.IsComplex)
            {
                statements.Add(GetArrayAssignmentExppressionImplicit(LinesVariableName, counter,
                    [
                       Argument(IdentifierName(child.ReportVarName)),
                        Token(SyntaxKind.CommaToken),
                        Argument(CollectionExpression(SeparatedList<CollectionElementSyntax>(
                                                                 new SyntaxNodeOrToken[]
                                                                 {
                                                                     ExpressionElement(IdentifierName(GetFieldName(child)))
                                                                 })))

                    ]));
                counter += 1;
                continue;
            }
            AddLineSyntaxForComplexField(child);
            foreach (var xMLData in child.XMLData)
            {
                if (xMLData.ChildSymbolData != null)
                {
                    AddLineSyntaxForComplexField(xMLData.ChildSymbolData);
                }
            }
            void AddLineSyntaxForComplexField(ChildSymbolData child)
            {
                List<SyntaxNodeOrToken> constructorArgs =
                    [
                        Argument(IdentifierName(child.ReportVarName)),
                    Token(SyntaxKind.CommaToken),
                    Argument(CollectionExpression(SeparatedList<CollectionElementSyntax>(new List<SyntaxNodeOrToken>()))),
                    Token(SyntaxKind.CommaToken),
                    Argument(CreateStringLiteral(child.XmlTag)),
                ];
                List<SyntaxNodeOrToken> lineIntializerArgs =
                    [
                        AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("Use"), IdentifierName($"{child.SymbolData!.TypeName}{nameof(ReportName)}"))
                    ];
                List<SyntaxNodeOrToken> lineIntializerLocalArgs = [];

                if (child.SymbolData?.IsTallyComplexObject ?? false)
                {
                    int i = 0;

                    foreach (var item in child.SymbolData.Children)
                    {
                        var c = item.Value;
                        string? set = child.TDLFieldDetails!.Set;
                        InterpolatedStringExpressionSyntax createStringInterpolation(string middleText, Func<TDLFieldData, string> func)
                        {
                            List<InterpolatedStringContentSyntax> nodes = [];
                            var prefixText = "Field    \t: ";
                            nodes.Add(InterpolatedStringText()
                                .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, prefixText, prefixText, TriviaList())));
                            nodes.Add(Interpolation(IdentifierName(GetFieldName(c))));

                            nodes.Add(InterpolatedStringText()
                                .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, middleText, middleText, TriviaList())));

                            nodes.Add(Interpolation(CreateFormatExpression(func(c.TDLFieldDetails!), set)));
                            InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken)).WithContents(List(nodes));

                            return interpolatedStringExpressionSyntax;
                        }

                        SafeAdd(lineIntializerLocalArgs, ExpressionElement(createStringInterpolation("\t: Set \t: ", c => c.Set!)));
                        if (c.TDLFieldDetails?.Invisible != null)
                        {
                            SafeAdd(lineIntializerLocalArgs, ExpressionElement(createStringInterpolation("\t: Invisible \t: ", c => c.Invisible!)));
                        }
                        i++;
                    }

                }

                SafeAdd(lineIntializerArgs, AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                             IdentifierName("Local"), CollectionExpression(SeparatedList<CollectionElementSyntax>(lineIntializerLocalArgs))));
                statements.Add(GetArrayAssignmentExppressionImplicit(LinesVariableName, counter, constructorArgs, lineIntializerArgs));
                counter++;
                if (child.Exclude)
                {
                    return;
                }
                string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{LinesVariableName.Substring(0, 1).ToUpper()}{LinesVariableName.Substring(1)}";
                statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                       InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                   GetGlobalNameforType(_symbol.MainFullName),
                                                                                                   IdentifierName(string.Format(GetTDLLinesMethodName, child.SymbolData?.TypeName ?? child.Name))))
                                                       .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                         new SyntaxNodeOrToken[]{
                            Argument(CreateStringLiteral(child.XmlTag)),
                         })))));

                statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                               IdentifierName(LinesVariableName),
                                                                                               IdentifierName(AddToArrayExtensionMethodName)))
                    .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                         new SyntaxNodeOrToken[]{
                            Argument(IdentifierName(ComplexFieldsVariableName)),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(counter))),
                         })))
                    ));
                counter += (child.SymbolData?.TDLLinesCount ?? 0) + 1;
            }
        }
        statements.Add(ReturnStatement(IdentifierName(LinesVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(LineFullTypeName),
                                                        Identifier(string.Format(GetTDLLinesMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        if (_symbol.IsChild)
        {
            methodDeclarationSyntax = methodDeclarationSyntax
                .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
                {
                    Parameter(Identifier(xmlTagArgName))
                    .WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                    .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                     Literal(_symbol.RootXmlTag))))
                })));
        }
        return methodDeclarationSyntax;


    }

    private MemberDeclarationSyntax GenerateGetFieldsMethodSyntax()
    {
        const string FieldsVariableName = "fields";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(FieldFullTypeName, FieldsVariableName, _symbol.SimpleFieldsCount));
        var counter = 0;
        if (_symbol.BaseSymbolData != null)
        {
            var child = _symbol.BaseSymbolData;
            if (!child.Exclude)
            {


                string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{FieldsVariableName.Substring(0, 1).ToUpper()}{FieldsVariableName.Substring(1)}";
                statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                       InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                   GetGlobalNameforType(_symbol.MainFullName),
                                                                                                   IdentifierName(string.Format(GetTDLFieldsMethodName, child.SymbolData.TypeName))))));

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
                counter += child.SymbolData.SimpleFieldsCount;
            }
        }
        foreach (var child in _symbol.Children.Values)
        {
            if (child.Exclude)
            {
                continue;
            }
            if (!child.IsComplex)
            {
                if (child.IsAttribute)
                {
                    continue;
                }
                string name = GetFieldName(child);
                List<SyntaxNodeOrToken> args = [Argument(IdentifierName(name)),
                    Token(SyntaxKind.CommaToken),
                    Argument(CreateStringLiteral(child.XmlTag)),
                    Token(SyntaxKind.CommaToken),
                    Argument(child.Parent.IsTallyComplexObject ? LiteralExpression(
                                                    SyntaxKind.NullLiteralExpression) : CreateStringLiteral(child.TDLFieldDetails!.Set!)),
                ];
                List<SyntaxNodeOrToken>? intializerArgs = null;
                void SafeAdd(SyntaxNodeOrToken token)
                {
                    intializerArgs ??= [];
                    if (intializerArgs.Count != 0)
                    {
                        intializerArgs.Add(Token(SyntaxKind.CommaToken));
                    }
                    intializerArgs.Add(token);
                }
                if (child.TDLFieldDetails!.TallyType != null)
                {
                    SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("Type"),
                                         LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.TDLFieldDetails.TallyType))));
                }

                if (child.TDLFieldDetails.Format != null)
                {
                    SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("Format"),
                                         LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.TDLFieldDetails.Format))));
                }
                if (!child.Parent.IsTallyComplexObject)
                {
                    if (child.TDLFieldDetails.Invisible != null)
                    {
                        SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                             IdentifierName("Invisible"),
                                             child.Parent.IsTallyComplexObject ? CreateFormatExpression(child.TDLFieldDetails!.Invisible!) : CreateStringLiteral(child.TDLFieldDetails!.Invisible!)));
                    }
                }


                var expressionStatementSyntax = GetArrayAssignmentExppressionImplicit(FieldsVariableName, counter, args, intializerArgs);
                statements.Add(expressionStatementSyntax);
                counter++;
            }
            else
            {
                string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{FieldsVariableName.Substring(0, 1).ToUpper()}{FieldsVariableName.Substring(1)}";
                List<SyntaxNodeOrToken>? arguments = [];
                //if (child.SymbolData!.IsTallyComplexObject)
                //{
                //    lineIntializerDeleteArgs.Add(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.TDLFieldDetails!.Set!))));
                //}
                statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                       InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                   GetGlobalNameforType(_symbol.MainFullName),
                                                                                                   IdentifierName(string.Format(GetTDLFieldsMethodName, child.SymbolData?.TypeName ?? child.Name))))
                                                       .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(arguments)))));

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
                counter += child.SymbolData?.SimpleFieldsCount ?? 0;
            }
        }
        statements.Add(ReturnStatement(IdentifierName(FieldsVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(FieldFullTypeName),
                                                        Identifier(string.Format(GetTDLFieldsMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        //if (symbol.IsTallyComplexObject)
        //{

        //    methodDeclarationSyntax = methodDeclarationSyntax
        //        .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
        //        {
        //            Parameter(Identifier(setValueParamName))
        //            .WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
        //        })));
        //}
        return methodDeclarationSyntax;


    }
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
    private MemberDeclarationSyntax GenerateGetCollectionsMethodSyntax()
    {
        const string CollectionsVariableName = "collections";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(CollectionFullTypeName, CollectionsVariableName, 1));
        List<SyntaxNodeOrToken> nodesAndTokens =
            [
              SpreadElement(  InvocationExpression(IdentifierName(string.Format(GetFetchListMethodName,_symbol.TypeName))).WithArgumentList(ArgumentList()))
            ];


        statements.Add(GetArrayAssignmentExppressionImplicit(CollectionsVariableName, 0,
            [
                Argument(IdentifierName(_collectionVariableName)),
                Token(SyntaxKind.CommaToken),
                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(_symbol.TDLCollectionDetails?.Type ?? _symbol.RootXmlTag))),
                Token(SyntaxKind.CommaToken),
                Argument(CollectionExpression(SeparatedList<CollectionElementSyntax>(nodesAndTokens)))
                .WithNameColon(NameColon(IdentifierName("nativeFields")))
            ]));
        statements.Add(ReturnStatement(IdentifierName(CollectionsVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(CollectionFullTypeName),
                                                        Identifier(string.Format(GetTDLCollectionsMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        return methodDeclarationSyntax;


    }


    private MemberDeclarationSyntax GenerateGetFetchListMethodSyntax()
    {
        const string prefixParamName = "prefix";
        List<SyntaxNodeOrToken> nodesAndTokens = [];
        //HashSet<string> names = new HashSet<string>();
        AddFetchItems(_symbol);

        void AddFetchItems(SymbolData symbol)
        {
            if (symbol.BaseSymbolData != null)
            {
                //AddFetchItems(symbol.BaseSymbolData.SymbolData, symbol.IsChild && !symbol.IsBaseSymbol);
                var exp = InvocationExpression(IdentifierName(string.Format(GetFetchListMethodName, symbol.BaseSymbolData.SymbolData.TypeName)));
                if (symbol.BaseSymbolData.IsParentChild)
                {
                    exp = exp.WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[] { Argument(IdentifierName("prefix")) })));
                }
                SafeAdd(nodesAndTokens, SpreadElement(exp));
            };
            var children = symbol.Children.Values.Where(c => !c.IsComplex || (c.SymbolData?.IsTallyComplexObject ?? false))
                .Where(child => !(child.TDLFieldDetails?.ExcludeInFetch ?? false))
                .Select(child => child.TDLFieldDetails?.FetchText ?? child.TDLFieldDetails?.Set)
                .Where(c => c != null)
                .ToList();
            int chunkSize = 10;
            for (int i = 0; i < children.Count; i += chunkSize)
            {
                var chunks = children.Skip(i).Take(chunkSize).ToList();

                if (symbol.IsChild && (!_symbol.IsBaseSymbol || _symbol.IsParentChild))
                {
                    List<InterpolatedStringContentSyntax> nodes = [];
                    int lastIndex = chunks.Count - 1;
                    for (int k = 0; k < chunks.Count; k++)
                    {
                        var chunk = chunks[k];
                        nodes.Add(Interpolation(IdentifierName(prefixParamName)));

                        string text = (k == lastIndex) ? $".{chunk}" : $".{chunk} ,";
                        nodes.Add(InterpolatedStringText().WithTextToken(Token(
                                                                                TriviaList(),
                                                                                SyntaxKind.InterpolatedStringTextToken,
                                                                                text,
                                                                                text,
                                                                                TriviaList())));
                    }
                    var stringSyntax = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
                        .WithContents(List(nodes));
                    SafeAdd(nodesAndTokens, ExpressionElement(stringSyntax));
                }
                else
                {
                    SafeAdd(nodesAndTokens, ExpressionElement(CreateStringLiteral(string.Join(", ", chunks))));
                }


            }

            IEnumerable<ChildSymbolData> complexCildren = symbol.Children.Values.Where(c => (c.IsComplex || c.IsList));
            foreach (var child in complexCildren)
            {

                if (child.ChildTypeFullName == symbol.FullName)
                {
                    return;
                }
                AddFetchListSyntaxofComplexChild(child);


                void AddFetchListSyntaxofComplexChild(ChildSymbolData child, int? counter = null)
                {
                    if (child.DefaultTDLCollectionDetails == null || child.DefaultTDLCollectionDetails.CollectionName == null)
                    {
                        return;
                    }
                    if (!child.IsComplex)
                    {
                        return;
                    }
                    var exp = InvocationExpression(IdentifierName(string.Format(GetFetchListMethodName, child.SymbolData!.TypeName)))
                        .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[] { Argument(IdentifierName(GetCollectionName(child, counter))) })));

                    //SafeAdd(args, ExpressionElement(IdentifierName(GetCollectionName(child))));
                    SafeAdd(nodesAndTokens, SpreadElement(exp));
                }
                int counter = 1;
                foreach (var item in child.XMLData)
                {
                    if (item.ChildSymbolData != null)
                    {
                        AddFetchListSyntaxofComplexChild(item.ChildSymbolData, counter);
                    }
                    counter++;
                }
            }


        }

        List<StatementSyntax> statements = [ReturnStatement(CollectionExpression(SeparatedList<CollectionElementSyntax>(nodesAndTokens)))];
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                                        Identifier(string.Format(GetFetchListMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        if (_symbol.IsChild && (!_symbol.IsBaseSymbol || _symbol.IsParentChild))
        {
            methodDeclarationSyntax = methodDeclarationSyntax.WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
                  {
                    Parameter(Identifier(prefixParamName))
                    .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))) })));



        }
        return methodDeclarationSyntax;
    }
    private static void SafeAdd(List<SyntaxNodeOrToken> nodesAndTokens, SyntaxNodeOrToken item)
    {
        if (nodesAndTokens.Count != 0)
        {
            nodesAndTokens.Add(Token(SyntaxKind.CommaToken));
        }
        nodesAndTokens.Add(item);
    }

    private MemberDeclarationSyntax GenerateGetNamesetsMethodSyntax()
    {
        const string nameSetsVariableName = "nameSets";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(TDLNameSetFullTypeName, nameSetsVariableName, 1));
        List<SyntaxNodeOrToken> intializerArgs = [];

        List<SyntaxNodeOrToken> nodesAndTokens = [];

        foreach (var childitem in _symbol.Children)
        {
            var child = childitem.Value;
            if (child.Name == "None")
            {
                continue;
            }

            if (child.EnumChoices != null)
            {
                foreach (var item in child.EnumChoices)
                {
                    if (nodesAndTokens.Count != 0)
                    {
                        nodesAndTokens.Add(Token(SyntaxKind.CommaToken));
                    }
                    nodesAndTokens.Add(ExpressionElement(CreateStringLiteral($"{item.Choice}:\"{child.Name}\"")));
                }
            }


        }
        intializerArgs.Add(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                         IdentifierName("List"),
                                                         CollectionExpression(SeparatedList<CollectionElementSyntax>(nodesAndTokens))));
        statements.Add(GetArrayAssignmentExppressionImplicit(nameSetsVariableName,
                                                     0,
                                                     [Argument(CreateStringLiteral(string.Format(GetEnumNameSetName, _symbol.Name)))],
                                                     intializerArgs));
        statements.Add(ReturnStatement(IdentifierName(nameSetsVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(TDLNameSetFullTypeName),
                                                        Identifier(string.Format(GetTDLNameSetsMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        return methodDeclarationSyntax;


    }
    private MemberDeclarationSyntax GenerateGetFunctionsMethodSyntax()
    {
        const string functionsVariableName = "functions";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(TDLFunctionFullTypeName, functionsVariableName, 1));

        List<SyntaxNodeOrToken> intializerArgs = [];
        intializerArgs.Add(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                         IdentifierName("Parameters"),
                                                         CollectionExpression(SeparatedList<CollectionElementSyntax>(new SyntaxNodeOrToken[]
                                                         {
                                                             ExpressionElement(CreateStringLiteral($"val : String : \"\""))
                                                         }))));
        intializerArgs.Add(Token(SyntaxKind.CommaToken));
        intializerArgs.Add(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                         IdentifierName("Actions"),
                                                         CollectionExpression(SeparatedList<CollectionElementSyntax>(new SyntaxNodeOrToken[]
                                                         {
                                                             ExpressionElement(CreateStringLiteral($"001 :Return : $$NameGetValue:##Val:{string.Format(GetEnumNameSetName,_symbol.Name)}"))
                                                         }))));
        statements.Add(GetArrayAssignmentExppressionImplicit(functionsVariableName,
                                                     0,
                                                     [Argument(CreateStringLiteral(string.Format(GetEnumFunctionName, _symbol.Name)))],
                                                     intializerArgs));

        statements.Add(ReturnStatement(IdentifierName(functionsVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(TDLFunctionFullTypeName),
                                                        Identifier(string.Format(GetTDLFunctionsMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        return methodDeclarationSyntax;
    }

    private MemberDeclarationSyntax GenerateGetTallyStringMethodSyntax()
    {
        List<SyntaxNodeOrToken> switchStatements = [];
        var objParamName = "obj";
        var versionVarName = "version";

        bool usesVersion = false;
        SafeAdd(switchStatements, SwitchExpressionArm(
                                             ConstantPattern(LiteralExpression(
                                                                      SyntaxKind.NullLiteralExpression)),
                                              LiteralExpression(SyntaxKind.NullLiteralExpression)));
        foreach (var childitem in _symbol.Children)
        {
            var child = childitem.Value;
            ExpressionSyntax? expressionSyntax = null;
            if (child.EnumChoices != null)
            {
                if (child.EnumChoices.Count == 1)
                {
                    expressionSyntax = LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.EnumChoices.First().Choice));
                }
                else if (child.EnumChoices.Count == 0)
                {
                    expressionSyntax = child.Name == "None" ? GetEmptyStringSyntax() : LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.Name));
                }
                else
                {
                    usesVersion = true;
                    List<SyntaxNodeOrToken> nestedSwitchStatements = [];
                    var addedDefault = false;
                    foreach (var item in child.EnumChoices)
                    {
                        ExpressionSyntax constExpression;
                        BinaryPatternSyntax? binaryPatternSyntax = null;
                        constExpression = LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(""));

                        if (item.Versions.Length == 1)
                        {
                            constExpression = LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(item.Versions.First()));
                        }
                        else if (item.Versions.Length > 1)
                        {
                            binaryPatternSyntax = BinaryPattern(SyntaxKind.OrPattern, ConstantPattern(LiteralExpression(
                                                                                SyntaxKind.StringLiteralExpression,
                                                                                Literal(item.Versions[0]))), ConstantPattern(LiteralExpression(
                                                                                SyntaxKind.StringLiteralExpression,
                                                                                Literal(item.Versions[1]))));
                            for (int i = 2; i < item.Versions.Length; i++)
                            {
                                binaryPatternSyntax = BinaryPattern(SyntaxKind.OrPattern, binaryPatternSyntax, ConstantPattern(LiteralExpression(
                                                                                SyntaxKind.StringLiteralExpression,
                                                                                Literal(item.Versions[i]))));
                            }
                        }
                        if (item.Versions.Length == 0)
                        {
                            addedDefault = true;
                            SafeAdd(nestedSwitchStatements, SwitchExpressionArm(DiscardPattern(), LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                                                                             Literal(item.Choice))));
                        }
                        else
                        {
                            SafeAdd(nestedSwitchStatements, SwitchExpressionArm(binaryPatternSyntax == null ? ConstantPattern(constExpression) : binaryPatternSyntax, LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                                                                                Literal(item.Choice))));
                        }

                    }
                    if (!addedDefault)
                    {
                        SafeAdd(nestedSwitchStatements, SwitchExpressionArm(DiscardPattern(), LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                                                                             Literal(child.EnumChoices.Where(c => !c.Versions.Any()).First().Choice))));
                    }
                    expressionSyntax = SwitchExpression(IdentifierName(versionVarName)).WithArms(SeparatedList<SwitchExpressionArmSyntax>(nestedSwitchStatements));
                }
            }
            else
            {
                expressionSyntax = LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.Name));
            }
            expressionSyntax ??= GetEmptyStringSyntax();

            SafeAdd(switchStatements, SwitchExpressionArm(ConstantPattern(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, GetGlobalNameforType(_symbol.FullName), IdentifierName(child.Name))),
                expressionSyntax));
        }

        SafeAdd(switchStatements, SwitchExpressionArm(
                                                DiscardPattern(),
                                                GetEmptyStringSyntax()));


        var methodDeclarationSyntax = MethodDeclaration(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword))), Identifier(GetTallyStringMethodName))
     .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
     .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
     {
         Parameter(Identifier(objParamName))
                .WithType(NullableType(GetGlobalNameforType(_symbol.FullName))),
         Token(SyntaxKind.CommaToken),
         Parameter(Identifier(LicenseInfoPropertyName))
         .WithType(GetGlobalNameforType(LicenseInfoFullTypeName))
     })));
        if (usesVersion)
        {
            List<StatementSyntax> statements = [];

            statements.Add(CreateVarInsideMethodWithExpression(versionVarName, InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                      IdentifierName(LicenseInfoPropertyName),
                                                                                                      IdentifierName($"{ShortVersionPropertyName}.ToString")))));

            statements.Add(ReturnStatement(SwitchExpression(
                                IdentifierName(objParamName))
                        .WithArms(SeparatedList<SwitchExpressionArmSyntax>(switchStatements))));
            methodDeclarationSyntax = methodDeclarationSyntax.WithBody(Block(statements));
        }
        else
        {
            methodDeclarationSyntax = methodDeclarationSyntax.WithExpressionBody(ArrowExpressionClause(SwitchExpression(
                                IdentifierName(objParamName))
                        .WithArms(SeparatedList<SwitchExpressionArmSyntax>(switchStatements))))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        return methodDeclarationSyntax;
    }


    private string GetFieldName(ChildSymbolData child)
    {
        return $"{child.Parent.TypeName}{child.Name}TDLFieldName";
    }


    private string GetCollectionName(ChildSymbolData child)
    {
        if (child.IsList && !child.IsComplex)
        {
            return $"{_symbol.TypeName}{child.Name}CollectionName";
        }
        return $"{_symbol.TypeName}{child.ChildType.Name}CollectionName";
    }
    private StatementSyntax CreateAssignFromMethodStatement(string varName,
                                                            string propName,
                                                            List<string> methodNames)
    {
        List<SyntaxNodeOrToken> nodes = [];
        for (int i = 0; i < methodNames.Count; i++)
        {
            var methodName = methodNames[i];
            if (i != 0)
            {
                nodes.Add(Token(SyntaxKind.CommaToken));
            }
            nodes.Add(SpreadElement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                GetGlobalNameforType(_symbol.MainFullName), IdentifierName(methodName)))
                                                   ));
        }

        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(varName),
                                    IdentifierName(propName)),
                                    CollectionExpression(SeparatedList<CollectionElementSyntax>(nodes))));
    }
    private StatementSyntax CreateAssignFromMethodStatement(string varName,
                                                            string propName,
                                                            List<ExpressionSyntax> expressions)
    {
        return CreateAssignFromMethodStatement(varName, propName, CreateAssignFromMethodStatementRightExp(expressions));
    }
    private StatementSyntax CreateAssignFromMethodStatement(string varName,
                                                            string propName, ExpressionSyntax right)
    {

        AssignmentExpressionSyntax assignmentExpression = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(varName),
                                            IdentifierName(propName)),
                                            right);
        return ExpressionStatement(assignmentExpression);
    }


    private static CollectionExpressionSyntax CreateAssignFromMethodStatementRightExp(List<ExpressionSyntax> expressions)
    {
        List<SyntaxNodeOrToken> nodes = [];
        for (int i = 0; i < expressions.Count; i++)
        {
            var expression = expressions[i];
            if (i != 0)
            {
                nodes.Add(Token(SyntaxKind.CommaToken));
            }

            nodes.Add(SpreadElement(expression));
        }

        CollectionExpressionSyntax right = CollectionExpression(SeparatedList<CollectionElementSyntax>(nodes));
        return right;
    }

    public string GetCreateDTOCompilationUnitString()
    {


        ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(GetDTOClassName());
        List<SyntaxNodeOrToken> nodesAndTokens = [];
        if (_symbol.BaseSymbolData != null)
        {
            SafeAdd(nodesAndTokens, SimpleBaseType(
                            IdentifierName(_symbol.BaseSymbolData.Name + "DTO")));
        }
        if (!_symbol.IsChild)
        {

            SafeAdd(nodesAndTokens, SimpleBaseType(
                            GetGlobalNameforType(TallyObjectDTOInterfaceName)));

        }
        if (nodesAndTokens.Count > 0)
        {

            classDeclarationSyntax = classDeclarationSyntax.WithBaseList(BaseList(SeparatedList<BaseTypeSyntax>(nodesAndTokens)));
        }
        var unit = CompilationUnit()
            .WithUsings(List([UsingDirective(IdentifierName(ExtensionsNameSpace)), UsingDirective(IdentifierName("System.Linq"))]))
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                FileScopedNamespaceDeclaration(IdentifierName($"{_symbol.MainNameSpace}.Models"))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                      true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    classDeclarationSyntax
                    .WithAttributeLists(List(new AttributeListSyntax[]{
                        AttributeList(SeparatedList<AttributeSyntax>(new SyntaxNodeOrToken[]
                        {
                             Attribute(GetGlobalNameforType(XmlRootAttributeName))
                             .WithArgumentList( AttributeArgumentList(SingletonSeparatedList(
                                                    AttributeArgument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal(_symbol.RootXmlTag)))
                                                    .WithNameEquals(
                                                        NameEquals(
                                                            IdentifierName("ElementName"))))))
                        })).WithOpenBracketToken(
                            Token(
                                TriviaList(
                                    Comment($@"/*
 * Generated based on {_symbol.FullName} for Service {_symbol.MainFullName}
 */")),
                                SyntaxKind.OpenBracketToken,
                                TriviaList()))
                    }))
                    .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)]))
                    .WithMembers(List(GetCreateDTOClassMembers()))
                }))
            })).NormalizeWhitespace().ToFullString();
        if (!_symbol.IsChild)
        {

        }
        return unit;
    }

    private string GetDTOClassName()
    {
        return $"{_symbol.Name}DTO";
    }

    private IEnumerable<MemberDeclarationSyntax> GetCreateDTOClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        HashSet<string> ComplexTypeNames = [];
        //If Implements BaseLedgerEntryInterface then add extra property to DTO
        //if (_symbol.Symbol.HasInterfaceWithFullyQualifiedMetadataName(BaseLedgerEntryInterfaceName))
        //{
        //    members.Add(GetPropertyMemberSyntax(PredefinedType(Token(SyntaxKind.StringKeyword)), "IsDeemedPositive"));
        //}
        if (!_symbol.IsChild)
        {
            members.Add(GetPropertyMemberSyntax(GetGlobalNameforType(ActionEnumFullTypeName), "Action").WithAttributeLists(List(new AttributeListSyntax[]
            {
                 AttributeList(SingletonSeparatedList<AttributeSyntax>(Attribute(
                                        IdentifierName(XMLAttributeAttributeName))
                .WithArgumentList(AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    AttributeArgument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal("Action")))
                                                    .WithNameEquals(
                                                        NameEquals(
                                                            IdentifierName("AttributeName"))),
                                                    })))))
            })));
            if (!CheckSymbolHasProperty(_symbol, "RemoteId"))
            {
                members.Add(GetPropertyMemberSyntax(PredefinedType(Token(SyntaxKind.StringKeyword)), "RemoteId"));
            }
            // members.Add(GetPropertyMemberSyntax(PredefinedType(Token(SyntaxKind.StringKeyword)), "RemoteId"));
        }
        CreateProperties(_symbol);

        members.Add(CreateImplicitConverterSyntax());

        return members;

        void CreateProperties(SymbolData symbol, bool isBaseSymbol = false)
        {
            ComplexTypeNames.Add(symbol.FullName);
            //if (symbol.BaseSymbolData != null)
            //{
            //    CreateProperties(symbol.BaseSymbolData.SymbolData, true);
            //}
            foreach (var child in symbol.Children.Values)
            {
                if (child.IgnoreForCreateDTO)
                {
                    continue;
                }
                if (isBaseSymbol)
                {
                    if (child.OverriddenBy.Any() && child.OverriddenBy.Where(c => ComplexTypeNames.Contains(c.Parent.FullName)).Any())
                    {
                        continue;
                    }
                }
                TypeSyntax typeSyntax;
                List<SyntaxNodeOrToken> attributes = [];
                foreach (var attribute in child.Attributes)
                {
                    if (attribute.AttributeClass!.ContainingNamespace.OriginalDefinition.ToString().Contains("System.Xml.Serialization"))
                    {
                        AttributeSyntax item = Attribute(GetGlobalNameforType(attribute.AttributeClass.OriginalDefinition.ToString()));

                        if (attribute.NamedArguments.Length != 0)
                        {
                            List<SyntaxNodeOrToken> attributeArgs = [];

                            foreach (var namedArg in attribute.NamedArguments)
                            {
                                if (attributeArgs.Count != 0)
                                {
                                    attributeArgs.Add(Token(SyntaxKind.CommaToken));
                                }
                                switch (namedArg.Value.Type?.SpecialType)
                                {
                                    case SpecialType.System_String:
                                        attributeArgs.Add(AttributeArgument(
                                                            LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,
                                                                Literal(namedArg.Value.Value.ToString())))
                                    .WithNameEquals(NameEquals(IdentifierName(namedArg.Key))));
                                        break;
                                    case SpecialType.System_Boolean or SpecialType.System_Int16 or SpecialType.System_Int64 or SpecialType.System_Int32:
                                        attributeArgs.Add(AttributeArgument(IdentifierName(namedArg.Value.Value.ToString()))
                                    .WithNameEquals(NameEquals(IdentifierName(namedArg.Key))));
                                        break;
                                    default:
                                        if (namedArg.Value.Kind == TypedConstantKind.Type)
                                        {
                                            if (namedArg.Value.Value is INamedTypeSymbol typeofsymbol)
                                            {
                                                attributeArgs.Add(AttributeArgument(
                                                           TypeOfExpression(IdentifierName((typeofsymbol.Name + "DTO").ToString())))
                                   .WithNameEquals(NameEquals(IdentifierName(namedArg.Key))));
                                            }

                                        }
                                        break;
                                }

                            }
                            item = item.WithArgumentList(AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>(attributeArgs)));
                        }

                        if (attributes.Count != 0)
                        {
                            attributes.Add(Token(SyntaxKind.CommaToken));
                        }
                        attributes.Add(item);
                    }
                }
                if (child.IsComplex)
                {
                    if (child.SymbolData!.MapToData == null)
                    {
                        if (child.SymbolData.IsTallyComplexObject)
                        {
                            typeSyntax = PredefinedType(Token(SyntaxKind.StringKeyword));
                        }
                        else
                        {
                            typeSyntax = IdentifierName($"{child.ChildType.Name}DTO");
                        }
                    }
                    else
                    {
                        typeSyntax = GetGlobalNameforType(child.SymbolData!.MapToData.TypeSymbol.OriginalDefinition.ToString());
                    }

                }
                else
                {
                    typeSyntax = PredefinedType(Token(SyntaxKind.StringKeyword));
                }
                if (child.IsList)
                {
                    typeSyntax = QualifiedName(GetGlobalNameforType(CollectionsNameSpace), GenericName(ListClassName)
                        .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(typeSyntax))));
                }
                List<string> nullabletypes = [LanguageNamesFullTypeName];
                if (child.IsNullable || nullabletypes.Contains(child.ChildType.GetClassMetaName()))
                {
                    typeSyntax = NullableType(typeSyntax);
                }

                PropertyDeclarationSyntax propertyDeclarationSyntax = GetPropertyMemberSyntax(typeSyntax, child.Name);
                if (attributes.Count > 0)
                {
                    propertyDeclarationSyntax = propertyDeclarationSyntax.WithAttributeLists(
                    List(
                        new AttributeListSyntax[]{
                            AttributeList(
                                SeparatedList<AttributeSyntax>(attributes)) }));
                };
                members.Add(propertyDeclarationSyntax);
            }




        }
    }


    private MemberDeclarationSyntax CreateImplicitConverterSyntax()
    {
        var srcArgName = "src";
        var dtoArgName = "dto";

        string className = GetDTOClassName();
        List<StatementSyntax> statements = [];
        HashSet<string> ComplexTypeNames = [];
        statements.Add(IfStatement(BinaryExpression(SyntaxKind.EqualsExpression, IdentifierName(srcArgName), LiteralExpression(SyntaxKind.NullLiteralExpression)), Block(SingletonList<StatementSyntax>(
                                        ReturnStatement(
                                            LiteralExpression(
                                                SyntaxKind.NullLiteralExpression))))));
        statements.Add(CreateVarInsideMethodWithExpression(dtoArgName, ObjectCreationExpression(IdentifierName(className))
            .WithArgumentList(ArgumentList())));

        AddAssignmentExpressions(_symbol);

        if (_symbol.Symbol.HasInterfaceWithFullyQualifiedMetadataName(BaseLedgerEntryInterfaceName))
        {
            var right = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                               IdentifierName(srcArgName),
                                               IdentifierName("Amount.IsDebit"));
            var expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, GetGlobalNameforType(_symbol.MainFullName), IdentifierName(GetTallyStringMethodName)))
                .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[] { Argument(right) })));
            statements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(dtoArgName), IdentifierName("IsDeemedPositive")),
                                                            expression)));
        }
        statements.Add(ReturnStatement(IdentifierName(dtoArgName)));

        ConversionOperatorDeclarationSyntax conversionOperatorDeclarationSyntax = ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword), IdentifierName(className))
            .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
            {
                Parameter(Identifier(srcArgName)).WithType(GetGlobalNameforType(_symbol.FullName))

            })))
            .WithModifiers(TokenList(
                            [
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return conversionOperatorDeclarationSyntax;

        void AddAssignmentExpressions(SymbolData symbol, bool isBaseSymbol = false)
        {
            ComplexTypeNames.Add(symbol.FullName);
            if (symbol.BaseSymbolData != null)
            {
                AddAssignmentExpressions(symbol.BaseSymbolData.SymbolData, true);
            }
            foreach (var child in symbol.Children.Values)
            {
                if (child.IgnoreForCreateDTO)
                {
                    continue;
                }
                if (isBaseSymbol)
                {
                    if (child.OverriddenBy.Any() && child.OverriddenBy.Where(c => ComplexTypeNames.Contains(c.Parent.FullName)).Any())
                    {
                        continue;
                    }
                }
                if (child.IsComplex)
                {
                    ExpressionSyntax right;
                    if (child.IsList)
                    {
                        List<SyntaxNodeOrToken> switchExpressions = [];
                        child.XMLData.ForEach(c =>
                        {
                            SafeAdd(switchExpressions, SwitchExpressionArm(
                                                        DeclarationPattern(
                                                            GetGlobalNameforType(c.ChildSymbolData.ChildTypeFullName), SingleVariableDesignation(
                                                                                        Identifier("obj"))),
                                                        CastExpression(
                                                            IdentifierName($"{c.ChildSymbolData.Name}DTO"),
                                                            IdentifierName("obj"))));
                        });
                        SafeAdd(switchExpressions, SwitchExpressionArm(DiscardPattern(),
                                                                       CastExpression(
                                                                           IdentifierName($"{child.ChildType.Name}DTO"),
                                                                           IdentifierName("c"))));
                        right = ConditionalAccessExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(srcArgName), IdentifierName(child.Name)),
                            InvocationExpression(MemberBindingExpression(IdentifierName("Select")))
                           .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(
                                                                       SimpleLambdaExpression(
                                                                           Parameter(
                                                                               Identifier("c")))
                                                                       .WithExpressionBody(
                                                                               SwitchExpression(IdentifierName("c")).WithArms(SeparatedList<SwitchExpressionArmSyntax>(switchExpressions))))))));

                        statements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(dtoArgName), IdentifierName(child.Name)),
                                                            InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, right, IdentifierName("ToList"))).WithArgumentList(ArgumentList()))));

                    }
                    if (child.SymbolData?.IsTallyComplexObject ?? false)
                    {
                        ExpressionSyntax expression = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(srcArgName), IdentifierName(child.Name));
                        if (child.SymbolData?.MapToData == null)
                        {
                            List<SyntaxNodeOrToken> toStringArgs = [];
                            if (child.SymbolData!.FullName == TallyRateClassName)
                            {
                                ChildSymbolData? childSymbolData = symbol.Children.Values.Where(c => c.SymbolData?.FullName == TallyAmountClassName).FirstOrDefault();
                                if (childSymbolData != null)
                                {
                                    toStringArgs.Add(Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(srcArgName), IdentifierName(childSymbolData.Name))));
                                }
                            }
                            if (child.IsNullable)
                            {
                                expression = ConditionalAccessExpression(expression, InvocationExpression(MemberBindingExpression(IdentifierName("ToString"))).
                                    WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(toStringArgs))));
                            }
                            else
                            {

                                expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, IdentifierName("ToString")))
                                    .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(toStringArgs)));
                            }
                        }

                        statements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(dtoArgName), IdentifierName(child.Name)),
                                                        expression)));
                    }

                }
                else
                {
                    ExpressionSyntax right = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(srcArgName), IdentifierName(child.Name));
                    if (child.ChildType.SpecialType != SpecialType.System_String)
                    {

                        var memberAcess = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(srcArgName), IdentifierName(child.Name));
                        List<SyntaxNodeOrToken> args =
                                                    [
                                                        Argument(memberAcess),
                                                    ];
                        if (child.ChildType.SpecialType == SpecialType.System_Enum)
                        {
                            SafeAdd(args, IdentifierName(LicenseInfoPropertyName));
                        }
                        right = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, GetGlobalNameforType(_symbol.MainFullName), IdentifierName(GetTallyStringMethodName)))
                            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(args)));
                    }
                    statements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(dtoArgName), IdentifierName(child.Name)),
                                                        right)));
                }
            }

        }
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

    internal ArrayTypeSyntax CreateArrayTypewithCount(string typeName, int Count)
    {
        var arrayTypeSyntax = ArrayType(GetGlobalNameforType(typeName))
            .WithRankSpecifiers(List([ArrayRankSpecifier(SingletonSeparatedList((ExpressionSyntax)LiteralExpression(
                                                                                SyntaxKind.NumericLiteralExpression,
                                                                                Literal(Count))))]));
        return arrayTypeSyntax;
    }



    internal LocalDeclarationStatementSyntax CreateVarArrayWithCount(string typeName, string varName, int count)
    {

        return CreateVarInsideMethodWithExpression(varName, ArrayCreationExpression(CreateArrayTypewithCount(typeName, count)));
    }


    private static FieldDeclarationSyntax CreateConstStringVar(string varName, string varText, bool isInternal = false)
    {
        TypeSyntax strSyntax = PredefinedType(Token(SyntaxKind.StringKeyword));

        VariableDeclarationSyntax variableDeclarationSyntax = CreateVariableDelaration(strSyntax, varName, LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(varText)));
        List<SyntaxToken> tokens = [];
        if (isInternal)
        {
            tokens.Add(Token(SyntaxKind.InternalKeyword));
        }
        tokens.Add(Token(SyntaxKind.ConstKeyword));
        return FieldDeclaration(variableDeclarationSyntax)
            .WithModifiers(TokenList(tokens));
    }


}