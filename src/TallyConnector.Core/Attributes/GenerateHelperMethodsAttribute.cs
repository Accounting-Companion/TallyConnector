namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GenerateHelperMethodsAttribute<T> : Attribute where T : Models.BasicTallyObject
{
    public string? PluralName { get; set; }

    public string? TypeName { get; set; }
}
