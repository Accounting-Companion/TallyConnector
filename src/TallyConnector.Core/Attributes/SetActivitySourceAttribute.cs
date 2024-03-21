namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
public class SetActivitySourceAttribute : Attribute
{
    

    public  string ActivitySource { get; set; }
}
