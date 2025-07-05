using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Class;
public class FunctionNameExtractor : ClassPropertyAttributeTransformer
{
    private readonly Func<ClassData, HashSet<string>> selector;

    public FunctionNameExtractor(Func<ClassData, HashSet<string>> selector)
    {
        this.selector = selector;
    }

    public override void TransformAsync(ClassData data, AttributeData attributeData)
    {
        string? functionName = null;
        if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeData.ConstructorArguments;
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        functionName = (string?)constructorArguments.First().Value;
                        break;
                    default:
                        break;
                }
            }
        }
        if (attributeData.NamedArguments != null && attributeData.NamedArguments.Length > 0)
        {
            var namedArguments = attributeData.NamedArguments;

            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "FunctionName":
                        functionName = (string?)namedArgument.Value.Value;
                        break;
                }
            }
        }
        if (!string.IsNullOrWhiteSpace(functionName))
        {
            var functionList = selector(data);
            functionList.Add(functionName);
        }
    }
}
