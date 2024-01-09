using Microsoft.CodeAnalysis;
using System.Reflection;
using TC.TDLReportSourceGenerator.Models;

namespace TC.TDLReportSourceGenerator.Execute;
internal class GenerateTDLReport
{

    private readonly SymbolData _symbol;
    readonly string _reportVariableName;
    readonly string _collectionVariableName;
    readonly string ReportName;
    readonly string _collectionName;
    private readonly List<ChildSymbolData> _complexChildren;
    private readonly List<ChildSymbolData> _simpleChildren;

    public GenerateTDLReport(SymbolData symbol)
    {
        _symbol = symbol;
        ReportName = $"TC_{_symbol.Name}List";
        _reportVariableName = $"{_symbol.TypeName}{nameof(ReportName)}";
        _collectionVariableName = $"{_symbol.TypeName}{nameof(_collectionName)}";
        _collectionName = _symbol.IsChild ? _symbol.Name : $"TC_{_symbol.Name}Collection";
        _complexChildren = symbol.Children.Where(c => c.IsComplex || c.IsList).ToList();
        _simpleChildren = symbol.Children.Where(c => !c.IsComplex).ToList();
    }

    public CompilationUnitSyntax GetCompilationUnit()
    {
        var unit = CompilationUnit()
            .WithUsings(List([UsingDirective(IdentifierName(ExtensionsNameSpace))]))
            .WithMembers(List(new MemberDeclarationSyntax[]
            {
                FileScopedNamespaceDeclaration(IdentifierName(_symbol.ParentNameSpace))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                      true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    ClassDeclaration(_symbol.ParentSymbol.Name)
                    .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword),Token(SyntaxKind.PartialKeyword)]))
                    .WithMembers(List(GetClassMembers()))
                }))
            })).NormalizeWhitespace();

        return unit;
    }

    private IEnumerable<MemberDeclarationSyntax> GetClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        // creates const variables for field names
        foreach (var child in _simpleChildren)
        {
            members.Add(CreateConstStringVar(GetFieldName(child), $"TC_{child.Parent.Name}_{child.Name}", true));
        }
        foreach (var child in _complexChildren)
        {
            if (child.TDLCollectionDetails != null)
            {
                members.Add(CreateConstStringVar(GetCollectionName(child), child.TDLCollectionDetails.CollectionName));
            }
        }
        members.AddRange(
            [
                CreateConstStringVar(_reportVariableName, ReportName, true),
                CreateConstStringVar(_collectionVariableName, _collectionName),
            ]);
        if (!_symbol.IsChild)
        {
            members.Add(GenerateGetRequestEnvolopeMethodSyntax());
        }
        members.AddRange(
            [
                GenerateGetPartsMethodSyntax(),
                GenerateGetLinesMethodSyntax(),
                GenerateGetFieldsMethodSyntax(),
            ]);
        if (!_symbol.IsChild)
        {
            members.Add(GenerateGetCollectionsMethodSyntax());
        }
        return members;
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


        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Part", string.Format(GetTDLPartsMethodName, _symbol.TypeName)));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Line", string.Format(GetTDLLinesMethodName, _symbol.TypeName)));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Field", string.Format(GetTDLFieldsMethodName, _symbol.TypeName)));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Collection", string.Format(GetTDLCollectionsMethodName, _symbol.TypeName)));
        // Return Statement
        statements.Add(ReturnStatement(IdentifierName(envelopeVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(RequestEnvelopeFullTypeName),
                                                        Identifier(string.Format(GetRequestEnvelopeMethodName, _symbol.TypeName)))
            .WithModifiers(TokenList([Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;
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

    private MemberDeclarationSyntax GenerateGetPartsMethodSyntax()
    {
        const string CollectionNameArgName = "collectionName";
        const string partNameArgName = "partName";
        const string XmlTagArgName = "xmlTag";
        const string PartsVariableName = "parts";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(PartFullTypeName, PartsVariableName, _symbol.ComplexFieldsCount + 1));
        List<SyntaxNodeOrToken> constructorArgs = [
            Argument(IdentifierName(_symbol.IsChild ? partNameArgName : _reportVariableName)),
            Token(SyntaxKind.CommaToken),
            Argument(IdentifierName(_symbol.IsChild ? CollectionNameArgName : _collectionVariableName)),
        ];
        List<SyntaxNodeOrToken>? firstPartIntializerArgs = null;
        if (_symbol.IsChild)
        {
            constructorArgs.AddRange([Token(SyntaxKind.CommaToken), Argument(IdentifierName(_reportVariableName))]);
            firstPartIntializerArgs = [AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                         IdentifierName("XMLTag"),
                                         IdentifierName(XmlTagArgName))];
        }
        statements.Add(GetArrayAssignmentExppression(PartsVariableName,
                                                     0,
                                                     constructorArgs,
                                                     firstPartIntializerArgs));


        int counter = 1;
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
                if (child.TDLCollectionDetails == null)
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


                statements.Add(GetArrayAssignmentExppression(PartsVariableName, counter,
                    lineConstructorArgs, intializerArgs));
                counter += 1;
            }
            else
            {
                string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{PartsVariableName.Substring(0, 1).ToUpper()}{PartsVariableName.Substring(1)}";
                List<SyntaxNodeOrToken> methodArgs =
                [
                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal($"TC_{_symbol.Name}{child.Name}List"))),
                    Token(SyntaxKind.CommaToken),
                    Argument(child.TDLCollectionDetails !=null ? IdentifierName(GetCollectionName(child)):LiteralExpression(SyntaxKind.NullLiteralExpression)),
                ];
                if (child.ListXmlTag != null)
                {
                    methodArgs.AddRange([Token(SyntaxKind.CommaToken), Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(child.ListXmlTag)))]);
                }
                statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                       InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                   GetGlobalNameforType(_symbol.ParentFullName),
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
                counter += (child.SymbolData?.ComplexFieldsIncludedCount ?? 0) + 1;
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
        if (childSymbolDatas.Count == 0)
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
                explodes.Add(ExpressionElement(InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
                    .WithContents(List<InterpolatedStringContentSyntax>(
                        [
                            //Interpolation(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(baseSymbol.ChildTypeFullName), IdentifierName(_reportVariableName))),
                            InterpolatedStringText().WithTextToken(Token(TriviaList(),
                                                                         SyntaxKind.InterpolatedStringTextToken,
                                                                         $"TC_{_symbol.Name}{child.Name}List:Yes",
                                                                         ":Yes",
                                                                         TriviaList())),
                        ]))));
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
                                         IdentifierName("Use"), IdentifierName($"{_symbol.BaseSymbolData.Name}{nameof(ReportName)}"))
                ]);
        }
        statements.Add(GetArrayAssignmentExppression(LinesVariableName,
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
        if (_symbol.BaseSymbolData != null)
        {
            string ComplexFieldsVariableName = $"{_symbol.BaseSymbolData.Name.Substring(0, 1).ToLower()}{_symbol.BaseSymbolData.Name.Substring(1)}{LinesVariableName.Substring(0, 1).ToUpper()}{LinesVariableName.Substring(1)}";
            statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                   InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                               GetGlobalNameforType(_symbol.ParentFullName),
                                                                                               IdentifierName(string.Format(GetTDLLinesMethodName, _symbol.BaseSymbolData.Name))))));

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
            counter += (_symbol.BaseSymbolData?.ComplexFieldsIncludedCount ?? 0) + 1;

        }
        foreach (var child in _complexChildren)
        {
            if (child.Exclude)
            {
                continue;
            }
            if (child.IsList && !child.IsComplex)
            {
                statements.Add(GetArrayAssignmentExppression(LinesVariableName, counter,
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
            string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{LinesVariableName.Substring(0, 1).ToUpper()}{LinesVariableName.Substring(1)}";
            statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                   InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                               GetGlobalNameforType(_symbol.ParentFullName),
                                                                                               IdentifierName(string.Format(GetTDLLinesMethodName, child.SymbolData?.TypeName ?? child.Name))))
                                                   .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
                     new SyntaxNodeOrToken[]{
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(child.XmlTag))),
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
            string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{FieldsVariableName.Substring(0, 1).ToUpper()}{FieldsVariableName.Substring(1)}";
            statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                   InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                               GetGlobalNameforType(_symbol.ParentFullName),
                                                                                               IdentifierName(string.Format(GetTDLFieldsMethodName, child.Name))))));

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
            counter += child.SimpleFieldsCount;
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
                    Argument(CreateStringLiteral(child.TDLFieldDetails!.Set!)),
                ];
                var expressionStatementSyntax = GetArrayAssignmentExppression(FieldsVariableName, counter, args);
                statements.Add(expressionStatementSyntax);
                counter++;
            }
            else
            {
                string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{FieldsVariableName.Substring(0, 1).ToUpper()}{FieldsVariableName.Substring(1)}";
                statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                       InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                   GetGlobalNameforType(_symbol.ParentFullName),
                                                                                                   IdentifierName(string.Format(GetTDLFieldsMethodName, child.SymbolData?.TypeName ?? child.Name))))));

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
            .WithBody(
            Block(statements));
        return methodDeclarationSyntax;
    }

    private MemberDeclarationSyntax GenerateGetCollectionsMethodSyntax()
    {
        const string CollectionsVariableName = "collections";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(CollectionFullTypeName, CollectionsVariableName, 1));
        List<SyntaxNodeOrToken> nodesAndTokens =
            [
                ExpressionElement(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("*"))),
            ];
        foreach (var child in _complexChildren)
        {
            if (child.TDLCollectionDetails == null)
            {
                continue;
            }
            nodesAndTokens.Add(Token(SyntaxKind.CommaToken));
            nodesAndTokens.Add(ExpressionElement(IdentifierName(GetCollectionName(child))));
        }
        statements.Add(GetArrayAssignmentExppression(CollectionsVariableName, 0,
            [
                Argument(IdentifierName(_collectionVariableName)),
                Token(SyntaxKind.CommaToken),
                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(_symbol.RootXmlTag))),
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

    private string GetFieldName(ChildSymbolData child)
    {
        return $"{_symbol.TypeName}{child.Name}TDLFieldName";
    }
    private string GetFieldName(BaseSymbolData baseSymbol)
    {
        return $"{_symbol.TypeName}{baseSymbol.Name}TDLFieldName";
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
                                                            string methodName)
    {
        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(varName),
                                    IdentifierName(propName)),
                                    CollectionExpression(SingletonSeparatedList<CollectionElementSyntax>(SpreadElement
                                    (InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                     GetGlobalNameforType(_symbol.ParentFullName), IdentifierName(methodName)))
                                        )))));
    }

    private static ExpressionStatementSyntax GetArrayAssignmentExppression(string FieldsVariableName,
                                                                           int index,
                                                                           List<SyntaxNodeOrToken> constructorArgs,
                                                                           List<SyntaxNodeOrToken>? intializerArgs = null)
    {
        var implicitObjectCreationExpressionSyntax = ImplicitObjectCreationExpression();
        if (intializerArgs != null)
        {
            implicitObjectCreationExpressionSyntax = implicitObjectCreationExpressionSyntax
                .WithInitializer(InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                                                       SeparatedList<ExpressionSyntax>(intializerArgs)));
        }
        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                                 ElementAccessExpression(IdentifierName(FieldsVariableName))
                                                                 .WithArgumentList(BracketedArgumentList(
                                                                     SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression,
                                                                                                                       Literal(index)))))),
                                                                 implicitObjectCreationExpressionSyntax
                                                                 // ObjectCreationExpression(GetGlobalNameforType(FieldFullTypeName))
                                                                 .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(constructorArgs)))));
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

    internal AliasQualifiedNameSyntax GetGlobalNameforType(string typeName)
    {
        return AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)),
                                                                   IdentifierName(typeName));
    }

    internal LocalDeclarationStatementSyntax CreateVarArrayWithCount(string typeName, string varName, int count)
    {

        return CreateVarInsideMethodWithExpression(varName, ArrayCreationExpression(CreateArrayTypewithCount(typeName, count)));
    }
    internal LocalDeclarationStatementSyntax CreateVarInsideMethodWithExpression(string varName, ExpressionSyntax expressionSyntax)
    {
        var varSyntax = LocalDeclarationStatement(
            CreateVariableDeclaration(varName, expressionSyntax));
        return varSyntax;
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
    private static VariableDeclarationSyntax CreateVariableDeclaration(string varName, ExpressionSyntax expressionSyntax)
    {
        IdentifierNameSyntax varSyntax = IdentifierName(Identifier(TriviaList(),
                                                                                  SyntaxKind.VarKeyword,
                                                                                  "var",
                                                                                  "var",
                                                                                  TriviaList()));
        return CreateVariableDelaration(varSyntax, varName, expressionSyntax);
    }

    private static VariableDeclarationSyntax CreateVariableDelaration(TypeSyntax varSyntax, string varName, ExpressionSyntax expressionSyntax)
    {
        return VariableDeclaration(varSyntax)
            .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(varName))
            .WithInitializer(EqualsValueClause(expressionSyntax))
            ));
    }

}