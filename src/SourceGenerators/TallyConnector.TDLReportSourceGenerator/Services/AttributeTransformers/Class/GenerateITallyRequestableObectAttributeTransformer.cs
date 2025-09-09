using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Class;
public class GenerateITallyRequestableObectAttributeTransformer : ClassPropertyAttributeTransformer
{
    public override void TransformAsync(ClassData data, AttributeData attributeData)
    {
        data.GenerateITallyRequestableObject = true;
        if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeData.ConstructorArguments;
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        var arg = constructorArguments.FirstOrDefault();

                        switch (arg.Kind)
                        {
                            case TypedConstantKind.Enum:
                                // Enum value: use arg.Value
                                if (arg.Value is int intVal)
                                {
                                    data.GenerationMode = (GenerationMode)intVal;
                                }
                                break;

                            case TypedConstantKind.Primitive:
                                // Could be int or something else
                                if (arg.Value is int intVal2)
                                    data.GenerationMode = (GenerationMode)intVal2;
                                break;

                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
