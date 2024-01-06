using Microsoft.CodeAnalysis;
using System.Reflection;
using TC.TDLReportSourceGenerator.Models;

namespace TC.TDLReportSourceGenerator.Execute;
internal class GenerateTDLReport
{

    private readonly SymbolData _symbol;
    const string _reportVariableName = nameof(ReportName);
    const string _collectionVariableName = nameof(_collectionName);
    readonly string ReportName;
    readonly string _collectionName;
    private readonly List<ChildSymbolData> _complexChildren;
    private readonly List<ChildSymbolData> _simpleChildren;

    public GenerateTDLReport(SymbolData symbol)
    {
        _symbol = symbol;
        ReportName = $"TC_{_symbol.Name}List";
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
                FileScopedNamespaceDeclaration(IdentifierName(_symbol.NameSpace))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                      true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    ClassDeclaration(_symbol.Name)
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
        foreach (var child in _symbol.Children)
        {
            if (child.IsComplex)
            {
                if (child.TDLCollectionDetails != null)
                {
                    members.Add(CreateConstStringVar($"{child.ChildType.Name}CollectionName", child.TDLCollectionDetails.CollectionName));
                }
            }
            else
            {
                members.Add(CreateConstStringVar($"{child.Name}TDLFieldName", $"TC_{child.Parent.Name}_{child.Name}"));
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


        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Part", GetTDLPartsMethodName));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Line", GetTDLLinesMethodName));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Field", GetTDLFieldsMethodName));
        statements.Add(CreateAssignFromMethodStatement(tdlMsgVariableName, "Collection", GetTDLCollectionsMethodName));
        // Return Statement
        statements.Add(ReturnStatement(IdentifierName(envelopeVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(GetGlobalNameforType(RequestEnvelopeFullTypeName),
                                                        Identifier(GetRequestEnvelopeMethodName))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(Block(statements));
        return methodDeclarationSyntax;
    }

    private static ExpressionStatementSyntax CreateReportAndFormAsssignStatement(string tdlMsgVariableName, string name)
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
        const string PartsVariableName = "parts";
        List<StatementSyntax> statements = [];
        statements.Add(CreateVarArrayWithCount(PartFullTypeName, PartsVariableName, _symbol.ComplexFieldsCount + 1));
        List<SyntaxNodeOrToken> constructorArgs = [
            Argument(IdentifierName(_symbol.IsChild ? partNameArgName : _reportVariableName)),
            Token(SyntaxKind.CommaToken),
            Argument(IdentifierName(_symbol.IsChild ? CollectionNameArgName : _collectionVariableName)),
        ];
        if (_symbol.IsChild)
        {
            constructorArgs.AddRange([Token(SyntaxKind.CommaToken), Argument(IdentifierName(_reportVariableName))]);
        }
        statements.Add(GetArrayAssignmentExppression(PartsVariableName,
                                                     0,
                                                     constructorArgs));


        int counter = 1;
        foreach (var child in _complexChildren)
        {
            if (child.IsList && !child.IsComplex)
            {
                var name = $"TC_{_symbol.Name}{child.Name}List";
                List<SyntaxNodeOrToken>? intializerArgs = null;
                List<SyntaxNodeOrToken> lineConstructorArgs = [
                                        Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(name))),
                    Token(SyntaxKind.CommaToken),
                   
                                    ];
                if (child.TDLCollectionDetails ==null)
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
                    
                    lineConstructorArgs.Add(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                       Literal(child.TDLCollectionDetails.CollectionName))));
                }

                
                statements.Add(GetArrayAssignmentExppression(PartsVariableName, counter,
                    lineConstructorArgs, intializerArgs));
                counter += 1;
            }
            else
            {
                string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{PartsVariableName.Substring(0, 1).ToUpper()}{PartsVariableName.Substring(1)}";
                statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                       InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                                   GetGlobalNameforType(child.ChildTypeFullName),
                                                                                                   IdentifierName(GetTDLPartsMethodName)))
                                                       .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                    {
                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal($"TC_{_symbol.Name}{child.Name}List"))),
                    Token(SyntaxKind.CommaToken),
                    Argument(child.TDLCollectionDetails !=null ? IdentifierName($"{child.ChildType.Name}CollectionName"):LiteralExpression(SyntaxKind.NullLiteralExpression)),
                    })))));

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
                counter += child.SymbolData?.SimpleFieldsCount ?? 0;
            }
            
        }
        statements.Add(ReturnStatement(IdentifierName(PartsVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(PartFullTypeName),
                                                        Identifier(GetTDLPartsMethodName))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
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
                    .WithDefault(EqualsValueClause(IdentifierName(_collectionVariableName)))
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

        var childSymbolDatas = _simpleChildren.Where(c => !c.IsList).Select(c => $"{c.Name}TDLFieldName").ToList();
        const int count = 5;
        if (childSymbolDatas.Count == 0)
        {
            args.Add(ExpressionElement(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal("SimpleField"))));
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
                            //Interpolation(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(child.ChildTypeFullName), IdentifierName(_reportVariableName))),
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
                                                                     ExpressionElement(IdentifierName($"{child.Name}TDLFieldName"))
                                                                 })))

                    ]));
                counter += 1;
                continue;
            }
            string ComplexFieldsVariableName = $"{child.Name.Substring(0, 1).ToLower()}{child.Name.Substring(1)}{LinesVariableName.Substring(0, 1).ToUpper()}{LinesVariableName.Substring(1)}";
            statements.Add(CreateVarInsideMethodWithExpression(ComplexFieldsVariableName,
                                                   InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                               GetGlobalNameforType(child.ChildTypeFullName),
                                                                                               IdentifierName(GetTDLLinesMethodName)))));

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
            counter += child.SymbolData?.SimpleFieldsCount ?? 0;
        }
        statements.Add(ReturnStatement(IdentifierName(LinesVariableName)));
        var methodDeclarationSyntax = MethodDeclaration(CreateEmptyArrayType(LineFullTypeName),
                                                        Identifier(GetTDLLinesMethodName))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
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
        foreach (var child in _symbol.Children)
        {
            if (child.Exclude)
            {
                continue;
            }
            if (!child.IsComplex)
            {
                string name = $"{child.Name}TDLFieldName";
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
                                                                                                   GetGlobalNameforType(child.ChildTypeFullName),
                                                                                                   IdentifierName(GetTDLFieldsMethodName)))));

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
                                                        Identifier(GetTDLFieldsMethodName))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
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
            nodesAndTokens.Add(Token(SyntaxKind.CommaToken));
            nodesAndTokens.Add(ExpressionElement(IdentifierName($"{child.ChildType.Name}CollectionName")));
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
                                                        Identifier(GetTDLCollectionsMethodName))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
            .WithBody(
            Block(statements));
        return methodDeclarationSyntax;
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
                                     GetGlobalNameforType(_symbol.FullName), IdentifierName(methodName)))
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