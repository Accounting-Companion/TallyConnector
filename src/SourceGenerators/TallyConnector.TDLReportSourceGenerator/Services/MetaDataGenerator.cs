using System.Xml.Linq;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public class MetaDataGenerator
{
    private readonly ClassData _modelData;
    private readonly SourceProductionContext _context;
    private CancellationToken _token;
    private readonly string _suffix;

    public bool IsBaseNull { get; }
    public bool IsBaseSameAssembly { get; }
    public bool IsBaseRequestableObject { get; }

    public MetaDataGenerator(ClassData modelData,
                             SourceProductionContext context,
                             CancellationToken token)
    {
        _modelData = modelData;
        _context = context;
        _token = token;
        _suffix = Utils.GenerateUniqueNameSuffix($"{_modelData.Symbol.ContainingAssembly.Name}\0{_modelData.FullName}");
        IsBaseNull = _modelData.BaseData == null;
        IsBaseSameAssembly = !IsBaseNull && SymbolEqualityComparer.Default.Equals(_modelData.BaseData!.Symbol.ContainingAssembly, _modelData.Symbol.ContainingAssembly);
        IsBaseRequestableObject = !IsBaseNull && _modelData.BaseData!.Symbol.CheckInterface(Constants.Models.Interfaces.TallyRequestableObjectInterfaceFullName);
    }

    public MetaDataGenerator GenerateMeta()
    {

        SimpleBaseTypeSyntax node;
        if (IsBaseSameAssembly)
        {
            node = SimpleBaseType(
                              GetGlobalNameforType($"{_modelData.BaseData!.Namespace}.Meta.{_modelData.BaseData.MetaName}"));
        }
        else
        {
            node = SimpleBaseType(
                              IdentifierName(Constants.Models.Abstractions.MetaObjectypeName));
        }

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
                            node)));

        var unit = CompilationUnit()
          .WithUsings(List(GetUsings()))
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
        _context.AddSource($"meta.{_modelData.Name}_{_modelData.Namespace}.g.cs", unit);
        return this;
    }


    private IEnumerable<MemberDeclarationSyntax> GetClassMembers()
    {
        List<MemberDeclarationSyntax> members = [];
        members.Add(GetModelInstanceMember());

        // If ENUM then we add only one property and early return
        if (_modelData.IsEnum)
        {
            members.Add(GetEnumProperties());
            return members;
        }

        members.Add(GetMainConstructor());

        members.Add(GetSecondConstructor(IsBaseSameAssembly, _modelData.MetaName));

        if (!IsBaseSameAssembly)
        {
            members.Add(PropertyDeclaration(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                            Constants.Meta.XMLTagVarName)
                  .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ReadOnlyKeyword)]))
                  .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            members.Add(PropertyDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)),
                                            Constants.Meta.IdentifierNameVarName)
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
        LiteralExpressionSyntax expressionSyntax;
        if (_modelData.IsTallyComplexObject)
        {
            expressionSyntax = CreateNullLiteral();
        }
        else
        {
            expressionSyntax = CreateStringLiteral(_modelData.TDLCollectionData?.CollectionName ?? (_modelData.GenerateITallyRequestableObject ? $"{_modelData.Name}Coll_{_suffix}" : _modelData.Name.ToUpper()));

        }

        members.Add(FieldDeclaration(CreateVariableDelaration(PredefinedType(Token(SyntaxKind.StringKeyword)),
                                                              Meta.CollectionNameVarName,
                                                              expressionSyntax))
                .WithModifiers(TokenList(modifiers)));
        List<SyntaxToken> requestableObjectmodifiers =
           [
               Token(SyntaxKind.PublicKeyword)
           ];
        if (IsBaseRequestableObject)
        {
            requestableObjectmodifiers.Add(Token(SyntaxKind.NewKeyword));
        }
        if (_modelData.GenerateITallyRequestableObject)
        {
            members.Add(CreateConstStringVar(Meta.ObjectTypeVarName, _modelData.TDLCollectionData?.Type ?? _modelData.Name.ToUpper())
                .WithModifiers(TokenList(requestableObjectmodifiers)));

        }
        if (_modelData.BaseData != null && !_modelData.BaseData.Symbol.CheckInterface(Constants.Models.Abstractions.IMetaGeneratedFullTypeName) &&
            !SymbolEqualityComparer.Default.Equals(_modelData.BaseData.Symbol.ContainingAssembly, _modelData.Symbol.ContainingAssembly))
        {
            AddBaseMembersAsProperties(_modelData.BaseData);
            
        }
        AddMembersAsProperties(_modelData.Members);

        HandleDirectProperties(members, modifiers);

        if (_modelData.GenerateITallyRequestableObject)
        {
            AddITallyRequestableRequiredMembers(members, requestableObjectmodifiers);

        }
        return members;

        void AddBaseMembersAsProperties(ClassData classData)
        {
            if(classData.BaseData != null)
            {
                AddBaseMembersAsProperties(classData.BaseData);
            }
            AddMembersAsProperties(classData.Members);
        }

        void AddMembersAsProperties(Dictionary<string, ClassPropertyData> modelMembers)
        {
            foreach (var member in modelMembers)
            {
                var val = member.Value;
                if (val.XmlIgnore) continue;
                AddPropertyMember(val);
            }
        }
        void AddPropertyMember(ClassPropertyData val)
        {
            List<SyntaxNodeOrToken> arguments = [];
            List<SyntaxNodeOrToken>? IntializerArguments = null;
            arguments.SafeAddArgument(CreateStringLiteral(val.UniqueName.ToUpper()));

            NameSyntax NameSyntax;
            if (val.IsComplex)
            {
                AddPartAndLine(val, members);
                if (val.ClassData == null) return;
                var typeName = val.ClassData.MetaName;
                if (val.XMLData.Count > 0)
                {
                    typeName = GetMultiXMLDataMetaClassName(val);
                    GenerateMultiXMLMetaData(val, _context, _token, members);
                    arguments.SafeAdd(Argument(IdentifierName(Meta.PathPrefixVarName))
                        .WithNameColon(NameColon(IdentifierName(MultiXMLMeta.Parameters.OriginalPrefix))));

                }
                string path = $"{val.TDLCollectionData?.CollectionName ?? val.Name.ToUpper()}";
                arguments.SafeAdd(Argument(InvocationExpression(IdentifierName("GeneratePath"))
                    .WithArgumentList(ArgumentList([Argument(CreateStringLiteral(path))])))
                    .WithNameColon(NameColon(IdentifierName(Meta.Parameters.PathPrefix))));

                NameSyntax = IdentifierName(typeName);
            }
            else
            {
                if (val.IsList)
                {
                    AddPartAndLine(val, members);
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

                NameSyntax = GenericName(Constants.Models.Abstractions.PropertyMetaDataTypeName)
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new SyntaxNodeOrToken[]{
                                                    IdentifierName(val.PropertyOriginalType.GetClassMetaName()) })));
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

            members.Add(PropertyDeclaration(NameSyntax, val.Name)

                    .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(arguments, IntializerArguments)))
                    .WithModifiers(TokenList(tokens))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }
    }

    private static void AddPartAndLine(ClassPropertyData propertyData,
                                       List<MemberDeclarationSyntax> members,
                                       bool isMultiXML = false,
                                       XMLData? xmlData = null)
    {
        var name = isMultiXML ? $"{propertyData.Name}.{xmlData!.ClassData!.Name}" : propertyData.Name;
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
            lineConstructorArgs.SafeAddArgument(CreateStringLiteral((isMultiXML ? xmlData?.XmlTag : propertyData.DefaultXMLData?.XmlTag) ?? name.ToUpper()));


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
        if (propertyData.TDLCollectionData?.CollectionName != null || isMultiXML && xmlData!.ClassData!.TDLCollectionData?.CollectionName != null)
        {
            constructorArgs.SafeAddArgument(isMultiXML ? CreateStringLiteral(xmlData!.ClassData!.TDLCollectionData!.CollectionName!) : CreateStringLiteral(propertyData.TDLCollectionData!.CollectionName!));
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
                                         $"{name.Replace(".", "")}Part")
             .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(constructorArgs, IntializerArgs)))
             .WithModifiers(TokenList(partModifiers))
             .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        members.Add(PropertyDeclaration(IdentifierName(LineTypeName),
                                        $"{name.Replace(".", "")}Line")
            .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(lineConstructorArgs, lineIntializerArgs)))
            .WithModifiers(TokenList(partModifiers))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

    }


    private PropertyDeclarationSyntax GetEnumProperties()
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
        PropertyDeclarationSyntax enumNameProperty = PropertyDeclaration(GetGlobalNameforType(TDLNameSetFullTypeName), "NameSet")
            .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(enumConstructorArgs, enumConstructorIntializerArgs)))
            .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword)]))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        return enumNameProperty;
    }


    private static List<UsingDirectiveSyntax> GetUsings()
    {
        return [
                    UsingDirective(IdentifierName(ExtensionsNameSpace)),
            UsingDirective(IdentifierName(TallyConnectorRequestModelsNameSpace)),
            UsingDirective(IdentifierName(Constants.Models.Abstractions.PREFIX))
                    ];
    }


    private PropertyDeclarationSyntax GetModelInstanceMember()
    {
        List<SyntaxToken> InstanceFieldModifiers =
        [
        Token(SyntaxKind.PublicKeyword)
        ];
        if (_modelData.BaseData != null)
        {
            InstanceFieldModifiers.Add(Token(SyntaxKind.NewKeyword));
        }
        InstanceFieldModifiers.Add(Token(SyntaxKind.StaticKeyword));

        return PropertyDeclaration(IdentifierName(_modelData.MetaName), Meta.InstanceVarName)
            .WithModifiers(TokenList(InstanceFieldModifiers))
              .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression([])))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    private ConstructorDeclarationSyntax GetMainConstructor()
    {
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
        ConstructorDeclarationSyntax mainConstructorSyntax = ConstructorDeclaration(Identifier(_modelData.MetaName))
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
             .WithBody(Block(constructorStatements));
        return mainConstructorSyntax;
    }

    private ConstructorDeclarationSyntax GetSecondConstructor(bool addBaseArgs,
                                                              string constructorIdentifier,
                                                              bool isInvokedFromMultiXML = false)
    {
        List<SyntaxNodeOrToken> contructorArgs = [];
        List<SyntaxNodeOrToken> baseArgs = [];
        List<StatementSyntax> constructorBodyMembers = [];
        if (addBaseArgs)
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
        if (isInvokedFromMultiXML)
        {
            contructorArgs.SafeAdd(Parameter(Identifier(MultiXMLMeta.Parameters.OriginalPrefix))
                .WithType(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword))))
                .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression))));

            constructorBodyMembers.Add(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName(Constants.MultiXMLMeta.OriginalPrefixVarName), IdentifierName(MultiXMLMeta.Parameters.OriginalPrefix))));
        }
        contructorArgs.SafeAdd(Parameter(Identifier(Meta.Parameters.PathPrefix))
                                    .WithType(
                                        PredefinedType(
                                            Token(SyntaxKind.StringKeyword)))
                                    .WithDefault(
                                        EqualsValueClause(
                                              CreateStringLiteral(""))));

        ConstructorDeclarationSyntax secondConstructor = ConstructorDeclaration(Identifier(constructorIdentifier)).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
         .WithParameterList(
                            ParameterList(
                                SeparatedList<ParameterSyntax>(contructorArgs)))
         .WithInitializer(ConstructorInitializer(
                                SyntaxKind.BaseConstructorInitializer,
                                ArgumentList(
                                    SeparatedList<ArgumentSyntax>(baseArgs))))
         .WithBody(Block(List<StatementSyntax>(
             constructorBodyMembers)));
        return secondConstructor;
    }

    private void HandleDirectProperties(List<MemberDeclarationSyntax> members,
                                        List<SyntaxToken> modifiers)
    {
        var directMembers = _modelData.AllDirectMembers.Values;
        var complexMembers = directMembers.Where(c => c.IsComplex || c.IsList);
        var simpleMembers = directMembers.Where(c => !c.IsComplex);

        List<SyntaxNodeOrToken> explodeExpressions = [];
        List<SyntaxNodeOrToken> fieldNameExpressions = [];
        List<SyntaxNodeOrToken> fetchTextExpressions = [];
        if (_modelData.IsTallyComplexObject)
        {
            fetchTextExpressions.SafeAddExpressionElement(IdentifierName("_pathPrefix"));
        }
        foreach (var member in directMembers)
        {
            var prop = member;
            if (prop.XmlIgnore) continue;
            if (member.IsComplex)
            {
                AddExplodes(Meta.IdentifierNameVarName, member.Name, member.TDLCollectionData?.ExplodeCondition, member.TDLFieldData?.Set);
                fetchTextExpressions.SafeAdd(SpreadElement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                  IdentifierName(prop.Name),
                                                                                  IdentifierName("FetchText"))));
                foreach (var xMLData in member.XMLData)
                {
                    if (xMLData.ClassData == null) continue;
                    ClassData classData = xMLData.ClassData;
                    AddExplodes(Meta.IdentifierNameVarName, $"{member.Name}.{classData.Name}", classData.TDLCollectionData?.ExplodeCondition);

                }
            }
            else
            {
                if (member.IsList)
                {
                    AddExplodes("Name", member.Name, member.TDLCollectionData?.ExplodeCondition, member.TDLFieldData?.Set);
                }
                else
                {
                    fieldNameExpressions.SafeAddExpressionElement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                            IdentifierName(prop.Name),
                                                                            IdentifierName("Name")));


                }
                if (!_modelData.IsTallyComplexObject)
                {
                    fetchTextExpressions.SafeAddExpressionElement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                               IdentifierName(prop.Name),
                                                                               IdentifierName("FetchText")));
                }
            }

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



        void AddExplodes(string idetifierVarName, string name, string? explodeCondition, string? set = null)
        {
            string text = ":YES";

            List<InterpolatedStringContentSyntax> nodes =
                [
                    Interpolation(IdentifierName($"{name}.{idetifierVarName}")),
                        //InterpolatedStringText()
                        //.WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, path, path, TriviaList()))
                    ];
            if (explodeCondition == null)
            {
                nodes.Add(InterpolatedStringText()
                    .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
            }
            else
            {
                text = $":{string.Format(explodeCondition, set ?? name)}";
                nodes.Add(InterpolatedStringText()
                    .WithTextToken(Token(TriviaList(), SyntaxKind.InterpolatedStringTextToken, text, text, TriviaList())));
                // nodes.Add(Interpolation(CreateFormatExpression(explodeCondition, varname)));
            }
            InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
          .WithContents(List(nodes));
            explodeExpressions.SafeAddExpressionElement(interpolatedStringExpressionSyntax);
        }
    }



    private void AddITallyRequestableRequiredMembers(List<MemberDeclarationSyntax> members,
                                                     List<SyntaxToken> modifiers)
    {
        AddMembersRelatedToAllUniqueMembersForRequestableObjectMeta(members, modifiers);
        AddMembersRelatedToAllMembersForRequestableObjectMeta(members, modifiers);
        AddDefaultMembersForRequestableObjectMeta(members, modifiers);
    }
    private void AddMembersRelatedToAllMembersForRequestableObjectMeta(List<MemberDeclarationSyntax> members,
                                                                   List<SyntaxToken> modifiers)
    {
        //List<SyntaxNodeOrToken> allFetchExpressions = [SpreadElement(IdentifierName($"FetchText"))];
        //foreach (var item in _modelData.AllMembers)
        //{
        //    if (!item.Value.IsComplex) continue;
        //    allFetchExpressions.SafeAdd(SpreadElement(IdentifierName($"{item.Key}.FetchText")));
        //}
        //members.Add(PropertyDeclaration(GenericName(
        //                       Identifier("List"))
        //                   .WithTypeArgumentList(
        //                       TypeArgumentList(
        //                           SingletonSeparatedList<TypeSyntax>(
        //                               PredefinedType(Token(SyntaxKind.StringKeyword))))),
        //                               Identifier("AllFetchList_new"))
        //   .WithModifiers(TokenList(modifiers))
        //   .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(allFetchExpressions))))
        //   .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
    }
    private void AddMembersRelatedToAllUniqueMembersForRequestableObjectMeta(List<MemberDeclarationSyntax> members,
                                                                       List<SyntaxToken> modifiers)
    {
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
            if (propertyData.XmlIgnore) continue;
            var name = propertyData.Name;
            if (propertyData.IsComplex || propertyData.IsList)
            {
                AddToExprresions(name);
                foreach (var XMLData in propertyData.XMLData)
                {
                    if (XMLData.ClassData == null) continue;
                    AddToExprresions($"{name}.{XMLData.ClassData.Name}", true);
                }
                void AddToExprresions(string varname, bool removedot = false)
                {
                    PartExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{(removedot ? varname.Replace(".", "") : varname)}Part"));
                    LineExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{(removedot ? varname.Replace(".", "") : varname)}Line"));
                    if (!propertyData.IsComplex)
                    {
                        fieldExpressions.SafeAddExpressionElement(IdentifierName($"{field.Path}{varname}"));
                    }
                    else
                    {
                        allFetchExpressions.SafeAdd(SpreadElement(IdentifierName($"{field.Path}{varname}.FetchText")));
                    }
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
    }

    private void AddDefaultMembersForRequestableObjectMeta(List<MemberDeclarationSyntax> members,
                                                                  List<SyntaxToken> modifiers)
    {
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
        if (!(_modelData.TDLCollectionData?.Exclude ?? false))
        {
            List<SyntaxNodeOrToken> collectionConstructorArgs = [];
            collectionConstructorArgs.SafeAddArgument(IdentifierName(Meta.CollectionNameVarName));
            collectionConstructorArgs.SafeAddArgument(IdentifierName(Meta.ObjectTypeVarName));
            collectionConstructorArgs.SafeAdd(Argument(IdentifierName("FetchText")).WithNameColon(NameColon(IdentifierName("nativeFields"))));
            members.Add(PropertyDeclaration(GetGlobalNameforType(CollectionFullTypeName),
                                            Identifier(Meta.DefaultCollectionVarName))
                .WithModifiers(TokenList(modifiers))
                .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(collectionConstructorArgs)))
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
        _context.AddSource($"{_modelData.Name}_{_modelData.Namespace}.g.cs", unit);
    }

    private void GenerateMultiXMLMetaData(ClassPropertyData val,
                                          SourceProductionContext context,
                                          CancellationToken token,
                                          List<MemberDeclarationSyntax> mainMembers)
    {

        var className = GetMultiXMLDataMetaClassName(val);
        List<UsingDirectiveSyntax> usings = [UsingDirective(IdentifierName(ExtensionsNameSpace))];
        ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(className)
                .WithModifiers(TokenList([Token(
                            TriviaList(
                                Comment($@"/*
* Generated based on Property {_modelData.FullName}.{val.Name}
*/")),
                            SyntaxKind.PublicKeyword,
                            TriviaList())]));

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
                    .WithMembers(List(GetMultiXMLMetaClassMembers()))
                    .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(GetGlobalNameforType($"{val.ClassData!.Namespace}.Meta.{val.ClassData.Name}Meta")))))
                }))
          })).NormalizeWhitespace().ToFullString();
        _context.AddSource($"MultiXMLMeta.{_modelData.Name}.{val.Name}_{_modelData.Namespace}.g.cs", unit);

        IEnumerable<MemberDeclarationSyntax> GetMultiXMLMetaClassMembers()
        {
            List<MemberDeclarationSyntax> members = [];
            members.Add(FieldDeclaration(VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
                .WithVariables(SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            Identifier(MultiXMLMeta.OriginalPrefixVarName))))));
            members.Add(GetSecondConstructor(true, className, true));
            List<SyntaxNodeOrToken> fetchTextExpressions = [];
            fetchTextExpressions.SafeAdd(SpreadElement(IdentifierName("base.FetchText")));
            foreach (var xMLData in val.XMLData)
            {
                token.ThrowIfCancellationRequested();
                if (xMLData.Symbol == null || xMLData.ClassData == null) continue;
                AddPartAndLine(val, mainMembers, true, xMLData);
                List<SyntaxNodeOrToken> constructorArgs = [];
                constructorArgs.SafeAddArgument(CreateStringLiteral(xMLData.FieldName!));
                constructorArgs.SafeAdd(Argument(InvocationExpression(IdentifierName(MultiXMLMeta.GeneratePathMethodName))
                    .WithArgumentList(ArgumentList(SeparatedList([Argument(CreateStringLiteral(xMLData.ClassData.TDLCollectionData!.CollectionName!))]))))
                    .WithNameColon(NameColon(IdentifierName(Meta.Parameters.PathPrefix))));

                members.Add(PropertyDeclaration(GetGlobalNameforType($"{xMLData.ClassData.Namespace}.Meta.{xMLData.ClassData.MetaName}"), xMLData.ClassData.Name)
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression(constructorArgs)))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

                fetchTextExpressions.SafeAdd(SpreadElement(IdentifierName($"{xMLData.ClassData.Name}.FetchText")));
            }
            members.Add(PropertyDeclaration(QualifiedName(GetGlobalNameforType(Constants.CollectionsNameSpace), GenericName(ListClassName)
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new SyntaxNodeOrToken[]{
                                                        PredefinedType(
                                                            Token(SyntaxKind.StringKeyword)) })))), "FetchText")
                 .WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword), (Token(SyntaxKind.NewKeyword))]))
                 .WithExpressionBody(ArrowExpressionClause(CollectionExpression(SeparatedList<CollectionElementSyntax>(fetchTextExpressions))))
                 .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
            members.Add(MethodDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)),
                                          Identifier(MultiXMLMeta.GeneratePathMethodName))
                           .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList<ParameterSyntax>(
                                        Parameter(
                                            Identifier("suffix"))
                                        .WithType(
                                            PredefinedType(
                                                Token(SyntaxKind.StringKeyword))))))
                             .WithBody(
                                Block(
                                    SingletonList<StatementSyntax>(
                                        ReturnStatement(
                                            ConditionalExpression(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        PredefinedType(
                                                            Token(SyntaxKind.StringKeyword)),
                                                        IdentifierName("IsNullOrEmpty")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName(MultiXMLMeta.OriginalPrefixVarName))))),
                                                IdentifierName("suffix"),
                                                InterpolatedStringExpression(
                                                    Token(SyntaxKind.InterpolatedStringStartToken))
                                                .WithContents(
                                                    List<InterpolatedStringContentSyntax>(
                                                        [
                                                            Interpolation(
                                                                IdentifierName(MultiXMLMeta.OriginalPrefixVarName)),
                                                            InterpolatedStringText()
                                                            .WithTextToken(
                                                                Token(
                                                                    TriviaList(),
                                                                    SyntaxKind.InterpolatedStringTextToken,
                                                                    ".",
                                                                    ".",
                                                                    TriviaList())),
                                                            Interpolation(
                                                                IdentifierName("suffix"))])))))))
                           );



            //List<SyntaxNodeOrToken> dictIntializerArgs = [];
            //List<SyntaxNodeOrToken> intializerArgs = [];

            //intializerArgs.SafeAdd(InitializerExpression(SyntaxKind.ComplexElementInitializerExpression,
            //                                              SeparatedList<ExpressionSyntax>(new SyntaxNodeOrToken[]
            //                                              {
            //                                                  CreateStringLiteral(val.ClassData!.FullName),
            //                                                  Token(SyntaxKind.CommaToken),
            //                                                 IdentifierName("this")
            //                                              })));

            //mainMembers.Add(PropertyDeclaration(QualifiedName(GetGlobalNameforType(CollectionsNameSpace),
            //   GenericName(DictionaryClassName)
            //   .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(new SyntaxNodeOrToken[]{
            //                                            PredefinedType(
            //                                                Token(SyntaxKind.StringKeyword)),
            //                                            Token(SyntaxKind.CommaToken),
            //                                            GetGlobalNameforType($"{val.ClassData!.Namespace}.Meta.{val.ClassData.MetaName}")})
            //   ))), Identifier(Constants.MultiXMLMeta.MetasVarName))
            //   .WithExpressionBody(ArrowExpressionClause(CreateImplicitObjectExpression([], intializerArgs)))
            //   .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

            return members;
        }
    }

    private string GetMultiXMLDataMetaClassName(ClassPropertyData val)
    {
        return $"{_modelData!.Name}{val.Name}MultiXMLMeta";
    }


}
