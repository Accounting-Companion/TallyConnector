namespace TallyConnector.TDLReportSourceGenerator.Services;

internal class PostDTOGenerator
{
    private ClassData _modelData;
    private SourceProductionContext _context;
    private CancellationToken token;

    public PostDTOGenerator(ClassData modelData, SourceProductionContext context, CancellationToken token)
    {
        _modelData = modelData;
        _context = context;
        this.token = token;
    }


    public PostDTOGenerator GenerateDTO()
    {

        ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(_modelData.DTOName)
                  .WithModifiers(TokenList([Token(
                            TriviaList(
                                Comment($@"/*
* Generated based on {_modelData.FullName}
*/")),
                            SyntaxKind.PublicKeyword,
                            TriviaList())]))
                     .WithBaseList(
                    BaseList(
                        SingletonSeparatedList<BaseTypeSyntax>(
                            SimpleBaseType(
                              _modelData.BaseData == null ? IdentifierName(Constants.Models.Abstractions.MetaObjectypeName) : GetGlobalNameforType($"{_modelData.BaseData.Namespace}.DTO.{_modelData.BaseData.DTOName}")))));

        var unit = CompilationUnit()
          .WithUsings(List(GetUsings()))
          .WithMembers(List(new MemberDeclarationSyntax[]
          {
                FileScopedNamespaceDeclaration(IdentifierName($"{_modelData.Namespace}.DTO"))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    classDeclarationSyntax
                    .WithMembers(List(GetClassMembers()))
                }))
          })).NormalizeWhitespace().ToFullString();
        _context.AddSource($"PostDTO.{_modelData.Name}_{_modelData.Namespace}.g.cs", unit);

        return this;
    }
    public int MyProperty { get; set; }
    private IEnumerable<MemberDeclarationSyntax> GetClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        foreach (var item in _modelData.Members)
        {
            var member = item.Value;
            if (member.IgnoreForDTO) continue;
            List<SyntaxToken> tokens = [Token(SyntaxKind.PublicKeyword)];
            if (member.IsOverridenProperty)
            {
                tokens.Add(Token(SyntaxKind.NewKeyword));
            }
            TypeSyntax NameSyntax = PredefinedType(Token(SyntaxKind.StringKeyword));
            if (member.IsComplex && !member.IsTallyComplexObject)
            {
                if (member.ClassData != null)
                {
                    var typeName = member.ClassData.DTOName;
                    NameSyntax = GetGlobalNameforType($"{member.ClassData.Namespace}.DTO.{typeName}");
                    if (member.IsList)
                    {
                        NameSyntax = GenericName(Identifier(ListClassName), TypeArgumentList([NameSyntax]));
                    }
                }
            }
            if (member.IsNullable) NameSyntax = NullableType(NameSyntax);
            members.Add(PropertyDeclaration( NameSyntax, item.Key)
                .WithAccessorList(
            AccessorList(
                List(
                    [
                        AccessorDeclaration(
                            SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken)),
                        AccessorDeclaration(
                            SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken))])))
                .WithModifiers(TokenList(tokens)));

        }
        members.Add(GetImplicitConverterSyntax());
        return members;
    }

    private MemberDeclarationSyntax GetImplicitConverterSyntax()
    {
        List<StatementSyntax> statements = [];
        const string srcParameterName = "src";
        const string dtoVarName = "dto";
        statements.Add(IfStatement(
                    BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        IdentifierName(srcParameterName),
                        LiteralExpression(
                            SyntaxKind.NullLiteralExpression)),
                    Block(
                        SingletonList<StatementSyntax>(
                            ReturnStatement(
                                LiteralExpression(
                                    SyntaxKind.NullLiteralExpression))))));

        statements.Add(LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName(
                            Identifier(
                                TriviaList(),
                                SyntaxKind.VarKeyword,
                                "var",
                                "var",
                                TriviaList())))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                Identifier(dtoVarName))
                            .WithInitializer(
                                EqualsValueClause(
                                    ObjectCreationExpression(
                                        IdentifierName(_modelData.DTOName))
                                    .WithArgumentList(
                                        ArgumentList())))))));

        statements.Add(ReturnStatement(IdentifierName(dtoVarName)));
        var declaration = ConversionOperatorDeclaration(
           Token(SyntaxKind.ImplicitKeyword),
           IdentifierName(_modelData.DTOName))
               .WithModifiers(
            TokenList(
                [
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword)]))
            .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[] 
            {
                Parameter(Identifier(srcParameterName)).WithType(GetGlobalNameforType(_modelData.FullName))
            })))
            .WithBody(Block(statements));
        return declaration;
    }

    private static List<UsingDirectiveSyntax> GetUsings()
    {
        return [
                    UsingDirective(IdentifierName(ExtensionsNameSpace)),
            UsingDirective(IdentifierName(TallyConnectorRequestModelsNameSpace)),
            UsingDirective(IdentifierName(Constants.Models.Abstractions.PREFIX))
                    ];
    }
}