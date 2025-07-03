namespace TallyConnector.TDLReportSourceGenerator.Services;
public class MetaDataGenerator
{
    private readonly ClassData _modelData;
    private readonly SourceProductionContext context;
    private CancellationToken token;

    public MetaDataGenerator(ClassData modelData,
                             SourceProductionContext context,
                             CancellationToken token)
    {
        this._modelData = modelData;
        this.context = context;
        this.token = token;
    }

    public MetaDataGenerator GenerateMeta()
    {
        List<UsingDirectiveSyntax> usings = [
            UsingDirective(IdentifierName(ExtensionsNameSpace)),
            UsingDirective(IdentifierName(TallyConnectorRequestModelsNameSpace)),
            UsingDirective(IdentifierName(Constants.Models.Abstractions.PREFIX))];

        ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(_modelData.MetaName)
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
                              _modelData.BaseData == null ? IdentifierName(Constants.Models.Abstractions.MetaObjectypeName) : GetGlobalNameforType($"{_modelData.BaseData.Namespace}.Meta.{_modelData.BaseData.MetaName}")))));

        var unit = CompilationUnit()
          .WithUsings(List(usings))
          .WithMembers(List(new MemberDeclarationSyntax[]
          {
                FileScopedNamespaceDeclaration(IdentifierName($"{_modelData.Namespace}.Meta"))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))
                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    classDeclarationSyntax
                    .WithMembers(List(GetClassMembers()))
                }))
          })).NormalizeWhitespace().ToFullString();
        context.AddSource($"meta.{_modelData.Name}_{_modelData.Namespace}.g.cs", unit);
        return this;
    }
    private IEnumerable<MemberDeclarationSyntax> GetClassMembers()
    {
        List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

        members.Add(PropertyDeclaration(IdentifierName(_modelData.MetaName), Meta.InstanceVarName)
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)]))
              .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression([])))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        members.Add(ConstructorDeclaration(Identifier(_modelData.MetaName))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
             .WithParameterList(
                                ParameterList(
                                    SeparatedList<ParameterSyntax>(
                                         new SyntaxNodeOrToken[]{
                                        Parameter(
                                            Identifier(Meta.Parameters.PathPrefix))
                                        .WithType(
                                            PredefinedType(
                                                Token(SyntaxKind.StringKeyword)))
                                        .WithDefault(
                                            EqualsValueClause(
                                                  CreateStringLiteral(""))) })))
             .WithInitializer(ConstructorInitializer(
                                    SyntaxKind.BaseConstructorInitializer,
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                IdentifierName(Meta.Parameters.PathPrefix))))))
             .WithBody(Block()));

        if (!_modelData.GenerateITallyRequestableObectAttribute)
        {
            List<SyntaxNodeOrToken> contructorArgs = [];
            List<SyntaxNodeOrToken> baseArgs = [];
            List<StatementSyntax> constructorBodyMembers = [];
            if (_modelData.BaseData != null)
            {
                baseArgs.SafeAddArgument(IdentifierName(Meta.Parameters.Name));
                baseArgs.SafeAddArgument(IdentifierName(Meta.Parameters.XMLTag));
            }
            else
            {
                constructorBodyMembers.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(Constants.Meta.IdentifierNameVarName), IdentifierName(Meta.Parameters.Name))));
                constructorBodyMembers.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(Constants.Meta.XMLTagVarName), IdentifierName(Meta.Parameters.XMLTag))));
              
            }
            baseArgs.SafeAddArgument(IdentifierName(Meta.Parameters.PathPrefix));
            contructorArgs.SafeAdd(Parameter(Identifier(Meta.Parameters.Name))
            .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))));

            contructorArgs.SafeAdd(Parameter(Identifier(Meta.Parameters.XMLTag))
                .WithType(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword))))
                .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression))));

            contructorArgs.SafeAdd(Parameter(Identifier(Meta.Parameters.PathPrefix))
                                        .WithType(
                                            PredefinedType(
                                                Token(SyntaxKind.StringKeyword)))
                                        .WithDefault(
                                            EqualsValueClause(
                                                  CreateStringLiteral(""))));

            members.Add(ConstructorDeclaration(Identifier(_modelData.MetaName))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
             .WithParameterList(
                                ParameterList(
                                    SeparatedList<ParameterSyntax>(contructorArgs)))
             .WithInitializer(ConstructorInitializer(
                                    SyntaxKind.BaseConstructorInitializer,
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(baseArgs))))
             .WithBody(Block(List<StatementSyntax>(
                 constructorBodyMembers))));
            if (_modelData.BaseData == null)
            {
                members.Add(PropertyDeclaration(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword))), Constants.Meta.XMLTagVarName)
                .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ReadOnlyKeyword)]))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

                members.Add(PropertyDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Constants.Meta.IdentifierNameVarName)
                    .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ReadOnlyKeyword)]))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
            }
        }
        List<SyntaxToken> modifiers =
    [
        Token(SyntaxKind.PublicKeyword)
    ];
        if (_modelData.BaseData != null)
        {
            modifiers.Add(Token(SyntaxKind.NewKeyword));
        }

        string _suffix = Utils.GenerateUniqueNameSuffix($"{_modelData.Symbol.ContainingAssembly.Name}\0{_modelData.FullName}");
        members.Add(CreateConstStringVar($"TDLReportName", $"{_modelData.Name}_{_suffix}")
            .WithModifiers(TokenList(modifiers)));
        members.Add(CreateConstStringVar($"TDLDefaultCollectionName", $"{_modelData.Name}Coll_{_suffix}")
            .WithModifiers(TokenList(modifiers)));

        members.Add(CreateConstStringVar($"TallyObjectType", _modelData.Name.ToUpper())
            .WithModifiers(TokenList(modifiers)));




        if (_modelData.BaseData != null &&
            !SymbolEqualityComparer.Default.Equals(_modelData.BaseData.Symbol.ContainingAssembly, _modelData.Symbol.ContainingAssembly))
        {
            AddMembersAsProperties(_modelData.BaseData.Members);
        }
        AddMembersAsProperties(_modelData.Members);

        var uniqueFields = _modelData.GetUniqueMemberNames();
        List<SyntaxNodeOrToken> fieldNameExpressions = [];
        foreach (var field in uniqueFields)
        {
            fieldNameExpressions.SafeAddExpressionElement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                      IdentifierName(field),
                                                                      IdentifierName("Name")));
        }
        members.Add(PropertyDeclaration(GenericName(
                                    Identifier("List"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            PredefinedType(
                                                Token(SyntaxKind.StringKeyword))))), Identifier("FieldNames"))
            .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(fieldNameExpressions))))
            .WithModifiers(TokenList(modifiers))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        if (_modelData.GenerateITallyRequestableObectAttribute)
        {
            var allMembers = _modelData.AllUniqueMembers.Values;
            List<SyntaxNodeOrToken> fieldExpressions = [];
            List<SyntaxNodeOrToken> PartExpressions = [];
            List<SyntaxNodeOrToken> LineExpressions = [];
            foreach (var field in allMembers)
            {
                var name = field.PropertyData.Name;
                if (field.PropertyData.IsComplex)
                {
                    List<SyntaxToken> partModifiers =
                    [
                        Token(SyntaxKind.PublicKeyword)
                    ];
                    List<SyntaxNodeOrToken> constructorArgs = [];
                    constructorArgs.SafeAddArgument(IdentifierName($"{name}.{Meta.IdentifierNameVarName}"));
                    constructorArgs.SafeAddArgument(IdentifierName($"{name}.{Meta.XMLTagVarName}"));
                    members.Add(PropertyDeclaration(IdentifierName(PartTypeName),
                                                    $"{name}Part")
                        .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(constructorArgs)))
                        .WithModifiers(TokenList(partModifiers))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
                    PartExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{name}Part"));
                    LineExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{name}Part"));
                }
                else
                {
                    fieldExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{name}"));
                }

            }
            members.Add(PropertyDeclaration(GenericName(
                                    Identifier("List"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            IdentifierName(Constants.Models.Abstractions.PropertyMetaDataTypeName)))), Identifier("Fields"))
                .WithModifiers(TokenList(modifiers))
                .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(fieldExpressions))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }

        return members;

        void AddMembersAsProperties(Dictionary<string, ClassPropertyData> modelMembers)
        {
            foreach (var member in modelMembers)
            {
                var val = member.Value;
                AddPropertyMember(val);
            }
        }
        void AddPropertyMember(ClassPropertyData val)
        {
            List<SyntaxNodeOrToken> arguments = [];
            List<SyntaxNodeOrToken>? IntializerArguments = null;
            arguments.SafeAddArgument(CreateStringLiteral(val.UniqueName.ToUpper()));

            var typeName = Constants.Models.Abstractions.PropertyMetaDataTypeName;
            if (val.IsComplex)
            {
                if (val.ClassData == null) return;
                typeName = val.ClassData.MetaName;
                arguments.SafeAdd(Argument(CreateStringLiteral(val.Name.ToUpper())).WithNameColon(NameColon(IdentifierName(Meta.Parameters.PathPrefix))));
            }
            else
            {
                arguments.SafeAddArgument(CreateStringLiteral(val.XMLTag ?? val.Name.ToUpper()));
            }
            List<SyntaxToken> tokens = [Token(SyntaxKind.PublicKeyword)];
            if (val.IsOverridenProperty)
            {
                tokens.Add(Token(SyntaxKind.NewKeyword));
            }
            members.Add(PropertyDeclaration(IdentifierName(typeName), val.Name)

                    .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(arguments, IntializerArguments)))
                    .WithModifiers(TokenList(tokens))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }
    }
    internal void GenerateMetaField()
    {
        List<SyntaxToken> tokens = [Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)];
        if (_modelData.BaseData != null)
        {
            tokens.Add(Token(SyntaxKind.NewKeyword));
        }
        string typeName = $"{_modelData.Namespace}.Meta.{_modelData.MetaName}";
        List<MemberDeclarationSyntax> members = [
            PropertyDeclaration(GetGlobalNameforType(typeName), "Meta")
             .WithExpressionBody(ArrowExpressionClause(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,GetGlobalNameforType(typeName), IdentifierName("Instance"))))
             .WithModifiers(TokenList(tokens))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            ];
        List<UsingDirectiveSyntax> usings = [UsingDirective(IdentifierName(ExtensionsNameSpace))];

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
                    .WithMembers(List(members))
                }))
          })).NormalizeWhitespace().ToFullString();
        context.AddSource($"{_modelData.Name}_{_modelData.Namespace}.g.cs", unit);
    }


}
