using System.Diagnostics;
using TallyConnector.SourceGenerators.Attributes;
using TallyConnector.SourceGenerators.Generators;
using TallyConnector.SourceGenerators.Models;

namespace TallyConnector.SourceGenerators;


[Generator(LanguageNames.CSharp)]
public class SourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        MainSyntaxReceiver receiver = (MainSyntaxReceiver)context.SyntaxReceiver;
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
            HelperMethodsGeneratorArgs helperMethodArgs = new (nameSpace,className,ObjectNamespace, ObjectName, PluralName, GenericTypeName);
            context.AddSource($"TallyService_HelperMethods_{TypeName}.g.cs", HelperMethodsGenerator.Generate(helperMethodArgs));
        }

        foreach (var item in receiver.Classes.Syntaxes)
        {
            List<string> members = new();
            foreach (var member in item.Members)
            {
                IEnumerable<string> enumerable = member.AttributeLists.Where(c=>c.Attributes.Count>0).SelectMany(k=>k.Attributes.Where(k=>((IdentifierNameSyntax)k.Name).Identifier.ValueText == "XmlElement")).SelectMany(c=>c.ArgumentList.Arguments).Select(c=>((LiteralExpressionSyntax)c.Expression).Token.ValueText);
                members.AddRange(enumerable);
            }
            var unused = string.Join(",", members);
            //Debugger.Launch();
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
        context.AddSource("GenerateGetMethodAttribute.g.cs", GenerateBulkGetMethodsAttribute.Generate());
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
    public List<ClassDeclarationSyntax> Syntaxes = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax { Identifier.Value: "Ledger" } cls)
        {
            return;
        }
        //Debugger.Launch();
        Syntaxes.Add(cls);
    }
}
