using System.Xml.Linq;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public class MetaDataGenerator
{
    private readonly ClassData _modelData;
    private readonly SourceProductionContext context;
    private CancellationToken token;
    private readonly string _suffix;

    public bool IsBaseNull { get; }
    public bool IsBaseSameAssembly { get; }

    public MetaDataGenerator(ClassData modelData,
                             SourceProductionContext context,
                             CancellationToken token)
    {
        _modelData = modelData;
        this.context = context;
        this.token = token;
        _suffix = Utils.GenerateUniqueNameSuffix($"{_modelData.Symbol.ContainingAssembly.Name}\0{_modelData.FullName}");
        IsBaseNull = _modelData.BaseData == null;
        IsBaseSameAssembly = !IsBaseNull && SymbolEqualityComparer.Default.Equals(_modelData.BaseData!.Symbol.ContainingAssembly, _modelData.Symbol.ContainingAssembly);
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
        List<MemberDeclarationSyntax> members = [];

        List<SyntaxToken> InstanceFieldModifiers =
        [
        Token(SyntaxKind.PublicKeyword)
        ];
        if (_modelData.BaseData != null)
        {
            InstanceFieldModifiers.Add(Token(SyntaxKind.NewKeyword));
        }
        InstanceFieldModifiers.Add(Token(SyntaxKind.StaticKeyword));
        members.Add(PropertyDeclaration(IdentifierName(_modelData.MetaName), Meta.InstanceVarName)
            .WithModifiers(TokenList(InstanceFieldModifiers))
              .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression([])))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        List<StatementSyntax> constructorStatements = [];
        List<SyntaxNodeOrToken> baseConstructorArgs = [];
        if (_modelData.GenerateITallyRequestableObject)
        {
            if (IsBaseNull)
            {
                constructorStatements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(Constants.Meta.IdentifierNameVarName), CreateStringLiteral($"{_modelData.Name}_{_suffix}"))));
                constructorStatements.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(Constants.Meta.XMLTagVarName), CreateStringLiteral(_modelData.XMLTag))));
            }
            else
            {
                baseConstructorArgs.SafeAddArgument(CreateStringLiteral($"{_modelData.Name}_{_suffix}"));
                baseConstructorArgs.SafeAddArgument(CreateStringLiteral(_modelData.XMLTag));
            }
        }

        baseConstructorArgs.SafeAddArgument(IdentifierName(Meta.Parameters.PathPrefix));
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
                                        SeparatedList<ArgumentSyntax>(
                                           baseConstructorArgs))))
             .WithBody(Block(constructorStatements)));

        if (!_modelData.GenerateITallyRequestableObject && !_modelData.IsEnum)
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

        }

        if (_modelData.BaseData == null && !_modelData.IsEnum)
        {
            members.Add(PropertyDeclaration(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword))), Constants.Meta.XMLTagVarName)
                  .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ReadOnlyKeyword)]))
                  .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            members.Add(PropertyDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Constants.Meta.IdentifierNameVarName)
                .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ReadOnlyKeyword)]))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        }
        List<SyntaxToken> modifiers =
        [
            Token(SyntaxKind.PublicKeyword)
        ];
        if (_modelData.BaseData != null)
        {
            modifiers.Add(Token(SyntaxKind.NewKeyword));
        }

        if (!_modelData.IsEnum)
        {
            members.Add(FieldDeclaration(CreateVariableDelaration((TypeSyntax)PredefinedType(Token(SyntaxKind.StringKeyword)),
                                                                                          Meta.CollectionNameVarName,
                                                                                         _modelData.IsTallyComplexObject ? CreateNullLiteral() : CreateStringLiteral(_modelData.GenerateITallyRequestableObject ? $"{_modelData.Name}Coll_{_suffix}" : _modelData.Name.ToUpper())))
                .WithModifiers(TokenList(modifiers)));
        }
        if (_modelData.GenerateITallyRequestableObject)
        {
            members.Add(CreateConstStringVar($"TallyObjectType", _modelData.TDLCollectionData?.Type ?? _modelData.Name.ToUpper())
                .WithModifiers(TokenList(modifiers)));

        }

        if (_modelData.BaseData != null &&
            !SymbolEqualityComparer.Default.Equals(_modelData.BaseData.Symbol.ContainingAssembly, _modelData.Symbol.ContainingAssembly))
        {
            AddMembersAsProperties(_modelData.BaseData.Members);
        }


        if (!_modelData.IsEnum)
        {
            AddMembersAsProperties(_modelData.Members);
            var uniqueSimpleFields = _modelData.GetUniqueSimpleMembers();
            List<SyntaxNodeOrToken> fieldNameExpressions = [];
            List<SyntaxNodeOrToken> fetchTextExpressions = [];
            List<SyntaxNodeOrToken> nameSetExpressions = [];
            foreach (var simpleField in uniqueSimpleFields)
            {
                var simpleProp = simpleField.Value;
                if (!simpleProp.IsList)
                {
                    fieldNameExpressions.SafeAddExpressionElement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                              IdentifierName(simpleProp.Name),
                                                                              IdentifierName("Name")));
                }
                if (!_modelData.IsTallyComplexObject)
                {
                    fetchTextExpressions.SafeAddExpressionElement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                              IdentifierName(simpleProp.Name),
                                                                              IdentifierName("FetchText")));
                }


            }
            if (_modelData.IsTallyComplexObject)
            {
                fetchTextExpressions.SafeAddExpressionElement(IdentifierName("_pathPrefix"));
            }
            members.Add(PropertyDeclaration(GenericName(
                                        Identifier("List"))
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                PredefinedType(
                                                    Token(SyntaxKind.StringKeyword))))), Identifier(Meta.FieldNamesVarName))
                .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(fieldNameExpressions))))
                .WithModifiers(TokenList(modifiers))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
            members.Add(PropertyDeclaration(GenericName(
                                        Identifier("List"))
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                PredefinedType(
                                                    Token(SyntaxKind.StringKeyword))))), Identifier("FetchText"))
                .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(fetchTextExpressions))))
                .WithModifiers(TokenList(modifiers))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));



            var uniqueComlexFields = _modelData.GetUniqueComplexMembers();
            List<SyntaxNodeOrToken> explodeExpressions = [];
            foreach (var field in uniqueComlexFields)
            {
                string text = ":YES";
                ClassPropertyData prop = field.Value;
                List<InterpolatedStringContentSyntax> nodes =
                    [
                        Interpolation(IdentifierName($"{prop.Name}.{(prop.IsComplex ? Meta.IdentifierNameVarName:"Name" )}")),
                        //InterpolatedStringText()
                        //.WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, path, path, TriviaList()))
                    ];
                var explodeCondition = prop?.TDLCollectionData?.ExplodeCondition;
                if (explodeCondition == null)
                {
                    nodes.Add(InterpolatedStringText()
                        .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
                }
                else
                {
                    text = $":{explodeCondition}";
                    nodes.Add(InterpolatedStringText()
                        .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
                    // nodes.Add(Interpolation(CreateFormatExpression(explodeCondition, name)));
                }
                InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
              .WithContents(List(nodes));
                explodeExpressions.SafeAddExpressionElement(interpolatedStringExpressionSyntax);
            }
            members.Add(PropertyDeclaration(GenericName(
                                   Identifier("List"))
                               .WithTypeArgumentList(
                                   TypeArgumentList(
                                       SingletonSeparatedList<TypeSyntax>(
                                           PredefinedType(
                                               Token(SyntaxKind.StringKeyword))))), Identifier(Meta.ExplodesVarName))
           .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(explodeExpressions))))
           .WithModifiers(TokenList(modifiers))
           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }
        if (_modelData.GenerateITallyRequestableObject)
        {
            List<SyntaxToken> requestableObjectmodifiers =
            [
                Token(SyntaxKind.PublicKeyword)
            ];
            var allMembers = _modelData.AllUniqueMembers.Values;
            List<SyntaxNodeOrToken> fieldExpressions = [];
            List<SyntaxNodeOrToken> allFetchExpressions = [SpreadElement(IdentifierName($"FetchText"))];
            List<SyntaxNodeOrToken> PartExpressions = [];
            List<SyntaxNodeOrToken> LineExpressions = [];
            List<SyntaxNodeOrToken> nameSetExpressions = [];
            HashSet<String> nameSetNames = [];
            foreach (var field in allMembers)
            {
                ClassPropertyData propertyData = field.PropertyData;
                var name = propertyData.Name;
                if (propertyData.IsComplex || propertyData.IsList)
                {
                    PartExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{name}Part"));
                    LineExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{name}Line"));
                    if (propertyData.IsComplex)
                    {
                        allFetchExpressions.SafeAdd(SpreadElement(IdentifierName($"{field.Path}{name}.FetchText")));
                    }
                    else
                    {
                        fieldExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{name}"));
                    }

                    continue;
                }

                fieldExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{name}"));
                if (propertyData.IsEnum && nameSetNames.Add(propertyData.ClassData!.FullName))
                {
                    nameSetExpressions.SafeAddExpressionElement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                              GetGlobalNameforType($"{propertyData.ClassData.Namespace}.Meta.{propertyData.ClassData.Name}Meta.Instance"),
                                                                              IdentifierName("NameSet")));
                }

            }

            members.Add(PropertyDeclaration(GenericName(
                                    Identifier("List"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            GetGlobalNameforType(PartFullTypeName)))), Identifier(Meta.AllPartsVarName))
                .WithModifiers(TokenList(modifiers))
                .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(PartExpressions))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
            members.Add(PropertyDeclaration(GenericName(
                                    Identifier("List"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            GetGlobalNameforType(LineFullTypeName)))), Identifier(Meta.AllLinesVarName))
                .WithModifiers(TokenList(modifiers))
                .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(LineExpressions))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
            members.Add(PropertyDeclaration(GenericName(
                                    Identifier("List"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            GetGlobalNameforType(FieldFullTypeName)))), Identifier(Meta.FieldsVarName))
                .WithModifiers(TokenList(modifiers))
                .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(fieldExpressions))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            members.Add(PropertyDeclaration(GenericName(
                                    Identifier("List"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            PredefinedType(Token(SyntaxKind.StringKeyword))))),
                                            Identifier(Meta.AllFetchTextVarName))
                .WithModifiers(TokenList(modifiers))
                .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(allFetchExpressions))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            members.Add(PropertyDeclaration(GenericName(Identifier("List"))
                                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(GetGlobalNameforType(TDLNameSetFullTypeName)))),
                                   Identifier(Meta.NameSetsVarName))
               .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(nameSetExpressions))))
               .WithModifiers(TokenList(modifiers))
               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            List<SyntaxNodeOrToken> partConstructorArgs = [];
            partConstructorArgs.SafeAddArgument(IdentifierName(Meta.IdentifierNameVarName));
            partConstructorArgs.SafeAddArgument(IdentifierName(Meta.CollectionNameVarName));
            partConstructorArgs.SafeAddArgument(IdentifierName(Meta.IdentifierNameVarName));

            members.Add(PropertyDeclaration(GetGlobalNameforType(PartFullTypeName),
                                            Identifier(Meta.TDLDefaultPartVarName))
                .WithModifiers(TokenList(modifiers))
                .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(partConstructorArgs)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            List<SyntaxNodeOrToken> lineConstructorArgs = [];
            List<SyntaxNodeOrToken> lineIntializerArgs = [];
            lineConstructorArgs.SafeAddArgument(IdentifierName(Meta.IdentifierNameVarName));
            lineConstructorArgs.SafeAddArgument(IdentifierName(Meta.FieldNamesVarName));
            lineConstructorArgs.SafeAddArgument(IdentifierName(Meta.XMLTagVarName));

            lineIntializerArgs.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("Explode"), IdentifierName(Meta.ExplodesVarName)));
            members.Add(PropertyDeclaration(GetGlobalNameforType(LineFullTypeName),
                                            Identifier(Meta.TDLDefaultLineVarName))
                .WithModifiers(TokenList(modifiers))
                .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(lineConstructorArgs, lineIntializerArgs)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            List<SyntaxNodeOrToken> collectionConstructorArgs = [];
            collectionConstructorArgs.SafeAddArgument(IdentifierName(Meta.CollectionNameVarName));
            collectionConstructorArgs.SafeAddArgument(IdentifierName(Meta.ObjectTypeVarName));
            collectionConstructorArgs.SafeAdd(Argument(IdentifierName(Meta.AllFetchTextVarName)).WithNameColon(NameColon(IdentifierName("nativeFields"))));
            members.Add(PropertyDeclaration(GetGlobalNameforType(CollectionFullTypeName),
                                            Identifier(Meta.DefaultCollectionVarName))
                .WithModifiers(TokenList(modifiers))
                .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(collectionConstructorArgs)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        }


        if (_modelData.IsEnum)
        {
            List<SyntaxNodeOrToken> nameSetListArgs = [];

            List<SyntaxNodeOrToken> enumConstructorArgs = [];
            List<SyntaxNodeOrToken> enumConstructorIntializerArgs = [];

            enumConstructorArgs.SafeAddArgument(CreateStringLiteral($"{_modelData.Name}_{_suffix}"));

            var items = _modelData.Members.Values.SelectMany(c => c.DefaultXMLData?.EnumChoices
                .Where(choice => !string.IsNullOrWhiteSpace(choice.Choice)).Select(ch => $"{ch.Choice}:\"{c.Name}\"")).Select(c => ExpressionElement(CreateStringLiteral(c)));

            enumConstructorIntializerArgs.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                      IdentifierName("List"),
                      CollectionExpression(SeparatedList<CollectionElementSyntax>(items))));
            members.Add(PropertyDeclaration(GetGlobalNameforType(TDLNameSetFullTypeName), "NameSet")
                .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(enumConstructorArgs, enumConstructorIntializerArgs)))
                .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword)]))
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
                AddPartAndLine(val);
                if (val.ClassData == null) return;
                typeName = val.ClassData.MetaName;

                string path = $"{val.TDLCollectionData?.CollectionName ?? val.Name.ToUpper()}";
                arguments.SafeAdd(Argument(InvocationExpression(IdentifierName("GeneratePath"))
                    .WithArgumentList(ArgumentList([Argument(CreateStringLiteral(path))]))).WithNameColon(NameColon(IdentifierName(Meta.Parameters.PathPrefix))));
            }
            else
            {
                if (val.IsList)
                {
                    AddPartAndLine(val);
                }
                arguments.SafeAddArgument(CreateStringLiteral(val.DefaultXMLData?.XmlTag ?? val.Name.ToUpper()));

                if (val.TDLFieldData?.Set != null)
                {
                    if (_modelData.IsTallyComplexObject)
                    {
                        arguments.SafeAddArgument(CreateNullLiteral());
                    }
                    else
                    {
                        arguments.SafeAddArgument(CreateStringLiteral(val.IsEnum ? $"$$NameGetValue:{val.TDLFieldData?.Set}:{val.ClassData!.Name}_{Utils.GenerateUniqueNameSuffix($"{val.ClassData.Symbol.ContainingAssembly.Name}\0{val.ClassData.FullName}")}" : val.TDLFieldData.Set));
                    }

                }

                string path = $"{val.TDLFieldData!.FetchText}";

                arguments.SafeAddArgument(InvocationExpression(IdentifierName("GeneratePath"))
                    .WithArgumentList(ArgumentList([Argument(CreateStringLiteral(path))])));
            }
            List<SyntaxToken> tokens = [Token(SyntaxKind.PublicKeyword)];
            if (val.IsOverridenProperty)
            {
                tokens.Add(Token(SyntaxKind.NewKeyword));
            }
            if (val.TDLFieldData?.Invisible != null)
            {
                IntializerArguments ??= [];
                IntializerArguments.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("Invisible"), CreateStringLiteral(val.TDLFieldData.Invisible)));
            }
            else if (val.IsEnum || val.IsNullable && !val.IsComplex)
            {
                IntializerArguments ??= [];
                IntializerArguments.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("Invisible"), CreateStringLiteral("$$ISEmpty:$$value")));
            }
            if (val.TDLFieldData?.TallyType != null)
            {
                IntializerArguments ??= [];
                IntializerArguments.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("TDLType"), CreateStringLiteral(val.TDLFieldData.TallyType)));
            }
            if (val.TDLFieldData?.Format != null)
            {
                IntializerArguments ??= [];
                IntializerArguments.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("Format"), CreateStringLiteral(val.TDLFieldData.Format)));
            }
            members.Add(PropertyDeclaration(IdentifierName(typeName), val.Name)

                    .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(arguments, IntializerArguments)))
                    .WithModifiers(TokenList(tokens))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }


        void AddPartAndLine(ClassPropertyData propertyData)
        {
            var name = propertyData.Name;
            List<SyntaxToken> partModifiers =
                  [
                      Token(SyntaxKind.PublicKeyword)
                  ];
            if (propertyData.IsOverridenProperty)
            {
                partModifiers.Add(Token(SyntaxKind.NewKeyword));
            }
            List<SyntaxNodeOrToken> constructorArgs = [];
            List<SyntaxNodeOrToken>? IntializerArgs = null;

            List<SyntaxNodeOrToken> lineConstructorArgs = [];
            List<SyntaxNodeOrToken>? lineIntializerArgs = null;
            string LineOrPartName = $"{name}.{(propertyData.IsComplex ? Meta.IdentifierNameVarName : "Name")}";
            constructorArgs.SafeAddArgument(IdentifierName(LineOrPartName));
            lineConstructorArgs.SafeAddArgument(IdentifierName(LineOrPartName));

            if (propertyData.IsComplex)
            {
                lineConstructorArgs.SafeAddArgument(IdentifierName($"{name}.{Meta.FieldNamesVarName}"));
                lineConstructorArgs.SafeAddArgument(CreateStringLiteral(propertyData.DefaultXMLData?.XmlTag ?? name.ToUpper()));


                lineIntializerArgs ??= [];
                lineIntializerArgs.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("Explode"), IdentifierName($"{name}.{Meta.ExplodesVarName}")));

            }
            else
            {
                lineConstructorArgs.SafeAddArgument(CollectionExpression(SeparatedList<CollectionElementSyntax>(new SyntaxNodeOrToken[]
                {
                    ExpressionElement(IdentifierName($"{name}.Name")),
                })));
            }
            if (propertyData.TDLCollectionData?.CollectionName != null)
            {
                constructorArgs.SafeAddArgument(CreateStringLiteral(propertyData.TDLCollectionData.CollectionName));
            }
            else
            {
                constructorArgs.SafeAddArgument(IdentifierName($"{name}.{Meta.CollectionNameVarName}"));
            }
            if (propertyData.IsList)
            {
                if (propertyData.ListXMLTag != null)
                {
                    IntializerArgs ??= [];
                    IntializerArgs.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("XMLTag"), CreateStringLiteral(propertyData.ListXMLTag)));
                }
            }
            if (propertyData.IsTallyComplexObject)
            {
                lineIntializerArgs ??= [];
                List<SyntaxNodeOrToken> LocalCollectionArgs = [];
                foreach (var tallySimpleProperty in propertyData.ClassData!.Members.Values)
                {
                    List<InterpolatedStringContentSyntax> interpolatedStringContentSyntaxes = [];
                    interpolatedStringContentSyntaxes.AddText("Field:");
                    interpolatedStringContentSyntaxes.AddIdentifier($"{propertyData.Name}.{tallySimpleProperty.Name}.Name");
                    interpolatedStringContentSyntaxes.AddText(":Set:");
                    interpolatedStringContentSyntaxes.AddText(string.Format(tallySimpleProperty.TDLFieldData!.Set, propertyData.TDLFieldData!.Set));
                    var syntax = InterpolatedStringExpression(
                           Token(SyntaxKind.InterpolatedStringStartToken)).WithContents(List(interpolatedStringContentSyntaxes));
                    LocalCollectionArgs.SafeAddExpressionElement(syntax);
                }
                lineIntializerArgs.SafeAdd(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("Local"), CollectionExpression(
                                                            SeparatedList<CollectionElementSyntax>(LocalCollectionArgs))));
            }
            members.Add(PropertyDeclaration(IdentifierName(PartTypeName),
                                             $"{name}Part")
                 .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(constructorArgs, IntializerArgs)))
                 .WithModifiers(TokenList(partModifiers))
                 .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            members.Add(PropertyDeclaration(IdentifierName(LineTypeName),
                                            $"{name}Line")
                .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(lineConstructorArgs, lineIntializerArgs)))
                .WithModifiers(TokenList(partModifiers))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        }
    }
    internal void GenerateMetaField()
    {
        if (_modelData.IsEnum)
        {
            return;
        }
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
                    .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(GetGlobalNameforType(Constants.Models.Abstractions.IMetaGeneratedFullTypeName)))))
                }))
          })).NormalizeWhitespace().ToFullString();
        context.AddSource($"{_modelData.Name}_{_modelData.Namespace}.g.cs", unit);
    }


}
