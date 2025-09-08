using TallyConnector.Abstractions.Models;

namespace TallyConnector.Abstractions.Attributes;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class GenerateITallyRequestableObectAttribute : Attribute
{

    public GenerateITallyRequestableObectAttribute(GenerationMode mode = GenerationMode.All)
    {
        Mode = mode;
    }

    public GenerationMode Mode { get; }
}
