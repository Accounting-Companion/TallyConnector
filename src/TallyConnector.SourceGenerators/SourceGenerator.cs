using TallyConnector.SourceGenerators.Extensions;
using TallyConnector.SourceGenerators.Generators;
using TallyConnector.SourceGenerators.Models;

namespace TallyConnector.SourceGenerators;


[Generator(LanguageNames.CSharp)]
public class SourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        MainSyntaxReceiver receiver = (MainSyntaxReceiver)context.SyntaxReceiver;
        ExcuteHelperMethodsGenerator(context, receiver);
        ExecuteXMLTDLReportGenerator(context, receiver);
    }

    private static void ExecuteXMLTDLReportGenerator(GeneratorExecutionContext context, MainSyntaxReceiver receiver)
    {
        //Debugger.Launch();
        foreach (var item in receiver.Classes.ReportClasses)
        {
            SemanticModel semanticModel = context.Compilation.GetSemanticModel(item.SyntaxTree);
            INamedTypeSymbol? namedTypeSymbol = semanticModel.GetDeclaredSymbol(item);
            if (namedTypeSymbol != null)
            {
                var propertyDeclarationSyntaxs = item.Members.OfType<PropertyDeclarationSyntax>();
                System.Collections.Immutable.ImmutableArray<ISymbol> symbols = namedTypeSymbol.GetMembers();
                System.Collections.Immutable.ImmutableArray<AttributeData> attributeDatas = namedTypeSymbol.GetAttributes();
                foreach (var attributeData in attributeDatas)
                {
                    System.Collections.Immutable.ImmutableArray<TypedConstant> constructorArguments = attributeData.ConstructorArguments;
                }
                List<object> members = new();
                foreach (var member in item.Members)
                {
                    IEnumerable<object> enumerable = member.AttributeLists
                        .Where(c => c.Attributes.Count > 0)
                        .SelectMany(k => k.Attributes)
                        .SelectMany(c => c.ArgumentList.Arguments);
                    //.Select(c => ((LiteralExpressionSyntax)c.Expression).Token.ValueText);
                    members.AddRange(enumerable);
                }
                var unused = string.Join(",", members);
            }
            //Debugger.Launch();
        }
    }

    private void ExcuteHelperMethodsGenerator(GeneratorExecutionContext context, MainSyntaxReceiver receiver)
    {
        foreach (var t in receiver.BulkGetMethodAttributes.Syntaxes)
        {
            var classDeclaration = ((ClassDeclarationSyntax)t.Parent.Parent);
            var className = classDeclaration.Identifier.ValueText;
            var nameSpace = ((FileScopedNamespaceDeclarationSyntax)classDeclaration.Parent).Name.ToString();
            SemanticModel semanticModel = context.Compilation.GetSemanticModel(t.SyntaxTree);
            var namesyntax = (GenericNameSyntax)t.Name;
            var arg = namesyntax.TypeArgumentList.Arguments.First();
            //Debugger.Launch();
            //throw new Exception(t.ArgumentList.Arguments.First().ToString());
            Dictionary<string, string> arguments = new();

            foreach (var item in t.ArgumentList.Arguments)
            {
                var Key = item.NameEquals?.Name.Identifier.Value.ToString();
                if (Key != null)
                {
                    var value = ((LiteralExpressionSyntax)item.Expression).Token.Value.ToString();
                    arguments.Add(Key, value);
                }

            }
            var TypeName = ((IdentifierNameSyntax)arg).Identifier.Value;
            TypeInfo typeInfo = semanticModel.GetTypeInfo(arg);
            string ObjectNamespace = string.Join(".", typeInfo.Type.ContainingNamespace.ConstituentNamespaces);
            //throw new Exception(ObjectNamespace);
            string ObjectName = typeInfo.Type.Name;
            arguments.TryGetValue("PluralName", out string PluralName);
            arguments.TryGetValue("TypeName", out string GenericTypeName);
            HelperMethodArgs helperMethodArgs = new(nameSpace, className, ObjectNamespace, ObjectName, PluralName, GenericTypeName);
            context.AddSource($"TallyService_HelperMethods_{TypeName}.g.cs", HelperMethodsGenerator.Generate(helperMethodArgs));
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        //Debugger.Launch();
        context.RegisterForSyntaxNotifications(() => new MainSyntaxReceiver());
        context.RegisterForPostInitialization(PostInitializationCallback);
    }

    private void PostInitializationCallback(GeneratorPostInitializationContext context)
    {
        //context.AddSource("GenerateGetMethodAttribute.g.cs", GenerateBulkGetMethodsAttribute.Generate());
    }


}

public class MainSyntaxReceiver : ISyntaxReceiver
{
    public BulkGetMethodSyntaxReceiver BulkGetMethodAttributes { get; } = new();
    public ClassSyntaxReceiver Classes { get; } = new();
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        BulkGetMethodAttributes.OnVisitSyntaxNode(syntaxNode);
        Classes.OnVisitSyntaxNode(syntaxNode);
    }
}
public class BulkGetMethodSyntaxReceiver : ISyntaxReceiver
{
    public List<AttributeSyntax> Syntaxes = new();
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeSyntax { Name: GenericNameSyntax { Identifier.Value: AttrubuteName, } } attr)
        {
            return;
        }

        //if (syntaxNode is not AttributeSyntax attr)
        //{
        //    return;
        //}
        Syntaxes.Add(attr);
        //AttributeArgumentListSyntax argumentList = attr.ArgumentList;
        //argumentList.Arguments.ToList().ForEach(arg => { arg.na});
    }
}

public class ClassSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> ReportClasses = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {

        if (syntaxNode is not ClassDeclarationSyntax cls)
        {
            return;
        }

        if (cls.BaseList != null && cls.BaseList.Types.Count > 0)
        {
            IEnumerable<BaseTypeSyntax> enumerable = cls.BaseList.Types.Where(type => type.GetBaseTypeName() == "IReportInterfaceGenerator" || type.GetGenericBaseTypeName() == "IReportInterfaceGenerator");
            if (enumerable.Count() > 0)
            {
                //Debugger.Launch();
                ReportClasses.Add(cls);
            }

        }


    }
}
