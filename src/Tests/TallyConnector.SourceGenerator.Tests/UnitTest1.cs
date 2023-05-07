using GeneratorDebugging;
using Microsoft.CodeAnalysis.CSharp;
using TallyConnector.SourceGenerators.Generators;

namespace TallyConnector.SourceGenerator.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var code = CSharpSyntaxTree.ParseText("using TallyConnector.Core.Models.Service;\r\nusing TallyConnector.Extensions;\r\n\r\nnamespace TallyConnector.Services;\r\n[GenerateHelperMethod<Group, Group, GroupCreate>]\r\npublic partial class TallyService : BaseTallyService\r\n{\r\n\r\n}\r\n");
        HelperMethodsGenerator helperMethodsGenerator = new HelperMethodsGenerator();
        Microsoft.CodeAnalysis.GeneratorDriverRunResult generatorDriverRunResult = GeneratorDebugger.RunDebugging(new[] { code },
                                       new Microsoft.CodeAnalysis.IIncrementalGenerator[] { helperMethodsGenerator });
        int v = generatorDriverRunResult.Results.Count();
        Assert.Pass();
    }
}