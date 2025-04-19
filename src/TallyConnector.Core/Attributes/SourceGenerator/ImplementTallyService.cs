namespace TallyConnector.Core.Attributes.SourceGenerator;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ImplementTallyService : Attribute
{
    public string HandlerName { get; set; }

    



    public ImplementTallyService(string handlerName)
    {
        HandlerName = handlerName;
    }
}
