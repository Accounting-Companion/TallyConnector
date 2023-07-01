using System.Collections.Immutable;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using TallyConnector.SourceGenerators.Extensions.Symbols;

namespace TallyConnector.SourceGenerators.Generators;
[Generator(LanguageNames.CSharp)]
public class TDLXMLReportGenerator : IIncrementalGenerator
{
    private List<TDLReportGeneratorArgs> _tDLReportGeneratorArgs = new();
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}
        IncrementalValuesProvider<INamedTypeSymbol> syntaxProvider = context.SyntaxProvider
           .CreateSyntaxProvider(SyntaxPredicate, SematicTransform)
           .Where(static (type) => type != null)!;
        var c = syntaxProvider.Collect();
        context.RegisterSourceOutput(c, Execute);
    }

    private void Execute(SourceProductionContext context, ImmutableArray<INamedTypeSymbol> array)
    {
        foreach (INamedTypeSymbol type in array)
        {
            GenerateTDLReportsForGettingObject(type);
        }
        foreach (var item in _tDLReportGeneratorArgs)
        {
            GenerateTDLReportMethod(item, context);
        }
        _tDLReportGeneratorArgs = new();
    }
    private TDLReportGeneratorArgs GenerateTDLReportsForGettingObject(INamedTypeSymbol type)
    {
        string TypeFullName = type.OriginalDefinition.ToString();
        IEnumerable<TDLReportGeneratorArgs> existing = _tDLReportGeneratorArgs.Where(c => c.FullName == TypeFullName);
        if (existing.Any())
        {
            return existing.First();

        }
        TDLReportGeneratorArgs tDLReportGeneratorArgs = new();
        tDLReportGeneratorArgs.FullName = TypeFullName;
        tDLReportGeneratorArgs.TypeName = type.Name;
        tDLReportGeneratorArgs.CollectionName = $"TC_{type.Name}Collection".ToUpper();
        tDLReportGeneratorArgs.NameSpace = type.ContainingNamespace.ToString();
        IEnumerable<IPropertySymbol> propertySymbols = type.GetAllPropertiesAndFields(true).OfType<IPropertySymbol>().Where(c => !c.IsReadOnly);
        foreach (var propertySymbol in propertySymbols)
        {
            bool IsSpecialType = propertySymbol.Type.SpecialType != SpecialType.None;
            if (IsSpecialType)
            {
                TDLFieldArgs tDLFieldArgs = new();
                tDLFieldArgs.TallyFieldName = $"TC_{propertySymbol.Name}".ToUpper();
                tDLFieldArgs.TypeName = propertySymbol.Type.Name;
                TDLFieldProperties? tDLFieldProperties = propertySymbol.GetTDLFieldProperties();
                XMlProperties? xMlProperties = propertySymbol.GetXmlProperties();
                if (tDLFieldProperties != null)
                {
                    tDLFieldArgs.Set = tDLFieldProperties.Set ?? (string.IsNullOrEmpty(xMlProperties?.XMLTag) ? $"${propertySymbol.Name}" : $"${xMlProperties?.XMLTag}");
                    tDLFieldArgs.Format = tDLFieldProperties.Format;
                    tDLFieldArgs.TallyType = tDLFieldProperties.TallyType;
                    tDLFieldArgs.Format = tDLFieldProperties.Format;
                    tDLFieldArgs.Use = tDLFieldProperties.Use;

                }
                else
                {
                    tDLFieldArgs.Set = xMlProperties?.XMLTag ?? $"${propertySymbol.Name}";
                }
                tDLFieldArgs.TallyXMLTag = xMlProperties?.XMLTag ?? propertySymbol.Name.ToUpper();
                tDLReportGeneratorArgs.Fields.Add(tDLFieldArgs);
            }
            else if (propertySymbol.Type is INamedTypeSymbol namedTypeSymbol)
            {
                TDLFieldArgs tDLFieldArgs = new();
                TDLFieldProperties? tDLFieldProperties = propertySymbol.GetTDLFieldProperties();
                XMlProperties? xMlProperties = propertySymbol.GetXmlProperties();
                tDLFieldArgs.IsComplex = true;
                tDLFieldArgs.TallyXMLTag = xMlProperties?.XMLTag ?? propertySymbol.Name.ToUpper();
                tDLFieldArgs.CollectionName = tDLFieldProperties?.CollectionName;

                tDLFieldArgs.TallyFieldName = $"TC_{propertySymbol.Name}".ToUpper();
                tDLFieldArgs.Parent = tDLReportGeneratorArgs;
                if (namedTypeSymbol.IsGenericType)
                {
                    INamedTypeSymbol innerTypeSymbol = (INamedTypeSymbol)namedTypeSymbol.TypeArguments.First();
                    if (innerTypeSymbol.SpecialType == SpecialType.None)
                    {
                        tDLFieldArgs.TypeName = innerTypeSymbol.Name;
                        tDLFieldArgs.TypeFullName = innerTypeSymbol.OriginalDefinition.ToString();

                        tDLReportGeneratorArgs.Fields.Add(tDLFieldArgs);

                        tDLFieldArgs.Child = GenerateTDLReportsForGettingObject(innerTypeSymbol);
                        tDLFieldArgs.Child.Parent = tDLReportGeneratorArgs;
                        tDLFieldArgs.Child.CollectionName = tDLFieldArgs.CollectionName;
                    }
                    else
                    {
                        tDLFieldArgs.Set = tDLFieldProperties?.Set ?? $"${propertySymbol.Name}";
                        tDLFieldArgs.Format = tDLFieldProperties?.Format;
                        tDLFieldArgs.TallyType = tDLFieldProperties?.TallyType;
                        tDLFieldArgs.Format = tDLFieldProperties?.Format;
                        tDLFieldArgs.Use = tDLFieldProperties?.Use;
                        tDLReportGeneratorArgs.Fields.Add(tDLFieldArgs);
                        if (propertySymbols.Count() == 1)
                        {
                            tDLReportGeneratorArgs.IsArrayItem = true;
                        }
                    }

                }
                else
                {
                    tDLFieldArgs.TypeName = namedTypeSymbol.Name;
                    tDLFieldArgs.TypeFullName = namedTypeSymbol.OriginalDefinition.ToString();
                    tDLReportGeneratorArgs.Fields.Add(tDLFieldArgs);
                    tDLFieldArgs.Child = GenerateTDLReportsForGettingObject(namedTypeSymbol);
                    tDLFieldArgs.Child.Parent = tDLReportGeneratorArgs;
                    tDLFieldArgs.Child.CollectionName = tDLFieldArgs.CollectionName;
                }

            }

        }
        if (type.BaseType != null && type.BaseType.OriginalDefinition.ToString() != "object")
        {
            if (SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, type.BaseType.ContainingAssembly))
            {
                tDLReportGeneratorArgs.IsBaseExternal = true;
                tDLReportGeneratorArgs.ContainingAssembly = type.ContainingAssembly.ToString();
            }
            GenerateTDLReportsForGettingObject(type.BaseType);
            tDLReportGeneratorArgs.UseTypeName = type.BaseType.Name;
            tDLReportGeneratorArgs.UseFullName = type.BaseType.OriginalDefinition.ToString();
        }
        _tDLReportGeneratorArgs.Add(tDLReportGeneratorArgs);
        return tDLReportGeneratorArgs;
    }

    private void GenerateTDLReportMethod(TDLReportGeneratorArgs tDLReportGeneratorArg, SourceProductionContext context)
    {
        var methodDeclarationSyntax = GetHelperMethodsToGenerateTDLReport(tDLReportGeneratorArg);
        CompilationUnitSyntax compilationUnitSyntax = CompilationUnit()
            .WithMembers(SingletonList<MemberDeclarationSyntax>(FileScopedNamespaceDeclaration(IdentifierName(tDLReportGeneratorArg.NameSpace))
            .WithNamespaceKeyword(Token(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword),
                                                                                   true))),
                  SyntaxKind.NamespaceKeyword,
                  TriviaList()))
            .WithMembers(List(
                new MemberDeclarationSyntax[]
                {
                        ClassDeclaration(tDLReportGeneratorArg.TypeName)
                        .WithModifiers(TokenList(new[]{Token(SyntaxKind.PublicKeyword),Token(SyntaxKind.PartialKeyword)}))
                        .WithMembers(List<MemberDeclarationSyntax>(methodDeclarationSyntax))
                }))
            )
            ).NormalizeWhitespace();
        string source = compilationUnitSyntax.ToFullString();
        context.AddSource($"{tDLReportGeneratorArg.TypeName}.TDLReport.{tDLReportGeneratorArg.TypeName}.g.cs", source);
    }
    private List<MemberDeclarationSyntax> GetHelperMethodsToGenerateTDLReport(TDLReportGeneratorArgs tDLReportGeneratorArg)
    {
        string FieldsMethodName = $"GetTDLReportFields";
        string TDLReportMethodName = $"GetTDLMessage";
        string TDLLineMethodName = $"GetTDLLine";
        List<MemberDeclarationSyntax> memberDeclarationSyntaxes = new();
        List<TDLFieldArgs> simpleFields = tDLReportGeneratorArg.Fields.Where(c => !c.IsComplex).ToList();
        List<TDLFieldArgs> complexFields = tDLReportGeneratorArg.Fields.Where(c => c.IsComplex).ToList();
        string useFullName = tDLReportGeneratorArg.UseFullName;


        MethodDeclarationSyntax GetTDLMessageMethodDeclaration = MethodDeclaration(IdentifierName("void"), TDLReportMethodName)
            .WithModifiers(TokenList(
                new[]
                {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                }))
            .WithBody(Block());
        memberDeclarationSyntaxes.Add(GetStaticPublicField("TDLReportName", $"TC_{tDLReportGeneratorArg.TypeName}".ToUpper(), !string.IsNullOrEmpty(tDLReportGeneratorArg.UseFullName)));
        memberDeclarationSyntaxes.Add(GetStaticPublicField("TDLCollectionName", $"{tDLReportGeneratorArg.CollectionName}", !string.IsNullOrEmpty(tDLReportGeneratorArg.UseFullName)));

        // memberDeclarationSyntaxes.Add(GetFieldsMethod("GetTDLReports", complexFields, useFullName, "Report"));
        memberDeclarationSyntaxes.Add(GetMethodDeclarationforType("GetTDLReport", "Report"));
        memberDeclarationSyntaxes.Add(GetMethodDeclarationforType("GetTDLForm", "Form"));
        memberDeclarationSyntaxes.Add(GetFieldsMethod(FieldsMethodName, tDLReportGeneratorArg, tDLReportGeneratorArg.Fields, useFullName, "Field", GetCreateFieldArgumentList));
        memberDeclarationSyntaxes.Add(GetFieldsMethod("GetTDLReportLines", tDLReportGeneratorArg, complexFields, useFullName, "Line", GetCreateLineArgumentList, (ex) => CreateLine(ex, tDLReportGeneratorArg)));
        memberDeclarationSyntaxes.Add(GetFieldsMethod("GetTDLReportParts", tDLReportGeneratorArg, complexFields, useFullName, "Part", GetCreatePartArgumentList, (ex) => CreatePart(ex, tDLReportGeneratorArg)));
        memberDeclarationSyntaxes.Add(GetMethodDeclarationforPart(tDLReportGeneratorArg));
        memberDeclarationSyntaxes.Add(GetMethodDeclarationforTDLLine(tDLReportGeneratorArg));

        memberDeclarationSyntaxes.Add(GetTDLMessageMethodDeclaration);
        return memberDeclarationSyntaxes;
    }

    private StatementSyntax CreateLine(ExpressionSyntax syntax, TDLReportGeneratorArgs tDLReportGeneratorArg)
    {
        var simplefields = tDLReportGeneratorArg.Fields.Where(c => !c.IsComplex || (c.IsComplex && string.IsNullOrEmpty(c.TypeFullName))).ToList();
        var complexfields = tDLReportGeneratorArg.Fields.Where(c => c.IsComplex).ToList();

        var argument = simplefields.Count > 0 ? Argument(ObjectCreationExpression(
                                                    GenericName(
                                                        Identifier("List"))
                                                    .WithTypeArgumentList(
                                                        TypeArgumentList(
                                                            SingletonSeparatedList<TypeSyntax>(
                                                                PredefinedType(
                                                                    Token(SyntaxKind.StringKeyword))))))
                                                .WithArgumentList(
                                                    ArgumentList())
                                                .WithInitializer(
                                                    InitializerExpression(
                                                        SyntaxKind.CollectionInitializerExpression,
                                                        SingletonSeparatedList<ExpressionSyntax>(
                                                            LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,
                                                                Literal(string.Join(",", simplefields.Select(c => c.TallyFieldName)))))))) : Argument(
                                                LiteralExpression(
                                                    SyntaxKind.NullLiteralExpression));

        var argsList = ArgumentList(SeparatedList<ArgumentSyntax>(
                                          new SyntaxNodeOrToken[]{
                                            Argument(
                                                IdentifierName($"TDLReportName")),
                                            Token(SyntaxKind.CommaToken),
                                            argument,
                                            Token(SyntaxKind.CommaToken),
                                           Argument(string.IsNullOrEmpty(tDLReportGeneratorArg.UseTypeName) ? LiteralExpression(
                                                    SyntaxKind.NullLiteralExpression) :  ConditionalExpression(
                                                            BinaryExpression(
                                                                SyntaxKind.GreaterThanExpression,
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    InvocationExpression(
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            IdentifierName($"global::{tDLReportGeneratorArg.UseFullName}"),
                                                                            IdentifierName("GetTDLReportFields"))),
                                                                    IdentifierName("Length")),
                                                                LiteralExpression(
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    Literal(0))),
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName($"global::{tDLReportGeneratorArg.UseFullName}"),
                                                                IdentifierName("TDLReportName")),
                                                            LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))),
                                            Token(SyntaxKind.CommaToken),
                                            Argument(IdentifierName("xmlTag")),

                                          }));
        ImplicitObjectCreationExpressionSyntax right = ImplicitObjectCreationExpression()
                                                      .WithArgumentList(argsList);

        if (complexfields.Count > 0)
        {

            right = right.WithInitializer(InitializerExpression(SyntaxKind.ObjectInitializerExpression, SeparatedList<ExpressionSyntax>(new SyntaxNodeOrToken[]
            {
                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                     IdentifierName("Explodes"),
                                     ImplicitObjectCreationExpression()
                                     .WithInitializer(InitializerExpression(SyntaxKind.CollectionInitializerExpression,SeparatedList<ExpressionSyntax>(
                                         complexfields.Select(c =>InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
                                         .WithContents(List<InterpolatedStringContentSyntax>(new InterpolatedStringContentSyntax[]
                                         {
                                             Interpolation(string.IsNullOrWhiteSpace(c.TypeFullName) ? IdentifierName($"TDLReportName") : IdentifierName($"{c.TypeFullName}.TDLReportName")),
                                             InterpolatedStringText().WithTextToken(Token(TriviaList(),SyntaxKind.InterpolatedStringTextToken,":Yes",":Yes",TriviaList()))
                                         }))).ToList()))))
            })));
        }
        return ExpressionStatement(AssignmentExpression(
                                              SyntaxKind.SimpleAssignmentExpression,
                                              ElementAccessExpression(
                                                  IdentifierName("currentLines"))
                                              .WithArgumentList(
                                                  BracketedArgumentList(
                                                      SingletonSeparatedList(
                                                          Argument(syntax)))),
                                              right));
    }

    private ArgumentListSyntax GetCreateLineArgumentList(TDLFieldArgs args)
    {
        TDLReportGeneratorArgs tDLReportGeneratorArg = args.Child;
        if (tDLReportGeneratorArg != null)
        {
            var simplefields = tDLReportGeneratorArg.Fields.Where(c => !c.IsComplex).ToList();
            var argument = simplefields.Count > 0 ? Argument(ObjectCreationExpression(
                                                        GenericName(
                                                            Identifier("List"))
                                                        .WithTypeArgumentList(
                                                            TypeArgumentList(
                                                                SingletonSeparatedList<TypeSyntax>(
                                                                    PredefinedType(
                                                                        Token(SyntaxKind.StringKeyword))))))
                                                    .WithArgumentList(
                                                        ArgumentList())
                                                    .WithInitializer(
                                                        InitializerExpression(
                                                            SyntaxKind.CollectionInitializerExpression,
                                                            SingletonSeparatedList<ExpressionSyntax>(
                                                                LiteralExpression(
                                                                    SyntaxKind.StringLiteralExpression,
                                                                    Literal(string.Join(",", simplefields.Select(c => c.TallyFieldName)))))))) : Argument(
                                                    LiteralExpression(
                                                        SyntaxKind.NullLiteralExpression));

            return ArgumentList(SeparatedList<ArgumentSyntax>(
                                              new SyntaxNodeOrToken[]{
                                            Argument(
                                                IdentifierName($"{args.TypeFullName}.TDLReportName")),
                                            Token(SyntaxKind.CommaToken),
                                            argument,
                                            Token(SyntaxKind.CommaToken),
                                           Argument(string.IsNullOrEmpty(tDLReportGeneratorArg.UseTypeName) ? LiteralExpression(
                                                    SyntaxKind.NullLiteralExpression) :  ConditionalExpression(
                                                            BinaryExpression(
                                                                SyntaxKind.GreaterThanExpression,
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    InvocationExpression(
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            IdentifierName($"global::{args.Child.UseFullName}"),
                                                                            IdentifierName("GetTDLReportFields"))),
                                                                    IdentifierName("Length")),
                                                                LiteralExpression(
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    Literal(0))),
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName($"global::{args.Child.UseFullName}"),
                                                                IdentifierName("TDLReportName")),
                                                            LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))),
                                            Token(SyntaxKind.CommaToken),
                                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                       Literal(args.TallyXMLTag))),

                                              }));
        }
        else
        {
            return ArgumentList(SeparatedList<ArgumentSyntax>(
                                              new SyntaxNodeOrToken[]{
                                                   Argument(
                                                LiteralExpression(
                                                                    SyntaxKind.StringLiteralExpression,
                                                                    Literal(args.TallyFieldName))),
                                            Token(SyntaxKind.CommaToken),
                                              Argument(ObjectCreationExpression(
                                                        GenericName(
                                                            Identifier("List"))
                                                        .WithTypeArgumentList(
                                                            TypeArgumentList(
                                                                SingletonSeparatedList<TypeSyntax>(
                                                                    PredefinedType(
                                                                        Token(SyntaxKind.StringKeyword))))))
                                                    .WithArgumentList(
                                                        ArgumentList())
                                                    .WithInitializer(
                                                        InitializerExpression(
                                                            SyntaxKind.CollectionInitializerExpression,
                                                            SingletonSeparatedList<ExpressionSyntax>(
                                                                LiteralExpression(
                                                                    SyntaxKind.StringLiteralExpression,
                                                                    Literal(args.TallyFieldName)))))),
                                               Token(SyntaxKind.CommaToken),
                                               Argument(LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression)),
                                              Token(SyntaxKind.CommaToken),
                                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                       Literal(args.TallyXMLTag))),
                 }));
        }
    }

    private StatementSyntax CreatePart(ExpressionSyntax Count, TDLReportGeneratorArgs tDLReportGeneratorArg)
    {
        return ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        ElementAccessExpression(
                                            IdentifierName("currentParts"))
                                        .WithArgumentList(
                                            BracketedArgumentList(
                                                SingletonSeparatedList(
                                                    Argument(Count)))),
                                        ImplicitObjectCreationExpression()
                                        .WithArgumentList(ArgumentList(
                                      SeparatedList<ArgumentSyntax>(
                                          new SyntaxNodeOrToken[]{
                                            Argument(
                                                IdentifierName("TDLReportName")),
                                            Token(SyntaxKind.CommaToken),
                                            Argument(
                                                IdentifierName("TDLCollectionName")),
                                             Token(SyntaxKind.CommaToken),
                                            Argument(IdentifierName("xmlTag"))
                                          })))));
    }

    private MethodDeclarationSyntax GetFieldsMethod(string FieldsMethodName,
        TDLReportGeneratorArgs tDLReportGeneratorArg,
                                                    List<TDLFieldArgs> allFields,
                                                    string useFullName,
                                                    string Type,
                                                    Func<TDLFieldArgs, ArgumentListSyntax> createFunc,
                                                    Func<ExpressionSyntax, StatementSyntax>? CurrentCreate = null)
    {
        List<SyntaxToken> tokens = new()
                                {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                        };
        if (!string.IsNullOrEmpty(useFullName))
        {
            tokens.Insert(1, Token(SyntaxKind.NewKeyword));
        }
        MethodDeclarationSyntax methodDeclarationSyntax = MethodDeclaration(CreateArrayType($"global::TallyConnector.Core.Models.Common.Request.{Type}"), FieldsMethodName)
                    .WithModifiers(TokenList(
                        tokens));
        if (Type != "Field")
        {
            methodDeclarationSyntax = methodDeclarationSyntax
            .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
            {
                 Parameter(Identifier("xmlTag"))
                                .WithType(
                                   NullableType(PredefinedType(
                                        Token(SyntaxKind.StringKeyword))))
                                .WithDefault(
                                    EqualsValueClause(
                                         Type == "Part" ? LiteralExpression(SyntaxKind.NullLiteralExpression) :LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(tDLReportGeneratorArg.TypeName.ToUpper())))),
            })));
        }
        return methodDeclarationSyntax
            .WithBody(Block(
                GetTDLReportFieldsBody(allFields, useFullName, Type, FieldsMethodName, createFunc, CurrentCreate)
            ));
    }

    private MemberDeclarationSyntax GetMethodDeclarationforTDLLine(TDLReportGeneratorArgs tDLReportGeneratorArg)
    {
        string name = "global::TallyConnector.Core.Models.Common.Request.Line";
        var argument = tDLReportGeneratorArg.Fields.Count > 0 ? Argument(ObjectCreationExpression(
                                                    GenericName(
                                                        Identifier("List"))
                                                    .WithTypeArgumentList(
                                                        TypeArgumentList(
                                                            SingletonSeparatedList<TypeSyntax>(
                                                                PredefinedType(
                                                                    Token(SyntaxKind.StringKeyword))))))
                                                .WithArgumentList(
                                                    ArgumentList())
                                                .WithInitializer(
                                                    InitializerExpression(
                                                        SyntaxKind.CollectionInitializerExpression,
                                                        SingletonSeparatedList<ExpressionSyntax>(
                                                            LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,
                                                                Literal(string.Join(",", tDLReportGeneratorArg.Fields.Select(c => c.TallyFieldName)))))))) : Argument(
                                                LiteralExpression(
                                                    SyntaxKind.NullLiteralExpression));


        return MethodDeclaration(CreateArrayType(name), "GetTDLLines")
              .WithModifiers(TokenList(
                  new[]
                  {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                  }))
              .WithBody(Block(ReturnStatement(CreateArrayIntializeExpression(name, new(){ ObjectCreationExpression(
                                  IdentifierName(name))
                              .WithArgumentList(
                                  ArgumentList(
                                      SeparatedList<ArgumentSyntax>(
                                          new SyntaxNodeOrToken[]{
                                            Argument(
                                                IdentifierName("TDLReportName")),
                                            Token(SyntaxKind.CommaToken),
                                            argument,
                                            Token(SyntaxKind.CommaToken),
                                           Argument( string.IsNullOrEmpty(tDLReportGeneratorArg.UseTypeName) ? LiteralExpression(
                                                    SyntaxKind.NullLiteralExpression) :  ConditionalExpression(
                                                            BinaryExpression(
                                                                SyntaxKind.GreaterThanExpression,
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    InvocationExpression(
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            IdentifierName($"global::{tDLReportGeneratorArg.UseFullName}"),
                                                                            IdentifierName("GetTDLReportFields"))),
                                                                    IdentifierName("Length")),
                                                                LiteralExpression(
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    Literal(0))),
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName($"global::{tDLReportGeneratorArg.UseFullName}"),
                                                                IdentifierName("TDLReportName")),
                                                            LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))),
                                            Token(SyntaxKind.CommaToken),
                                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                       Literal(tDLReportGeneratorArg.TypeName.ToUpper()))),

                                          }))) }))
                  ));
    }

    private MemberDeclarationSyntax GetMethodDeclarationforPart(TDLReportGeneratorArgs tDLReportGeneratorArg)
    {
        var complexFields = tDLReportGeneratorArg.Fields.Where(c => c.IsComplex).ToList();
        string name = "global::TallyConnector.Core.Models.Common.Request.Part";
        return MethodDeclaration(CreateArrayType(name), "GetTDLParts")
              .WithModifiers(TokenList(
                  new[]
                  {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                  }))

              .WithBody(Block(ReturnStatement(
                  CreateArrayIntializeExpression(name, new List<SyntaxNodeOrToken> {
                                ObjectCreationExpression(
                                  IdentifierName(name))
                              .WithArgumentList(
                                  ArgumentList(
                                      SeparatedList<ArgumentSyntax>(
                                          new SyntaxNodeOrToken[]{
                                            Argument(
                                                IdentifierName("TDLReportName")),
                                            Token(SyntaxKind.CommaToken),
                                            Argument(
                                                IdentifierName("TDLCollectionName")),
                                          })))
                                    }))
                  ));
    }

    private static ArrayCreationExpressionSyntax CreateArrayIntializeExpression(string name, List<SyntaxNodeOrToken> ObjExpressions)
    {
        return ArrayCreationExpression(CreateArrayType(name))
                          .WithInitializer(InitializerExpression(
                              SyntaxKind.ArrayInitializerExpression, SeparatedList<ExpressionSyntax>(
                                    ObjExpressions
                                  )));
    }

    private static ArrayTypeSyntax CreateArrayType(string name)
    {
        return ArrayType(IdentifierName(name)).WithRankSpecifiers(
                                    SingletonList<ArrayRankSpecifierSyntax>(
                                        ArrayRankSpecifier(
                                            SingletonSeparatedList<ExpressionSyntax>(
                                                OmittedArraySizeExpression()))));
    }


    private static FieldDeclarationSyntax GetStaticPublicField(string name, string value, bool isOverriden)
    {
        List<SyntaxToken> tokens = new(){
                                Token(SyntaxKind.PublicKeyword),

                                Token(SyntaxKind.StaticKeyword)
                                    };
        if (isOverriden)
        {
            tokens.Insert(1, Token(SyntaxKind.NewKeyword));
        }
        return FieldDeclaration(
                                VariableDeclaration(
                                    PredefinedType(
                                        Token(SyntaxKind.StringKeyword)))
                                .WithVariables(
                                    SingletonSeparatedList(
                                        VariableDeclarator(
                                            Identifier(name))
                                        .WithInitializer(
                                            EqualsValueClause(
                                                LiteralExpression(
                                                    SyntaxKind.StringLiteralExpression,
                                                    Literal(value))))))).WithModifiers(
                                TokenList(
                                    tokens));
    }

    private static MethodDeclarationSyntax GetMethodDeclarationforType(string methodName, string type)
    {
        string name = $"global::TallyConnector.Core.Models.Common.Request.{type}";
        return MethodDeclaration(IdentifierName(name), methodName)
            .WithModifiers(TokenList(
                new[]
                {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                }))
            .WithBody(Block(ReturnStatement(ObjectCreationExpression(
                                IdentifierName(name))
                            .WithArgumentList(
                                ArgumentList(
                                    SeparatedList<ArgumentSyntax>(
                                        new SyntaxNodeOrToken[]{
                                            Argument(
                                                IdentifierName("TDLReportName"))}))))
        ));
    }

    private List<StatementSyntax> GetTDLReportFieldsBody(List<TDLFieldArgs> fields, string useFullName, string type, string fieldsMethodName, Func<TDLFieldArgs, ArgumentListSyntax> createFunc, Func<ExpressionSyntax, StatementSyntax>? CurrentCreate = null)
    {
        List<StatementSyntax> statementSyntaxes = new List<StatementSyntax>();
        string fullName = $"global::TallyConnector.Core.Models.Common.Request.{type}";
        var simpleFields = fields.Where(c => !c.IsComplex || (c.IsComplex && string.IsNullOrEmpty(c.TypeFullName))).ToList();
        var count = simpleFields.Count;
        bool currentCreate = CurrentCreate != null;
        bool hasFields = count > 0 || currentCreate;
        bool hasBaseClass = !string.IsNullOrEmpty(useFullName);
        string baseFieldName = $"base{type}s";
        string allFieldName = $"current{type}s";
        List<ArgumentSyntax> args = new();
        if (hasBaseClass)
        {
            statementSyntaxes.Add(LocalDeclarationStatement(
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
                                            Identifier(baseFieldName))
                                        .WithInitializer(
                                            EqualsValueClause(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName($"global::{useFullName}"),
                                                        IdentifierName(fieldsMethodName)))))))));

            args.Add(Argument(IdentifierName(baseFieldName)));

        }
        //if (hasBaseClass && hasFields)
        //{
        //    statementSyntaxes.Add(LocalDeclarationStatement(
        //                 CreateLocalIntVariable("localFieldsCount", count)));

        //}

        ExpressionSyntax countExpression2 = LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0));

        if (hasFields)
        {
            args.Add(Argument(IdentifierName(allFieldName)));
            var intiation = LocalDeclarationStatement(
                        VariableDeclaration(
                            ArrayType(IdentifierName(fullName))
                            .WithRankSpecifiers(
                                SingletonList<ArrayRankSpecifierSyntax>(
                                    ArrayRankSpecifier(
                                        SingletonSeparatedList<ExpressionSyntax>(
                                            OmittedArraySizeExpression())))))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier(allFieldName))
                                .WithInitializer(
                                    EqualsValueClause(
                                        ArrayCreationExpression(
                                            ArrayType(
                                                IdentifierName(fullName))
                                            .WithRankSpecifiers(
                                                SingletonList<ArrayRankSpecifierSyntax>(
                                                    ArrayRankSpecifier(
                                                        SingletonSeparatedList<ExpressionSyntax>(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(currentCreate ? 1 : count))))))))))));

            statementSyntaxes.Add(intiation);

            if (!currentCreate)
            {
                foreach (var field in simpleFields)
                {
                    var index = simpleFields.IndexOf(field);
                    statementSyntaxes.Add(ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        ElementAccessExpression(
                                            IdentifierName(allFieldName))
                                        .WithArgumentList(
                                            BracketedArgumentList(
                                                SingletonSeparatedList(
                                                    Argument(
                                                        LiteralExpression(
                                                            SyntaxKind.NumericLiteralExpression,
                                                            Literal(index)))))),
                                        ImplicitObjectCreationExpression()
                                        .WithArgumentList(createFunc(field)))));
                }
            }
        }
        foreach (var item in fields.Where(c => c.IsComplex && !string.IsNullOrEmpty(c.TypeFullName)))
        {
            InvocationExpressionSyntax invocationExpressionSyntax = InvocationExpression(MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName($"global::{item.TypeFullName}"),
                                                                    IdentifierName(fieldsMethodName)));
            if (type != "Field")
            {
                invocationExpressionSyntax = invocationExpressionSyntax.WithArgumentList(ArgumentList(SeparatedList(new ArgumentSyntax[] { Argument(((item.Child.IsArrayItem && type == "Line") || (!item.Child.IsArrayItem && type == "Part")) ? LiteralExpression(SyntaxKind.NullLiteralExpression) : LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(item.TallyXMLTag))) })));
            }
            args.Add(Argument(invocationExpressionSyntax));
        }
        if (CurrentCreate != null)
        {
            statementSyntaxes.Add(CurrentCreate(countExpression2));
        }

        if (!hasBaseClass && !hasFields)
        {
            statementSyntaxes.Add(ReturnStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("Array"),
                                    GenericName(
                                        Identifier("Empty"))
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                IdentifierName(fullName))))))));
            //statementSyntaxes.Add(ReturnStatement(InvocationExpression(IdentifierName("global::TallyConnector.Core.Extensions.ArrayExtensions.CombineMultipleArrays")).WithArgumentList(ArgumentList(SeparatedList(args)))));
        }
        else
        {
            statementSyntaxes.Add(ReturnStatement(args.Count > 1 ? InvocationExpression(IdentifierName("global::TallyConnector.Core.Extensions.ArrayExtensions.CombineMultipleArrays")).WithArgumentList(ArgumentList(SeparatedList(args))) : hasBaseClass ? IdentifierName(baseFieldName) : IdentifierName(allFieldName)));
        }
        return statementSyntaxes;
    }

    private static ArgumentListSyntax GetCreateFieldArgumentList(TDLFieldArgs field)
    {
        return ArgumentList(SeparatedList<ArgumentSyntax>(
                                                         new SyntaxNodeOrToken[]
                                                         {
                                             Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                        Literal(field.TallyFieldName))),
                                             Token(SyntaxKind.CommaToken),
                                             Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                        Literal(field.TallyXMLTag))),
                                             Token(SyntaxKind.CommaToken),
                                             Argument(field.Set == null ? LiteralExpression(SyntaxKind.NullLiteralExpression):LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                        Literal(field.Set))),
                                             Token(SyntaxKind.CommaToken),
                                             Argument(field.Use  == null ? LiteralExpression(SyntaxKind.NullLiteralExpression):LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                        Literal(field.Use))),
                                                         }));
    }
    private static ArgumentListSyntax GetCreatePartArgumentList(TDLFieldArgs field)
    {
        if (string.IsNullOrEmpty(field.TypeFullName))
        {
            return ArgumentList(SeparatedList<ArgumentSyntax>(
                                                         new SyntaxNodeOrToken[]
                                                         {
                                             Argument(LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,Literal(field.TallyFieldName))),
                                            Token(SyntaxKind.CommaToken),
                                            Argument(LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,Literal(field.CollectionName)))
                                                         }));
        }
        return ArgumentList(SeparatedList<ArgumentSyntax>(
                                                         new SyntaxNodeOrToken[]
                                                         {
                                             Argument(
                                                IdentifierName($"global::{field.TypeFullName}.TDLReportName")),
                                            Token(SyntaxKind.CommaToken),
                                            Argument(
                                                IdentifierName($"global::{field.TypeFullName}.TDLCollectionName")),
                                                         }));
    }

    private static VariableDeclarationSyntax CreateLocalIntVariable(string text, int value)
    {
        return VariableDeclaration(PredefinedType(
                                            Token(SyntaxKind.IntKeyword)))
                                    .WithVariables(
                                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                                            VariableDeclarator(
                                                Identifier(text))
                                            .WithInitializer(
                                                EqualsValueClause(
                                                    LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        Literal(value))))));
    }

    private static List<StatementSyntax> GetTDLFieldsStatement(TDLReportGeneratorArgs tDLReportGeneratorArg)
    {
        List<StatementSyntax> statementSyntaxes = new();
        statementSyntaxes.Add(LocalDeclarationStatement(
                            VariableDeclaration(
                                ArrayType(
                                    IdentifierName("global::TallyConnector.Core.Models.Common.Request.Field"))
                                .WithRankSpecifiers(
                                    SingletonList<ArrayRankSpecifierSyntax>(
                                        ArrayRankSpecifier(
                                            SingletonSeparatedList<ExpressionSyntax>(
                                                OmittedArraySizeExpression())))))
                            .WithVariables(
                                SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    VariableDeclarator(
                                        Identifier("fields"))
                                    .WithInitializer(
                                        EqualsValueClause(
                                            ArrayCreationExpression(
                                                ArrayType(
                                                    IdentifierName("global::TallyConnector.Core.Models.Common.Request.Field"))
                                                .WithRankSpecifiers(
                                                    SingletonList<ArrayRankSpecifierSyntax>(
                                                        ArrayRankSpecifier(
                                                            SingletonSeparatedList<ExpressionSyntax>(
                                                                LiteralExpression(
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    Literal(tDLReportGeneratorArg.Fields.Count)))))))))))));
        foreach (var field in tDLReportGeneratorArg.Fields)
        {
            var index = tDLReportGeneratorArg.Fields.IndexOf(field);
            statementSyntaxes.Add(ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                ElementAccessExpression(
                                    IdentifierName("fields"))
                                .WithArgumentList(
                                    BracketedArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                LiteralExpression(
                                                    SyntaxKind.NumericLiteralExpression,
                                                    Literal(index)))))),
                                ImplicitObjectCreationExpression()
                                .WithArgumentList(
                                    ArgumentList(
                                       SeparatedList<ArgumentSyntax>(
                                         new SyntaxNodeOrToken[]
                                         {
                                             Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                        Literal(field.TallyFieldName))),
                                             Token(SyntaxKind.CommaToken),
                                             Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                        Literal(field.TallyXMLTag))),
                                             Token(SyntaxKind.CommaToken),
                                             Argument(field.Set == null ? LiteralExpression(SyntaxKind.NullLiteralExpression):LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                        Literal(field.Set))),
                                             Token(SyntaxKind.CommaToken),
                                             Argument(field.Use  == null ? LiteralExpression(SyntaxKind.NullLiteralExpression):LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                        Literal(field.Use))),
                                         }))))));
        }
        statementSyntaxes.Add(ReturnStatement(IdentifierName("fields")));
        return statementSyntaxes;
    }
    private INamedTypeSymbol? SematicTransform(GeneratorSyntaxContext context, CancellationToken token)
    {
        var classDeclaration = Unsafe.As<ClassDeclarationSyntax>(context.Node);
        ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, token);
        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol;
        }
        return null;
    }

    private bool SyntaxPredicate(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax
        {

        } candidate
        && candidate.Modifiers.Any(SyntaxKind.PartialKeyword)
        && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
        && candidate.AttributeLists.Any();
    }
}

//public class HelperMethodArgs
//{
//    public HelperMethodArgs(string nameSpace,
//                            string className)
//    {
//        NameSpace = nameSpace;
//        ClassName = className;
//    }

//    public string NameSpace { get; }
//    public string ClassName { get; }
//    public ClassDeclarationSyntax ClassSyntax { get; }
//}

