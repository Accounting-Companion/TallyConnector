using System.Collections.Immutable;
using TallyConnector.TDLReportSourceGenerator.Models;
using TallyConnector.TDLReportSourceGenerator.Services;

namespace TallyConnector.TDLReportSourceGenerator;
[Generator(LanguageNames.CSharp)]
public class ServiceHelperMethodGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!System.Diagnostics.Debugger.IsAttached)
        //{
        //    System.Diagnostics.Debugger.Launch();
        //}

        IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> data = context.SyntaxProvider.ForAttributeWithMetadataName(GenerateHelperMethodGenericAttrName,
                                                                           SyntaxPredicate,
                                                                           Transform).Where(static symbol => symbol is not null).Collect()!;

        context.RegisterSourceOutput(data, Execute);
    }






    private void Execute(SourceProductionContext context, ImmutableArray<INamedTypeSymbol> serviceSymbols)
    {
        List<ServiceData> services = [];
        foreach (var item in serviceSymbols)
        {
            ServiceData serviceData = new(item);
            services.Add(serviceData);
            foreach (var atrr in item.GetAttributes())
            {
                if (atrr.HasFullyQualifiedMetadataName(GenerateHelperMethodAttrName)
                    && atrr.AttributeClass is { TypeArguments.Length: 1 } attrClass
                    && attrClass.TypeArguments[0] is INamedTypeSymbol objectSymbol)
                {
                    string? PluralName = null;
                    foreach (var namedArg in atrr.NamedArguments)
                    {
                        switch (namedArg.Key)
                        {
                            case "MethodNameSuffixPlural":
                                PluralName = (string?)namedArg.Value.Value;
                                break;
                            default:
                                break;
                        }
                    }
                    ObjectData objectData = new(objectSymbol, objectSymbol.Name, PluralName);

                    serviceData.Objects[objectData.UniqueSuffix] = objectData;
                }
            }
        }
        foreach (var item in services)
        {
            new HelperMethodsGenerator(context, item).Generate();
        }


    }

    private bool SyntaxPredicate(SyntaxNode node, CancellationToken token)
    {
        if (node is ClassDeclarationSyntax classDeclaration)
        {

            return classDeclaration.HasPartialKeyword() && classDeclaration.HasOrPotentiallyHasBaseTypes();
        }
        return false;
    }
    private INamedTypeSymbol? Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        if (context.TargetSymbol is INamedTypeSymbol namedType && namedType.HasOrInheritsFromFullyQualifiedMetadataName(TallyBaseClientFullName))
        {
            return namedType;

        }
        return null;
    }
}
public class HelperMethodsGenerator
{
    private SourceProductionContext _context;
    private ServiceData _service;

    public HelperMethodsGenerator(SourceProductionContext context, ServiceData serviceData)
    {
        this._context = context;
        this._service = serviceData;
    }

    internal void Generate()
    {
        ClassDeclarationSyntax classDeclarationSyntax = ClassDeclaration(_service.Name)
.WithModifiers(TokenList([Token(
                            TriviaList(
                                Comment($@"/*
* Generated based on {_service.FullName}
*/")),
                            SyntaxKind.PartialKeyword,
                            TriviaList())]));
        var unit = CompilationUnit()
             .WithUsings(List(GetUsings()))
          .WithMembers(List(new MemberDeclarationSyntax[]
          {
                FileScopedNamespaceDeclaration(IdentifierName(_service.Namespace))
                .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),true))),
                                            SyntaxKind.NamespaceKeyword,
                                            TriviaList()))

                .WithMembers(List(new MemberDeclarationSyntax[]
                {
                    classDeclarationSyntax
                    .WithMembers(List(GetMembers()))

    }))
          })).NormalizeWhitespace().ToFullString();

        _context.AddSource($"{_service.FullName}.g.cs", unit);
    }
    private static List<UsingDirectiveSyntax> GetUsings()
    {
        return [
                    UsingDirective(IdentifierName(ExtensionsNameSpace))
                    ];
    }
    private IEnumerable<MemberDeclarationSyntax> GetMembers()
    {
        List<MemberDeclarationSyntax> members = new();

        foreach (var item in _service.Objects)
        {
            var obj = item.Value;
            if (obj.GenerationMode is GenerationMode.All or GenerationMode.Post)
            {
                members.AddRange(GetGetMethods(obj));
                members.AddRange(GetPostMethods(obj));
            }
        }

        members.Add(GetPostXMLOverrides());
        return members;
    }

    private IEnumerable<MemberDeclarationSyntax> GetGetMethods(ObjectData obj)
    {
        List<SyntaxNodeOrToken> getMethodArgs = [];
        List<SyntaxNodeOrToken> getMethodParams = [];
        string reqOptionsParamName = "requestOptions";

        getMethodParams.SafeAdd(Parameter(Identifier(reqOptionsParamName)).WithType((GetGlobalNameforType(RequestOptionsClassName))));
        getMethodParams.SafeAdd(GetCancellationTokenParameterSyntax());

        getMethodArgs.SafeAddArgument(IdentifierName(reqOptionsParamName));
        getMethodArgs.SafeAddArgument(IdentifierName(CancellationTokenArgName));

        var methodDeclartion = MethodDeclaration(GenericName(Identifier("Task"))
        .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                            new SyntaxNodeOrToken[] { QualifiedName(IdentifierName(CollectionsNameSpace), GenericName(Identifier(ListClassName))
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] { GetGlobalNameforType(obj.FullName) })))) })))
        , $"Get{obj.PluralSuffix}Async")
        .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(
                    getMethodParams)))
        .WithModifiers(TokenList(
                        [
                            Token(SyntaxKind.PublicKeyword)]))

        .WithExpressionBody(ArrowExpressionClause(InvocationExpression(GenericName("GetObjectsAsync")
        .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                            new SyntaxNodeOrToken[] { GetGlobalNameforType(obj.FullName) }))))
        .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(getMethodArgs)))))
             .WithSemicolonToken(
                Token(SyntaxKind.SemicolonToken));

        List<SyntaxNodeOrToken> getPaginatedMethodArgs = [];
        List<SyntaxNodeOrToken> getPaginatedMethodParams = [];

        getPaginatedMethodParams.SafeAdd(Parameter(Identifier(reqOptionsParamName)).WithType(NullableType(GetGlobalNameforType(PaginatedRequestOptionsClassName)))
        .WithDefault(EqualsValueClause(LiteralExpression(
                                                    SyntaxKind.DefaultLiteralExpression,
                                                    Token(SyntaxKind.NullKeyword)))));
        getPaginatedMethodParams.SafeAdd(GetCancellationTokenParameterSyntax());

        getPaginatedMethodArgs.SafeAddArgument(IdentifierName(reqOptionsParamName));
        getPaginatedMethodArgs.SafeAddArgument(IdentifierName(CancellationTokenArgName));

        var paginatedMethodDeclartion = MethodDeclaration(GenericName(Identifier("Task"))
   .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                       new SyntaxNodeOrToken[] {  GenericName(Identifier(PaginatedResponseClassName))
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] { GetGlobalNameforType(obj.FullName) }))) })))
   , $"Get{obj.PluralSuffix}Async")
   .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(
               getPaginatedMethodParams)))
   .WithModifiers(TokenList(
                   [
                       Token(SyntaxKind.PublicKeyword)]))

   .WithExpressionBody(ArrowExpressionClause(InvocationExpression(GenericName("GetObjectsAsync")
   .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                       new SyntaxNodeOrToken[] { GetGlobalNameforType(obj.FullName) }))))
   .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(getPaginatedMethodArgs)))))
        .WithSemicolonToken(
           Token(SyntaxKind.SemicolonToken));

        return [methodDeclartion, paginatedMethodDeclartion];
    }

    private IEnumerable<MemberDeclarationSyntax> GetPostMethods(ObjectData obj)
    {
        const string objectsParamName = "objects";
        const string optionsParamName = "options";

        List<SyntaxNodeOrToken> methodArgs = [];

        methodArgs.SafeAddArgument(IdentifierName(objectsParamName));
        methodArgs.SafeAddArgument(IdentifierName(optionsParamName));
        methodArgs.SafeAddArgument(IdentifierName(CancellationTokenArgName));

        List<SyntaxNodeOrToken> methodParams = [];

        List<SyntaxNodeOrToken> method2Params = [];

        methodParams.SafeAdd(Parameter(Identifier(objectsParamName)).WithType(QualifiedName(IdentifierName(CollectionsNameSpace), GenericName(IEnumerableClassName)
            .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] { GetGlobalNameforType(obj.FullName) }))))));
        method2Params.SafeAdd(Parameter(Identifier(objectsParamName)).WithType(QualifiedName(IdentifierName(CollectionsNameSpace), GenericName(IEnumerableClassName)
            .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] { GetGlobalNameforType(obj.DTOFullName) }))))));
        ParameterSyntax optionsParam = Parameter(Identifier(optionsParamName)).WithType(NullableType(GetGlobalNameforType(PostRequestOptionsFullName)))
            .WithDefault(EqualsValueClause(LiteralExpression(
                                                        SyntaxKind.DefaultLiteralExpression,
                                                        Token(SyntaxKind.NullKeyword))));
        methodParams.SafeAdd(optionsParam);
        method2Params.SafeAdd(optionsParam);
        methodParams.SafeAdd(GetCancellationTokenParameterSyntax());
        method2Params.SafeAdd(GetCancellationTokenParameterSyntax());

        var methodDeclartion = MethodDeclaration(GenericName(Identifier("Task"))
            .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] { QualifiedName(IdentifierName(CollectionsNameSpace), GenericName(Identifier(ListClassName))
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] { GetGlobalNameforType(PostResponseFullName) })))) })))
            , $"Post{obj.PluralSuffix}Async")
            .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(
                        methodParams)))
            .WithModifiers(TokenList(
                            [
                                Token(SyntaxKind.PublicKeyword)]))

            .WithExpressionBody(ArrowExpressionClause(InvocationExpression(GenericName("PostObjectsAsync")
            .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] { GetGlobalNameforType(obj.FullName) }))))
            .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(methodArgs)))))
                 .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken));
        var dtomethodDeclartion = MethodDeclaration(GenericName(Identifier("Task"))
           .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                               new SyntaxNodeOrToken[] { QualifiedName(IdentifierName(CollectionsNameSpace), GenericName(Identifier(ListClassName))
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] { GetGlobalNameforType(PostResponseFullName) })))) })))
           , $"PostDTO{obj.PluralSuffix}Async")
           .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(
                       method2Params)))
           .WithModifiers(TokenList(
                           [
                               Token(SyntaxKind.PublicKeyword)]))

           .WithExpressionBody(ArrowExpressionClause(InvocationExpression(GenericName("PostDTOObjectsAsync")
           .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(
                               new SyntaxNodeOrToken[] { GetGlobalNameforType(obj.DTOFullName) }))))
           .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(methodArgs)))))
                .WithSemicolonToken(
                   Token(SyntaxKind.SemicolonToken));
        return [methodDeclartion, dtomethodDeclartion];
    }

    private MemberDeclarationSyntax GetPostXMLOverrides()
    {
        List<StatementSyntax> statements = [];

        const string VarName = "xMLOverrides";
        statements.Add(CreateVarInsideMethodWithExpression(VarName, ObjectCreationExpression(
                                                    IdentifierName("XMLOverrideswithTracking")).WithArgumentList(ArgumentList())));

        foreach (var item in _service.Objects)
        {
            var obj = item.Value;
            if (obj.GenerationMode is not GenerationMode.All and not GenerationMode.Post)
            {
                continue;
            }
            List<SyntaxNodeOrToken> args = [];
            args.SafeAddArgument(PostfixUnaryExpression(
                                                        SyntaxKind.SuppressNullableWarningExpression, MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, GetGlobalNameforType(item.Value.FullName), IdentifierName($"{Meta.Name}.{Meta.XMLTagVarName}"))));
            args.SafeAddArgument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, GetGlobalNameforType(item.Value.FullName), IdentifierName($"DTOTypeInfo")));
            statements.Add(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(VarName), IdentifierName("AddMessageArrayItemAttributeOverrides")))
                .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(args)))));
        }

        statements.Add(ReturnStatement(IdentifierName(VarName)));
        var declaration = MethodDeclaration(NullableType(GetGlobalNameforType(XmlAttributeOverridesClassName)), Identifier("GetPostXMLOverrides"))
            .WithModifiers(TokenList(
                            [
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.OverrideKeyword)]))
            .WithBody(Block(statements));
        return declaration;
    }
}
public class ServiceData
{
    public ServiceData(INamedTypeSymbol symbol)
    {
        TypeSymbol = symbol;
        Name = symbol.Name;
        Namespace = symbol.ContainingNamespace.ToString();
        FullName = symbol.GetClassMetaName();
    }

    public string Name { get; set; }
    public string FullName { get; set; }
    public string Namespace { get; set; }

    public INamedTypeSymbol TypeSymbol { get; set; }

    public Dictionary<string, ObjectData> Objects { get; set; } = [];

}
public class ObjectData
{
    public ObjectData(INamedTypeSymbol objectSymbol, string suffix, string? pluralSuffix = null)
    {
        TypeSymbol = objectSymbol;
        PluralSuffix = pluralSuffix ?? $"{suffix}s";
        UniqueSuffix = suffix;
        Name = TypeSymbol.Name;
        FullName = TypeSymbol.GetClassMetaName();
        DTOFullName = $"{TypeSymbol.ContainingNamespace}.DTO.{Name}DTO";
    }

    public string UniqueSuffix { get; set; }

    public string PluralSuffix { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string DTOFullName { get; set; }
    public INamedTypeSymbol TypeSymbol { get; set; }
    public GenerationMode GenerationMode { get; set; }
}