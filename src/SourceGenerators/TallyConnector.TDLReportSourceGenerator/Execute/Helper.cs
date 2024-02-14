using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using TallyConnector.TDLReportSourceGenerator.Extensions.Symbols;
using TallyConnector.TDLReportSourceGenerator.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TallyConnector.TDLReportSourceGenerator.Execute;
internal class Helper
{

    private readonly SymbolData _symbol;
    readonly string _reportVariableName;
    readonly string _collectionVariableName;
    readonly string ReportName;
    readonly string _collectionName;
    const string _cancellationTokenArgName = "token";
    const string _reqOptionsArgName = "reqOptions";
    private readonly List<ChildSymbolData> _complexChildren;
    private readonly List<ChildSymbolData> _simpleChildren;
    private readonly bool _includeCollection;
    public Helper(SymbolData symbol)
    {
        _symbol = symbol;
        ReportName = $"TC_{_symbol.TypeName}List";
        _reportVariableName = $"{_symbol.TypeName}{nameof(ReportName)}";
        _collectionVariableName = $"{_symbol.TypeName}{nameof(_collectionName)}";
        _collectionName = _symbol.IsChild ? _symbol.Name : $"TC_{_symbol.Name}Collection";
        _complexChildren = symbol.Children.Where(c => c.IsComplex || c.IsList).ToList();
        _simpleChildren = symbol.Children.Where(c => !c.IsComplex).ToList();
        _includeCollection = !(_symbol.TDLCollectionDetails?.Exclude ?? false);
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
                    .WithModifiers(TokenList([Token(SyntaxKind.PartialKeyword)]))
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
                members.Add(CreateConstStringVar(child.ReportVarName, $"TC_{_symbol.Name}{child.Name}List", true));
                if (child.TDLCollectionDetails != null && child.TDLCollectionDetails.CollectionName != null)
                {
                    members.Add(CreateConstStringVar(GetCollectionName(child), child.TDLCollectionDetails.CollectionName));
                }
            }
            members.AddRange(
                [
                    CreateConstStringVar(_reportVariableName, ReportName, true),
                    CreateConstStringVar(_collectionVariableName, _collectionName),
                ]);
        }
        if (!_symbol.IsChild)
        {
            members.Add(GenerateGetObjectsMethodSyntax());
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
        }

        return members;
    }

    private MemberDeclarationSyntax GenerateGetObjectsMethodSyntax()
    {
        string reqEnvelopeVarName = "reqEnvelope";
        string xmlVarName = "reqXml";
        string xmlRespVarName = "resp";
        string xmlAttributeOverridesVarName = "XMLAttributeOverrides";
        string xmlAttributesVarName = "XMLAttributes";
        string xmlRespEnvlopeVarName = "respEnv";
        List<StatementSyntax> statements = [];
        List<SyntaxNodeOrToken> methodArgs = [];

        InvocationExpressionSyntax mainExpression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                               GetGlobalNameforType(_symbol.MainFullName),
                                                                                                               IdentifierName(string.Format(GetRequestEnvelopeMethodName, _symbol.TypeName))));
        statements.Add(CreateVarInsideMethodWithExpression(reqEnvelopeVarName, mainExpression));
        bool isMasterObject = _symbol.Symbol.HasInterfaceWithFullyQualifiedMetadataName(MasterObjectInterfaceName);
        bool isVoucherObject = _symbol.Symbol.HasInterfaceWithFullyQualifiedMetadataName(VoucherObjectInterfaceName);
        bool haveAnyArgs = _symbol.Args.Any();
        if (haveAnyArgs)
        {
            INamedTypeSymbol namedTypeSymbol = _symbol.Args.First();

            methodArgs.Add(Parameter(Identifier(_reqOptionsArgName))
                .WithType(NullableType(GetGlobalNameforType(namedTypeSymbol.OriginalDefinition.ToString())))
                .WithDefault(EqualsValueClause(LiteralExpression(
                                                SyntaxKind.NullLiteralExpression))));
        }
        if (!haveAnyArgs && (isMasterObject || isVoucherObject))
        {
            methodArgs.Add(Parameter(Identifier(_reqOptionsArgName))
                .WithType(NullableType(GetGlobalNameforType(PaginatedRequestOptionsClassName)))
                .WithDefault(EqualsValueClause(LiteralExpression(
                                                SyntaxKind.NullLiteralExpression))));
        }
        if (haveAnyArgs || isMasterObject || isVoucherObject)
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
        statements.Add(ExpressionStatement(AwaitExpression(InvocationExpression(IdentifierName(PopulateDefaultOptionsMethodName))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
            {
                Argument(IdentifierName(reqEnvelopeVarName)),
                Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(_cancellationTokenArgName)),
            }))))));
        statements.Add(CreateVarInsideMethodWithExpression(xmlVarName,
                                                           InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                           IdentifierName(reqEnvelopeVarName), IdentifierName("GetXML")))));
        statements.Add(CreateVarInsideMethodWithExpression(xmlRespVarName, AwaitExpression(InvocationExpression(IdentifierName(SendRequestMethodName))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
            new SyntaxNodeOrToken[]{
                Argument(IdentifierName(xmlVarName)),
                Token(SyntaxKind.CommaToken),
                 Argument(CreateStringLiteral($"Getting {_symbol.MethodNameSuffixPlural}")),
                 Token(SyntaxKind.CommaToken),
                 Argument(IdentifierName(_cancellationTokenArgName))
            }))))));
        //statements.Add(CreateVarInsideMethodWithExpression(xmlAttributeOverridesVarName, ObjectCreationExpression(GetGlobalNameforType(XmlAttributeOverridesClassName)).WithArgumentList(ArgumentList())));
        //statements.Add(CreateVarInsideMethodWithExpression(xmlAttributesVarName, ObjectCreationExpression(GetGlobalNameforType(XmlAttributesClassName)).WithArgumentList(ArgumentList())));
        //statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlAttributesVarName), IdentifierName("XmlElements.Add")))
        //    .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(ImplicitObjectCreationExpression()
        //        .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(CreateStringLiteral(_symbol.RootXmlTag)))))))))));

        //QualifiedNameSyntax genericNameforEnv = QualifiedName(GetGlobalNameforType(TallyConnectorResponseNameSpace), GenericName(ReportResponseEnvelopeClassName)
        //            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(GetGlobalNameforType(_symbol.FullName)))));

        //statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlAttributeOverridesVarName), IdentifierName("Add")))
        //    .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
        //    {
        //        Argument(TypeOfExpression(genericNameforEnv)),
        //        Token(SyntaxKind.CommaToken),
        //        Argument(CreateStringLiteral("Objects")),
        //        Token(SyntaxKind.CommaToken),
        //        Argument(IdentifierName(xmlAttributesVarName)),
        //    })))));


        statements.Add(CreateVarInsideMethodWithExpression(xmlRespEnvlopeVarName,
                                                           InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                       GetGlobalNameforType(XMLToObjectClassName),
                                                                                                       GenericName(GetObjfromXmlMethodName)
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList((TypeSyntax)GetGlobalNameforType($"{_symbol.MainNameSpace}.Models.{GetReportResponseEnvelopeName(_symbol)}"))))))
                                                           .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                                                           {
                                                               Argument(PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression,MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespVarName), IdentifierName("Response")))),
                                                               Token(SyntaxKind.CommaToken),
                                                               Argument( LiteralExpression(
                                                        SyntaxKind.NullLiteralExpression)),
                                                               Token(SyntaxKind.CommaToken),
                                                               Argument( IdentifierName("_logger"))
                                                           })))));


        statements.Add(ReturnStatement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(xmlRespEnvlopeVarName), IdentifierName("Objects"))));
        if (methodArgs.Count > 0)
        {
            methodArgs.Add(Token(SyntaxKind.CommaToken));
        }
        methodArgs.Add(GetCancellationTokenParameterSyntax());
        var methodDeclarationSyntax = MethodDeclaration(QualifiedName(GetGlobalNameforType("System.Threading.Tasks"), GenericName("Task")
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(QualifiedName(GetGlobalNameforType(CollectionsNameSpace), GenericName(ListClassName)
            .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(GetGlobalNameforType(_symbol.FullName))))))))),
                                                        Identifier(string.Format(GetObjectsMethodName, _symbol.MethodNameSuffixPlural)))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword)]))
            .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(methodArgs)))
            .WithBody(Block(statements));

        return methodDeclarationSyntax;
    }

    private ParameterSyntax GetCancellationTokenParameterSyntax()
    {
        return Parameter(Identifier(_cancellationTokenArgName))
                    .WithType(GetGlobalNameforType(CancellationTokenStructName))
                    .WithDefault(EqualsValueClause(LiteralExpression(
                                                        SyntaxKind.DefaultLiteralExpression,
                                                        Token(SyntaxKind.DefaultKeyword))));
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
        GetMethodNames(_symbol.TDLFunctionMethods, functionMethodNames);
        //foreach (var child in _simpleChildren)
        //{
        //    if (child.IsEnum && child.SymbolData!.TDLFunctionMethods.Count == 0)
        //    {
        //        functionMethodNames.Add(InvocationExpression(
        //            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
        //                                   GetGlobalNameforType(_symbol.MainFullName),
        //                                   IdentifierName(string.Format(GetTDLFunctionsMethodName,
        //                                                                child.SymbolData!.TypeName)))));
        //    }
        //}
        AddFunctions(_symbol, functionMethodNames, GetTDLFunctionsMethodName,c => c.TDLFunctionMethods.Count);
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Functions", functionMethodNames));

        List<ExpressionSyntax> NameSetMethodNames = [];
        AddFunctions(_symbol, NameSetMethodNames, GetTDLNameSetsMethodName,c=>c.TDLNameSetMethods.Count);
        if (_symbol.TDLNameSetMethods.Count > 0 || NameSetMethodNames.Count > 0)
        {
            statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "NameSet", GetMethodNames(_symbol.TDLNameSetMethods, NameSetMethodNames)));
        }
        // Return Statement
        statements.Add(ReturnStatement(IdentifierName(envelopeVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(RequestEnvelopeFullTypeName),
                                                        Identifier(string.Format(GetRequestEnvelopeMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;


        List<ExpressionSyntax> GetMethodNames(FunctionDetails tDLFunctionMethods,
                                              List<ExpressionSyntax>? methodNames = null)
        {
            methodNames ??= [];
            foreach (var dictvalue in tDLFunctionMethods)
            {
                FunctionDetail value = dictvalue.Value;
                methodNames.Add(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                            GetGlobalNameforType(value.SymbolData.FullName),
                                                                            IdentifierName(value.FunctionName))));

            }

            return methodNames;
        }

        void AddFunctions(SymbolData symbol, List<ExpressionSyntax> methodNames, string methodFormatString,Func<SymbolData,int> action)
        {
            foreach (var child in symbol.Children.Where(c => !c.IsComplex))
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
            if (symbol.BaseSymbolData != null)
            {
                AddFunctions(symbol.BaseSymbolData.SymbolData, methodNames, methodFormatString, action);
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
        statements.Add(CreateVarArrayWithCount(PartFullTypeName, PartsVariableName, IsBaseSymbol ? _symbol.ComplexFieldsCount : _symbol.ComplexFieldsCount + 1));


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
            counter += _symbol.BaseSymbolData.SymbolData?.ComplexFieldsCount ?? 0;

        }
        foreach (var child in _complexChildren)
        {
            if (child.IsList && !child.IsComplex)
            {
                var name = $"TC_{_symbol.Name}{child.Name}List";
                List<SyntaxNodeOrToken>? intializerArgs = null;
                List<SyntaxNodeOrToken> lineConstructorArgs =
                    [
                        Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(name))),
                        Token(SyntaxKind.CommaToken),
                    ];
                if (child.TDLCollectionDetails == null || child.TDLCollectionDetails.CollectionName == null)
                {
                    intializerArgs
                    =
                    [
                        AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("Repeat"),
                                         LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(name))),
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
                string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{PartsVariableName.Substring(0, 1).ToUpper()}{PartsVariableName.Substring(1)}";
                List<SyntaxNodeOrToken> methodArgs =
                [
                    Argument(IdentifierName(child.ReportVarName)),
                    Token(SyntaxKind.CommaToken),
                    Argument(child.TDLCollectionDetails != null && child.TDLCollectionDetails.CollectionName != null ? IdentifierName(GetCollectionName(child)) : LiteralExpression(SyntaxKind.NullLiteralExpression))
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
                    continue;
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
                counter += (child.SymbolData?.ComplexFieldsCount ?? 0) + 1;
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
        statements.Add(CreateVarArrayWithCount(LineFullTypeName, LinesVariableName, _symbol.ComplexFieldsIncludedCount + 1));
        List<SyntaxNodeOrToken> args = [];

        var childSymbolDatas = _simpleChildren.Where(c => !c.IsList).Select(c => GetFieldName(c)).ToList();
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
                if (i != 0)
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
                if (child.TDLCollectionDetails?.ExplodeCondition == null)
                {
                    nodes.Add(InterpolatedStringText()
                        .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
                }
                else
                {
                    text = ":";
                    nodes.Add(InterpolatedStringText()
                        .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
                    nodes.Add(Interpolation(CreateFormatExpression(child.TDLCollectionDetails.ExplodeCondition, child.TDLFieldDetails!.Set)));
                }
                InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
                    .WithContents(List(nodes));
                explodes.Add(ExpressionElement(interpolatedStringExpressionSyntax));
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
            counter += (_symbol.BaseSymbolData?.SymbolData.ComplexFieldsIncludedCount ?? 0) + 1;

        }
        foreach (var child in _complexChildren)
        {
            if (child.IsList && !child.IsComplex)
            {
                statements.Add(GetArrayAssignmentExppressionImplicit(LinesVariableName, counter,
                    [
                       Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal($"TC_{_symbol.Name}{child.Name}List"))),
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
            if (child.SymbolData?.IsTallyComplexObject ?? false)
            {
                lineIntializerArgs.Add(Token(SyntaxKind.CommaToken));
                List<SyntaxNodeOrToken> arguments = [];
                for (int i = 0; i < child.SymbolData.Children.Count; i++)
                {
                    var c = child.SymbolData.Children[i];
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

                    SafeAdd(ExpressionElement(createStringInterpolation("\t: Set \t: ", c => c.Set!)));
                    if (c.TDLFieldDetails?.Invisible != null)
                    {
                        SafeAdd(ExpressionElement(createStringInterpolation("\t: Invisible \t: ", c => c.Invisible!)));
                    }
                }
                void SafeAdd(SyntaxNodeOrToken item)
                {
                    if (arguments.Count != 0)
                    {
                        arguments.Add(Token(SyntaxKind.CommaToken));
                    }
                    arguments.Add(item);
                }
                lineIntializerArgs.Add(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("Local"), CollectionExpression(SeparatedList<CollectionElementSyntax>(arguments))));
            }
            statements.Add(GetArrayAssignmentExppressionImplicit(LinesVariableName, counter, constructorArgs, lineIntializerArgs));
            counter++;
            if (child.Exclude)
            {
                continue;
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
            counter += (child.SymbolData?.ComplexFieldsIncludedCount ?? 0) + 1;
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
        foreach (var child in _symbol.Children)
        {
            if (child.Exclude)
            {
                continue;
            }
            if (!child.IsComplex)
            {
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
                //    arguments.Add(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.TDLFieldDetails!.Set!))));
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
        //if (_symbol.IsTallyComplexObject)
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
                //ExpressionElement(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("*"))),
            ];
        AddFetchItems(nodesAndTokens, _symbol);

        static void AddFetchItems(List<SyntaxNodeOrToken> nodesAndTokens, SymbolData symbol)
        {
            foreach (var child in symbol.Children.Where(c => !c.IsComplex))
            {
                string? name = child.TDLFieldDetails?.FetchText ?? child.TDLFieldDetails?.Set;
                if (name != null)
                {
                    SafeAdd(nodesAndTokens, ExpressionElement(CreateStringLiteral(name)));
                }
            }
            if (symbol.BaseSymbolData != null)
            {
                AddFetchItems(nodesAndTokens, symbol.BaseSymbolData.SymbolData);
            }
        }
        foreach (var child in _complexChildren)
        {
            if (child.TDLCollectionDetails == null || child.TDLCollectionDetails.CollectionName == null)
            {
                continue;
            }
            SafeAdd(nodesAndTokens, ExpressionElement(IdentifierName(GetCollectionName(child))));
        }

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

        for (int i = 0; i < _symbol.Children.Count; i++)
        {
            var child = _symbol.Children[i];
            if (child.Name == "None")
            {
                continue;
            }
          
            if (child.EnumChoices !=null)
            {
                foreach (var item in child.EnumChoices)
                {
                    if (nodesAndTokens.Count != 0)
                    {
                        nodesAndTokens.Add(Token(SyntaxKind.CommaToken));
                    }
                    nodesAndTokens.Add(ExpressionElement(CreateStringLiteral($"{item}:\"{child.Name}\"")));
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

        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(varName),
                                    IdentifierName(propName)),
                                    CollectionExpression(SeparatedList<CollectionElementSyntax>(nodes))));
    }



    public string GetCreateDTOCompilationUnitString()
    {
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
                    ClassDeclaration(GetClassName())
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
                        }))
                    }))
                    .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword),Token(SyntaxKind.PartialKeyword)]))
                    .WithMembers(List(GetCreateDTOClassMembers()))
                }))
            })).NormalizeWhitespace().ToFullString();

        return unit;
    }

    private string GetClassName()
    {
        return $"{_symbol.Name}DTO";
    }

    private IEnumerable<MemberDeclarationSyntax> GetCreateDTOClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        //If Implements BaseLedgerEntryInterface then add extra property to DTO
        if (_symbol.Symbol.HasInterfaceWithFullyQualifiedMetadataName(BaseLedgerEntryInterfaceName))
        {
            members.Add(GetPropertyMemberSyntax(PredefinedType(Token(SyntaxKind.StringKeyword)), "IsDeemedPositive"));
        }
        CreateProperties(_symbol);

        members.Add(CreateImplicitConverterSyntax());

        return members;

        void CreateProperties(SymbolData symbol)
        {
            foreach (var child in symbol.Children)
            {
                if (child.IgnoreForCreateDTO)
                {
                    continue;
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
                                attributeArgs.Add(AttributeArgument(
                                                            LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,
                                                                Literal(namedArg.Value.Value.ToString())))
                                    .WithNameEquals(NameEquals(IdentifierName(namedArg.Key))));
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
                if (child.IsNullable)
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

            if (symbol.BaseSymbolData != null)
            {
                CreateProperties(symbol.BaseSymbolData.SymbolData);
            }


        }
    }


    private MemberDeclarationSyntax CreateImplicitConverterSyntax()
    {
        var srcArgName = "src";
        var dtoArgName = "dto";

        string className = GetClassName();
        List<StatementSyntax> statements = [];
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

        void AddAssignmentExpressions(SymbolData symbol)
        {
            if (symbol.BaseSymbolData != null)
            {
                AddAssignmentExpressions(symbol.BaseSymbolData.SymbolData);
            }
            foreach (var child in symbol.Children)
            {
                if (child.IgnoreForCreateDTO)
                {
                    continue;
                }
                if (child.IsComplex)
                {
                    ExpressionSyntax right;
                    if (child.IsList)
                    {
                        right = ConditionalAccessExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(srcArgName), IdentifierName(child.Name)),
                            InvocationExpression(MemberBindingExpression(GenericName("Select")
                           .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new SyntaxNodeOrToken[]
                           {
                        GetGlobalNameforType(child.ChildType.OriginalDefinition.ToString()),
                        Token(SyntaxKind.CommaToken),
                        IdentifierName($"{child.ChildType.Name}DTO"),
                           })))))
                           .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(
                                                                       SimpleLambdaExpression(
                                                                           Parameter(
                                                                               Identifier("c")))
                                                                       .WithExpressionBody(
                                                                           IdentifierName("c")))))));

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
                                ChildSymbolData? childSymbolData = symbol.Children.Where(c => c.SymbolData?.FullName == TallyAmountClassName).FirstOrDefault();
                                if (childSymbolData != null)
                                {
                                    toStringArgs.Add(Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(srcArgName), IdentifierName(childSymbolData.Name))));
                                }
                            }
                            expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, IdentifierName("ToString")))
                                .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(toStringArgs)));
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
                        right = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, GetGlobalNameforType(_symbol.MainFullName), IdentifierName(GetTallyStringMethodName)))
                            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                            {
                            Argument(memberAcess),
                            })));
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

    private static LiteralExpressionSyntax CreateStringLiteral(string name)
    {
        return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(name));
    }

    internal ArrayTypeSyntax CreateEmptyArrayType(string typeName)
    {
        var arrayTypeSyntax = ArrayType(GetGlobalNameforType(typeName))
            .WithRankSpecifiers(List([ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))]));
        return arrayTypeSyntax;
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