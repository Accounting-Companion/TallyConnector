
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
                              _modelData.BaseData == null ? IdentifierName(Constants.Models.Abstractions.MetaObjectypeName) : GetGlobalNameforType(_modelData.BaseData.DTOFullName)))));


        classDeclarationSyntax = classDeclarationSyntax.WithAttributeLists(
            SingletonList<AttributeListSyntax>(
                AttributeList(
                    SeparatedList<AttributeSyntax>(
                        new SyntaxNodeOrToken[]{
                            Attribute(
                                IdentifierName("XmlRoot"))
                            .WithArgumentList(
                                AttributeArgumentList(
                                    SingletonSeparatedList<AttributeArgumentSyntax>(
                                        AttributeArgument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal(_modelData.XMLTag)))))),
                            Token(SyntaxKind.CommaToken),
                            Attribute(
                                IdentifierName("XmlType"))
                            .WithArgumentList(
                                AttributeArgumentList(
                                    SingletonSeparatedList<AttributeArgumentSyntax>(
                                        AttributeArgument(
                                            LiteralExpression(
                                                SyntaxKind.TrueLiteralExpression))
                                        .WithNameEquals(
                                            NameEquals(
                                                IdentifierName("AnonymousType"))))))}))));
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
    private IEnumerable<MemberDeclarationSyntax> GetClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        foreach (var item in _modelData.Members)
        {
            List<SyntaxNodeOrToken> attributes = [];
            var member = item.Value;
            if (member.IgnoreForDTO || member.XmlIgnore) continue;
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

                }
            }
            if (member.IsList)
            {
                NameSyntax = GenericName(Identifier(ListClassName), TypeArgumentList([NameSyntax]));
            }
            if (member.IsNullable) NameSyntax = NullableType(NameSyntax);

            string attributeName = XMLElementAttributeName;
            if (member.IsList && member.ListXMLTag != null)
            {
                attributeName = XMLArrayItemAttributeName;
                attributes.SafeAdd(Attribute(GetGlobalNameforType(XMLArrayAttributeName))
               .WithArgumentList(AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>(
                                               new SyntaxNodeOrToken[] { AttributeArgument(CreateStringLiteral(member.ListXMLTag)) }))));
            }
            List<SyntaxNodeOrToken> attributeArgs = [AttributeArgument(CreateStringLiteral(member.DefaultXMLData?.XmlTag ?? member.Name.ToUpper()))];
            if (member.XMLData.Count > 0 && member.ClassData != null)
            {
                attributeArgs.SafeAdd(AttributeArgument(TypeOfExpression(
                                                                GetGlobalNameforType(member.ClassData.DTOFullName)))
                                                        .WithNameEquals(
                                                            NameEquals(
                                                                IdentifierName("Type"))));
            }

            attributes.SafeAdd(Attribute(GetGlobalNameforType(attributeName))
                .WithArgumentList(AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>(
                                                attributeArgs))));
            foreach (var xmlData in member.XMLData)
            {
                if (xmlData.ClassData == null)
                {
                    continue;
                }
                attributes.SafeAdd(Attribute(GetGlobalNameforType(XMLElementAttributeName))
                    .WithArgumentList(AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>(
                                                [
                        AttributeArgument(CreateStringLiteral(xmlData.XmlTag ?? member.Name.ToUpper())),
                         Token(SyntaxKind.CommaToken),
                         AttributeArgument(
                                         TypeOfExpression(
                                             GetGlobalNameforType(xmlData.ClassData.DTOFullName)))
                                     .WithNameEquals(
                                         NameEquals(
                                     IdentifierName("Type")))
                                                ]))));
            }
            PropertyDeclarationSyntax propertyDeclarationSyntax = PropertyDeclaration(NameSyntax, item.Key)
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
                .WithModifiers(TokenList(tokens))
                .WithAttributeLists(List(
                         [
                             AttributeList( SeparatedList<AttributeSyntax>(attributes))
                         ]));



            members.Add(propertyDeclarationSyntax);

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


        AssignAllMembers();

        if (_modelData.Symbol.CheckBaseClass(Constants.Models.BaseAliasedMasterObjectFullName))
        {
            statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,IdentifierName(dtoVarName),IdentifierName("SetLanguageNameListAndAlias")))
                .WithArgumentList(ArgumentList([Argument(IdentifierName($"{srcParameterName}.Alias"))]))));
        }

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


        void AssignAllMembers()
        {
            foreach (var item in _modelData.AllDirectMembers)
            {
                var member = item.Value;
                if (member.IgnoreForDTO || member.XmlIgnore) continue;
                ExpressionSyntax right = MemberAccessExpression(
                             SyntaxKind.SimpleMemberAccessExpression,
                             IdentifierName(srcParameterName),
                             IdentifierName(member.Name));

                if (member.IsList && member.IsComplex)
                {
                    if (member.ClassData == null) continue;
                    if (!member.ClassData.IsTallyComplexObject)
                    {
                        if (member.XMLData.Count > 0)
                        {
                            continue;
                        }
                        else
                        {

                            var rightSelect = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                           right,
                                                                           IdentifierName("Select")))
                                  .WithArgumentList(ArgumentList(SingletonSeparatedList<ArgumentSyntax>(
                                                          Argument(
                                                              SimpleLambdaExpression(
                                                                  Parameter(
                                                                      Identifier("c")))
                                                              .WithExpressionBody(
                                                                  CastExpression(
                                                                     GetGlobalNameforType(member.ClassData.DTOFullName),
                                                                      IdentifierName("c")))))));
                            var rightCollection = CollectionExpression(SeparatedList<CollectionElementSyntax>(SeparatedList<CollectionElementSyntax>(new SyntaxNodeOrToken[] { SpreadElement(rightSelect) })));
                            if (true)
                            {
                                right = ConditionalExpression(IsPatternExpression(right,
                                                                                  ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                                                                                   LiteralExpression(SyntaxKind.NullLiteralExpression), rightCollection);
                            }
                            else
                            {
                                right = rightCollection;
                            }
                        }
                    }
                    else
                    {
                        right = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, right, IdentifierName("ToString")));
                    }

                }
                else
                {
                    switch (member.PropertyOriginalType.SpecialType)
                    {
                        case SpecialType.System_String:
                            break;
                        case SpecialType.System_DateTime or SpecialType.System_Boolean:
                            right = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, right, IdentifierName("ToTallyString")));
                            break;
                        default:
                            right = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, right, IdentifierName("ToString")));
                            continue;
                    }

                }

                statements.Add(ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(dtoVarName),
                            IdentifierName(member.Name)),
                        right)));
            }
        }

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
